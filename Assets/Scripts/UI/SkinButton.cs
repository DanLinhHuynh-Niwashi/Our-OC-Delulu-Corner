using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    [SerializeField] private SkinSet skinList;
    [SerializeField] private SkinData skinData;

    public Button button;
    public Image buttonImage;

    public SkinDataEventChannel eventChannel;

    public void OnClick()
    {
        if (skinList == null || skinList.skinList.Count == 0)
            return;

        Debug.Log("[SKIN] Switching skin...");
        int currentIndex = skinList.skinList.IndexOf(skinData);

        if (currentIndex < 0)
            currentIndex = 0;

        int nextIndex = (currentIndex + 1) % skinList.skinList.Count;

        skinData = skinList.skinList[nextIndex];

        if (eventChannel)
            eventChannel.RaiseEvent(skinData);
    }


    public void SetSkinData(SkinSet skinList)
    {
        if (skinList == null || skinList.skinList.Count == 0)
            return;
        this.skinList = skinList;
        this.skinData = skinList.skinList[0];
        buttonImage.sprite = skinList.buttonDeselect;
        buttonImage.preserveAspect = true;

    }

    public SkinData GetSkinData() => skinData;
}
