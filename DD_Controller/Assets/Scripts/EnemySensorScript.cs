using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySensorScript : MonoBehaviour
{
    [DoNotSerialize] public List<Collider> monstersNearby = new();

    private void OnTriggerEnter(Collider monster)
    {
        monstersNearby.Add(monster);
    }

    private void OnTriggerExit(Collider monster)
    {
        monstersNearby.Remove(monster);
    }
}