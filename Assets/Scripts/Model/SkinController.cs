using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkinController : MonoBehaviour
{
    private Dictionary<string, CubismPart> partCache;
    private Dictionary<string, List<CubismDrawable>> drawableCache;
    private CubismModel cubismModel;
    public CharacterModel model;

    // NEW: param lookup dictionary
    private Dictionary<string, CubismParameter> paramLookup;

    public void Init(CharacterModel characterModel)
    {
        if (!characterModel || !characterModel.cubismModel) return;
        model = characterModel;
        cubismModel = characterModel.cubismModel;

        partCache = new Dictionary<string, CubismPart>();
        drawableCache = new Dictionary<string, List<CubismDrawable>>();
        paramLookup = new Dictionary<string, CubismParameter>();

        Debug.Log("[SKIN] Initializing caches...");

        // Cache parts
        foreach (CubismPart part in cubismModel.Parts)
        {
            Debug.Log("[SKIN] Caching Part: " + part.Id);
            partCache[part.Id] = part;
            drawableCache[part.Id] = new List<CubismDrawable>();
        }

        // Cache drawables
        for (int i = 0; i < cubismModel.Drawables.Length; i++)
        {
            CubismDrawable drawable = cubismModel.Drawables[i];
            int parentIndex = drawable.ParentPartIndex;

            if (parentIndex < 0 || parentIndex >= cubismModel.Parts.Length)
                continue;

            CubismPart parentPart = cubismModel.Parts[parentIndex];

            if (!drawableCache.ContainsKey(parentPart.Id))
                drawableCache[parentPart.Id] = new List<CubismDrawable>();

            drawableCache[parentPart.Id].Add(drawable);

            Debug.Log($"[SKIN] Drawable {drawable.name} -> Part {parentPart.Id}");
        }

        // Cache params
        foreach (var param in cubismModel.Parameters)
        {
            if (!paramLookup.ContainsKey(param.Id))
                paramLookup[param.Id] = param;
        }
    }

    public void SetSkin(SkinData skinData)
    {
        Debug.Log("[SKIN] Setting skin " + skinData.skinId);

        if (skinData.isDrivenByParam)
        {
            SetParam(skinData);
            return;
        }

        foreach (string part in skinData.partsToHide)
            TogglePart(part, false);

        foreach (string part in skinData.partsToShow)
            TogglePart(part, true);
    }

    private Dictionary<string, ParamTarget> paramTargets = new Dictionary<string, ParamTarget>();
    private class ParamTarget
    {
        public string ParamName;
        public float TargetValue;
        public float FadeTime;
        public float StartTime;
        public float StartValue;
    }

    private void SetParam(SkinData skinData)
    {
        if (cubismModel == null) return;

        foreach (var paramSetting in skinData.paramSettings.ToArray())
        {
            if (!paramLookup.TryGetValue(paramSetting.ParamName, out var param))
                continue;

            paramTargets[skinData.setId] = new ParamTarget
            {
                ParamName = paramSetting.ParamName,
                TargetValue = paramSetting.Value,
                FadeTime = skinData.fadeTime,
                StartTime = Time.time,
                StartValue = param.Value
            };
        }
    }

    private void LateUpdate()
    {
        if (cubismModel == null) return;

        foreach (var kvp in paramTargets.ToArray())
        {
            if (!paramLookup.TryGetValue(kvp.Value.ParamName, out var param))
            {
                paramTargets.Remove(kvp.Key);
                continue;
            }

            // Stable lerp using deltaTime
            float delta = (kvp.Value.TargetValue - param.Value) / kvp.Value.FadeTime * Time.deltaTime;
            param.BlendToValue(CubismParameterBlendMode.Override, param.Value + delta);

            if (Mathf.Abs(param.Value - kvp.Value.TargetValue) < 0.001f)
                paramTargets.Remove(kvp.Key);
        }
    }

    private void TogglePart(string partId, bool visible)
    {
        if (partCache.TryGetValue(partId, out var part))
            part.Opacity = visible ? 1f : 0f;

        if (drawableCache.TryGetValue(partId, out var drawables))
        {
            foreach (CubismDrawable drawable in drawables)
                drawable.gameObject.SetActive(visible);
        }

        Debug.Log($"[SKIN] {(visible ? "SHOW" : "HIDE")} part {partId}");
    }
}
