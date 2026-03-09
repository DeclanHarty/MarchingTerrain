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
            terrain.InitGrid();
            terrain.GenerateTerrain(terrain.grid);
        }
    }
}
