using Monster;
using UnityEngine;

public class DamageDealerScript : MonoBehaviour
{
    public float damageMax;
    public float damageReductionPerHit;
    public float currentDamage;

    private void Start()
    {
        currentDamage = damageMax;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (MonsterLifeManager.MainInstance == null)
        {
            Debug.LogWarning("Monster life manager main instance not specified");
            return;
        }

        MonsterLifeManager.MainInstance.Hit(other.gameObject, damageMax);
        currentDamage -= damageReductionPerHit;

        if (currentDamage > 0) return;
        
        gameObject.SetActive(false);
        currentDamage = damageMax;
    }
}