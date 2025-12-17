using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/CompositeLogic")]
public class CompositeInteractionLogic : InteractionLogic
{
    public InteractionLogic[] logics;

    public override void Execute(InteractionContext ctx)
    {
        foreach (var logic in logics)
            logic?.Execute(ctx);
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        foreach (var logic in logics)
            if (logic != null && logic.IsBusy(ctx))
                return true;
        return false;
    }

    public override void End(InteractionContext ctx)
    {
        foreach (var logic in logics)
            logic?.End(ctx);
    }
}
