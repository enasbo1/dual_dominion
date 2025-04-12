using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class ActionManager : MonoBehaviour
    {
        private readonly List<IDdAction> _actions = new();
        private int _serial;

        private void FixedUpdate()
        {
            _actions.RemoveAll(action => action.update());
        }

        public int AddAction(IDdAction action)
        {
            _actions.Add(action);
            action.launch();
            action.id = _serial;
            return _serial++;
        }

        public void RemoveAction(int actionID)
        {
            _actions.RemoveAll(action =>
            {
                if (action.id == actionID)
                {
                    action.end();
                    return true;
                }

                return false;
            });
        }

        public IDdAction GetAction(int actionID)
        {
            return _actions.Find(action => action.id == actionID);
        }
    }

    public interface IDdAction
    {
        public int id { get; set; }

        public void launch();

        /*
         * return true if action ended
         */
        public bool update();
        public void end();
    }
}