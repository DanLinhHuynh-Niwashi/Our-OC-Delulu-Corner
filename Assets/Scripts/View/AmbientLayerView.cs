using UnityEngine;
using UnityEngine.UI;

public class AmbientLayerView : MonoBehaviour
{
    Image image;
    RectTransform rt;
    CanvasGroup canvasGroup;

    // Linear move
    Vector2 speed;
    bool loopX, loopY;
    Vector2 startPos;
    Vector2 size;

    // Orbit
    bool orbit;
    Vector2 orbitCenter;
    float orbitRadiusX;
    float orbitRadiusY;
    float orbitSpeed;
    bool orbitClockwise;
    float currentAngle; // in degrees

    bool selfRotate;
    float selfRotateSpeed;
    void Awake()
    {
        image = GetComponentInChildren<Image>();
        rt = image.rectTransform;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(AmbientLayerData data)
    {
        image.sprite = data.sprite;
        rt.localScale = data.scale;
        canvasGroup.alpha = data.alpha;

        // Linear
        speed = data.moveSpeed;
        loopX = data.loopX;
        loopY = data.loopY;
        size = rt.sizeDelta + data.sizeOffset;
        rt.sizeDelta = size;
        startPos = rt.anchoredPosition;

        // Orbit
        orbit = data.orbit;
        orbitCenter = data.orbitCenter;
        orbitRadiusX = data.orbitRadiusX;
        orbitRadiusY = data.orbitRadiusY;
        orbitSpeed = data.orbitSpeed;
        orbitClockwise = data.orbitClockwise;
        currentAngle = Random.Range(0f, 360f); // random start

        //Self Rotate
        selfRotate = data.selfRotate;
        selfRotateSpeed = data.selfRotateSpeed;
    }

    void Update()
    {
        if (orbit)
        {
            float deltaAngle = orbitSpeed * Time.deltaTime * (orbitClockwise ? -1f : 1f);
            currentAngle += deltaAngle;

            float rad = currentAngle * Mathf.Deg2Rad;

            // Elipse
            float x = orbitCenter.x + orbitRadiusX * Mathf.Cos(rad);
            float y = orbitCenter.y + orbitRadiusY * Mathf.Sin(rad);

            rt.anchoredPosition = new Vector2(x, y);
        }
        else
        {
            // Linear movement
            Vector2 pos = rt.anchoredPosition;
            pos += speed * Time.deltaTime;

            if (loopX && Mathf.Abs(pos.x - startPos.x) > size.x) pos.x = startPos.x;
            if (loopY && Mathf.Abs(pos.y - startPos.y) > size.y) pos.y = startPos.y;

            rt.anchoredPosition = pos;
        }

        // Optional self rotation
        if (selfRotate)
            rt.Rotate(Vector3.forward, selfRotateSpeed * Time.deltaTime);
    }

}
