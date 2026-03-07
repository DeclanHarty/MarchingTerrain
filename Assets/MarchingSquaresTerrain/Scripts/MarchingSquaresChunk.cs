using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;

public class MarchingSquaresChunk : MonoBehaviour
{
    public int chunkSize;

    private Dictionary<Vector3, int> pointIndexPairs;
    public List<Vector3> vertices;
    private List<int> triangles;

    public Mesh mesh;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    public void Init(int chunkSize)
    {
        this.chunkSize = chunkSize;
        pointIndexPairs = new Dictionary<Vector3, int>();
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }


    public void AddVertex(Vector3 vertex)
    {
        if (!pointIndexPairs.ContainsKey(vertex))
        {
            pointIndexPairs.Add(vertex, pointIndexPairs.Count);
            vertices.Add(vertex);
        }

        triangles.Add(pointIndexPairs[vertex]);
    }

    public void GenerateMesh()
    {
        if(mesh == null)
        {
            mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32
            };
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        meshCollider.sharedMesh = mesh;
        meshFilter.mesh = mesh;
    }

    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        pointIndexPairs.Clear();
    }
}
