using System.Collections.Generic;
using UnityEngine;

public class DamageDealerScript : MonoBehaviour
{
    public EnemySensorScript enemySensorScript;
    public float damageMax;
    public float damageReductionPerHit;

    private List<Collider> _monstersNearby;

    private void Start()
    {
        _monstersNearby = enemySensorScript.monstersNearby;
    }

    private void OnTriggerEnter(Collider other)
    {
        float damageToReduce = 0;
        _monstersNearby.ForEach(_ => { damageToReduce += damageReductionPerHit; });

        damageMax -= damageToReduce;

        if (damageMax <= 0) gameObject.SetActive(false);
    }
}