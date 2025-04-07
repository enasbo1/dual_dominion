using System;
using System.Collections.Generic;
using UnityEngine;

namespace Move
{
    public class FollowRotationManager : MonoBehaviour
    {
        public Transform[] leaders;
        public Transform[] followers;
        public float[] limit;
        public bool[] onlyY;
        public float[] speed;
        
        private List<Rigidbody> _rigidBodies;
        private Quaternion[] _start;
        private Vector3[] _offsets;
        private bool[] _hasRB;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (leaders.Length != followers.Length)
            {
                Debug.LogError("leaders and followers must have the same length");
                return;
            }

            if (limit == null || limit.Length == 0)
                limit = new float[leaders.Length];
            if (onlyY == null || onlyY.Length == 0)
                onlyY = new bool[leaders.Length];
            if (speed == null || speed.Length == 0)
                speed = new float[leaders.Length];
            if (followers.Length != limit.Length)
            {
                Debug.LogError("followers and limits must have the same length");
                return;
            }
            _rigidBodies = new List<Rigidbody>();
            var i = 0;
            _offsets = new Vector3[leaders.Length];
            _hasRB = new bool[leaders.Length];
            _start = new Quaternion[leaders.Length];
            foreach (var follow in followers)
            {
                _start[i] = follow.localRotation;
                var rb = follow.GetComponent<Rigidbody>();
                if (rb != null) {
                    _hasRB[i] = true;
                    _rigidBodies.Add(rb);
                }
                else
                    _hasRB[i] = false;
                _offsets[i] = follow.rotation.eulerAngles - leaders[i++].rotation.eulerAngles;
            }
            Debug.Log(_rigidBodies.Count);
        }

        private static float angleDistanceTo_0(float angle)
        {
            return Math.Abs((angle + 180) % 360 - 180);
        }

        private static void ApplyLimitedRotation(Transform targetTransform, Quaternion target, Quaternion start, float limit, bool onlyY)
        {
            if (limit == 0) return;
            var angle = (target * Quaternion.Inverse(start)).eulerAngles;
            var n = new[] {angle.x, angle.y, angle.z};
            var i = 0;

            foreach (var coord in n)
            {
                if (angleDistanceTo_0(coord) > limit)
                    n[i] = (coord<180) ? limit : -limit;
                ++i;
            }
            var rotation = Quaternion.Euler(n[0], n[1], n[2])*start;
            if (onlyY)
            {
                var rot = rotation.eulerAngles;
                var fix = target.eulerAngles;
                rot.x = fix.x;
                rot.z = fix.z;
                rotation = Quaternion.Euler(rot);
            }
            targetTransform.localRotation = rotation;
        }

        private static float RotateAngle(float angle, float target, float speed)
        {
            var dis = (target - angle) % 360;
            if (angleDistanceTo_0(dis) < speed * 2)
                return target;
            if (dis < - 180)
                return angle + speed;
            if ((dis > 180) | (dis < 0))
                return angle - speed;
            return angle + speed;
        }
        private static Quaternion RotationApply(Transform follower, Vector3 lead, Vector3 offset, float speed, bool onlyY)
        {
            var rot = follower.rotation.eulerAngles;
            var target = lead + offset;
            if (onlyY)
            {
                target.x = rot.x;
                target.z = rot.z;
            }
        
            if (speed == 0)
                return Quaternion.Euler(target);
            var stepSpeed = speed * Time.deltaTime;
            rot.y = RotateAngle(rot.y, target.y, stepSpeed);
        
            if (onlyY) return Quaternion.Euler(rot);
        
            rot.x = RotateAngle(rot.x, target.x, stepSpeed);
            rot.z = RotateAngle(rot.z, target.z, stepSpeed);
            return Quaternion.Euler(rot);
        }
    
        // Update is called once per frame
        void Update()
        {
            if (leaders.Length != followers.Length) return;
            var i = 0;
            var rb = 0;
            foreach (var follow in followers)
            {
                if (_hasRB[i])
                {
                    _rigidBodies[rb].rotation = RotationApply(follow, leaders[i].rotation.eulerAngles, _offsets[i], speed[i], onlyY[i]);
                    ++rb;
                }
                else
                    follow.rotation = RotationApply(follow, leaders[i].rotation.eulerAngles, _offsets[i], speed[i], onlyY[i]);
                ApplyLimitedRotation(follow, follow.localRotation, _start[i], limit[i], onlyY[i]);
                ++i;
            }
        }
    }
}
