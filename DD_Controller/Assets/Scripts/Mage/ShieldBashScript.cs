using System.Collections.Generic;
using GameRule;
using Monster;
using Shared;
using UnityEngine;
using UnityEngine.Serialization;

public class ShieldBashScript : MonoBehaviour
{
    public SensorScript sensorScript;
    public Dealer dealer;
    public float knockBack = 10f;
    public float damage = 10f;
    public MonsterLifeManager lifeScript;
    
    private List<Collider> _monstersNearby;

    private void Start()
    {
        sensorScript.Listener += Damage;
    }

    private void Push(Collider collider)
    {
    }

    private void Damage(Collider collider)
    {
        lifeScript.Hit(collider.attachedRigidbody, damage);
        if (dealer?.mainTransform != null)
            collider.attachedRigidbody.AddForce((collider.attachedRigidbody.position-dealer.mainTransform.position).normalized * knockBack, ForceMode.Impulse);
    }
}
