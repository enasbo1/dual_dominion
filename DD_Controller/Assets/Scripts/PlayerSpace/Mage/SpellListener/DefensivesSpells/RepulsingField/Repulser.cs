using System.Collections.Generic;
using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class Repulser : MonoBehaviour
    {
        public SensorScript sensorScript;

        private List<Collider> _monstersNearby;
        private SphereCollider _collider;

        private void Start()
        {
            _monstersNearby = sensorScript.nearby;
            _collider = this.GetComponent<SphereCollider>();
        }

        private void FixedUpdate()
        {
            if (!sensorScript) return;
            
            Vector3 center = this.transform.position;
            float radius = _collider.radius;
            
            _monstersNearby.ForEach(monsterCollider =>
            {
                Vector3 toMonster = monsterCollider.transform.position - center;
                float distance = toMonster.magnitude;

                Vector3 direction = toMonster.normalized;
                float forceFactor = -1f + (distance / radius);

                Vector3 force = direction * (forceFactor * 20f);

                monsterCollider.attachedRigidbody.AddForce(force);
            });
        }
    }
}