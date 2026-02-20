using System;
using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{ 
    [SerializeField] int damageAmount = 10;
    StarterAssetsInputs starterAssetsInputs;  
    
     void Awake() 
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
    }
    void Update()
    {
        if (starterAssetsInputs.shoot)
        {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if(enemyHealth)
                {
                    Debug.Log(hit.collider.name);
                    enemyHealth.TakeDamage(damageAmount);
                }
            starterAssetsInputs.ShootInput(false);
        }
        
        }
    }
}
