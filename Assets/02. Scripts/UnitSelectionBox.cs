using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionBox : MonoBehaviour
{
    [SerializeField] private RectTransform boxVisual;

    private Camera myCam;
    private CommandPanel panel;

    private Rect selectionBox;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isDragging = false;

    [SerializeField] private float dragThreshold = 20f; // 드래그 시작 거리

    private void Start()
    {
        myCam = Camera.main;
        boxVisual.gameObject.SetActive(false);

        // CommandPanel 캐싱
        panel = FindObjectOfType<CommandPanel>();
    }

    private void Update()
    {
        // UI 위면 드래그 무시
        if (panel != null && panel.IsPointerOverUIElement())
            return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // 마우스 클릭 시작
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            endPosition = startPosition;
            isDragging = false;
            boxVisual.gameObject.SetActive(false);

            // Shift 없으면 기존 선택 해제
            // if (!Input.GetKey(KeyCode.LeftShift))
            //     UnitSelectionManager.Instance.DeselectAll();
        }

        // 드래그 중
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;

            if (!isDragging && Vector2.Distance(startPosition, endPosition) > dragThreshold)
            {
                isDragging = true;
                boxVisual.gameObject.SetActive(true);
            }

            if (isDragging)
            {
                UpdateSelectionBox();
                DrawVisual();
                UnitSelectionManager.Instance.DeselectAll();
                SelectUnits();
            }
        }

        // 마우스 클릭 종료
        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                // 단일 클릭 선택 처리
                Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clickedObj = hit.collider.gameObject;

                    if (UnitSelectionManager.Instance.allUnitsList.Contains(clickedObj))
                        UnitSelectionManager.Instance.DragSelect(clickedObj);
                }
            }

            isDragging = false;
            boxVisual.gameObject.SetActive(false);

            // CommandPanel과 선택 정보 동기화
            panel?.SetSelectedUnit();
        }
    }

    private void DrawVisual()
    {
        Vector2 boxSize = new Vector2(Mathf.Abs(endPosition.x - startPosition.x),
                                      Mathf.Abs(endPosition.y - startPosition.y));
        Vector2 boxCenter = (startPosition + endPosition) / 2f;

        boxVisual.position = boxCenter;
        boxVisual.sizeDelta = boxSize;
    }

    private void UpdateSelectionBox()
    {
        selectionBox = new Rect(
            Mathf.Min(startPosition.x, endPosition.x),
            Mathf.Min(startPosition.y, endPosition.y),
            Mathf.Abs(endPosition.x - startPosition.x),
            Mathf.Abs(endPosition.y - startPosition.y)
        );
    }

    private void SelectUnits()
    {
        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            Vector3 screenPos = myCam.WorldToScreenPoint(unit.transform.position);

            // 카메라 뒤 유닛 제외
            if (screenPos.z < 0)
                continue;

            if (selectionBox.Contains(screenPos))
                UnitSelectionManager.Instance.DragSelect(unit);
        }
    }
}
