using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core.Health;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }        

    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();
    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask attackable;
    public bool attackCursorVisible;
    public static event Action OnUnitSelected;

    public GameObject groundMarker;

    private Camera cam;

    public bool blockClick = false;
    public List<RectTransform> uiRects = new List<RectTransform>();
    private int physicsRaycastMask;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }

        physicsRaycastMask = ~LayerMask.GetMask("UI");

    }

    void Start()
    {
        cam = Camera.main;       
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            DeselectAll();
        
        // if (IsPointerOverUI())
        // return;

        HandleLeftClick();
        HandleRightClick();
    }

    private void HandleLeftClick()
    {
        if (!Input.GetMouseButtonDown(0))
        return;

        
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, physicsRaycastMask))
        {
        Debug.Log("Raycast hit: " + hit.collider.name + " Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
        GameObject clickedObject = hit.collider.gameObject;
        int hitLayer = clickedObject.layer;

        if (((1 << hitLayer) & clickable) != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                MultiSelect(hit.collider.gameObject);
            // else    
                SelectByClicking(hit.collider.gameObject);
        }

        else if (ground.ContainsLayer(clickedObject.layer) || attackable.ContainsLayer(clickedObject.layer))
        {
        // Ground 또는 공격 가능한 오브젝트 클릭 시 선택 해제 안 함
            return;
        }
        else
        {
            // if (!Input.GetKey(KeyCode.LeftShift))
            //     DeselectAll();
        }
        }
        else
        {
            Debug.Log("[Raycast Missed] No object hit by raycast");
        }
    }

    private void HandleRightClick()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        if (unitsSelected.Count == 0)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, physicsRaycastMask))
            return;

        if (((1 << hit.collider.gameObject.layer) & ground) != 0)
        {
            groundMarker.transform.position = hit.point;
            groundMarker.SetActive(false);
            groundMarker.SetActive(true);
        }
    }

     private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool overUI = results.Count > 0;
        blockClick = overUI; // UI 위면 클릭 무시
        return overUI;
    }


    private bool AtLeastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit.GetComponent<AttackController>())
            {
                return true;
            }

        }
        return false;
    }


    private void MultiSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
            unitsSelected.Remove(unit);
        }
    }



    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit, false);;
        }
        unitsSelected.Clear();
        groundMarker.SetActive(false);

        OnUnitSelected?.Invoke();
    }


    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        unitsSelected.Add(unit);

        SelectUnit(unit, true);
    }

    // private void EnableUnitMovement(GameObject unit, bool shouldMove)
    // {
    //     unit.GetComponent<UnitController>().enabled = shouldMove;
    // }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.Find("Indicator").gameObject.SetActive(isVisible);
    }

    internal void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }   
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerSelectionIndicator(unit, isSelected);
        // EnableUnitMovement(unit, isSelected);

        OnUnitSelected?.Invoke();
    }

    

}

public static class LayerMaskExtensions
{
    public static bool ContainsLayer(this LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}

