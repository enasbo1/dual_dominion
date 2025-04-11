using UnityEngine;

namespace Monster.Behavior
{
    public class Regroup : IMonsterBehavior
    {
        public bool IsAvailable(int index, BehaviorManager behaviorManager)
        {
            return true;
        }

        public float Start(int index, BehaviorManager behaviorManager)
        {
            return 5;
        }

        public bool Step(int index, BehaviorManager behaviorManager)
        {
            return false;
        }

        public void Stop(int index, BehaviorManager behaviorManager)
        {
        }
    }
}