using UnityEngine;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Motion;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    public InteractionDataEventChannel interactionChannel;

    private CharacterModel model;

    private InteractListById interactionSet;
    private InteractActionData currentInteraction;

    private Vector2 startPos;
    private float pressTime;

    private float interactionTimer = -1f;
    private string currentHitArea = null;

    public float maxClickDistance = 10f;
    public float maxClickTime = 0.3f;

    public void Init(CharacterModel model, InteractListById set)
    {
        this.model = model;
        interactionSet = set;
    }

    void Update()
    {
        DetectHitArea();
        ProcessInput();
        UpdateInteractionTimer();
    }

    // -----------------------------
    // HIT AREA
    // -----------------------------
    void DetectHitArea()
    {
        var results = new CubismRaycastHit[4];
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int hitCount = model.cubismRaycaster.Raycast(ray, results);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = results[i].Drawable.GetComponent<CubismHitDrawable>();
            if (hit != null)
            {
                currentHitArea = results[i].Drawable.name;
                return;
            }
        }

        currentHitArea = null;
    }

    // -----------------------------
    // INPUT
    // -----------------------------
    void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            pressTime = Time.time;
            model.Pressed = true;
        }

        if (Input.GetMouseButtonUp(0) && model.Pressed)
        {
            model.Pressed = false;

            float dist = Vector2.Distance(Input.mousePosition, startPos);
            float time = Time.time - pressTime;

            if (dist <= maxClickDistance && time <= maxClickTime)
                TryClick();
        }
    }

    // -----------------------------
    // CLICK
    // -----------------------------
    void TryClick()
    {
        if (currentHitArea == null) return;
        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return;
        }
        var interaction = interactionSet.GetInteraction(currentHitArea, CursorManager.Instance.CurrentCursor);
        if (interaction != null)
            DoInteraction(interaction);
    }

    // -----------------------------
    // INTERACTION
    // -----------------------------
    public void DoInteraction(InteractActionData data)
    {
        if (currentInteraction != null)
        {
            if (currentInteraction == data) return;
            RemoveInteraction(true);
        }

        currentInteraction = data;

        interactionChannel.RaiseEvent(currentInteraction);

        if (data.duration > 0)
            interactionTimer = data.duration;
    }

    // -----------------------------
    // UPDATE TIMER & LOOP
    // -----------------------------

    public void OnMotionEnd()
    {
        if (currentInteraction == null) return;
        if (currentInteraction.isEndOnAnimEnd)
            RemoveInteraction();
    }
    void UpdateInteractionTimer()
    {

        if (interactionTimer > 0)
        {
            interactionTimer -= Time.deltaTime;
            if (interactionTimer <= 0)
                RemoveInteraction();
        }
    }

    public void RemoveInteraction(bool isFromInteraction = false)
    {
        currentInteraction = null;
        interactionTimer = -1f;
        model.HaltDialog();
        if (!isFromInteraction)
            model.PlayDefault();


    }

    public InteractActionData CurrentInteraction { get { return currentInteraction; } }
}
