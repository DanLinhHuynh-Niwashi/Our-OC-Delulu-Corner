using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System.Collections.Generic;
using System.Linq;

public class ModelParamController : MonoBehaviour
{
    public CharacterModel model;
    public CubismModel cubismModel;

    private class ParamTarget
    {
        public float FromValue;
        public float ToValue;
        public float Duration;
        public float Elapsed;
        public bool IsAdditive;
    }

    private Dictionary<string, ParamTarget> paramTargets = new();

    public void Init(CharacterModel model)
    {
        this.model = model;
        cubismModel = model.cubismModel;
    }

    public void MoveParameter(string paramName, float delta)
    {
        if (cubismModel == null) return;

        var param = cubismModel.Parameters.FirstOrDefault(p => p.Id == paramName);
        if (param == null) return;

        float target = param.Value + delta;
        Debug.Log(target);
        paramTargets[paramName] = new ParamTarget
        {
            FromValue = param.Value,
            ToValue = target,
            Duration = 0f,
            Elapsed = 0f,
            IsAdditive = true
        };
    }
    public void ResetParameter(string paramName, float fadeTime = 0.5f)
    {
        if (cubismModel == null) return;

        var param = cubismModel.Parameters.FirstOrDefault(p => p.Id == paramName);
        if (param == null) return;

        paramTargets[paramName] = new ParamTarget
        {
            FromValue = param.Value,
            ToValue = param.DefaultValue,
            Duration = Mathf.Max(0.001f, fadeTime),
            Elapsed = 0f,
            IsAdditive = false
        };
    }

    private void LateUpdate()
    {
        if (cubismModel == null) return;

        foreach (var kvp in paramTargets.ToArray())
        {
            var param = cubismModel.Parameters.FirstOrDefault(p => p.Id == kvp.Key);
            if (param == null)
            {
                paramTargets.Remove(kvp.Key);
                continue;
            }

            var data = kvp.Value;

            data.Elapsed += Time.deltaTime;

            float t = data.Duration <= 0f
                ? 1f
                : Mathf.Clamp01(data.Elapsed / data.Duration);

            float easedT = t * t * (3f - 2f * t);

            float value = Mathf.Lerp(data.FromValue, data.ToValue, easedT);
            value = Mathf.Clamp(value, param.MinimumValue, param.MaximumValue);

            param.BlendToValue(
                CubismParameterBlendMode.Override,
                value
            );

            if (t >= 1f)
            {
                param.BlendToValue(
                    CubismParameterBlendMode.Override,
                    data.ToValue
                );

                paramTargets.Remove(kvp.Key);
            }
        }
    }
}
