using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/SequenceLogic")]
public class SequenceInteractionLogic : InteractionLogic
{
    [Tooltip("Các logic sẽ chạy theo thứ tự")]
    public InteractionLogic[] sequence;

    public override void Execute(InteractionContext ctx)
    {
        ctx.Controller.StartInteractionCoroutine(RunSequence(ctx));
    }

    private IEnumerator RunSequence(InteractionContext ctx)
    {
        foreach (var logic in sequence)
        {
            if (logic == null) continue;

            Debug.Log("EXECUTING " + logic);
            logic.Execute(ctx);

            while (logic.IsBusy(ctx))
                yield return null;
        }
    }

    public override void End(InteractionContext ctx)
    {
        foreach (var logic in sequence)
            logic?.End(ctx);
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        foreach (var logic in sequence)
            if (logic != null && logic.IsBusy(ctx))
                return true;
        return false;
    }
}
