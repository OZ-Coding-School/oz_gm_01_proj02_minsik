using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridData
{
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    public bool canPlace(Vector3Int startCell, Vector2Int size)
    {
        foreach (Vector3Int cell in GetCells(startCell, size))
        {
            if (occupiedCells.Contains(cell))
            {
                Debug.Log($"BLOCKED CELL: {cell}");
                return false;
            }
        }
        return true;

    }

    public void Place(Vector3Int startCell, Vector2Int size)
    {
        foreach (Vector3Int cell in GetCells(startCell, size))
        {
            occupiedCells.Add(cell);
            Debug.Log($"OCCUPY CELL: {cell}");
        }
    }

    private IEnumerable<Vector3Int> GetCells(Vector3Int start, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                yield return new Vector3Int(start.x + x, 0, start.z + z);
            }
        }
    }
}
