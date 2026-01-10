using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework;

public class CharacterModel : MonoBehaviour
{
    [Header("Live2D")]
    public CubismMotionController motionController;
    public CubismExpressionController expressionController;
    public CubismLookController lookController;
    public CubismRaycaster cubismRaycaster;
    public CubismEyeBlinkController eyeBlinkController;
    public CubismModel cubismModel;

    [Header("Custom Controllers")]
    public SkinController skinController;
    public InteractionController interactionController;
    public MouseTargetController mouseTargetController;
    public DialogController dialogController;
    public ModelMotionController modelMotionController;
    public ModelParamController modelParamController;

    [Header("Default")]
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

        modelMotionController.PlayDefaultState();
    }

    void SetUpFromModelData(ModelData modelData)
    {
        topScreenMarginPercent = modelData.topScreenMarginPercent;
        screenFill = modelData.modelScreenFill;

        SetCursor(modelData.starterCursor);
        SetModel(modelData.modelPrefab);

        interactionController.Init(this, modelData.interactionSet);
        skinController.Init(this);
        mouseTargetController.Init(this, -modelHeight/2);
        dialogController.Init(this);
        modelParamController.Init(this);
        modelMotionController.Init(this, modelData.starterExpression, modelData.starterMotion);
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
        cubismModel.gameObject.SetActive(true);
        LoadModel();
        ComputeModelBounds();
        PositionAndScale();
    }

    public void LoadModel()
    {
        if (cubismModel == null) return;

        motionController = cubismModel.GetComponent<CubismMotionController>();
        motionController.LayerCount = 3;
        eyeBlinkController = cubismModel.GetComponent<CubismEyeBlinkController>();
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

        float portraitRatio = (float)Screen.height / Screen.width;
        float ratio01 = Mathf.InverseLerp(1.7f, 2.3f, portraitRatio);

        // Scale
        float baseScale = (camHeight * screenFill) / modelHeight;
        float scaleFactor = Mathf.Lerp(1.05f, 0.9f, ratio01);
        float scale = baseScale * scaleFactor;

        cubismModel.transform.localScale = Vector3.one * scale;

        // Position
        float modelTopY = (modelHeight + modelMinY) * scale;

        float topMargin = Mathf.Lerp(
            topScreenMarginPercent * 0.6f,
            topScreenMarginPercent * 1.3f,
            ratio01
        );

        float offsetY = camHeight / 2f
                      - camHeight * topMargin
                      - modelTopY;

        transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y + offsetY,
            transform.position.z
        );
    }


    public InteractActionData CurrentInteraction
    {
        get { return interactionController.CurrentInteraction; }
    }
    public bool Pressed { get; set; }
}
