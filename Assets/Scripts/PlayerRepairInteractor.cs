using UnityEngine;

public class PlayerRepairInteractor : MonoBehaviour
{
    [Header("Repair")]
    [SerializeField] private float repairAmountPerPress = 10f;
    [SerializeField] private float cooldown = 0.25f;
    [SerializeField] private KeyCode repairKey = KeyCode.E;

    [Header("Refs")]
    [SerializeField] private RepairManager repairManager;

    private FencePart target;
    private bool repairPhase;
    private float nextTime;

    public void SetRepairPhase(bool enabled)
    {
        repairPhase = enabled;
        target = null;
        Debug.Log($"[PlayerRepairInteractor] repairPhase={enabled}");

        // Optional: refresh icons when entering/exiting repair phase
        if (repairManager != null)
            repairManager.RefreshAll();
    }

    private void Update()
    {
        if (!repairPhase) return;

        // Input debug (keep while testing)
        if (Input.GetKeyDown(repairKey))
            Debug.Log("[PlayerRepairInteractor] E pressed");

        if (target == null) return;

        // If this fence no longer needs repair/rebuild, clear target
        if (!target.CanBeRepairedOrRebuilt)
        {
            target = null;
            return;
        }

        if (Time.time < nextTime) return;

        if (Input.GetKeyDown(repairKey))
        {
            target.RepairOrRebuild(repairAmountPerPress);
            nextTime = Time.time + cooldown;

            if (repairManager != null)
                repairManager.RefreshAll();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!repairPhase) return;

        var fence = other.GetComponentInParent<FencePart>();
        if (fence == null) return;

        // Target broken OR damaged pieces
        if (fence.CanBeRepairedOrRebuilt)
        {
            target = fence;
            Debug.Log($"[PlayerRepairInteractor] Target = {fence.name}");

            if (repairManager != null)
                repairManager.RefreshAll();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var fence = other.GetComponentInParent<FencePart>();
        if (fence != null && fence == target)
        {
            Debug.Log($"[PlayerRepairInteractor] Target cleared = {fence.name}");
            target = null;

            if (repairManager != null)
                repairManager.RefreshAll();
        }
    }
}