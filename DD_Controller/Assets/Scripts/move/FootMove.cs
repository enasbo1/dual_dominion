using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Move
{
    public class FootMove : MonoBehaviour
    {
        public Transform characterTransform;
        public bool onUpdate;
        public bool canMove = true;
        [FormerlySerializedAs("MoveSpeed")] public float movementSpeed = 1.0f;
        public List<Transform> footList = new ();


        private Vector3 _lastFootPosition=Vector3.zero;
        private Transform _footTransform;
        
        private void Move()
        {
            if (footList.Count == 0) return;
            if (!canMove) return;
            if (_lastFootPosition != Vector3.zero)
            {
                var move = (_footTransform.position - characterTransform.position) - _lastFootPosition ;
                move.y = 0;
                characterTransform.position -= move * movementSpeed;
            }
            var floorFoot = footList[0];
            var rot = characterTransform.rotation;
            var unit = rot * Vector3.up;
            var y = Vector3.Dot(unit, floorFoot.position - characterTransform.position);
            foreach (var foot in footList.Where(t => t != floorFoot))
            {
                var i = Vector3.Dot(unit, foot.position - characterTransform.position);
                if (i < y)
                {
                    y = i;
                    floorFoot = foot;
                }
            }
            _footTransform = floorFoot;
            _lastFootPosition =  _footTransform.position-characterTransform.position;
        }

        private void Update()
        {
            if (onUpdate) Move();
        }
        private void FixedUpdate()
        {
            Move();
        }
    }
}
