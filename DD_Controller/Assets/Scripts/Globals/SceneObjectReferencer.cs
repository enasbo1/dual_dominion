using System;
using System.Collections.Generic;
using UnityEngine;

namespace Globals
{
    public class SceneObjectReferencer : MonoBehaviour
    {
        public static SceneObjectReferencer MainInstance;
        public static event Action<SceneObjectReferencer> WaitingInit 
        {
            add => waitingAction(value);
            remove => _onInitialized.Remove(value);
        }
        private static readonly List<Action<SceneObjectReferencer>> _onInitialized = new();
        public Camera camera;

        private void Start()
        {
            if (MainInstance != null)
                throw new Exception("there is More than one instance of LifeManager marked as the Main Instance");
            
            MainInstance = this;
            foreach (Action<SceneObjectReferencer> actions in _onInitialized)
                actions.Invoke(this);
            
        }

        private static void waitingAction(Action<SceneObjectReferencer> action)
        {
            if (MainInstance == null) _onInitialized.Add(action);
            else action.Invoke(MainInstance);
        }
        
    }
}