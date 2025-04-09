using System.Collections.Generic;
using Actions;
using Shared;
using UnityEngine;

namespace Move
{
    public class MidAirManager : Manager
    {
        private readonly List<ComponentDdDealer> _componentDealers = new();
        
        private readonly List<Animator> _animators = new();
        private readonly List<Transform> _transforms = new();
        private readonly List<Rigidbody> _body = new();
        private readonly List<bool> _isActive = new();
        private readonly List<MoveScript> _moveScript = new();
        private readonly List<Vector3> _lastPosition = new();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private static readonly int Mid_air = Animator.StringToHash("mid-air");
        
        public override void AddElement(ComponentDdDealer element)
        {
            var i = _componentDealers.FindIndex(d => d == element);

            if (i != -1)
            {
                _isActive[i] = true;
                return;
            }

            _componentDealers.Add(element);
            _animators.Add(element.animator);
            _transforms.Add(element.mainTransform);
            _body.Add(element.body);
            _lastPosition.Add(element.transform.position);
            _moveScript.Add(element.moveScript);
            _isActive.Add(true);
        }

        public override void DisableElement(ComponentDdDealer element)
        {
            var i = _componentDealers.FindIndex(d => d == element);

            if (i != -1)
                _isActive[i] = false;
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _isActive.Count; i++)
            {
                if (!_isActive[i]) continue;

                var floored = Physics.SphereCast(_transforms[i].position + (Vector3.up * 0.55f),
                    0.45f,
                    Vector3.down,
                    out var _,
                    1.0f);
                var hist = _moveScript[i].canMove;

                _animators[i].SetBool(Mid_air , !floored);
                _moveScript[i].canMove = floored;
                if (hist && !floored)
                {
                    _body[i].linearVelocity = (_transforms[i].position - _lastPosition[i])/Time.deltaTime;
                }
                _lastPosition[i] = _transforms[i].position;

            }
        }
    }
}
