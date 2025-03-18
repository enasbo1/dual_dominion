using System.Collections.Generic;
using UnityEngine;
namespace script

{
    public class FootMove : MonoBehaviour
    {
        public Transform characterTransform;
        public bool onUpdate = false;
        public bool canMove = true;
        public float MoveSpeed = 1.0f;
        public List<Transform> footList = new List<Transform>();


        private Vector3 lastFootPosition=Vector3.zero;
        private Transform footTransform=null;
        private void move()
        {
            if (footList.Count == 0) return;
            if (!canMove) return;
            if (lastFootPosition != Vector3.zero)
            {
                var move = (footTransform.position - characterTransform.position) - lastFootPosition ;
                move.y = 0;
                characterTransform.localPosition -= move * MoveSpeed;
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
            footTransform = floorFoot;
            lastFootPosition =  footTransform.position-characterTransform.position;
        }

        private void Update()
        {
            if (onUpdate) move();
        }
        private void FixedUpdate()
        {
            move();
        }
    }
}
