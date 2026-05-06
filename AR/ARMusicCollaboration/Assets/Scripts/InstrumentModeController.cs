using UnityEngine;

/// <summary>
/// Holds two "instrument banks" (e.g. piano vs drums). Swipe cycles the active bank.
/// Assign short AudioClips in the Inspector (or generate placeholder tones in Unity).
/// </summary>
public class InstrumentModeController : MonoBehaviour
{
    public enum InstrumentMode
    {
        Piano = 0,
        Drums = 1
    }

    [Header("Audio — one clip per pad index (0..3)")]
    [SerializeField] private AudioClip[] pianoClips = new AudioClip[4];
    [SerializeField] private AudioClip[] drumClips = new AudioClip[4];

    [SerializeField] private AudioSource audioSource;

    private InstrumentMode _mode = InstrumentMode.Piano;

    public InstrumentMode CurrentMode => _mode;

    /// <summary>Fired when swipe changes Piano ↔ Drums (for UI).</summary>
    public event System.Action<string> OnModeChanged;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    /// <summary>Play sound for pad index (0–3) using current instrument mode.</summary>
    public void PlayPad(int padIndex)
    {
        padIndex = Mathf.Clamp(padIndex, 0, 3);
        AudioClip clip = _mode == InstrumentMode.Piano
            ? SafeClip(pianoClips, padIndex)
            : SafeClip(drumClips, padIndex);

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    private static AudioClip SafeClip(AudioClip[] clips, int i)
    {
        if (clips == null || i < 0 || i >= clips.Length) return null;
        return clips[i];
    }

    /// <summary>Cycle Piano → Drums → Piano …</summary>
    public void CycleInstrumentMode()
    {
        _mode = _mode == InstrumentMode.Piano ? InstrumentMode.Drums : InstrumentMode.Piano;
        OnModeChanged?.Invoke(GetModeLabel());
    }

    public string GetModeLabel()
    {
        return _mode == InstrumentMode.Piano ? "Piano" : "Drums";
    }
}
