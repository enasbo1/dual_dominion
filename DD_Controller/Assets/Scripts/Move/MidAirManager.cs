using System.Collections.Generic;
using Shared;
using UnityEngine;

namespace Move
{
    public class MidAirManager : WalkerManager
    {
        
        private readonly TableList<Animator> _animators = new(0);
        private readonly TableList<Transform> _transforms = new(0);
        private readonly TableList<Rigidbody> _body = new(0);
        private readonly TableList<MoveScript> _moveScript = new(0);
        private readonly TableList<Vector3> _lastPosition = new(0);
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private static readonly int Mid_air = Animator.StringToHash("mid-air");

        protected override void AddChunk(int size)
        {
            _animators.AddChunk(size);
            _transforms.AddChunk(size);
            _body.AddChunk(size);
            _lastPosition.AddChunk(size);
            _moveScript.AddChunk(size);
        }

        protected override void RestoreElement(int i, WalkerDdDealer element)
        {
            _lastPosition[i] = _transforms[i].position;
            _animators[i].SetBool(Mid_air, true);
        }

        protected override void AddElementInChunk(WalkerDdDealer element)
        {
            _animators[Size] = element.animator;
            _transforms[Size] = element.mainTransform;
            _body[Size] = element.body;
            _lastPosition[Size] = element.transform.position;
            _moveScript[Size] = element.moveScript;
        }

        protected override void AddElementInNew(WalkerDdDealer element)
        {
            _animators.Add(element.animator);
            _transforms.Add(element.mainTransform);
            _body.Add(element.body);
            _lastPosition.Add(element.transform.position);
            _moveScript.Add(element.moveScript);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < Active.Count; i++) if (Active[i])
            {
                bool floored = Physics.SphereCast(_transforms[i].position + (Vector3.up * 0.55f),
                    0.45f,
                    Vector3.down,
                    out RaycastHit _,
                    1.0f);
                bool hist = _moveScript[i].canMove;

                _animators[i].SetBool(Mid_air , !floored);
                _moveScript[i].canMove = floored;
                if (hist && !floored)
                    _body[i].linearVelocity = (_transforms[i].position - _lastPosition[i])/Time.deltaTime;
                
                _lastPosition[i] = _transforms[i].position;
            }
        }
    }
}
