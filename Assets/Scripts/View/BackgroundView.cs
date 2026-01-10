using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(CanvasGroup))]
public class BackgroundView : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Image backImageOld;
    [SerializeField] private Image backImageNew;
    [SerializeField] private Image foregroundImageOld;
    [SerializeField] private Image foregroundImageNew;

    [Header("Ambient")]
    [SerializeField] private Transform ambientRoot;
    [SerializeField] private AmbientLayerView ambientPrefab;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float ambientFadeDuration = 0.2f;

    private BackgroundData currentBackground;
    public BackgroundData CurrentBackground { get { return currentBackground; } }

    private CanvasGroup canvasGroup;
    private bool isTransitioning = false;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Apply new background with professional sequence:
    /// Fade out old ambient -> Crossfade base+foreground -> Add new ambient
    /// </summary>
    public void Apply(BackgroundData data)
    {
        if (isTransitioning)
            return;
        currentBackground = data;
        StopAllCoroutines();
        StartCoroutine(ApplyRoutine(data));
    }

    void ClearAmbient()
    {
        for (int i = ambientRoot.childCount - 1; i >= 0; i--)
            Destroy(ambientRoot.GetChild(i).gameObject);
    }

    void ApplyForeground(BackgroundData data, Image foregroundImage)
    {
        if (data.foregroundSprite == null)
        {
            foregroundImage.gameObject.SetActive(false);
            return;
        }

        foregroundImage.gameObject.SetActive(true);
        foregroundImage.sprite = data.foregroundSprite;
        foregroundImage.preserveAspect = true;
        var rect = foregroundImage.rectTransform;
        var layout = data.foregroundLayoutData;

        rect.anchorMin = layout.anchorMin;
        rect.anchorMax = layout.anchorMax;
        rect.pivot = layout.pivot;
        rect.anchoredPosition = layout.anchoredPosition;
        rect.localScale = layout.scale;
    }

    private IEnumerator CrossfadeBG(BackgroundData newBackgroundData)
    {
        Sprite newBack = newBackgroundData.backSprite;
        Sprite newFG = newBackgroundData.foregroundSprite;
        // Setup new sprites
        backImageNew.sprite = newBack;
        backImageNew.preserveAspect = true;
        backImageNew.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = newBack.bounds.size.x / newBack.bounds.size.y;
        backImageNew.gameObject.SetActive(true);
        backImageNew.canvasRenderer.SetAlpha(0f);

        ApplyForeground(newBackgroundData, foregroundImageNew);
        foregroundImageNew.canvasRenderer.SetAlpha(0f);

        backImageOld.canvasRenderer.SetAlpha(1f);
        foregroundImageOld.canvasRenderer.SetAlpha(1f);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            backImageOld.canvasRenderer.SetAlpha(1f - alpha);
            backImageNew.canvasRenderer.SetAlpha(alpha);

            if (foregroundImageNew.gameObject.activeSelf)
            {
                foregroundImageOld.canvasRenderer.SetAlpha(1f - alpha);
                foregroundImageNew.canvasRenderer.SetAlpha(alpha);
            }

            yield return null;
        }

        // Swap references
        var tmpBack = backImageOld; backImageOld = backImageNew; backImageNew = tmpBack;
        var tmpFG = foregroundImageOld; foregroundImageOld = foregroundImageNew; foregroundImageNew = tmpFG;

        backImageOld.canvasRenderer.SetAlpha(1f);
        foregroundImageOld.canvasRenderer.SetAlpha(1f);
        backImageNew.gameObject.SetActive(false);
        foregroundImageNew.gameObject.SetActive(false);
        isTransitioning = false;
    }


    private IEnumerator ApplyRoutine(BackgroundData data)
    {
        isTransitioning = true;
        // 1️⃣ Fade out old ambient
        CanvasGroup[] oldAmbientGroups = GetAmbientCanvasGroups();
        float t = 0f;
        while (t < ambientFadeDuration && oldAmbientGroups.Length > 0)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / ambientFadeDuration);
            foreach (var cg in oldAmbientGroups)
                cg.alpha = alpha;
            yield return null;
        }

        // Destroy old ambient
        ClearAmbient();

        StartCoroutine(CrossfadeBG(data));
        
        // Add new ambient
        if (data.ambientLayers != null)
        {
            foreach (var layer in data.ambientLayers)
            {
                var view = Instantiate(ambientPrefab, ambientRoot);
                view.Init(layer);
            }
        }        
    }

    private CanvasGroup[] GetAmbientCanvasGroups()
    {
        var groups = new System.Collections.Generic.List<CanvasGroup>();
        foreach (Transform t in ambientRoot)
        {
            var cg = t.GetComponent<CanvasGroup>();
            if (cg == null) cg = t.gameObject.AddComponent<CanvasGroup>();
            groups.Add(cg);
        }
        return groups.ToArray();
    }

    public bool IsTransitioning
    {
        get { return isTransitioning; }
    }
}
