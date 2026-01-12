using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;

public class GridObjectPlacer : MonoBehaviour
{
    [Header("Grid Settings")]
    public Grid grid;
    public Camera cam;

    [Header("Prefabs")]
    public GameObject hqPrefab;
    public GameObject woodProducerPrefab;
    public GameObject towerPrefab;
    public GameObject upgradePrefab;


    [Header("Preview")]
    public GameObject previewObject;
    public Material previewValidMaterial;
    public Material previewInvalidMaterial;
    

    [Header("Raycast")]
    [SerializeField] private LayerMask groundMask;

    private Vector2Int currentSize = Vector2Int.one;
    private GameObject currentPrefab;
    private bool canPlaceCurrent;
    private Vector3Int currentCell;

    public GridData gridData;

    
    


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

    public void SetBuilding(GameObject prefab, Vector2Int size)
    {
        currentPrefab = prefab;
        currentSize = size;

        if (previewObject != null)
            Destroy(previewObject);

        if (currentPrefab != null)
        {
            previewObject = Instantiate(currentPrefab);

            foreach (var c in previewObject.GetComponentsInChildren<Collider>())
                c.enabled = false;

            ApplyPreviewMaterial(previewObject, true);
        }
    }

    private bool TryGetCell(out Vector3Int cell)
    {
        cell = default;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
            return false;

        cell = grid.WorldToCell(hit.point);
        cell.y = 0;
        
        canPlaceCurrent = gridData.canPlace(cell, currentSize);

        return true;
    }   

    private void UpdatePreview(Vector3Int cell)
    {
            if (previewObject == null)
            return;

            Vector3 cellStart = grid.CellToWorld(cell);
            Vector3 cellCenter = cellStart + new Vector3(currentSize.x, 0, currentSize.y) * 0.5f;        

            previewObject.transform.position = cellCenter;
            ApplyPreviewMaterial(previewObject, canPlaceCurrent); 
    }

    private void HandlePreview()
    {
            if (!TryGetCell(out Vector3Int cell))
            return;
        
            currentCell = cell;
            UpdatePreview(cell);
    }
    

    private void HandlePlacement()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        if (EventSystem.current.IsPointerOverGameObject())
            return;


        if (!canPlaceCurrent || currentPrefab == null)
            return;
        
        
        Vector3 cellStart = grid.CellToWorld(currentCell);
        Vector3 cellCenter = cellStart + new Vector3(currentSize.x, 0, currentSize.y)*0.5f;

        Instantiate(currentPrefab, cellCenter, Quaternion.identity);

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
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = targetMat;

                r.materials = mats;
            }
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
