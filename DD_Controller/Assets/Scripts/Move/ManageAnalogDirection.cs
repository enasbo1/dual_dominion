using UnityEngine;

namespace Move
{
    public class ManageAnalogDirection : MonoBehaviour
    {
        public MageController directionSetter;
        public Transform target;

        private void Update()
        {
            float? currentDirection = directionSetter.GetCurrentWalkDirection();
            if (currentDirection is null) return;

            float targetDirection = directionSetter.GetTargetWalkDirection();

            Vector3 rot = target.localRotation.eulerAngles;
            rot.y = targetDirection - (float)currentDirection;

            target.localRotation = Quaternion.Euler(rot);
        }
    }
}