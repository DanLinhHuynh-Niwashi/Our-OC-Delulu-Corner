using UnityEngine;

[CreateAssetMenu(fileName = "NewCursorData", menuName = "Controller/Cursor/CursorData")]
public class CursorData : ScriptableObject
{
    public string id;
    public Texture2D image;
    public Sprite buttonIcon;
    public Sprite buttonSelect;
    public Sprite buttonDeselect;
    public Sprite selectOverlay;
    public Vector3 overlayScale = Vector3.one;
    public Vector2 hotspot = Vector2.zero;
}
