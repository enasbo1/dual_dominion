using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WithEndMonoBehavior : MonoBehaviour
{
    private readonly List<Action> _cleanActions = new List<Action>();

    protected Action<InputAction.CallbackContext> ToBeCleanedAction(Action<InputAction.CallbackContext> action, Action<Action<InputAction.CallbackContext>> cleanAction)
    {
        _cleanActions.Add(() => cleanAction(action));
        return action;
    }

    private void OnDestroy()
    {
        foreach (Action actions in _cleanActions)
        {
            actions.Invoke();
        }
        _cleanActions.Clear();
    }
}