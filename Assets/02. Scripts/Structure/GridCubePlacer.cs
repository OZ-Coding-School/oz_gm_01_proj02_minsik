// using System.Collections;
// using System.Collections.Generic;
// using Unity.Mathematics;
// using UnityEngine;

// public class GridCubePlacer : MonoBehaviour
// {
//     public Grid grid;
//     public Camera cam;
//     public GameObject cubePrefab;
    

//     private void Update()
//     {
//         if (Input.GetMouseButtonDown(0))
//         {
//             Ray ray = cam.ScreenPointToRay(Input.mousePosition);

//             if (Physics.Raycast(ray, out RaycastHit hit))
//             {
//                 Vector3Int cell = grid.WorldToCell(hit.point);
//                 cell.y = 0;

//                 Vector3 worldPos = grid.CellToWorld(cell);

//                 Vector3 cellCenter = worldPos + grid.cellSize / 2.0f;

//                 Instantiate(cubePrefab, cellCenter, quaternion.identity);
                
//             }
//         }
//     }
// }
