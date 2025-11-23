using UnityEngine;

[CreateAssetMenu(fileName = "NewCursorData", menuName = "Controller/Cursor/CursorData")]
public class CursorData : ScriptableObject
{
    public string id;
    public Texture2D image;
    public Sprite buttonIcon;
    public Sprite buttonSelect;
    public Sprite buttonDeselect;
    public Vector2 hotspot = Vector2.zero;
}
