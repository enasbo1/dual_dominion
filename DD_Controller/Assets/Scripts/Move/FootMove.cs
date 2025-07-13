using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Move
{
    public class FootMove : MoveScript
    {
        public Transform characterTransform;
        public bool onUpdate;
        public List<Transform> footList = new List<Transform>();
        private Transform _footTransform;
        private Vector3 _moveValue = Vector3.zero;

        private Vector3 _lastFootPosition = Vector3.zero;

        public override Vector3 GetMove()
        {
            return _moveValue;
        }
        
        private void Update()
        {
            if (onUpdate) Move();
        }

        private void FixedUpdate()
        {
            Move();
        }

        // TODO : optimisation
        private void Move()
        {
            if (footList.Count == 0) return;
            if (!canMove)
            {
                couldMove = false;
                return;
            }

            if (couldMove && _lastFootPosition != Vector3.zero)
            {
                Vector3 move = _footTransform.position - characterTransform.position - _lastFootPosition;
                move.y = 0;
                _moveValue = - move * movementSpeed;
                characterTransform.position += _moveValue;
            }
            else
            {
                _moveValue = Vector3.zero;
            }

            Transform floorFoot = footList[0];
            Quaternion rot = characterTransform.rotation;
            Vector3 unit = rot * Vector3.up;
            float y = Vector3.Dot(unit, floorFoot.position - characterTransform.position);
            foreach (Transform foot in footList)
                if (foot != floorFoot)
                {
                    float i = Vector3.Dot(unit, foot.position - characterTransform.position);
                    if (!(i < y)) continue;
                    y = i;
                    floorFoot = foot;
                }

            _footTransform = floorFoot;
            _lastFootPosition = _footTransform.position - characterTransform.position;
            couldMove = canMove;
        }
    }

    public abstract class MoveScript : MonoBehaviour
    {
        [FormerlySerializedAs("MoveSpeed")] public float movementSpeed = 1.0f;
        public bool canMove = true;
        [DoNotSerialize] public bool couldMove;

        public abstract Vector3 GetMove();
    }
}