using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/AnimExpressionLogic")]
public class AnimExpressionLogic : InteractionLogic
{
    public override void Execute(InteractionContext ctx)
    {
        ctx.Model.modelMotionController.OnInteraction(ctx.Data);
    }

    public override void End(InteractionContext ctx)
    {
        ctx.Model.modelMotionController.ForceStop();
        ctx.Model.modelMotionController.PlayDefault();
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        return ctx.Model.modelMotionController.IsPlayingAnimation();
    }

}
