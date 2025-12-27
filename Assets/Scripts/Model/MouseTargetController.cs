using UnityEngine;
using Live2D.Cubism.Framework.LookAt;

public class MouseTargetController : MonoBehaviour
{
    private CharacterModel model;
    private Camera cam;

    private CubismLookTargetBehaviour lookTarget;
    private float targetOffset = 0.0f;
    public void Init(CharacterModel model, float targetOffset)
    {
        this.model = model;
        cam = Camera.main;
        this.targetOffset = targetOffset;

        GameObject go = new GameObject("LookTarget");
        go.transform.SetParent(model.transform, false);

        lookTarget = go.AddComponent<CubismLookTargetBehaviour>();
        model.lookController.Target = lookTarget;
    }

    void Update()
    {
        if (lookTarget == null) return;
        if (CursorManager.Instance.IsCursorBlocked()) return;
        if (CursorManager.Instance.IgnoreNextFrame) return;

        if (model.Pressed && model.CurrentInteraction == null) {
            var world = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            world.y += targetOffset * model.cubismModel.transform.localScale.y;
            lookTarget.transform.position = world;
        }
        else
            lookTarget.transform.localPosition = Vector3.zero; } 
}
