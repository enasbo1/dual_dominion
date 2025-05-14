using System.Collections.Generic;
using UnityEngine;

public class ShieldBashScript : MonoBehaviour
{
    public EnemySensorScript enemySensorScript;
    public float yVelocity = 50f;
    public float zVelocity = 50f;
    
    private List<Collider> _monstersNearby;
    
    void Start()
    {
        enemySensorScript.withTriggerExit = false;
        _monstersNearby = enemySensorScript.monstersNearby;
    }

    void Update()
    {
        for (int i = _monstersNearby.Count - 1; i >= 0; i--)
        {
            Rigidbody monster = _monstersNearby[i].attachedRigidbody;

            if (monster != null)
            {
                monster.linearVelocity = new Vector3(0f, yVelocity, zVelocity);
            }

            _monstersNearby.RemoveAt(i);
        }
    }
}
