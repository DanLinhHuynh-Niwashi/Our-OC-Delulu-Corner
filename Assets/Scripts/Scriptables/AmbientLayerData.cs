using UnityEngine;

[System.Serializable]
public class AmbientLayerData
{
    public Sprite sprite;

    [Header("Linear Movement")]
    public Vector2 moveSpeed;
    public bool loopX;
    public bool loopY;

    [Header("Orbit / Ellipse")]
    public bool orbit;             // bật quay
    public Vector2 orbitCenter;    // tâm quay
    public float orbitRadiusX = 50f;  // bán trục X
    public float orbitRadiusY = 50f;  // bán trục Y
    public float orbitSpeed = 30f;     // độ / giây
    public bool orbitClockwise = true;

    [Header("Visual")]
    [Range(0f, 1f)] public float alpha = 1f;
    public Vector2 scale = Vector2.one;
    public Vector2 sizeOffset;

    [Header("Optional Self Rotation")]
    public bool selfRotate;
    public float selfRotateSpeed;
}

