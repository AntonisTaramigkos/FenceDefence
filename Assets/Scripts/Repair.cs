using UnityEngine;

public class Repair : MonoBehaviour
{
    private FencePart fence;
    private Canvas canvas;
    private bool repairPhase;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        fence = GetComponentInParent<FencePart>();

        // Always start hidden (VERY IMPORTANT)
        if (canvas != null)
            canvas.enabled = false;
    }

    public void SetRepairPhase(bool enabled)
    {
        repairPhase = enabled;
        Refresh();
    }

    public void Refresh()
    {
        if (canvas == null || fence == null) return;

        // Show only during repair phase AND only if fence needs repair
        // If you don't have rebuild yet, use fence.IsDamaged only.
        bool needsRepair = fence.IsDamaged; // <-- keep it simple for now
        canvas.enabled = repairPhase && needsRepair;
    }
}