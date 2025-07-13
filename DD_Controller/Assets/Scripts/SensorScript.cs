using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class SensorScript : MonoBehaviour
{
    [SerializeField][CanBeNull] public Renderer renderer;
    [DoNotSerialize] public List<Collider> nearby = new List<Collider>();
    [DoNotSerialize] public bool keepPresent = false;

    public event Action<Collider> Listener
    {
        add => listener.Add(value);
        remove => listener.Remove(value);
    }

    private List<Action<Collider>> listener = new List<Action<Collider>>();
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