using UnityEngine;

[CreateAssetMenu(menuName = "Controller/InteractEntry")]
public class InteractEntry : ScriptableObject
{
    public string hitAreaId;
    public InteractActionData action;
    public CursorData cursor;
}
