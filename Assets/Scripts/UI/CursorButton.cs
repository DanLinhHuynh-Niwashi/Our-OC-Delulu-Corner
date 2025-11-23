using UnityEngine;
using UnityEngine.UI;

public class CursorButton : MonoBehaviour
{
    [SerializeField] private CursorData cursorData;

    public Button button;
    public Image buttonImage;
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

    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            buttonImage.sprite = cursorData.buttonSelect;
        }
        else
        {
            buttonImage.sprite = cursorData.buttonDeselect;
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
