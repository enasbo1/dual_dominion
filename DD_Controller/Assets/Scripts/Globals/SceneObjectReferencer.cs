using System;
using System.Collections.Generic;
using UnityEngine;

namespace Globals
{
    public class SceneObjectReferencer : MonoBehaviour
    {
        public static SceneObjectReferencer MainInstance;
        public Camera camera;

        public static event Action<SceneObjectReferencer> WaitingInit 
        {
            add => waitingAction(value);
            remove => OnInitialized.Remove(value);
        }
        private static readonly List<Action<SceneObjectReferencer>> OnInitialized = new();

        private void Start()
        {
            if (MainInstance != null)
                throw new Exception("there is More than one instance of LifeManager marked as the Main Instance");
            
            MainInstance = this;
            foreach (Action<SceneObjectReferencer> actions in OnInitialized)
                actions.Invoke(this);
            
        }

        private static void waitingAction(Action<SceneObjectReferencer> action)
        {
            if (MainInstance == null) OnInitialized.Add(action);
            else action.Invoke(MainInstance);
        }
        
    }
}