using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class GridObjectPlacer : MonoBehaviour
{
    [Header("Grid Settings")]
    public Grid grid;
    public Camera cam;

    [Header("Prefabs")]
    public GameObject cube1x1Prefab;
    public GameObject cube2x2Prefab;    

    [Header("Preview")]
    public GameObject previewObject;
    public Material previewValidMaterial;
    public Material previewInvalidMaterial;
    private bool canPlaceCurrent;
    

    [Header("Raycast")]
    [SerializeField] private LayerMask groundMask;

    private Vector2Int currentSize = Vector2Int.one;

    public GridData gridData;
    private Vector3Int currentCell;

    
    


    void Awake()
    {
        gridData = new GridData();
        Debug.Log($"GridData created in {gameObject.name}");
    }

    void Update()
    {
        HandlePreview();
        HandlePlacement();

    }

    private bool TryGetCell(out Vector3Int cell)
    {
        cell = default;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
            return false;

        cell = grid.WorldToCell(hit.point);
        cell.y = 0;
        
        bool canPlaceHere = gridData.canPlace(cell, currentSize);
        Debug.Log($"Ray hit: {hit.collider.gameObject.name}, Hit point: {hit.point}, Grid cell: {cell}, CanPlace: {canPlaceHere}");

        return true;
    }   

    private void UpdatePreview(Vector3Int cell)
    {
            Vector3 cellStart = grid.CellToWorld(cell);
            Vector3 cellCenter = cellStart + new Vector3(currentSize.x, 0, currentSize.y) * 0.5f;

            if (previewObject == null)
            {
                GameObject prefab = currentSize == Vector2Int.one ? cube1x1Prefab : cube2x2Prefab;
                previewObject = Instantiate(prefab);

                foreach (Collider c in previewObject.GetComponentsInChildren<Collider>())
                c.enabled = false;
            
            }

            previewObject.transform.position = cellCenter;
            ApplyPreviewMaterial(previewObject, canPlaceCurrent); 
    }

    private void HandlePreview()
    {
         if (!TryGetCell(out Vector3Int cell))
            return;

            currentCell = cell;
            canPlaceCurrent = gridData.canPlace(cell, currentSize);

            UpdatePreview(cell);
    }
    

    private void HandlePlacement()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        if (!canPlaceCurrent)
        return;
        
        
        Vector3 cellStart = grid.CellToWorld(currentCell);
        Vector3 cellCenter = cellStart + new Vector3(currentSize.x, 0, currentSize.y)*0.5f;

        GameObject prefab = currentSize == Vector2Int.one ? cube1x1Prefab : cube2x2Prefab;
        GameObject newObj = Instantiate(prefab, cellCenter, Quaternion.identity);

        // foreach (Renderer r in newObj.GetComponentsInChildren<Renderer>())
        // {
        //     r.material = null;
        // }
        
        gridData.Place(currentCell, currentSize);

            
            
    }

    private void ApplyPreviewMaterial(GameObject obj, bool canPlaceCurrent)
    {
        Material targetMat = canPlaceCurrent ? previewValidMaterial : previewInvalidMaterial;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            Material[] mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = targetMat;

            }
            r.materials = mats;
        }

        // Debug.Log($"Preview Material Applied: {targetMat.name}");

    }

    public void SetBuildingSize(Vector2Int size)
    {
        currentSize = size;

        if (previewObject != null)
        Destroy(previewObject);
    }


    
}
