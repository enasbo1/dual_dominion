using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shared;
using UnityEngine;

public class DealingManager : MonoBehaviour
{
    [SerializeField] private Manager[] managers;
    [SerializeField] private List<ComponentDdDealer> objectsDealers;
    private ListWithListener<ComponentDdDealer> _objectsDealers;

    void Start()
    {
        _objectsDealers = new(AddElement, RemoveElement);
        _objectsDealers.AddRange(objectsDealers);
        foreach (var m in managers)
        {
            _objectsDealers.ForEach(od => m.AddElement(od));
        }
    }


    public void Add(ComponentDdDealer element)
    {
        _objectsDealers.Add(element);
    }

    public void Remove(ComponentDdDealer element)
    {
        _objectsDealers.Remove(element);
    }

    private void AddElement(ComponentDdDealer element)
    {
        if (!objectsDealers.Contains(element))
            objectsDealers.Add(element);
        foreach (var manager in managers)
        {
            manager.AddElement(element);
        }
    }

    private void RemoveElement(ComponentDdDealer element)
    {
        objectsDealers.Remove(element);
        foreach (var manager in managers)
        {
            manager.DisableElement(element);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (objectsDealers.SequenceEqual(_objectsDealers)) return;

        objectsDealers.ForEach(cdd =>
        {
            if (_objectsDealers.Contains(cdd)) return;
            _objectsDealers.AddSilently(cdd);
            foreach (var manager in managers)
            {
                manager.AddElement(cdd);
            }
        });

        _objectsDealers.ForEach(cdd =>
        {
            if (objectsDealers.Contains(cdd)) return;
            _objectsDealers.RemoveSilently(cdd);
            foreach (var manager in managers)
                manager.DisableElement(cdd);

        });

        if (objectsDealers.SequenceEqual(_objectsDealers)) return;
        _objectsDealers.Clear();
        _objectsDealers.AddRange(objectsDealers);
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
