using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/Dialogue")]
public class DialogueInteractionLogic : InteractionLogic
{
    public override void Execute(InteractionContext ctx)
    {
        ctx.Model.dialogController.OnInteraction(ctx.Data);
    }

    public override void End(InteractionContext ctx)
    {
        ctx.Model.dialogController.HaltDialog();
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        return ctx.Model.dialogController.IsBusy();
    }
}
