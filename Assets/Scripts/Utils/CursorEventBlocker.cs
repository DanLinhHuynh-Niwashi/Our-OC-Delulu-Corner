using UnityEngine.EventSystems;
using UnityEngine;

public class CursorEventBlocker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.BlockCursor(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.BlockCursor(false);
    }
}
