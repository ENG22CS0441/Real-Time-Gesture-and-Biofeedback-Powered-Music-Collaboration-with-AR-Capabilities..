using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simulates heart rate (bpm) for biofeedback demo — no hardware required.
/// Wire the slider in the Inspector or call SetHeartRate from code.
/// </summary>
public class HeartRateSimulator : MonoBehaviour
{
    [Header("Simulated heart rate (beats per minute)")]
    [Range(40f, 180f)]
    [SerializeField] private float heartRate = 72f;

    [Header("Optional UI")]
    [SerializeField] private Slider heartRateSlider;
    [SerializeField] private Text heartRateLabel;

    /// <summary>Current simulated BPM — other scripts read this.</summary>
    public float HeartRate => heartRate;

    public event System.Action<float> OnHeartRateChanged;

    private void Awake()
    {
        if (heartRateSlider != null)
        {
            heartRateSlider.minValue = 40f;
            heartRateSlider.maxValue = 180f;
            heartRateSlider.value = heartRate;
            heartRateSlider.onValueChanged.AddListener(OnSliderChanged);
        }
        RefreshLabel();
    }

    private void OnSliderChanged(float value)
    {
        heartRate = value;
        RefreshLabel();
        OnHeartRateChanged?.Invoke(heartRate);
    }

    /// <summary>For tests or external scripts.</summary>
    public void SetHeartRate(float bpm)
    {
        heartRate = Mathf.Clamp(bpm, 40f, 180f);
        if (heartRateSlider != null)
            heartRateSlider.SetValueWithoutNotify(heartRate);
        RefreshLabel();
        OnHeartRateChanged?.Invoke(heartRate);
    }

    private void RefreshLabel()
    {
        if (heartRateLabel != null)
            heartRateLabel.text = $"Heart rate: {Mathf.RoundToInt(heartRate)} BPM";
    }
}
