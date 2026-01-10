using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class UIContainer : MonoBehaviour
{
    public List<CursorButton> cursorButtons;
    public List<BackgroundButton> backgroundButtons;
    public List<SkinButton> skinButtons;

    public BackgroundView backgroundView;
    public Image modelImage;
    //public TextMeshProUGUI modelName;
    //public Image modelAvatar;
    public Image modelLogo;
    public Image backButtonImage;

    void Start()
    {
        if (GameManager.Instance.GetCurrentModel() != null)
        {
            SetUpFromModelData(GameManager.Instance.GetCurrentModel());
        }
    }

    private void SetUpFromModelData(ModelData modelData)
    {
        if (!modelData) return;
        SetUpCursorButtons(modelData);
        SetSkinButtons(modelData);
        SetUpBackgroundButtons(modelData);
        SetUpTopPanel(modelData);
    }
    private void SetUpTopPanel(ModelData modelData)
    {
        backButtonImage.sprite = modelData.backSprite;
        backButtonImage.preserveAspect = true;
        modelImage.sprite = modelData.modelImage;
        modelImage.preserveAspect = true;
        modelImage.rectTransform.sizeDelta = new Vector2 (modelData.modelImageWidth, modelImage.rectTransform.sizeDelta.y);
        modelLogo.sprite = modelData.logoSprite;
        modelLogo.preserveAspect = true;
    }
    private void SetSkinButtons(ModelData modelData)
    {
        SkinSet hair = modelData.skinListHair;
        SkinSet top = modelData.skinListTop;
        SkinSet bot = modelData.skinListBot;

        if (hair)
            skinButtons[0].SetSkinData(hair);
        else skinButtons[0].gameObject.SetActive(false);
        if (top)
            skinButtons[1].SetSkinData(top);
        else skinButtons[1].gameObject.SetActive(false);
        if (bot)
            skinButtons[2].SetSkinData(bot);
        else skinButtons[2].gameObject.SetActive(false);
    }

    private void SetUpBackgroundButtons(ModelData modelData)
    {
        List<BackgroundData> backgroundDatas = modelData.backgroundList;
        Debug.Log("[UI Manager] Found " + backgroundDatas.Count + " backgrounds...");

        for (int i = 0; i < backgroundButtons.Count; i++)
        {
            if (i >= backgroundDatas.Count)
            {
                backgroundButtons[i].gameObject.SetActive(false);
                continue;
            }

            backgroundButtons[i].SetBackgroundData(backgroundDatas[i]);
            backgroundButtons[i].SetBackgroundView(backgroundView);
            bool isStarter = backgroundDatas[i] == modelData.starterBackground;
            backgroundButtons[i].SetSelected(isStarter);

            backgroundButtons[i].gameObject.SetActive(true);
        }

        backgroundView.Apply(modelData.starterBackground);
    }
    private void SetUpCursorButtons(ModelData modelData)
    {
        List<CursorData> cursorDatas = modelData.cursorList;
        Debug.Log("[UI Manager] Found " + cursorDatas.Count + " cursor...");

        int maxSize = Mathf.Min(cursorDatas.Count, cursorButtons.Count);

        for (int i = 0; i < cursorButtons.Count; i++)
        {
            if (i >= cursorDatas.Count)
            {
                cursorButtons[i].gameObject.SetActive(false);
                continue;
            }

            if (!cursorDatas[i])
            {
                cursorButtons[i].gameObject.SetActive(false);
                continue;
            }

            cursorButtons[i].SetCursorData(cursorDatas[i]);

            bool isStarter = cursorDatas[i] == modelData.starterCursor;
            cursorButtons[i].SetSelected(isStarter);

            cursorButtons[i].gameObject.SetActive(true);
        }
    }

}
