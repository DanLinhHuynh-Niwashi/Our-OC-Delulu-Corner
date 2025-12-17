using UnityEngine;
using System.Collections;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    public CharacterModel model;
    //public InteractionDataEventChannel interactionChannel;

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
        Debug.Log("Clicked");
        clickStartPos = clickCurrentPos;
        model.Pressed = true;
        pressTime = Time.time;

        DetectHitArea((Vector2)clickCurrentPos);
        TryDrag();
    }

    public void OnClickRelease(InputAction.CallbackContext ctx)
    {
        Debug.Log("Click Released");
        model.Pressed = false;

        if (clickCurrentPos != null && clickStartPos != null)
        {
            float dist = Vector2.Distance((Vector2)clickCurrentPos, (Vector2)clickStartPos);
            float time = Time.time - pressTime;

            if (dist <= maxClickDistance && time <= maxClickTime && clickCurrentPos != null)
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

    //private bool isDragging = false;
    //public bool IsDragging { get { return isDragging; } }
    

    //public void OnDrag(InputAction.CallbackContext ctx)
    //{
    //    currentDragDelta = ctx.ReadValue<Vector2>();

    //    if (ctx.started)
    //    {
    //        TryStartDrag(ctx);
    //    }
    //    else if (ctx.canceled)
    //    {
    //        EndDrag();
    //    }
    //}

    //void TryStartDrag(InputAction.CallbackContext ctx)
    //{
    //    Vector2 screenPos = Pointer.current.position.ReadValue();

    //    DetectHitArea(screenPos);
    //    TryDrag();
    //}


    public Vector2 GetDragDelta(float sensitivity = 1f)
    {
        return currentDragDelta * sensitivity;
    }

    //public void EndDrag()
    //{
    //    isDragging = false;
    //    currentDragDelta = Vector2.zero;
    //}

    //void ProcessInput()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        startPos = Input.mousePosition;
    //        pressTime = Time.time;
    //        model.Pressed = true;
    //    }

    //    if (Input.GetMouseButtonUp(0) && model.Pressed)
    //    {
    //        model.Pressed = false;

    //        float dist = Vector2.Distance(Input.mousePosition, startPos);
    //        float time = Time.time - pressTime;

    //        if (dist <= maxClickDistance && time <= maxClickTime)
    //            TryClick();
    //    }
    //}

    // -----------------------------
    // CLICK
    // -----------------------------
    void TryClick()
    {
        if (currentHitArea == null) return;
        if (CursorManager.Instance.IsCursorBlocked()) return;
        if (CursorManager.Instance.IgnoreNextFrame) return;
        if (model.dialogController.IsBusy()) return;

        var interaction = interactionSet.GetInteraction(currentHitArea, CursorManager.Instance.CurrentCursor);
        if (interaction != null)
            ExecuteInteraction(interaction);
    }

    void TryDrag()
    {
        if (currentHitArea == null) return;
        if (CursorManager.Instance.IsCursorBlocked()) return;
        if (CursorManager.Instance.IgnoreNextFrame) return;
        if (model.dialogController.IsBusy()) return;

        var interaction = interactionSet.GetInteraction(currentHitArea, CursorManager.Instance.CurrentCursor);
        if (interaction != null && interaction.isDrag)
            ExecuteInteraction(interaction);
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
        //DetectHitArea();
        //ProcessInput();

        if (currentInteraction == null) return;

        var ctx = new InteractionContext
        {
            Model = model,
            Controller = this,
            Data = currentInteraction
        };

        bool animEnded = currentInteraction.isEndOnAnimEnd && !currentInteraction.logic.IsBusy(ctx);
        bool dialogEnded = currentInteraction.isEndOnDialogEnd && !model.dialogController.IsBusy();

        if (animEnded || dialogEnded)
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
            currentInteraction.logic.End(new InteractionContext { Model = model, Controller = this, Data = currentInteraction});
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

    public InteractActionData CurrentInteraction { 
        get
        {
            return currentInteraction;
        }
    }
}
