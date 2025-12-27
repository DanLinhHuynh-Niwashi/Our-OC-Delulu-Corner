using System.Collections.Generic;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.Motion;
using UnityEngine;

public class ModelMotionController : MonoBehaviour
{
    [Header("Model References")]
    public CharacterModel cubismModel;

    [Header("Default Settings")]
    public int defaultExpression = 0;
    public AnimationClip defaultMotion;
    public bool defaultMotionLoop = true;

    private class MotionLayer
    {
        public AnimationClip currentClip;
        public bool isLoop;
        public int priority;
    }

    private Dictionary<int, MotionLayer> motionLayers = new Dictionary<int, MotionLayer>();

    public void Init(CharacterModel cubismModel, int defaultExpression, AnimationClip defaultMotion) {
        this.cubismModel = cubismModel; 
        this.defaultExpression = defaultExpression; 
        this.defaultMotion = defaultMotion; 
    }
    private void Update()
    {
        foreach (var kvp in motionLayers)
        {
            int layer = kvp.Key;
            MotionLayer motion = kvp.Value;

            if (motion.currentClip != null && !cubismModel.motionController.IsPlayingAnimation(layer))
            {
                if (motion.isLoop)
                {
                    // Lặp lại animation
                    cubismModel.motionController.PlayAnimation(motion.currentClip, layer, motion.priority, isLoop: false);
                }
                else
                {
                    motion.currentClip = null;
                }
            }
        }
    }

    public void PlayMotion(AnimationClip clip, int layer = 0, bool isLoop = false, int priority = CubismMotionPriority.PriorityForce)
    {
        if (clip == null || cubismModel == null) return;

        cubismModel.motionController.PlayAnimation(clip, layer, priority, isLoop: false);

        motionLayers[layer] = new MotionLayer
        {
            currentClip = clip,
            isLoop = isLoop,
            priority = priority
        };
    }

    public void StopMotion(int layer)
    {
        if (motionLayers.ContainsKey(layer))
        {
            motionLayers.Remove(layer);
        }
    }

    public void OnInteraction(List<TargetAnimation> targetAnimations, int targetExpression)
    {
        if (targetAnimations != null)
        {
            foreach (var anim in targetAnimations)
            {
                if (anim.clip != null)
                {
                    PlayMotion(anim.clip, anim.layer, anim.loop);
                }
            }
        }

        if (targetExpression >= 0)
            ChangeExpression(targetExpression);
        else
            ChangeExpression(defaultExpression);
    }

    public bool IsPlayingAnimation(int layer = 0)
    {
        return motionLayers.ContainsKey(layer) && motionLayers[layer].currentClip != null;
    }

    public void ForceStopAll()
    {
        motionLayers.Clear();
    }

    public void ChangeExpression(int expressionIndex)
    {
        if (cubismModel == null || cubismModel.expressionController == null) return;
        cubismModel.expressionController.CurrentExpressionIndex = expressionIndex;
    }
    public void PlayDefaultExpression()
    {
        ChangeExpression(defaultExpression);
    }

    public void PlayDefaultState()
    {
        PlayDefaultExpression();

        if (defaultMotion != null)
        {
            PlayMotion(defaultMotion, layer: 0, isLoop: defaultMotionLoop, priority: CubismMotionPriority.PriorityForce);
        }
    }
}
