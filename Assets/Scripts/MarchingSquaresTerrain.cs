using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Rendering;

public class MarchingSquaresTerrain : MonoBehaviour
{
    [Range(0,1)]
    public float surfaceValue;

    public Transform bottomLeftMarkerTransform;
    private Vector3 bottomLeftPosition;

    public MarchingSquaresGrid grid;

    public Vector2 randOffset;
    [Min(1)]
    public float noiseScale;
    [Range(.1f, 1)]
    public float gridScale;

    [Range(0, 1)]
    public float interpolationScale;

    [Min(2)]
    public int gridChunksX = 2;
    [Min(2)]
    public int gridChunksY = 2;

    [Min(2)]
    public int chunkSize = 2;



    public GameObject caseTilePrefab;
    public GameObject[,] chunkObjects;
    private Mesh[,] chunkMeshes;
    public Material meshMaterial;

    public int MESH_VERTS_UPPER_LIMIT = 65000;
    // Start is called before the first frame update
    void Start()
    {
        bottomLeftPosition = bottomLeftMarkerTransform.position;
        InitGrid();
        GenerateMesh(grid);
    }

    public void InitGrid()
    {
        grid = new MarchingSquaresGrid(gridChunksX * chunkSize, gridChunksY * chunkSize, surfaceValue, GenerateNoise);
    }

    public void GenerateMesh(MarchingSquaresGrid grid)
    {
        byte[,] caseValues = grid.GetCaseValues(surfaceValue);

        Vector3[] translations = {new Vector3(-gridScale/2, -gridScale/2), new Vector3(0, -gridScale/2), new Vector3(gridScale/2, -gridScale/2), new Vector3(gridScale/2, 0), new Vector3(gridScale/2, gridScale/2), new Vector3(0, gridScale/2), new Vector3(-gridScale/2, gridScale/2), new Vector3(-gridScale/2, 0)};
        Dictionary<Vector3, int> pointIndexPairs = new Dictionary<Vector3, int>();
        List<int> triangleIndices = new List<int>();
        List<Vector3> vertices = new List<Vector3>();

        float[,] horizontalInterp = grid.GetHorizontalInterpolatedValues();
        float[,] verticalInterp = grid.GetVerticalInterpolatedValues();

        for(int y = 0; y < caseValues.GetLength(1); y++)
        {
            for(int x = 0; x < caseValues.GetLength(0); x++)
            {
                int[] triangles = grid.GetTrianglesFromIndex(caseValues[x,y]);
                
                for(int i = 0; i < triangles.Length; i++)
                {
                    Vector3 vertex = bottomLeftMarkerTransform.position + new Vector3(x,y) * gridScale + translations[triangles[i]] + new Vector3(gridScale/2, gridScale/2);
                    Vector3 interpOffset = Vector3.zero;

                    switch (triangles[i])
                    {
                        case 1:
                            interpOffset = new Vector3(horizontalInterp[x,y] - .5f, 0);
                            break;
                        case 3:
                            interpOffset = new Vector3(0, verticalInterp[x+1,y] - .5f);
                            break;
                        case 5:
                            interpOffset = new Vector3(horizontalInterp[x,y+1] - .5f, 0);
                            break;
                        case 7:
                            interpOffset = new Vector3(0, verticalInterp[x,y] - .5f);
                            break;
                    }

                    vertex += interpOffset* interpolationScale * gridScale;

                    if (!pointIndexPairs.ContainsKey(vertex))
                    {
                        pointIndexPairs.Add(vertex, pointIndexPairs.Count);
                        vertices.Add(vertex);
                    }

                    triangleIndices.Add(pointIndexPairs[vertex]);
                }
            }
        }

        if(mesh == null)
        {
            mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32
            };
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);

        Vector3[] normals = mesh.normals;

        MeshFilter meshFilter;
        MeshCollider meshCollider;

        if(meshObject == null)
        {
            meshObject = new GameObject("MarchingSquaresMesh");
            MeshRenderer meshRenderer = meshObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshFilter = meshObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            meshCollider = meshObject.AddComponent(typeof(MeshCollider)) as MeshCollider;

            meshRenderer.material = meshMaterial;
        }
        else
        {
            meshFilter = meshObject.GetComponent<MeshFilter>();
            meshCollider = meshObject.GetComponent<MeshCollider>();
        }

        meshCollider.sharedMesh = mesh;
        meshFilter.mesh = mesh;
    }

    public float GenerateNoise(Vector2 pos)
    {
        float value = Mathf.PerlinNoise(pos.x / noiseScale + randOffset.x, (pos.y) / noiseScale + randOffset.y);
        
        return value;
    }

    public Vector2 WorldToGrid(Vector3 worldPos)
    {
        Vector2 worldToGridCornerDifference = worldPos - bottomLeftPosition;
        Vector2 gridPosition = worldToGridCornerDifference / gridScale;
        Debug.Log(gridPosition);

        return gridPosition;
    }

    public void Subtract(Vector2 gridPosition, float radius, float subtractAmount)
    {
        float[,] gridValues = grid.GetGridValues();
        radius = radius / gridScale;
        
        for(int y = 0; y < gridValues.GetLength(1); y++)
        {
            for(int x = 0; x < gridValues.GetLength(0); x++)
            {
                Vector2Int pos = new Vector2Int(x,y);
                float distanceFromCenter = Vector2.Distance(gridPosition, new Vector2(x,y));
               if(distanceFromCenter < radius)
                {
                    
                    grid.SetGridValue(pos, grid.GetGridValue(pos) - subtractAmount);
                } 
            }
        }
        GenerateMesh(grid);
    }

    public void Add(Vector2 gridPosition, float radius, float addAmount)
    {
        float[,] gridValues = grid.GetGridValues();
        radius = radius / gridScale;
        for(int y = 0; y < gridValues.GetLength(1); y++)
        {
            for(int x = 0; x < gridValues.GetLength(0); x++)
            {
                Vector2Int pos = new Vector2Int(x,y);
                float distanceFromCenter = Vector2.Distance(gridPosition, new Vector2(x,y));
               if(distanceFromCenter < radius)
                {
                    grid.SetGridValue(pos, grid.GetGridValue(pos) + addAmount);
                } 
            }
        }
        GenerateMesh(grid);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(bottomLeftMarkerTransform.position, .1f);

        if(grid != null)
        {
            float[,] gridValues = grid.GetGridValues();
            for(int y = 0; y < gridValues.GetLength(1); y++)
            {
                for(int x = 0; x < gridValues.GetLength(0); x++)
                {
                    if(gridValues[x,y] < surfaceValue)
                    {
                        Gizmos.color = new Color(gridValues[x,y], gridValues[x,y], gridValues[x,y]);
                        Gizmos.DrawSphere(bottomLeftMarkerTransform.position + new Vector3(x,y) * gridScale, .2f * gridScale);
                    }
                    
                }
            }
        }
    }

    
}
