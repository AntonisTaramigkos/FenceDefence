using UnityEngine;

public class RepairManager : MonoBehaviour
{
    private Repair[] repairs;

    private void Awake()
    {
        repairs = FindObjectsByType<Repair>(FindObjectsSortMode.None);
    }

    public void SetRepairPhase(bool enabled)
    {
        if (repairs == null || repairs.Length == 0)
            repairs = FindObjectsByType<Repair>(FindObjectsSortMode.None);

        foreach (var r in repairs)
        {
            if (r != null)
                r.SetRepairPhase(enabled);
        }
    }

    public void RefreshAll()
    {
        if (repairs == null || repairs.Length == 0)
            repairs = FindObjectsByType<Repair>(FindObjectsSortMode.None);

        foreach (var r in repairs)
        {
            if (r != null)
                r.Refresh();
        }
    }
}