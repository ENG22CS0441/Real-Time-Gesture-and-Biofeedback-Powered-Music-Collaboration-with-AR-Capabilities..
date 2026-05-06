using UnityEngine;

/// <summary>
/// Marks a 3D pad and stores its index (which note/sample to play).
/// Attach to each cube; interaction is driven by ARInstrumentInteraction raycasts.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InstrumentPad : MonoBehaviour
{
    [SerializeField] [Range(0, 3)] private int padIndex = 0;

    public int PadIndex => padIndex;

    public void SetPadIndex(int index)
    {
        padIndex = Mathf.Clamp(index, 0, 3);
    }

    [Header("Optional tap feedback")]
    [SerializeField] private float punchScale = 1.08f;
    [SerializeField] private float punchDuration = 0.12f;

    private Vector3 _baseScale;
    private float _punchTimer;

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    private void Update()
    {
        if (_punchTimer > 0f)
        {
            _punchTimer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_punchTimer / punchDuration);
            float s = Mathf.Lerp(punchScale, 1f, t);
            transform.localScale = _baseScale * s;
        }
    }

    /// <summary>Called by ARInstrumentInteraction when user taps this pad.</summary>
    public void OnTapped(InstrumentModeController audioController)
    {
        audioController?.PlayPad(padIndex);
        _punchTimer = punchDuration;
        transform.localScale = _baseScale * punchScale;
    }
}
