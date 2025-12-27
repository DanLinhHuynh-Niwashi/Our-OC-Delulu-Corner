using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;

public class CubismAutoBreathInput : MonoBehaviour
{
    [Header("Parameter")]
    public string breathParamId = "ParamBreath";
    public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Additive;

    [Header("Breath Shape")]
    public float baseAmplitude = 0.3f;
    public float amplitudeRandom = 0.1f;

    public float basePeriod = 4.0f;
    public float periodRandom = 1.5f;

    public float offset = 0f;

    [Header("Timing")]
    public float minInterval = 2.0f;
    public float maxInterval = 5.0f;

    private CubismParameter breathParam;

    private float timer;
    private float currentAmplitude;
    private float currentPeriod;

    private float nextBreathTime;
    private bool isBreathing;

    private void Start()
    {
        var model = this.FindCubismModel();
        if (model == null) return;

        breathParam = model.Parameters.FindById(breathParamId);
        ScheduleNextBreath();
    }

    private void LateUpdate()
    {
        if (breathParam == null) return;

        // Chưa tới lúc thở
        if (!isBreathing)
        {
            if (Time.time >= nextBreathTime)
                StartBreath();

            return;
        }

        // Đang thở
        timer += Time.deltaTime;
        float phase = (timer / currentPeriod) * Mathf.PI * 2f;

        float breathValue = offset + Mathf.Sin(phase) * currentAmplitude;
        breathParam.BlendToValue(BlendMode, breathValue);

        // Kết thúc 1 chu kỳ thở
        if (timer >= currentPeriod)
        {
            isBreathing = false;
            timer = 0f;
            ScheduleNextBreath();
        }
    }

    private void StartBreath()
    {
        isBreathing = true;
        timer = 0f;

        currentAmplitude = baseAmplitude + Random.Range(-amplitudeRandom, amplitudeRandom);
        currentPeriod = basePeriod + Random.Range(-periodRandom, periodRandom);
    }

    private void ScheduleNextBreath()
    {
        nextBreathTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}
