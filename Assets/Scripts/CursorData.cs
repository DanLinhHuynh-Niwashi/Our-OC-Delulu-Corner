using UnityEngine;

[CreateAssetMenu(fileName = "NewCursorData", menuName = "Controller/Cursor/CursorData")]
public class CursorData : ScriptableObject
{
    public string id;
    public Texture2D image;
}
