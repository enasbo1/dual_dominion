using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Monster;
using Shared;
using UnityEngine;

public class WalkerDealingManager : DealingManager<WalkerDdDealer, WalkerEnum, MonsterVariants>
{
}

public abstract class WalkerManager : Manager<WalkerDdDealer, WalkerEnum, MonsterVariants>
{
}

public class DealingManager<TDealer, TEnum, TVariant> : MonoBehaviour where TDealer : Dealer<TEnum, TVariant>
    where TEnum : Enum
    where TVariant : Enum
{
    [SerializeField] private int initialCapacity;
    [SerializeField] private Manager<TDealer, TEnum, TVariant>[] managers;
    [SerializeField] private List<TDealer> objectsDealers;
    private ListWithListener<TDealer> _objectsDealed;
    private bool _hasStarted = false;

    public void ForceStart()
    {
        Start();
    }
    
    private void Start()
    {
        if (!_hasStarted)
            ContextStart(AddElement, RemoveElement);
        _hasStarted = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (objectsDealers.SequenceEqual(_objectsDealed)) return;

        objectsDealers.ForEach(cdd =>
        {
            if (_objectsDealed.Contains(cdd)) return;
            _objectsDealed.AddSilently(cdd);
            foreach (Manager<TDealer, TEnum, TVariant> manager in managers) manager.AddElement(cdd);
        });

        _objectsDealed.ForEach(cdd =>
        {
            if (objectsDealers.Contains(cdd)) return;
            _objectsDealed.RemoveSilently(cdd);
            foreach (Manager<TDealer, TEnum, TVariant> manager in managers)
                manager.DisableElement(cdd);
        });

        if (objectsDealers.SequenceEqual(_objectsDealed)) return;
        _objectsDealed.Clear();
        _objectsDealed.AddRange(objectsDealers);
    }

    protected void ContextStart(Action<TDealer> addAction, Action<TDealer> removeAction)
    {
        _objectsDealed = new ListWithListener<TDealer>(addAction, removeAction);
        _objectsDealed.AddRange(objectsDealers);
        int size = initialCapacity > _objectsDealed.Count ? initialCapacity : _objectsDealed.Count;
        foreach (Manager<TDealer, TEnum, TVariant> m in managers)
        {
            m.InitializeChunk(size);
            _objectsDealed.ForEach(od => m.AddElement(od));
        }
    }

    public int GetNbDealers()
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

    protected void AddElement(TDealer element)
    {
        if (!objectsDealers.Contains(element))
            objectsDealers.Add(element);
        foreach (Manager<TDealer, TEnum, TVariant> manager in managers) manager.AddElement(element);
    }

    protected void RemoveElement(TDealer element)
    {
        objectsDealers.Remove(element);
        foreach (Manager<TDealer, TEnum, TVariant> manager in managers) manager.DisableElement(element);
    }
}

public class ListWithListener<T> : List<T>
{
    [CanBeNull] private readonly Action<T> _addAction;
    [CanBeNull] private readonly Action<T> _removeAction;

    public ListWithListener(Action<T> addAction)
    {
        _addAction = addAction;
    }

    public ListWithListener(Action<T> addAction, Action<T> removeAction)
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