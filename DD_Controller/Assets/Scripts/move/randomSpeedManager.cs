using UnityEngine;

namespace move
{
    public class RandomSpeedManager : MonoBehaviour
    {
        public Animator[] animators;
        
        public float speed_target;
        public string speed_target_param = "walkSpeed";

        private float[] _c_speed;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _c_speed = new float[animators.Length];
            for(int i = 0; i < _c_speed.Length; i++)
                _c_speed[i] = animators[i].GetFloat(speed_target_param);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            var i = 0;
            foreach (var anim in animators)
            {
                _c_speed[i] += Random.Range(-speed_target, speed_target)/20;
                _c_speed[i] *= 0.999f;
                if (_c_speed[i] * 2 < speed_target)
                    _c_speed[i] = speed_target/2;
                anim.SetFloat(speed_target_param, _c_speed[i]);
            }
        }
    }
}
