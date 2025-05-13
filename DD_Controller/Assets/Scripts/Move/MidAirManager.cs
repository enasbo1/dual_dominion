using System;
using Shared;
using Shared.AnimParameter;
using UnityEngine;

namespace Move
{
    public class MidAirManager : WalkerManager
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private readonly TableList<Animator> _animators = new(0);
        private readonly TableList<Rigidbody> _body = new(0);
        private readonly TableList<Vector3> _lastPosition = new(0);
        private readonly TableList<MoveScript> _moveScript = new(0);
        private readonly TableList<Transform> _transforms = new(0);
        private readonly TableList<float> _size = new(0);

        private void FixedUpdate()
        {
            var deb = 0;
            try
            {
                for (int i = 0; i < Size; ++i)
                    if (Active[i])
                    {
                        deb = 1;
                        
                        float size = _transforms[i].localScale.x * _size[i];
                        bool floored = Physics.SphereCast(_transforms[i].position + Vector3.up * (0.55f * size),
                            0.45f * size,
                            Vector3.down,
                            out RaycastHit _,
                            1.0f * size);
                        bool hist = _moveScript[i].canMove;

                        _animators[i].SetBool(WalkerAnimP.MidAir, !floored);
                        _moveScript[i].canMove = floored;

                        if (hist && !floored)
                            _body[i].linearVelocity = (_transforms[i].position - _lastPosition[i]) / Time.deltaTime;

                        _lastPosition[i] = _transforms[i].position;
                        deb = 0;
                    }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception("there " + Size + " "  + Active.Values.Length);
            }

        }

        protected override void AddChunk(int size)
        {
            _animators.AddChunk(size);
            _transforms.AddChunk(size);
            _body.AddChunk(size);
            _lastPosition.AddChunk(size);
            _moveScript.AddChunk(size);
            _size.AddChunk(size);
        }

        protected override void RestoreElement(int i, WalkerDdDealer element)
        {
            _lastPosition[i] = _transforms[i].position;
            _moveScript[i].canMove = false;
            _animators[i].SetBool(WalkerAnimP.MidAir, true);
            _size[i] = element.size;
        }

        protected override void AddElementInChunk(WalkerDdDealer element)
        {
            _animators[Size] = element.animator;
            _transforms[Size] = element.mainTransform;
            _body[Size] = element.body;
            _lastPosition[Size] = element.transform.position;
            _moveScript[Size] = element.moveScript;
            _size[Size] = element.size;
        }

        protected override void AddElementInNew(WalkerDdDealer element)
        {
            _animators.Add(element.animator);
            _transforms.Add(element.mainTransform);
            _body.Add(element.body);
            _lastPosition.Add(element.transform.position);
            _moveScript.Add(element.moveScript);
            _size.Add(element.size);
        }
    }
}