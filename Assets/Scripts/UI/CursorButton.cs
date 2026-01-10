using UnityEngine;
using UnityEngine.UI;

public class CursorButton : MonoBehaviour
{
    [SerializeField] private CursorData cursorData;

    public Button button;
    public Image buttonImage;
    public Image selectOverlay;
    public Image buttonIcon;

    public CursorDataEventChannel eventChannel;

    public void OnClick()
    {
        if (eventChannel)
            eventChannel.RaiseEvent(cursorData);
    }

    public void SetCursorData(CursorData cursorData)
    {
        this.cursorData = cursorData;
        buttonIcon.sprite = cursorData.buttonIcon;
        buttonIcon.preserveAspect = true;
        buttonImage.sprite = cursorData.buttonDeselect;
        selectOverlay.sprite = cursorData.selectOverlay;
        selectOverlay.transform.localScale = cursorData.overlayScale;
        selectOverlay.enabled = false;

    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            if (!cursorData.buttonSelect)
            {
                buttonImage.color = new Color(255,255,255,0);
            }
            buttonImage.sprite = cursorData.buttonSelect;
            selectOverlay.enabled = true;
        }
        else
        {
            if (!cursorData.buttonDeselect)
            {
                buttonImage.color = new Color(255, 255, 255, 0);
            }
            buttonImage.sprite = cursorData.buttonDeselect;
            selectOverlay.enabled = false;
        }
    }

    public void OnButtonSelected(CursorData cursorData)
    {
        if (cursorData != this.cursorData)
            SetSelected(false);
        else
            SetSelected(true);
    }

    public CursorData GetCursorData() => cursorData;
}
