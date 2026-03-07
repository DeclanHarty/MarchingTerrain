using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainExplosion : MonoBehaviour
{
    public float radius;
    public float terrainChange;

    public MarchingSquaresTerrain terrain;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(0))
        {
            Vector2 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosInGrid = terrain.WorldToGrid(mousePosInWorld);
            terrain.Add(mousePosInGrid, radius, terrainChange * Time.deltaTime);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosInGrid = terrain.WorldToGrid(mousePosInWorld);
            terrain.Subtract(mousePosInGrid, radius, terrainChange * Time.deltaTime);
        }
        
    }
}
