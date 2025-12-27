using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/SequenceLogic")]
public class SequenceInteractionLogic : InteractionLogic
{
    [Tooltip("Logic will play in turn")]
    public InteractionLogic[] sequence;

    // Track state
    private int currentIndex = -1;
    private bool isRunning = false;

    public override void Execute(InteractionContext ctx)
    {
        currentIndex = 0;
        isRunning = true;
        ctx.Controller.StartInteractionCoroutine(RunSequence(ctx));
    }

    private IEnumerator RunSequence(InteractionContext ctx)
    {
        while (currentIndex < sequence.Length)
        {
            var logic = sequence[currentIndex];
            if (logic == null)
            {
                currentIndex++;
                continue;
            }

            Debug.Log("[Sequence] EXECUTE " + logic.name);
            logic.Execute(ctx);

            while (logic.IsBusy(ctx))
                yield return null;

            currentIndex++;
            yield return null;
        }

        isRunning = false;
        currentIndex = -1;
    }

    public override void End(InteractionContext ctx)
    {
        foreach (var logic in sequence)
            logic?.End(ctx);

        isRunning = false;
        currentIndex = -1;
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        if (!isRunning)
            return false;

        if (currentIndex >= 0 && currentIndex < sequence.Length)
        {
            var logic = sequence[currentIndex];
            if (logic != null && logic.IsBusy(ctx))
                return true;
        }

        return isRunning;
    }
}
