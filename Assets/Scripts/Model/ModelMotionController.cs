using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.Motion;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public class ModelMotionController : MonoBehaviour
{
    public CharacterModel cubismModel;
    //public EventChannel motionEndChannel;

    private AnimationClip currentAnim;
    private int defaultExpression;
    private AnimationClip defaultMotion;

    private bool isLoop = false;
    public void Init(CharacterModel cubismModel, int defaultExpression, AnimationClip defaultMotion)
    {
        this.cubismModel = cubismModel;
        this.defaultExpression = defaultExpression;
        this.defaultMotion = defaultMotion;
    }

    private void Update()
    {
        if (!cubismModel.motionController.IsPlayingAnimation())
        {
            if (!isLoop)
            {
                currentAnim = null;
                //motionEndChannel.RaiseEvent();
            }
            else OnLoopAnimationEnded();
        }
    }
    public void OnInteraction(InteractActionData data)
    {
        if (data.targetAnim != null)
            PlayMotion(data.targetAnim, data.animLoop);

        if (data.targetExpression >= 0)
            ChangeExpression(data.targetExpression);
        else
            ChangeExpression(defaultExpression);

    }

    void OnLoopAnimationEnded()
    {
        if (currentAnim != null && !cubismModel.motionController.IsPlayingAnimation())
            cubismModel.motionController.PlayAnimation(currentAnim, isLoop: false);
    }

    public bool IsPlayingAnimation()
    {
        return (currentAnim != null && cubismModel.motionController.IsPlayingAnimation());
    }    

    public void PlayMotion(AnimationClip clip, bool isLoop)
    {
        cubismModel.motionController.PlayAnimation(clip, priority: CubismMotionPriority.PriorityForce, isLoop: false);
        currentAnim = clip;
        this.isLoop = isLoop;
    }
    
    public void ForceStop()
    {
        isLoop = false;
        currentAnim = null;
    }

    public void ChangeExpression(int expression)
    {
        cubismModel.expressionController.CurrentExpressionIndex = expression;
    }
    public void PlayDefault()
    {
        if (defaultMotion == null) return;
        if (defaultExpression < 0) defaultExpression = 0;
        
        
        cubismModel.motionController.PlayAnimation(defaultMotion, isLoop: true);
        cubismModel.expressionController.CurrentExpressionIndex = defaultExpression;

        currentAnim = defaultMotion;
        isLoop = true;
    }


}
