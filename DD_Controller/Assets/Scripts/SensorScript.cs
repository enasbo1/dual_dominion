using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SensorScript : MonoBehaviour
{
    [DoNotSerialize] public List<Collider> nearby = new();
    [DoNotSerialize] public bool keepPresent = false;

    public event Action<Collider> Listener
    {
        add => listener.Add(value);
        remove => listener.Remove(value);
    }

    private List<Action<Collider>> listener = new ();
    private void OnTriggerEnter(Collider monster)
    {
        if (keepPresent)
            nearby.Add(monster);
        
        foreach (Action<Collider> listen in listener)
        {
            listen(monster);
        }
    }

    private void OnTriggerExit(Collider monster)
    {
        if (keepPresent)
            nearby.Remove(monster);
    }

    private void OnDisable()
    {
        nearby.Clear();
    }
}