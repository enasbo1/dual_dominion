using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Move
{
    public class RandomSpeedManager : Manager<WalkerDdDealer>
    {
        private readonly List<WalkerDdDealer> _componentDealers = new();
        private readonly List<Animator> _animators = new();
        private bool[] _isActive = Array.Empty<bool>();
        
        [SerializeField] private float speedTarget;
        [SerializeField] private  string speedTargetParam = "walkSpeed";

        private float[] _currentSpeed =  Array.Empty<float>();
        
        public override void AddElement(WalkerDdDealer element)
        {
            int i = _componentDealers.FindIndex(d => d == element);

            if (i != -1)
            {
                _isActive[i] = true;
                return;
            }

            _componentDealers.Add(element);
            _animators.Add(element.animator);
            _isActive = _isActive.Append(true).ToArray();
            
            _currentSpeed = _currentSpeed.Append(element.animator?.GetFloat(speedTargetParam)??1f).ToArray();
            
        }

        public override void DisableElement(WalkerDdDealer element)
        {
            var i = _componentDealers.FindIndex(d => d == element);
            if (i != -1)
                _isActive[i] = false;
        }
        void Start()
        {
            
            for(int i = 0; i < _currentSpeed.Length; i++)
                _currentSpeed[i] = _animators[i].GetFloat(speedTargetParam);
        }

        void FixedUpdate()
        {
            var i = 0;
            foreach (var anim in _animators) if (_isActive[i])
            {
                _currentSpeed[i] += Random.Range(-speedTarget, speedTarget)/20;
                _currentSpeed[i] *= 0.999f;
                if (_currentSpeed[i] * 2 < speedTarget)
                    _currentSpeed[i] = speedTarget/2;
                anim.SetFloat(speedTargetParam, _currentSpeed[i]);
                ++i;
            }else 
                ++i;
        }
    }
}
