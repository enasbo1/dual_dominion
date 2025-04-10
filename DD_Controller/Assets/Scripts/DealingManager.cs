using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shared;
using UnityEngine;


public class WalkerDealingManager : DealingManager<WalkerDdDealer>
{
}
public class DealingManager<TDealer> : MonoBehaviour
{
    [SerializeField] private Manager<TDealer>[] managers;
    [SerializeField] private List<TDealer> objectsDealers;
    private ListWithListener<TDealer> _objectsDealed;

    void Start()
    {
        _objectsDealed = new ListWithListener<TDealer>(AddElement, RemoveElement);
        _objectsDealed.AddRange(objectsDealers);
        foreach (Manager<TDealer> m in managers)
        {
            _objectsDealed.ForEach(od => m.AddElement(od));
        }
    }

     public int getNbDealers()
    {
        return _objectsDealed.Count;
    }

    public void Add(TDealer element)
    {
        _objectsDealed.Add(element);
    }

    public void Remove(TDealer element)
    {
        _objectsDealed.Remove(element);
    }

    private void AddElement(TDealer element)
    {
        if (!objectsDealers.Contains(element))
            objectsDealers.Add(element);
        foreach (Manager<TDealer> manager in managers)
        {
            manager.AddElement(element);
        }
    }

    private void RemoveElement(TDealer element)
    {
        objectsDealers.Remove(element);
        foreach (Manager<TDealer> manager in managers)
        {
            manager.DisableElement(element);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (objectsDealers.SequenceEqual(_objectsDealed)) return;

        objectsDealers.ForEach(cdd =>
        {
            if (_objectsDealed.Contains(cdd)) return;
            _objectsDealed.AddSilently(cdd);
            foreach (Manager<TDealer> manager in managers)
            {
                manager.AddElement(cdd);
            }
        });

        _objectsDealed.ForEach(cdd =>
        {
            if (objectsDealers.Contains(cdd)) return;
            _objectsDealed.RemoveSilently(cdd);
            foreach (Manager<TDealer> manager in managers)
                manager.DisableElement(cdd);

        });

        if (objectsDealers.SequenceEqual(_objectsDealed)) return;
        _objectsDealed.Clear();
        _objectsDealed.AddRange(objectsDealers);
    }
}

public class ListWithListener<T> : List<T>
{
    [CanBeNull] private readonly System.Action<T> _addAction;
    [CanBeNull] private readonly System.Action<T> _removeAction;

    public ListWithListener(System.Action<T> addAction)
    {
        _addAction = addAction;
    }
    public ListWithListener(System.Action<T> addAction, System.Action<T> removeAction)
    {
        _addAction = addAction;
        _removeAction = removeAction;
    }
    public new void Add(T item)
    {
        base.Add(item);
        _addAction?.Invoke(item);
    }

    public void AddSilently(T item)
    {
        base.Add(item);
    }

    public new void Remove(T item)
    {
        base.Remove(item);
        _removeAction?.Invoke(item);
    }

    public void RemoveSilently(T item)
    {
        base.Remove(item);
    }
}
