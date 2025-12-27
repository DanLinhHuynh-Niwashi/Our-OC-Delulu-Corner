using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Mobile Cursor UI (Optional)")]
    [SerializeField] private Image cursorUI;
    [SerializeField] private RectTransform canvasTransform;

    private CursorData currentCursor;

    // ========== Block cursor ==========
    [SerializeField] private bool blockCursor = false;

    // Backing field cho IgnoreNextFrame
    [SerializeField] private bool ignoreNextFrame = false;
    [SerializeField] private bool ignoreNextFramePending = false;

    // Property
    public bool IgnoreNextFrame
    {
        get => ignoreNextFrame;
        set
        {
            ignoreNextFrame = value;
            if (value)
                ignoreNextFramePending = true; // đánh dấu để tự reset frame kế
        }
    }

    public void BlockCursor(bool block)
    {
        blockCursor = block;
        if (!block) IgnoreNextFrame = true;
    }
    public bool IsCursorBlocked() => blockCursor;
    // =================================

    private void LateUpdate()
    {
        // Tự reset IgnoreNextFrame vào frame kế
        if (ignoreNextFramePending)
        {
            ignoreNextFrame = false;
            ignoreNextFramePending = false;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (cursorUI != null)
        {
            cursorUI.preserveAspect = true;
            cursorUI.gameObject.SetActive(false); // ẩn lúc đầu
        }
    }

    private void Update()
    {
        if (cursorUI != null)
        {
            if (blockCursor || Input.touchCount == 0 || ignoreNextFrame)
            {
                cursorUI.gameObject.SetActive(false);
                return;
            }

            cursorUI.gameObject.SetActive(true);

            Vector2 touchPos = Input.GetTouch(0).position;
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasTransform,
                touchPos,
                null,
                out pos
            );
            cursorUI.rectTransform.localPosition = pos;
        }
    }

    public void SetCursor(CursorData data)
    {
        currentCursor = data;

        Cursor.SetCursor(data.image, data.hotspot, CursorMode.Auto);

        if (cursorUI != null)
        {
            cursorUI.sprite = data.buttonIcon;
        }
    }

    public CursorData CurrentCursor { get { return currentCursor; } }
}
