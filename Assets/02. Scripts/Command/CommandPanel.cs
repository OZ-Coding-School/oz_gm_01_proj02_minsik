using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandPanel : MonoBehaviour
{
    public GameObject buildPanel;
    private enum InputMode { None, Move, Attack, Patrol }
    [SerializeField] private InputMode currentInputMode = InputMode.None;
    [SerializeField] private Texture2D actionCursor;
    public List<RectTransform> uiRects = new List<RectTransform>();

    private List<UnitController> selectedUnitControllers = new List<UnitController>();
    private bool isActionCursorActive = false;
    
    
    private void OnEnable()
    {
        UnitSelectionManager.OnUnitSelected += SetSelectedUnit;       
    }

    private void OnDisable()
    {
        UnitSelectionManager.OnUnitSelected -= SetSelectedUnit;
    }

    void Update()
    {
            // if (IsPointerOverUIElement())
            // return; 
        
        // if (EventSystem.current != null &&
        // EventSystem.current.IsPointerOverGameObject())
        // return;

        if (Input.GetKeyDown(KeyCode.B)) ShowBuild();
        if (Input.GetKeyDown(KeyCode.Escape)) CloseBuild();
        
        HandleRightClick();
        HandleLeftClick();
        

    }

    private void HandleRightClick()
    {
        
        if (!Input.GetMouseButtonDown(1)) return;
        
        if (selectedUnitControllers.Count == 0)
            return;
        
        if (!Raycast(out RaycastHit hit)) 
            return;


         bool isGround = ((1 << hit.collider.gameObject.layer) & LayerMask.GetMask("Ground")) != 0;
         Unit enemy = hit.collider.GetComponent<Unit>();

         if (enemy != null)
        {
            Attack(enemy.transform);
        }

        else if (isGround)
        {
            Move(hit.point);
        }
    }

    private void HandleLeftClick()
    {   
        
        if (!Input.GetMouseButtonDown(0)) return;

        if (currentInputMode == InputMode.None) 
        {
            return;
        }
        if (selectedUnitControllers.Count == 0)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) 
        {
            return;
        }

        bool isGround = ((1 << hit.collider.gameObject.layer) & LayerMask.GetMask("Ground")) != 0;
        Unit enemy = hit.collider.GetComponent<Unit>();


        switch (currentInputMode)
        {
            case InputMode.Attack:
                if(enemy != null)
                    Attack(enemy.transform);
                else if (isGround)
                    AttackMove(hit.point);
                break;            

            case InputMode.Move:
                if (isGround)
                    Move(hit.point);
                    else
                Debug.Log("MoveMode but clicked non-ground → ignoring");
                break;

            case InputMode.Patrol:
                if (isGround)
                    Patrol(hit.point);
                     else
                Debug.Log("PatrolMode but clicked non-ground → ignoring");
                break;
        }   

        
        
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        currentInputMode = InputMode.None;
    
        
    }

    public void OnMoveButtonPressed()
    {
        currentInputMode = InputMode.Move;
        
    }
    public void OnAttackButtonPressed()
    {
        Debug.Log("OnAttackButtonPressed 호출됨!");
        currentInputMode = InputMode.Attack;
        Cursor.SetCursor(actionCursor, Vector2.zero, CursorMode.Auto);  
        isActionCursorActive = true; // ← 여기 필요 
    }

    public void OnPatrolButtonPressed()
    {
        currentInputMode = InputMode.Patrol;
        Cursor.SetCursor(actionCursor, Vector2.zero, CursorMode.Auto);
        isActionCursorActive = true; // ← 여기 필요
        
    }

    public void SetSelectedUnit()
    {
        selectedUnitControllers.Clear();
        
        foreach (var unit in UnitSelectionManager.Instance.unitsSelected)
        {
            var unitselect = unit.GetComponent<UnitController>();
            if (unitselect != null)
                selectedUnitControllers.Add(unitselect);
        }
        Debug.Log("Selected Units: " + selectedUnitControllers.Count);
    }

    public void ShowBuild()
    {
        buildPanel.SetActive(true);
    }

    public void CloseBuild()
    {
        buildPanel.SetActive(false);
    }

    public void Move(Vector3 destination)
    {
        if (selectedUnitControllers.Count == 0)
    {
        Debug.LogWarning("Move called but no units selected");
        return;
    }

    Debug.Log("Move called to position: " + destination);
        foreach (var unitselect in selectedUnitControllers)
        unitselect.MoveTo(destination);

        Debug.Log("Move command sent to units: " + selectedUnitControllers.Count);

    }

    public void Attack(Transform target)
    {
        
        foreach (var unitselect in selectedUnitControllers)
        {
            unitselect.SetCommandMode(CommandMode.Attack, target);

        }
    }

    private void AttackMove(Vector3 destination)
    {
        foreach (var unit in selectedUnitControllers)
        {
            unit.SetCommandMode(CommandMode.AttackMove);
            unit.MoveTo(destination);
        }
    }

    private void Patrol(Vector3 point)
    {
        foreach (var unit in selectedUnitControllers)
            unit.SetPatrolPoints(unit.transform.position, point);
    }

    // public void OnPatrolClicked()
    // {   
    //     foreach (var unit in selectedUnitControllers)
    //     {
    //          unit.SetCommandMode(CommandMode.Patrol);
    //     }
    // }

    public void OnStopClicked()
    {
        currentInputMode = InputMode.None;

        foreach (var unitselect in selectedUnitControllers)
        unitselect.SetCommandMode(CommandMode.Stop);
    }

    private bool Raycast(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity);
    }

    public bool IsPointerOverUIElement()
    {
        Vector2 mousePos = Input.mousePosition;

        foreach (var rect in uiRects)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos))
            {
                return true; // UI 위
            }
        }
        return false; // UI 위 아님
    }
  
    
}
