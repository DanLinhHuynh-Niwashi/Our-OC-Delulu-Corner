using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ModelContainer : MonoBehaviour
{
    public List<CursorButton> cursorButtons;
    public List<SkinButton> skinButtons;
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

    private void SetUpCursorButtons(ModelData modelData)
    {
        List<CursorData> cursorDatas = modelData.cursorList;
        Debug.Log("[UI Manager] Found " + cursorDatas.Count + " cursor...");

        int maxSize = Mathf.Min(cursorDatas.Count, cursorButtons.Count);

        int skipIndex = -1;
        if (maxSize == 5 || maxSize == 6)
            skipIndex = 3;

        int dataIndex = 0;

        for (int i = 0; i < cursorButtons.Count; i++)
        {
            if (dataIndex >= cursorDatas.Count)
            {
                cursorButtons[i].gameObject.SetActive(false);
                continue;
            }

            if (i == skipIndex)
            {
                cursorButtons[i].gameObject.SetActive(false);
                continue;
            }

            cursorButtons[i].SetCursorData(cursorDatas[dataIndex]);

            bool isStarter = cursorDatas[dataIndex] == modelData.starterCursor;
            cursorButtons[i].SetSelected(isStarter);

            cursorButtons[i].gameObject.SetActive(true);

            dataIndex++;
        }
    }

}
