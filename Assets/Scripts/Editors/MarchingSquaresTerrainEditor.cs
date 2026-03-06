using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MarchingSquaresTerrain))]
public class MarchingSquaresTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            MarchingSquaresTerrain terrain = (MarchingSquaresTerrain)target;
            if(terrain.grid == null) terrain.InitGrid();
            terrain.GenerateMesh(terrain.grid);
        }
    }
}
