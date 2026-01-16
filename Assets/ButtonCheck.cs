using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonCheck : MonoBehaviour, IPointerClickHandler
{
    public string buttonName = "TestButton";

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{buttonName} clicked! PointerID: {eventData.pointerId}, Button: {eventData.button}");
        
        // UI가 EventSystem에서 감지되는지 체크
        if (EventSystem.current != null)
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            Debug.Log("EventSystem currentSelected: " + (currentSelected != null ? currentSelected.name : "None"));
        }
    }
}