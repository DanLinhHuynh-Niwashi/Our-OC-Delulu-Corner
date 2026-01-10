using UnityEngine;
using UnityEngine.UI;
public class BackgroundButton : MonoBehaviour
{
    [SerializeField] private BackgroundData backgroundData;

    public Button button;
    public Image buttonImage;
    public Image selectOverlay;

    public BackgroundDataEventChannel eventChannel;

    private BackgroundView backgroundView;

    public void OnClick()
    {
        if (backgroundView != null && backgroundView.IsTransitioning) return;
        if (backgroundView != null && backgroundView.CurrentBackground == backgroundData) return;
        if (eventChannel)
            eventChannel.RaiseEvent(backgroundData);
    }

    public void SetBackgroundView(BackgroundView backgroundView)
    {
        this.backgroundView = backgroundView;
    }
    public void SetBackgroundData(BackgroundData backgroundData)
    {
        this.backgroundData = backgroundData;
        buttonImage.sprite = backgroundData.buttonDeselect;
        selectOverlay.sprite = backgroundData.selectOverlay;
        selectOverlay.transform.localScale = backgroundData.overlayScale;
        selectOverlay.enabled = false;

    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            buttonImage.sprite = backgroundData.buttonSelect;
            selectOverlay.enabled = true;
        }
        else
        {
            buttonImage.sprite = backgroundData.buttonDeselect;
            selectOverlay.enabled = false;
        }
    }

    public void OnButtonSelected(BackgroundData backgroundData)
    {
        if (backgroundData != this.backgroundData)
            SetSelected(false);
        else
            SetSelected(true);
    }

    public BackgroundData GetBackgroundData() => backgroundData;
}
