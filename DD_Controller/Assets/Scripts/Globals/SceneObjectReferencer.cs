using System;
using System.Collections.Generic;
using Monster;
using UnityEngine;

namespace Globals
{
    public class SceneObjectReferencer : MonoBehaviour
    {
        public static SceneObjectReferencer MainInstance;
        public LayerMask MapLayer;
        public Camera camera;
        public bool isNetworkScene;
        public MonsterStandByManager monsterStandByManager;
        public static event Action<SceneObjectReferencer> WaitingInit 
        {
            add => WaitingAction(value);
            remove => OnInitialized.Remove(value);
        }
        private static readonly List<Action<SceneObjectReferencer>> OnInitialized = new();

        private void Awake()
        {
            if (MainInstance != null)
                throw new Exception("there is More than one instance of SceneReferencer");
            
            MainInstance = this;
            foreach (Action<SceneObjectReferencer> actions in OnInitialized)
                actions.Invoke(this);
            
        }

        private static void WaitingAction(Action<SceneObjectReferencer> action)
        {
            if (MainInstance == null) OnInitialized.Add(action);
            else action.Invoke(MainInstance);
        }

        private void OnDestroy()
        {
            if (MainInstance == this)
                MainInstance = null;

        }
        
    }
}