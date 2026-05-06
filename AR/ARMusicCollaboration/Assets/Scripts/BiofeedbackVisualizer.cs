using UnityEngine;

/// <summary>
/// Maps simulated heart rate to color and particle speed.
/// Low → blue + slow particles | Medium → green | High → red + fast particles.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class BiofeedbackVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HeartRateSimulator heartRateSimulator;
    [SerializeField] private Renderer targetRenderer;

    [Header("Heart rate zones (BPM)")]
    [SerializeField] private float lowMax = 70f;
    [SerializeField] private float mediumMax = 110f;

    [Header("Colors (low / medium / high)")]
    [SerializeField] private Color lowColor = new Color(0.2f, 0.5f, 1f);
    [SerializeField] private Color mediumColor = new Color(0.2f, 0.85f, 0.35f);
    [SerializeField] private Color highColor = new Color(1f, 0.25f, 0.2f);

    [Header("Particle emission rate (approximate)")]
    [SerializeField] private float particleRateLow = 8f;
    [SerializeField] private float particleRateHigh = 45f;

    private ParticleSystem _particles;
    private Material _materialInstance;
    private ParticleSystem.EmissionModule _emission;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        _emission = _particles.emission;

        if (targetRenderer != null)
        {
            _materialInstance = targetRenderer.material;
        }
    }

    private void OnEnable()
    {
        if (heartRateSimulator != null)
            heartRateSimulator.OnHeartRateChanged += ApplyFromHeartRate;
    }

    private void OnDisable()
    {
        if (heartRateSimulator != null)
            heartRateSimulator.OnHeartRateChanged -= ApplyFromHeartRate;
    }

    private void Start()
    {
        if (heartRateSimulator != null)
            ApplyFromHeartRate(heartRateSimulator.HeartRate);
    }

    private void ApplyFromHeartRate(float bpm)
    {
        float t;
        Color c;

        if (bpm <= lowMax)
        {
            t = Mathf.InverseLerp(40f, lowMax, bpm);
            c = Color.Lerp(lowColor * 0.7f, lowColor, t);
            _emission.rateOverTime = Mathf.Lerp(particleRateLow * 0.5f, particleRateLow, t);
        }
        else if (bpm <= mediumMax)
        {
            t = Mathf.InverseLerp(lowMax, mediumMax, bpm);
            c = Color.Lerp(lowColor, mediumColor, t);
            _emission.rateOverTime = Mathf.Lerp(particleRateLow, (particleRateLow + particleRateHigh) * 0.5f, t);
        }
        else
        {
            t = Mathf.InverseLerp(mediumMax, 180f, bpm);
            c = Color.Lerp(mediumColor, highColor, t);
            _emission.rateOverTime = Mathf.Lerp((particleRateLow + particleRateHigh) * 0.5f, particleRateHigh, t);
        }

        var main = _particles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(c);

        if (_materialInstance != null)
            _materialInstance.color = c;
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
            Destroy(_materialInstance);
    }
}
