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
    public ForegroundLayoutData foregroundLayoutData;

    [Header("Ambient Layers")]
    public AmbientLayerData[] ambientLayers;
}

[System.Serializable]
public class ForegroundLayoutData
{
    [Header("Anchor")]
    public Vector2 anchorMin = new Vector2(0.5f, 0.5f);
    public Vector2 anchorMax = new Vector2(0.5f, 0.5f);

    [Header("Pivot")]
    public Vector2 pivot = new Vector2(0.5f, 0.5f);

    [Header("Transform")]
    public Vector2 anchoredPosition;
    public Vector2 scale = Vector2.one;
}
