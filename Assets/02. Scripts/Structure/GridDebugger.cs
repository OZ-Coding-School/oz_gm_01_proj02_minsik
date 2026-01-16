using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebugger : MonoBehaviour
{
    public Grid grid;
    public Camera cam;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Vector3Int cell = grid.WorldToCell(hit.point);
                Vector3 world = grid.CellToWorld(cell);

                Debug.Log($"Grid: {cell} World: {world}");
            }
        }
    }
}
