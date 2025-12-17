using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;

[CreateAssetMenu(menuName = "Interaction/Logic/Drag")]
public class DragInteractionLogic : InteractionLogic
{
    public override void Execute(InteractionContext ctx)
    {
        ctx.Controller.StartInteractionCoroutine(DragRoutine(ctx));
    }

    private IEnumerator DragRoutine(InteractionContext ctx)
    {
        while (ctx.Model.Pressed)
        {
            float delta = ctx.Controller.GetDragDelta().x;
            ctx.Model.modelParamController.MoveParameter(ctx.Data.targetParamName, delta);
            yield return null;
        }

        ctx.Model.modelParamController.ResetParameter(ctx.Data.targetParamName);
    }


    public override void End(InteractionContext ctx)
    {
        Debug.Log($"[DragStep] End Drag {ctx.Data.targetParamName}");
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        return Input.GetMouseButton(0);
    }
}
