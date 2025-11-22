using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Rendering;

public class CharacterModel : MonoBehaviour
{
    [Header("Live2D")]
    public CubismMotionController motionController;
    public CubismExpressionController expressionController;
    public CubismLookController lookController;
    public CubismRaycaster cubismRaycaster;
    public CubismModel cubismModel;

    [Header("Interactions")]
    public InteractListById interactionSet;
    public CursorData currentCursor;
    private InteractActionData currentInteraction;
    private bool justRemoved = false;

    [Header("Default")]
    public AnimationClip defaultMotion;
    public int defaultExpression = 0;
    public float screenFill = 0.75f;
    public float topScreenMarginPercent = 0.25f;
    private float interactionTimer = -1f;
    private string currentHitArea = null;
    private bool pressed = false;

    [Header("Click Settings")]
    public float maxClickDistance = 10f;
    public float maxClickTime = 0.3f;

    private Vector2 startPos;
    private float pressTime;


    // LookAt
    private CubismLookTargetBehaviour mouseLookTarget;

    private float modelHeight; 
    private float modelMinY;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;

        ModelData currentModel = GameManager.Instance.GetCurrentModel();
        if (GameManager.Instance.GetCurrentModel() != null)
            SetUpFromModelData(currentModel);
        PlayDefault();
    }

    void Update()
    {
        UpdateInteractionTimer();
        UpdateLookTarget();
        DetectHitArea();
        ProcessInput();
        
    }

    void ComputeModelBounds()
    {
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var d in cubismModel.Drawables)
        {
            var renderer = d.GetComponent<CubismRenderer>();
            var mesh_renderer = d.GetComponent<MeshRenderer>();
            Bounds b = renderer.Mesh.bounds;
            if (!mesh_renderer.enabled) continue;

            minY = Mathf.Min(minY, b.min.y);
            maxY = Mathf.Max(maxY, b.max.y);
        }
        modelMinY = minY;
        modelHeight = maxY - minY;


    }

    void PositionAndScale()
    {
        if (cam == null) cam = Camera.main;
        float camHeight = cam.orthographicSize * 2f;
        Debug.Log("Cam height: " + camHeight);
        Debug.Log("Model height: " + modelHeight);

        float scale = (camHeight * screenFill) / modelHeight;
        cubismModel.transform.localScale = Vector3.one * scale;

        float modelTopY = (modelHeight + modelMinY) * scale;
        float offsetY = camHeight / 2f - camHeight * topScreenMarginPercent - modelTopY;

        Vector3 camPos = cam.transform.position;
        transform.position = new Vector3(camPos.x, camPos.y + offsetY, transform.position.z);
    }

    public void LoadModel()
    {
        if (cubismModel == null) return;

        motionController = cubismModel.GetComponent<CubismMotionController>();
        expressionController = cubismModel.GetComponent<CubismExpressionController>();
        lookController = cubismModel.GetComponent<CubismLookController>();
        cubismRaycaster = cubismModel.GetComponent<CubismRaycaster>();
    }
    private void SetUpFromModelData(ModelData modelData)
    {
        topScreenMarginPercent = modelData.topScreenMarginPercent;
        screenFill = modelData.modelScreenFill;
        defaultExpression = modelData.starterExpression;
        defaultMotion = modelData.starterMotion;
        interactionSet = modelData.interactionSet;
        currentCursor = modelData.starterCursor;

        SetModel(modelData.modelPrefab);
    }

    public void SetModel(CubismModel newModel)
    {
        if (newModel == null)
        {
            Debug.LogWarning("SetModel: newModel is null!");
            return;
        }


        if (cubismModel != null)
        {
            Destroy(cubismModel.gameObject);
        }

        cubismModel = Instantiate(newModel, transform);


        LoadModel();
        ComputeModelBounds();
        PositionAndScale();

        if (mouseLookTarget == null)
        {
            GameObject go = new GameObject("MouseLookTarget");
            go.transform.SetParent(transform, false);
            mouseLookTarget = go.AddComponent<CubismLookTargetBehaviour>();
        }
        lookController.Target = mouseLookTarget;

        Debug.Log("Model set and initialized: " + cubismModel.name);


    }

    public void RefreshPositionAndScale()
    {
        PositionAndScale();
    }

    void DetectHitArea()
    {
        var results = new CubismRaycastHit[4];
        // Cast ray from pointer position.
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitCount = cubismRaycaster.Raycast(ray, results);

        for (var i = 0; i < hitCount; i++)
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

    // ============================================================
    // INTERACTION SYSTEM
    // ============================================================
    void TryClick()
    {
        if (currentHitArea == null)
        {
            Debug.Log("No hit area");
            return;
        }

        var interaction = interactionSet.GetInteraction(currentHitArea, currentCursor);
        if (interaction != null)
            DoInteraction(interaction);
    }

    void ProcessInput()
    {
        // Mouse
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            pressTime = Time.time;
            pressed = true;
        }

        if (Input.GetMouseButtonUp(0) && pressed)
        {
            pressed = false;
            float dist = Vector2.Distance(Input.mousePosition, startPos);
            float duration = Time.time - pressTime;

            if (dist <= maxClickDistance && duration <= maxClickTime)
                TryClick();
        }

        // Touch
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                startPos = t.position;
                pressTime = Time.time;
                pressed = true;
            }
            else if (t.phase == TouchPhase.Ended && pressed)
            {
                pressed = false;
                float dist = Vector2.Distance(t.position, startPos);
                float duration = Time.time - pressTime;

                if (dist <= maxClickDistance && duration <= maxClickTime)
                    TryClick();
            }
        }
    }

    private void OnLoopAnimationEnded()
    {
        if (currentInteraction != null && !motionController.IsPlayingAnimation())
            motionController.PlayAnimation(currentInteraction.targetAnim, isLoop: false);
    }

    public void DoInteraction(InteractActionData data)
    {
        if (justRemoved)
        {
            justRemoved = false;
            return;
        }

        if (currentInteraction != null)
            RemoveInteraction();

        currentInteraction = data;

        if (data.targetAnim != null)
        {
            motionController.PlayAnimation(data.targetAnim, priority: CubismMotionPriority.PriorityForce, isLoop: false);
        }
        if (data.targetExpression >= 0)
            expressionController.CurrentExpressionIndex = data.targetExpression;

        if (data.duration > 0)
            interactionTimer = data.duration;
    }
    
    void UpdateInteractionTimer()
    {
        if (currentInteraction != null)
        {
            if (currentInteraction.isEndOnAnimEnd)
            {
                if (!motionController.IsPlayingAnimation())
                    RemoveInteraction();
            }
            else if (currentInteraction.animLoop)
            {
                if (!motionController.IsPlayingAnimation())
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

    public void RemoveInteraction()
    {
        if (currentInteraction.isEndOnDialogEnd)
            justRemoved = true;

        currentInteraction = null;
        interactionTimer = -1f;
        

        PlayDefault();
    }

    void PlayDefault()
    {
        if (defaultMotion != null)
            motionController.PlayAnimation(defaultMotion, isLoop: true);

        if (defaultExpression >= 0)
            expressionController.CurrentExpressionIndex = defaultExpression;
    }

    // ============================================================
    // LOOK AT
    // ============================================================
    void UpdateLookTarget()
    {
        if (mouseLookTarget == null) return;

        if (pressed && currentInteraction == null)
        {
            var world = cam.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, 1f));

            mouseLookTarget.transform.position = world;
        }
        else
        {
            mouseLookTarget.transform.localPosition = Vector3.zero;
        }
    }
}
