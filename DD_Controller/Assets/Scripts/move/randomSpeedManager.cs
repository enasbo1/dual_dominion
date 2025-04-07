using UnityEngine;
using UnityEngine.Serialization;

namespace Move
{
    public class RandomSpeedManager : MonoBehaviour
    {
        public Animator[] animators;
        
        [FormerlySerializedAs("speed_target")] public float speedTarget;
        [FormerlySerializedAs("speed_target_param")] public string speedTargetParam = "walkSpeed";

        private float[] _currentSpeed;
        
        void Start()
        {
            _currentSpeed = new float[animators.Length];
            for(int i = 0; i < _currentSpeed.Length; i++)
                _currentSpeed[i] = animators[i].GetFloat(speedTargetParam);
        }

        void FixedUpdate()
        {
            var i = 0;
            foreach (var anim in animators)
            {
                _currentSpeed[i] += Random.Range(-speedTarget, speedTarget)/20;
                _currentSpeed[i] *= 0.999f;
                if (_currentSpeed[i] * 2 < speedTarget)
                    _currentSpeed[i] = speedTarget/2;
                anim.SetFloat(speedTargetParam, _currentSpeed[i]);
            }
        }
    }
}
