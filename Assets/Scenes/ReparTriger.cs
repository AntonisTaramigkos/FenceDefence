using UnityEngine;

public class ReparTriger : MonoBehaviour
{
    [SerializeField] private GameObject repairHammer;
    [SerializeField] private FenceHealth fenceHealth;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] int repairPoints = 5;
    private bool isInsideTrinker;
    private  bool isDamaged;
    void Start()
    {
        if(repairHammer != null ) repairHammer.SetActive(false);
    }

    void Update()
    {
       if (repairHammer == null || fenceHealth == null) return;
        isDamaged = fenceHealth.currentHealth < fenceHealth.maxHealth;
        // Hammer should be visible ONLY when:
        // player is inside AND fence is damaged
        repairHammer.SetActive(isInsideTrinker && isDamaged);
        RepairAction(isDamaged);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            isInsideTrinker = true;         
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrinker = false;
            if(repairHammer != null)
            {
                repairHammer.SetActive(false);
            }
        }
    }
    public void RepairAction(bool isDamaged){
         if (!isInsideTrinker || !isDamaged) return;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
         fenceHealth.Repair(repairPoints);
        }
    }
    

}