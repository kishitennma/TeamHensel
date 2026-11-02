using System.Collections.Generic;
using UnityEngine;

public class PlacedMachine
{
    public GameObject machineObject;
    public Vector3Int startCell;
    public int width;
    public int height;

    public List<Vector3Int> GetOccupiedCells()
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells.Add(startCell + new Vector3Int(x, y, 0));
            }
        }

        return cells;
    }
}
