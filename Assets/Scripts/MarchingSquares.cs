using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarchingSquaresGrid
{

    // Reference https://nils-olovsson.se/articles/marching_squares/
    public float surfaceValue;
    float[,] valueGrid;
    byte[,] caseValues;

    /*
    Each case contains the relative triangles 
    6 - - 5 - - 4
    |           |
    7           3
    |           |
    0 - - 1 - - 2
    */

    int[][] triangleCases = new int[][]
    {
        new int[] {}, // EMPTY
        new int[] {0, 1, 7}, // FILLED BOTTOM LEFT
        new int[] {1, 2, 3}, // FILLED BOTTOM RIGHT
        new int[] {0, 2, 7, 2, 3, 7}, // EMPTY TOP
        new int[] {3, 4, 5}, // FILLED TOP RIGHT
        new int[] {0, 1, 7, 3, 4, 5}, // EMPTY DESCENDING MIDDLE
        new int[] {1, 2, 4, 1, 4, 5}, // EMPTY LEFT
        new int[] {0, 2, 7, 2, 5, 7, 2, 4, 5}, // EMPTY TOP LEFT
        new int[] {5, 6, 7}, // FILLED TOP LEFT
        new int[] {0, 1, 5, 0, 5, 6}, // EMPTY RIGHT
        new int[] {1, 2, 3, 5, 6, 7}, // EMPTY ASCENDING MIDDLE
        new int[] {0, 2, 3, 0, 3, 5, 0, 5, 6}, // EMPTY TOP RIGHT
        new int[] {4, 6, 7, 3, 4, 7}, // EMPTY BOTTOM
        new int[] {0, 1, 6, 3, 4, 6, 1, 3, 6}, // EMPTY BOTTOM RIGHT
        new int[] {1, 2, 4, 4, 6, 7, 1, 4, 7}, // EMPTY BOTTOM LEFT
        new int[] {0, 2, 6, 2, 4, 6}, // SOLID
    };

    public MarchingSquaresGrid(int width, int height, float surfaceValue, Func<Vector2, float> valueFunction)
    {
        if(width < 2 || height < 2)
        {
            throw new System.ArgumentException("Both width and height must be greater than 2");
        }
        this.surfaceValue = surfaceValue;
        valueGrid = new float[width,height];
        FillGrid(valueGrid, valueFunction);
        caseValues = GetCaseValues(surfaceValue);
    }

    public void FillGrid(float[,] grid, Func<Vector2, float> valueFunction)
    {
        for(int y = 0; y < grid.GetLength(1); y++)
        {
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x,y] = valueFunction(new Vector2(x,y));
            }
        }
    }

    public float[,] GetGridValues()
    {
        return valueGrid;
    }

    public float GetGridValue(Vector2Int pos)
    {
        return valueGrid[pos.x, pos.y];
    }

    public void SetGridValue(Vector2Int pos, float value)
    {
        valueGrid[pos.x,pos.y] = Mathf.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// Gets a case value based on the values of the four vertices and a given surface value
    /// </summary>
    /// <param name="bottomLeft"></param>
    /// <param name="bottomRight"></param>
    /// <param name="topRight"></param>
    /// <param name="topLeft"></param>
    /// <param name="isoValue"></param>
    /// <returns>Returns a single byte to represent the case value to be used as an index in the edges table</returns>
    public byte GetCaseValue(float bottomLeft, float bottomRight, float topRight, float topLeft, float surfaceValue)
    {
        byte caseValue = 0;

        // Generates the byte mask
        if(bottomLeft < surfaceValue) caseValue |= 1;
        if(bottomRight < surfaceValue) caseValue |= 2;
        if(topRight < surfaceValue) caseValue |= 4;
        if(topLeft < surfaceValue) caseValue |= 8;

        return caseValue;
    }

    public byte[,] GetCaseValues(float surfaceValue)
    {
        byte[,] caseValues = new byte[valueGrid.GetLength(0) - 1, valueGrid.GetLength(1) - 1];

        for(int y = 0; y < caseValues.GetLength(1); y++)
        {
            for(int x = 0; x < caseValues.GetLength(0); x++)
            {
                caseValues[x,y] = GetCaseValue(valueGrid[x,y], valueGrid[x+1,y], valueGrid[x+1,y+1], valueGrid[x,y+1], surfaceValue);
            }
        }

        return caseValues;
    }

    public float[,] GetHorizontalInterpolatedValues()
    {
        float[,] interpValues = new float[valueGrid.GetLength(0) - 1, valueGrid.GetLength(1)];
        for(int y = 0; y < valueGrid.GetLength(1); y++)
        {
            for(int x = 0; x < valueGrid.GetLength(0) - 1; x++)
            {
                float interpValue = (surfaceValue - valueGrid[x,y]) / (valueGrid[x+1,y] - valueGrid[x,y]);
                interpValues[x,y] = interpValue >= 0 && interpValue <= 1 ? interpValue : .5f;
            }
        }

        return interpValues;
    }

    public float[,] GetVerticalInterpolatedValues()
    {
        float[,] interpValues = new float[valueGrid.GetLength(0), valueGrid.GetLength(1) - 1];
        for(int y = 0; y < valueGrid.GetLength(1) - 1; y++)
        {
            for(int x = 0; x < valueGrid.GetLength(0); x++)
            {
                float interpValue = (surfaceValue - valueGrid[x,y]) / (valueGrid[x,y+1] - valueGrid[x,y]);
                interpValues[x,y] = interpValue >= 0 && interpValue <= 1 ? interpValue : .5f;
            }
        }

        return interpValues;
    }

    public int[] GetTrianglesFromIndex(byte index)
    {
        return triangleCases[index];
    } 

}
