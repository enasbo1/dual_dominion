using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Move
{
    public class RandomSpeedManager : WalkerManager
    {
        
        [SerializeField] private float speedTarget;
        [SerializeField] private  string speedTargetParam = "walkSpeed";
        
        private readonly TableList<Animator> _animators = new (0);
        private TableArray<float> _currentSpeed =  new (0);

        protected override void _InitializeChunk(int size)
        {
            _currentSpeed.AddChunk(size);
            _animators.AddChunk(size);
        }

        protected override void AddElementInChunk(WalkerDdDealer element)
        {
            _animators[Size] = element.animator;
            _currentSpeed[Size] = element.animator?.GetFloat(speedTargetParam) ?? 1f;
        }

        protected override void AddElementInNew(WalkerDdDealer element)
        {
            _animators.Add(element.animator);
            _currentSpeed.Add(element.animator?.GetFloat(speedTargetParam) ?? 1f);
        }

        private void Start()
        {
            
            for(int i = 0; i < Size; i++)
                _currentSpeed[i] = _animators[i].GetFloat(speedTargetParam);
        }

        private void FixedUpdate()
        {
            for (int i =0; i<Size; ++i) if (Active[i])
            {
                _currentSpeed[i] += Random.Range(-speedTarget, speedTarget) / 20;
                _currentSpeed[i] *= 0.999f;
                if (_currentSpeed[i] * 2 < speedTarget)
                    _currentSpeed[i] = speedTarget / 2;
                _animators[i].SetFloat(speedTargetParam, _currentSpeed[i]);
            }
        }
    }
}
