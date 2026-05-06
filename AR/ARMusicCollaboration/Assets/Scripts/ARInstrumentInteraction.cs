using UnityEngine;

/// <summary>
/// Touch input: tap on a pad plays sound; horizontal swipe switches instrument mode.
/// Pads are hit via Physics.Raycast from the AR camera (requires Colliders on pads).
/// </summary>
public class ARInstrumentInteraction : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private InstrumentModeController instrumentMode;

    [Header("Swipe")]
    [SerializeField] private float minSwipePixels = 80f;
    [SerializeField] private float swipeVsTapRatio = 1.5f;

    private Vector2 _touchStart;
    private bool _hasTouchStart;

    private void Awake()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Update()
    {
        if (arCamera == null || instrumentMode == null)
            return;

        if (Input.touchCount == 0)
            return;

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            _touchStart = t.position;
            _hasTouchStart = true;
        }
        else if (t.phase == TouchPhase.Ended && _hasTouchStart)
        {
            _hasTouchStart = false;
            Vector2 delta = t.position - _touchStart;
            float dist = delta.magnitude;

            if (dist >= minSwipePixels && Mathf.Abs(delta.x) > Mathf.Abs(delta.y) * swipeVsTapRatio)
            {
                instrumentMode.CycleInstrumentMode();
                return;
            }

            if (dist < minSwipePixels)
                TryTapPad(t.position);
        }
    }

    private void TryTapPad(Vector2 screenPos)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
            return;

        var pad = hit.collider.GetComponent<InstrumentPad>();
        if (pad != null)
            pad.OnTapped(instrumentMode);
    }
}
