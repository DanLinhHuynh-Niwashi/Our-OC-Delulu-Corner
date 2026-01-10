using UnityEngine;

[CreateAssetMenu(fileName = "NewBackgroundData", menuName = "Controller/Background/BackgroundData")]
public class BackgroundData : ScriptableObject
{
    public Sprite buttonIcon;
    public Sprite buttonSelect;
    public Sprite buttonDeselect;
    public Sprite selectOverlay;
    public Vector3 overlayScale = Vector3.one;

    [Header("Base")]
    public Sprite backSprite;
    public Sprite foregroundSprite;

    [Header("Foreground Layout")]
    public Vector2 foregroundOffset;
    public Vector2 foregroundScale = Vector2.one;

    [Header("Ambient Layers")]
    public AmbientLayerData[] ambientLayers;
}
