using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// When AR planes are detected, spawns four instrument pads in a row "in front" of the hit area.
/// Runs once; anchors pads in world space at the first suitable placement.
/// </summary>
public class ARInstrumentSpawner : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject padPrefab;

    [Header("Layout")]
    [SerializeField] private float spacing = 0.12f;
    [SerializeField] private float heightAbovePlane = 0.05f;

    private bool _spawned;

    private void Awake()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Update()
    {
        if (_spawned || planeManager == null || padPrefab == null || arCamera == null)
            return;

        foreach (var plane in planeManager.trackables)
        {
            if (plane.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                continue;

            Vector3 planeCenter = plane.transform.TransformPoint(plane.centerInPlaneSpace);
            Vector3 camPos = arCamera.transform.position;
            Vector3 toPlane = planeCenter - camPos;
            toPlane.y = 0f;
            if (toPlane.sqrMagnitude < 0.01f)
                toPlane = arCamera.transform.forward;
            toPlane.Normalize();

            Vector3 rowCenter = planeCenter + Vector3.up * heightAbovePlane;
            Vector3 right = Vector3.Cross(Vector3.up, toPlane).normalized;
            if (right.sqrMagnitude < 0.01f)
                right = arCamera.transform.right;

            for (int i = 0; i < 4; i++)
            {
                float offset = (i - 1.5f) * spacing;
                Vector3 pos = rowCenter + right * offset;
                GameObject instance = Instantiate(padPrefab, pos, Quaternion.LookRotation(toPlane, Vector3.up));
                var pad = instance.GetComponent<InstrumentPad>();
                if (pad != null)
                    pad.SetPadIndex(i);
            }

            _spawned = true;
            break;
        }
    }
}
