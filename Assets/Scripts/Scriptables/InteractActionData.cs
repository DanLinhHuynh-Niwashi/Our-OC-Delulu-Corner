using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractActionData", menuName = "Controller/Interact/InteractActionData")]
public class InteractActionData : ScriptableObject
{
    public bool isDrag = false;

    [Tooltip("Target dialog to show")]
    public string targetDialog = "";

    [Tooltip("End when animation ends")]
    public bool isEndOnAnimEnd = false;

    [Tooltip("End when dialog ends")]
    public bool isEndOnDialogEnd = false;

    public InteractionLogic logic;
}

