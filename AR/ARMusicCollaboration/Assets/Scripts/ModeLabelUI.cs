using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows current instrument mode (Piano / Drums). Assign the same InstrumentModeController as other scripts.
/// </summary>
public class ModeLabelUI : MonoBehaviour
{
    [SerializeField] private InstrumentModeController instrumentMode;
    [SerializeField] private Text label;

    private void OnEnable()
    {
        if (instrumentMode != null)
            instrumentMode.OnModeChanged += OnModeChanged;
    }

    private void OnDisable()
    {
        if (instrumentMode != null)
            instrumentMode.OnModeChanged -= OnModeChanged;
    }

    private void Start()
    {
        if (label != null && instrumentMode != null)
            label.text = "Mode: " + instrumentMode.GetModeLabel();
    }

    private void OnModeChanged(string modeName)
    {
        if (label != null)
            label.text = "Mode: " + modeName;
    }
}
