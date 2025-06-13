using System.Collections.Generic;
using GameRule;
using Monster;
using Shared;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageDealerScript : MonoBehaviour
{
    [FormerlySerializedAs("enemySensorScript")] public SensorScript sensorScript;
    public float damageMax;
    public float damageReductionPerHit;

    private List<Collider> _monstersNearby;

    private void Start()
    {
        _monstersNearby = sensorScript.nearby;
    }

    private void OnTriggerEnter(Collider other)
    {
        float damageToReduce = 0;
        _monstersNearby.ForEach(_ => { damageToReduce += damageReductionPerHit; });

        if (MonsterLifeManager.MainInstance == null)
        {
            Debug.LogWarning("Monster life manager main instance not specified");
            return;
        }

        MonsterLifeManager.MainInstance.Hit(other.gameObject, damageMax);
        damageMax -= damageToReduce;

        if (damageMax <= 0) gameObject.SetActive(false);
    }
}