using UnityEngine;

namespace Move
{
    public class ManageAnalogDirection : MonoBehaviour
    {
        public AnimationChanger directionSetter;
        public Transform target;

        void Update()
        {
            var currentDirection = directionSetter.GetCurrentWalkDirection();
            if (currentDirection is null) return;
            
            var targetDirection = directionSetter.GetTargetWalkDirection();
            
            var rot  = target.localRotation.eulerAngles;
            rot.y = targetDirection - ((float)currentDirection);
            
            target.localRotation = Quaternion.Euler(rot);
        }
    }
}
