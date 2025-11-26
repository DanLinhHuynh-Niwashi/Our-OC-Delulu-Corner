using UnityEngine;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Motion;

public class InteractionController : MonoBehaviour
{
    private CharacterModel model;

    private InteractListById interactionSet;
    private InteractActionData currentInteraction;

    private bool justRemoved = false;
    private Vector2 startPos;
    private float pressTime;

    private float interactionTimer = -1f;
    private string currentHitArea = null;

    public System.Action OnInteractionEnd;

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

        var interaction = interactionSet.GetInteraction(currentHitArea, CursorManager.Instance.CurrentCursor);
        if (interaction != null)
            DoInteraction(interaction);
    }

    // -----------------------------
    // INTERACTION
    // -----------------------------
    public void DoInteraction(InteractActionData data)
    {
        if (justRemoved)
        {
            justRemoved = false;
            return;
        }

        if (currentInteraction != null)
            RemoveInteraction(true);

        currentInteraction = data;

        if (data.targetAnim != null)
            model.motionController.PlayAnimation(data.targetAnim, priority: CubismMotionPriority.PriorityForce, isLoop: false);

        if (data.targetExpression >= 0)
            model.expressionController.CurrentExpressionIndex = data.targetExpression;

        if (data.duration > 0)
            interactionTimer = data.duration;
    }

    // -----------------------------
    // UPDATE TIMER & LOOP
    // -----------------------------
    void UpdateInteractionTimer()
    {
        if (currentInteraction != null)
        {
            if (currentInteraction.isEndOnAnimEnd)
            {
                if (!model.motionController.IsPlayingAnimation())
                    RemoveInteraction();
            }
            else if (currentInteraction.animLoop)
            {
                if (!model.motionController.IsPlayingAnimation())
                    OnLoopAnimationEnded();
            }
        }

        if (interactionTimer > 0)
        {
            interactionTimer -= Time.deltaTime;
            if (interactionTimer <= 0)
                RemoveInteraction();
        }
    }

    void OnLoopAnimationEnded()
    {
        if (currentInteraction != null && !model.motionController.IsPlayingAnimation())
            model.motionController.PlayAnimation(currentInteraction.targetAnim, isLoop: false);
    }

    public void RemoveInteraction(bool isFromInteraction = false)
    {
        if (currentInteraction != null && currentInteraction.isEndOnDialogEnd)
            justRemoved = true;

        currentInteraction = null;
        interactionTimer = -1f;

        if (!isFromInteraction)
            model.PlayDefault();
    }

    public InteractActionData CurrentInteraction { get { return currentInteraction; } }
}
