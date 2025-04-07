using System.Collections.Generic;
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
        public List<Transform> footList = new List<Transform>();


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
                characterTransform.localPosition -= move * movementSpeed;
            }
            var floorFoot = footList[0];
            var rot = characterTransform.rotation;
            var y = (rot * floorFoot.position).y;
            foreach (var foot in footList)
            {
                var i = (rot * foot.position).y;
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
