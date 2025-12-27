using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Interaction/Logic/Drag")]
public class DragInteractionLogic : InteractionLogic
{
    private bool isBusy = false;
    public List<DragParamConfig> dragParams;
    public override void Execute(InteractionContext ctx)
    {
        if (isBusy) return;
        ctx.Controller.StartInteractionCoroutine(DragRoutine(ctx));
    }

    private IEnumerator DragRoutine(InteractionContext ctx)
    {
        isBusy = true;

        ctx.Controller.GetDragDelta(); // reset buffer

        while (ctx.Model.Pressed)
        {
            Vector2 dragDelta = ctx.Controller.GetDragDelta();

            foreach (var param in dragParams)
            {
                float axisDelta = param.axis switch
                {
                    DragAxis.X => dragDelta.x,
                    DragAxis.Y => dragDelta.y,
                    DragAxis.XY => (dragDelta.x + dragDelta.y) * 0.5f,
                    _ => 0f
                };

                float delta = axisDelta * param.sensitivity;
                ctx.Model.modelParamController.MoveParameter(param.paramName, delta);
            }

            yield return null;
        }

        foreach (var param in dragParams)
        {
            ctx.Model.modelParamController.ResetParameter(
                param.paramName,
                param.fadeOutTime
            );
        }

        float maxFade = 0f;
        foreach (var p in dragParams)
            maxFade = Mathf.Max(maxFade, p.fadeOutTime);

        yield return new WaitForSeconds(maxFade);
        isBusy = false;
    }

    public override void End(InteractionContext ctx)
    {
        Debug.Log("[DragInteraction] End");
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        return isBusy;
    }
}

[System.Serializable]
public class DragParamConfig
{
    public string paramName;
    public float sensitivity = 0.005f;
    public float fadeOutTime = 0.5f;
    public DragAxis axis = DragAxis.X;
}
public enum DragAxis
{
    X,
    Y,
    XY
}
