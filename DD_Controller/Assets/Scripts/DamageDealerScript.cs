using System.Collections.Generic;
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

        damageMax -= damageToReduce;

        if (damageMax <= 0) gameObject.SetActive(false);
    }
}