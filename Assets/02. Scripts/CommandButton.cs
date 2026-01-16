using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CommandButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        UnitSelectionManager.Instance.blockClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 코루틴으로 한 프레임 기다린 후 해제
        UnitSelectionManager.Instance.StartCoroutine(ResetBlockClickNextFrame());
    }

    private IEnumerator ResetBlockClickNextFrame()
    {
        yield return null;
        UnitSelectionManager.Instance.blockClick = false;
    }
}