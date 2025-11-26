using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Rendering;

public class CharacterModel : MonoBehaviour
{
    [Header("Live2D")]
    public CubismMotionController motionController;
    public CubismExpressionController expressionController;
    public CubismLookController lookController;
    public CubismRaycaster cubismRaycaster;
    public CubismModel cubismModel;

    [Header("Custom Controllers")]
    public SkinController skinController;
    public InteractionController interactionController;
    public MouseTargetController mouseTargetController;

    [Header("Default")]
    public AnimationClip defaultMotion;
    public int defaultExpression = 0;
    public float screenFill = 0.75f;
    public float topScreenMarginPercent = 0.25f;

    private float modelHeight;
    private float modelMinY;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        ModelData currentModel = GameManager.Instance.GetCurrentModel();
        if (currentModel != null)
            SetUpFromModelData(currentModel);

        PlayDefault();
    }

    void SetUpFromModelData(ModelData modelData)
    {
        topScreenMarginPercent = modelData.topScreenMarginPercent;
        screenFill = modelData.modelScreenFill;
        defaultExpression = modelData.starterExpression;
        defaultMotion = modelData.starterMotion;

        SetCursor(modelData.starterCursor);
        SetModel(modelData.modelPrefab);

        interactionController.Init(this, modelData.interactionSet);
        skinController.Init(this);
        mouseTargetController.Init(this);
    }

    public void SetCursor(CursorData data)
    {
        CursorManager.Instance.SetCursor(data);
    }

    public void SetModel(CubismModel newModel)
    {
        if (newModel == null)
        {
            Debug.LogWarning("SetModel: newModel is null!");
            return;
        }

        if (cubismModel != null)
            Destroy(cubismModel.gameObject);

        cubismModel = Instantiate(newModel, transform);

        LoadModel();
        ComputeModelBounds();
        PositionAndScale();
    }

    public void LoadModel()
    {
        if (cubismModel == null) return;

        motionController = cubismModel.GetComponent<CubismMotionController>();
        expressionController = cubismModel.GetComponent<CubismExpressionController>();
        lookController = cubismModel.GetComponent<CubismLookController>();
        cubismRaycaster = cubismModel.GetComponent<CubismRaycaster>();
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
            if (!mesh_renderer.enabled)
                continue;

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

        float scale = (camHeight * screenFill) / modelHeight;

        Debug.Log("[SETTING MODEL] Cam height " + camHeight);
        Debug.Log("[SETTING MODEL] Model height " + modelHeight);

        cubismModel.transform.localScale = Vector3.one * scale;

        float modelTopY = (modelHeight + modelMinY) * scale;
        float offsetY = camHeight / 2f - camHeight * topScreenMarginPercent - modelTopY;

        transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y + offsetY,
            transform.position.z
        );
    }

    public void PlayDefault()
    {
        if (defaultMotion != null)
            motionController.PlayAnimation(defaultMotion, isLoop: true);

        if (defaultExpression >= 0)
            expressionController.CurrentExpressionIndex = defaultExpression;
    }

    public bool Pressed { get; set; }
    public InteractActionData CurrentInteraction { get { return interactionController.CurrentInteraction; } }
}
