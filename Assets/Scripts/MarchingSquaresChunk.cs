using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MarchingSquaresChunk : MonoBehaviour
{
    public int chunkSize;
    public Mesh mesh;

    private Dictionary<Vector3, int> pointIndexPairs;
    private List<Vector3> vertices;
    private List<int> triangles;

    public void AddVertex(Vector3 vertex)
    {
        if (!pointIndexPairs.ContainsKey(vertex))
        {
            pointIndexPairs.Add(vertex, pointIndexPairs.Count);
            vertices.Add(vertex);
        }

        triangles.Add(pointIndexPairs[vertex]);
    }
}
