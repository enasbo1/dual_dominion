using UnityEngine;

namespace script.move
{
    public class ManageAnalogicDirection : MonoBehaviour
    {
        public AnimationChanger directionSetter;
        public Transform target;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            var currentDirection = directionSetter.GetCurrentWalkDirection();
            if (currentDirection is null) return;
            
            var targetDirection = directionSetter.GetTargetWalkDirection();
            
            var rot  = target.localRotation.eulerAngles;
            rot.y = targetDirection - (currentDirection??0f);
            
            target.localRotation = Quaternion.Euler(rot);
        }
    }
}
