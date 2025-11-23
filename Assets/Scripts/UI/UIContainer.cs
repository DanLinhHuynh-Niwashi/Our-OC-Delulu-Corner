using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ModelContainer : MonoBehaviour
{
    public List<CursorButton> cursorButtons;

    void Start()
    {
        if (GameManager.Instance.GetCurrentModel() != null)
        {
            SetUpFromModelData(GameManager.Instance.GetCurrentModel());
        }
    }

    private void SetUpFromModelData(ModelData modelData)
    {
        Debug.Log("[UI Manager] Setting up UI...");
        if (!modelData)
            return;

        List<CursorData> cursorDatas = modelData.cursorList;
        Debug.Log("[UI Manager] Found" + modelData.cursorList.Count + "cursor...");

        int maxSize = Mathf.Min(cursorDatas.Count, cursorButtons.Count);

        for (int i = 0; i < maxSize; i++)
        {
            cursorButtons[i].SetCursorData(cursorDatas[i]);

            bool isStarter = cursorDatas[i] == modelData.starterCursor;
            cursorButtons[i].SetSelected(isStarter);

            cursorButtons[i].gameObject.SetActive(true);
        }

        for (int i = maxSize; i < cursorButtons.Count; i++)
        {
            cursorButtons[i].gameObject.SetActive(false);
        }
    }

}
