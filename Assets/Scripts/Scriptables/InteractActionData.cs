using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractActionData", menuName = "Controller/Interact/InteractActionData")]
public class InteractActionData : ScriptableObject
{
    [Tooltip("Target animations to play")]
    public AnimationClip targetAnim = null;

    [Tooltip("Whether to loop the animation")]
    public bool animLoop = false;

    [Tooltip("Target expression to apply <ID>")]
    public int targetExpression = -1;

    [Tooltip("Target dialog to show")]
    public string targetDialog = "";

    [Tooltip("Duration of the interaction (-1 = default)")]
    public float duration = -1f;

    [Tooltip("End when animation ends")]
    public bool isEndOnAnimEnd = false;

    [Tooltip("End when dialog ends")]
    public bool isEndOnDialogEnd = false;
}

