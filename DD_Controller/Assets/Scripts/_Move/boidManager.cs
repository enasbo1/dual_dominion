using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Move
{
    public class BoidsManager : Manager<WalkerDdDealer>
    {
        private readonly List<WalkerDdDealer> _boids = new();
        private readonly List<Transform> _transform = new();
        private bool[] _hasRb = Array.Empty<bool>();
        private readonly List<Rigidbody> _boidsRb = new();
        private Vector2[] _boidsPos;
        private float[] _angleList;
        private bool[] _active = Array.Empty<bool>();

        public override void AddElement(WalkerDdDealer element)
        {
            int i = _boids.FindIndex(d => d == element);

            if (i != -1)
            {
                _active[i] = true;
                return;
            }

            Rigidbody rb = element.body;

            _hasRb = _hasRb.Append(rb).ToArray();
            if (rb)
                _boidsRb.Add(rb);
            
            _boids.Add(element);
            _transform.Add(element.transform);
            _boidsPos = new Vector2[_boids.Count];
            _angleList= new float[_boids.Count];
            
            _active = _active.Append(true).ToArray();
        }

        public override void DisableElement(WalkerDdDealer element)
        {
            int i = _boids.FindIndex(d => d == element);

            if (i != -1)
                _active[i] = false;
        }
        
        
        private static float normal_scalar(Vector2 a, Vector2 b){
            return  a.y * b.x-a.x * b.y;
        }
        
        private static float BoidRuleApply(Vector2 pos, float angle, float dist, Vector2 target, float targetAngle, float fact = 1)
        {
            angle += Random.Range(-2, 3) * fact;
            float side;
            switch (dist)
            {
                case < 1:
                {
                    side =  normal_scalar(new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)), target - pos);
                    if (side > 0)
                        return angle + 45* fact;
                    return angle - 45* fact;
                }
                case > 4:
                    side = normal_scalar(new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)), target - pos);
                    if (side > 0)
                        return angle - 15 * fact;
                    return angle + 15* fact;
                default:
                    if (targetAngle>angle)
                        return angle + 12 * fact;
                    return angle - 12* fact;
            }
        }

        
        private static (int?, float) LookForNearest(int current, Vector2[] targetList, bool[] actives)
        {
            Vector2 birdV = targetList[current];

            Vector2? nearestV = null;
            float near = 0;
            int? i = null;

            for (int k = 0; k < targetList.Length; ++k) if ((k != current) && actives[k]) {
                Vector2 bV= targetList[k];
                float dist = (bV - birdV).SqrMagnitude();
                if (!((nearestV == null) | (near > (bV - birdV).SqrMagnitude()))) continue;
                nearestV = bV;
                i = k;
                near = dist;
            }
            
            return (i, near);
        }
        
        // Update is called once per frame
        private void FixedUpdate()
        {
            Vector2[] posList = _boidsPos;
            float[] angleList = _angleList;
            int i = 0;
            int rbIndex = 0;
            foreach (Transform bidT in _transform)  if (_active[i])
            {
                {
                    Vector3 tamp = bidT.position;
                    posList[i].x = tamp.x;
                    posList[i].y = tamp.z;
                    if (_hasRb[i])
                    {
                        angleList[i] = _boidsRb[rbIndex].rotation.eulerAngles.y;
                        ++rbIndex;
                    }
                    else
                        angleList[i] = bidT.rotation.eulerAngles.y;   
                }
                ++i;
            } else
            {
                if (_hasRb[i])
                    ++rbIndex;
                ++i;
            }
            
            rbIndex = 0;
            for (int j = 0; j < _transform.Count; ++j) if (_active[j])
            {
                Vector2 birdV = posList[j];

                (int? n, float near) = LookForNearest(j, posList, _active);
                if (n == null) return;

                float newAngle = BoidRuleApply(birdV, angleList[j], near, posList[(int)n], angleList[(int)n], Time.deltaTime*6);
                
                if (_hasRb[j])
                {
                    Vector3 rot = _boidsRb[rbIndex].rotation.eulerAngles;
                    rot.y = newAngle;
                    _boidsRb[rbIndex].rotation = Quaternion.Euler(rot);
                    ++rbIndex;
                }
                else
                {
                    Vector3 rot = _transform[j].rotation.eulerAngles;
                    rot.y = newAngle;
                    _transform[j].rotation = Quaternion.Euler(rot);
                }
            } else if (_hasRb[j]) ++rbIndex;

        }
    }
}
