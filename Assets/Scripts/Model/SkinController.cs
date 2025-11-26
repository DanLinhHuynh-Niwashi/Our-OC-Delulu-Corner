using Live2D.Cubism.Core;
using UnityEngine;
using System.Collections.Generic;

public class SkinController : MonoBehaviour
{
    private Dictionary<string, CubismPart> partCache;
    private Dictionary<string, List<CubismDrawable>> drawableCache;

    public void Init(CharacterModel characterModel)
    {
        if (!characterModel || !characterModel.cubismModel) return;

        CubismModel cubismModel = characterModel.cubismModel;

        partCache = new Dictionary<string, CubismPart>();
        drawableCache = new Dictionary<string, List<CubismDrawable>>();

        Debug.Log("[SKIN] Initializing caches...");

        // Cache parts
        foreach (CubismPart part in cubismModel.Parts)
        {
            Debug.Log("[SKIN] Caching Part: " + part.Id);
            partCache[part.Id] = part;

            // Tạo list drawable rỗng trước để lát nữa add vào
            drawableCache[part.Id] = new List<CubismDrawable>();
        }

        // Cache drawables và gán chúng vào parent part
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
    }

    public void SetSkin(SkinData skinData)
    {
        Debug.Log("[SKIN] Setting skin " + skinData.skinId);

        foreach (string part in skinData.partsToHide)
            TogglePart(part, false);

        foreach (string part in skinData.partsToShow)
            TogglePart(part, true);
    }

    private void TogglePart(string partId, bool visible)
    {
        // Set opacity của Part (nếu model có dùng)
        if (partCache.TryGetValue(partId, out var part))
            part.Opacity = visible ? 1f : 0f;

        // Ẩn hiện Drawable thật sự
        if (drawableCache.TryGetValue(partId, out var drawables))
        {
            foreach (CubismDrawable drawable in drawables)
            {
                Debug.Log("[Skin] Hide drawable " + drawable.name);
                drawable.gameObject.SetActive(visible);
            }
        }

        Debug.Log($"[SKIN] {(visible ? "SHOW" : "HIDE")} part {partId}");
    }
}
