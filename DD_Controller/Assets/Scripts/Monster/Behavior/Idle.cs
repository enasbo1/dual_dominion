namespace Monster.Behavior
{
    public class Idle : IMonsterBehavior
    {
        public bool IsAvailable(int index, BehaviorManager behaviorManager)
        {
            return true;
        }

        public float Start(int index, BehaviorManager behaviorManager)
        {
            return 3;
        }

        public bool Step(int index, BehaviorManager behaviorManager)
        {
            return false;
        }

        public float Stop(int index, BehaviorManager behaviorManager)
        {
            return 0f;
        }
    }
}