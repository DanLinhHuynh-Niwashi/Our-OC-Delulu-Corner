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
        public float TargetValue;
        public bool IsAdditive; // true nếu MoveParameter, false nếu Reset
    }

    private Dictionary<string, ParamTarget> paramTargets = new Dictionary<string, ParamTarget>();
    private float lerpSpeed = 5f;

    public void Init(CharacterModel model)
    {
        this.model = model;
        this.cubismModel = model.cubismModel;
    }

    // Thêm delta, giữ nguyên trong paramTargets
    public void MoveParameter(string paramName, float delta)
    {
        if (cubismModel == null) return;

        var param = cubismModel.Parameters.FirstOrDefault(p => p.Id == paramName);
        if (param == null) return;

        if (!paramTargets.ContainsKey(paramName))
            paramTargets[paramName] = new ParamTarget { TargetValue = param.Value, IsAdditive = true };

        paramTargets[paramName].TargetValue += delta;
        paramTargets[paramName].IsAdditive = true;
    }

    // Reset về default, giữ target trong paramTargets
    public void ResetParameter(string paramName, float speed = 5f)
    {
        if (cubismModel == null) return;

        var param = cubismModel.Parameters.FirstOrDefault(p => p.Id == paramName);
        if (param == null) return;

        paramTargets[paramName] = new ParamTarget { TargetValue = param.DefaultValue, IsAdditive = false };
        lerpSpeed = speed;
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

            // Nếu additive thì cộng delta, còn reset thì lerp về default
            if (kvp.Value.IsAdditive)
            {
                param.Value = Mathf.Clamp(kvp.Value.TargetValue, param.MinimumValue, param.MaximumValue);
            }
            else
            {
                param.Value = Mathf.Lerp(param.Value, kvp.Value.TargetValue, Time.deltaTime * lerpSpeed);
                if (Mathf.Abs(param.Value - kvp.Value.TargetValue) < 0.001f)
                    paramTargets.Remove(kvp.Key);
            }
        }
    }
}
