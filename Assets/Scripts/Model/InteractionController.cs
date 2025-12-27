using UnityEngine;
using System.Collections;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    public CharacterModel model;
    //public InteractionDataEventChannel interactionChannel;
    [SerializeField]
    private InteractActionData currentInteraction;

    private string currentHitArea = null;
    private Vector2? clickStartPos = null;
    private Vector2? clickCurrentPos = null;
    private Vector2 currentDragDelta = Vector2.zero;

    private float pressTime = -1;

    public float maxClickDistance = 10f;
    public float maxClickTime = 0.3f;

    private InteractListById interactionSet;

    public InputActionReference clickAction;
    public InputActionReference positionAction;
    public InputActionReference deltaAction;

    [Header("Coyote Time")]
    public float interactionCoyoteTime = 0.2f;
    private InteractActionData pendingInteraction;
    private float pendingExpireTime;

    private void OnEnable()
    {
        clickAction.action.Enable();
        positionAction.action.Enable();
        deltaAction.action.Enable();
        clickAction.action.started += OnClick;
        clickAction.action.canceled += OnClickRelease;
        positionAction.action.performed += OnPosition;
        deltaAction.action.performed += OnDelta;
    }

    private void OnDisable()
    {
        clickAction.action.Disable();
        positionAction.action.Disable();
        deltaAction.action.Disable();
        clickAction.action.started -= OnClick;
        clickAction.action.canceled -= OnClickRelease;
        positionAction.action.performed -= OnPosition;
        deltaAction.action.performed -= OnDelta;
    }

    // -----------------------------
    // INPUT
    // -----------------------------

    public void OnClick(InputAction.CallbackContext ctx)
    {
        clickStartPos = clickCurrentPos;
        model.Pressed = true;
        pressTime = Time.time;

        DetectHitArea((Vector2)clickCurrentPos);
        TryDrag();
    }

    public void OnClickRelease(InputAction.CallbackContext ctx)
    {
        model.Pressed = false;

        if (clickCurrentPos != null && clickStartPos != null)
        {
            float dist = Vector2.Distance((Vector2)clickCurrentPos, (Vector2)clickStartPos);
            float time = Time.time - pressTime;

            if (dist <= maxClickDistance && time <= maxClickTime)
            {
                DetectHitArea((Vector2)clickCurrentPos);
                TryClick();
            }
        }
    }

    public void OnPosition(InputAction.CallbackContext ctx)
    {
        clickCurrentPos = ctx.ReadValue<Vector2>();
    }

    public void OnDelta(InputAction.CallbackContext ctx)
    {
        currentDragDelta = ctx.ReadValue<Vector2>();
    }

    public Vector2 GetDragDelta(float sensitivity = 1f)
    {
        Vector2 delta = currentDragDelta;
        currentDragDelta = new Vector2(0, 0);
        return delta * sensitivity;
    }

    // -----------------------------
    // CLICK / DRAG
    // -----------------------------
    void TryClick()
    {
        if (currentHitArea == null) return;

        var interaction = interactionSet.GetInteraction(currentHitArea, CursorManager.Instance.CurrentCursor);
        if (interaction == null) return;

        if (CanExecuteInteraction())
        {
            ExecuteInteraction(interaction);
        }
        else if (!IsDialogInteraction())
        {
            BufferInteraction(interaction);
        }
    }

    void TryDrag()
    {
        if (currentHitArea == null) return;

        var interaction = interactionSet.GetInteraction(currentHitArea, CursorManager.Instance.CurrentCursor);
        if (interaction == null || !interaction.isDrag) return;

        if (CanExecuteInteraction())
        {
            ExecuteInteraction(interaction);
        }
        else if (!IsDialogInteraction())
        {
            BufferInteraction(interaction);
        }
    }

    private bool IsDialogInteraction()
    {
        return currentInteraction != null && currentInteraction.isEndOnDialogEnd;
    }
    private bool CanExecuteInteraction()
    {
        if (CursorManager.Instance.IsCursorBlocked()) return false;
        if (CursorManager.Instance.IgnoreNextFrame) return false;
        if (model.dialogController.IsBusy()) return false;
        if (currentInteraction != null) return false;
        return true;
    }

    private void BufferInteraction(InteractActionData interaction)
    {
        pendingInteraction = interaction;
        pendingExpireTime = Time.time + interactionCoyoteTime;
    }

    private void TryConsumeBufferedInteraction()
    {
        if (pendingInteraction == null) return;

        if (Time.time > pendingExpireTime)
        {
            pendingInteraction = null;
            return;
        }

        if (!CanExecuteInteraction()) return;

        ExecuteInteraction(pendingInteraction);
        pendingInteraction = null;
    }

    // -----------------------------
    // INIT
    // -----------------------------
    public void Init(CharacterModel model, InteractListById set)
    {
        this.model = model;
        this.interactionSet = set;
    }

    void Update()
    {
        // xử lý interaction buffer trước
        TryConsumeBufferedInteraction();

        if (currentInteraction == null) return;

        var ctx = new InteractionContext
        {
            Model = model,
            Controller = this,
            Data = currentInteraction
        };

        bool interactionEnded = !currentInteraction.logic.IsBusy(ctx);
        //bool animEnded = currentInteraction.isEndOnAnimEnd && interactionEnded;
        //bool dialogEnded = currentInteraction.isEndOnDialogEnd && interactionEnded;

        if (interactionEnded)
        {
            EndInteraction();
        }
    }

    // -----------------------------
    // HIT AREA
    // -----------------------------
    private void DetectHitArea(Vector2 screenPos)
    {
        if (model == null) return;

        var results = new CubismRaycastHit[4];
        var ray = Camera.main.ScreenPointToRay(screenPos);
        int hitCount = model.cubismRaycaster.Raycast(ray, results);

        currentHitArea = null;
        for (int i = 0; i < hitCount; i++)
        {
            var hit = results[i].Drawable.GetComponent<CubismHitDrawable>();
            if (hit != null)
            {
                currentHitArea = results[i].Drawable.name;
                return;
            }
        }
    }

    // -----------------------------
    // EXECUTE INTERACTION
    // -----------------------------
    public void ExecuteInteraction(InteractActionData data)
    {
        if (currentInteraction != null)
            EndInteraction();

        currentInteraction = data;
        //interactionChannel?.RaiseEvent(currentInteraction);

        var ctx = new InteractionContext { Model = model, Controller = this, Data = currentInteraction };
        data.logic?.Execute(ctx);
    }

    public void EndInteraction()
    {
        if (currentInteraction?.logic != null)
        {
            currentInteraction.logic.End(new InteractionContext { Model = model, Controller = this, Data = currentInteraction });
        }

        currentInteraction = null;
    }

    // -----------------------------
    // UTILITY
    // -----------------------------
    public Coroutine StartInteractionCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public InteractActionData CurrentInteraction
    {
        get { return currentInteraction; }
    }
}
