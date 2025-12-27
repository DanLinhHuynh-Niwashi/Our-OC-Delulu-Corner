using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/AnimExpressionLogic")]


public class AnimExpressionLogic : InteractionLogic
{
    public List<TargetAnimation> targetAnims;
    public int targetExpression = 0;

    private bool eyeBlinkDisabled = false;
    public override void Execute(InteractionContext ctx)
    {
        ctx.Model.modelMotionController.OnInteraction(targetAnims, targetExpression);

        if (ctx.Model.eyeBlinkController && ctx.Model.eyeBlinkController.enabled)
        {
            ctx.Model.eyeBlinkController.enabled = false;
            eyeBlinkDisabled = true;
        }
    }

    public override void End(InteractionContext ctx)
    {
        ctx.Model.modelMotionController.ForceStopAll();
        ctx.Model.modelMotionController.PlayDefaultState();

        RestoreEyeBlink(ctx);
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        bool isPlaying = false;

        foreach (var anim in targetAnims)
        {
            if (ctx.Model.modelMotionController.IsPlayingAnimation(anim.layer))
            {
                isPlaying = true;
                break;
            }
        }

        if (!isPlaying && eyeBlinkDisabled)
        {
            RestoreEyeBlink(ctx);
        }

        return isPlaying;
    }

    private void RestoreEyeBlink(InteractionContext ctx)
    {
        if (!eyeBlinkDisabled) return;

        if (ctx.Model.eyeBlinkController)
            ctx.Model.eyeBlinkController.enabled = true;

        eyeBlinkDisabled = false;
    }
}


[System.Serializable]
public struct TargetAnimation
{
    public AnimationClip clip;
    public int layer;
    public bool loop;
}
