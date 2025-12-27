using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/AnimExpressionLogic")]


public class AnimExpressionLogic : InteractionLogic
{
    public List<TargetAnimation> targetAnims;
    public int targetExpression = 0;
    public override void Execute(InteractionContext ctx)
    {
        ctx.Model.modelMotionController.OnInteraction(targetAnims, targetExpression);
    }

    public override void End(InteractionContext ctx)
    {
        ctx.Model.modelMotionController.ForceStopAll();
        ctx.Model.modelMotionController.PlayDefaultState();
    }

    public override bool IsBusy(InteractionContext ctx)
    {
        foreach (var anim in targetAnims)
        {
            if (ctx.Model.modelMotionController.IsPlayingAnimation(anim.layer))
                return true;
        }
        return false;
    }

}

[System.Serializable]
public struct TargetAnimation
{
    public AnimationClip clip;
    public int layer;
    public bool loop;
}
