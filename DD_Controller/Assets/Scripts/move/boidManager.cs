using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace move
{
    public class BoidsManager : MonoBehaviour
    {
        public Transform[] boids;
        

        private bool[] _hasRb;
        private readonly List<Rigidbody> _boidsRb = new List<Rigidbody>();
        private Vector2[] _boidsPos;
        private float[] _angleList;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _hasRb = new bool[boids.Length];
            _boidsPos = new Vector2[boids.Length];
            _angleList = new float[boids.Length];
            
            
            var i = 0;
            foreach (var boidT in boids)
            {
                var rb = boidT.GetComponent<Rigidbody>();
                if (rb!= null)
                {
                    _boidsRb.Add(rb);
                    _hasRb[i] = true;
                }

                ++i;
            }
        }

        private static float normal_scalar(Vector2 a, Vector2 b){
            return  a.y * b.x-a.x * b.y;
        }
        
        private static float BoidRuleApply(Vector2 pos, float angle, float dist, Vector2 target, float targetAngle, float fact = 1)
        {
            angle += Random.Range(-2, 3) * fact;
            var side = 0f;
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

        
        private static (int?, float) LookForNearest(int current, Vector2[] targetList)
        {
            var birdV = targetList[current];

            Vector2? nearestV = null;
            float near = 0;
            int? i = null;

            for (var k = 0; k < targetList.Length; ++k) if (k != current){
                var bV= targetList[k];
                var dist = (bV - birdV).SqrMagnitude();
                if ((nearestV == null) | (near > (bV - birdV).SqrMagnitude())){
                    nearestV = bV;
                    i = k;
                    near = dist;
                }
            }
            
            return (i, near);
        }
        
        // Update is called once per frame
        void FixedUpdate()
        {
            var posList = _boidsPos;
            var angleList = _angleList;
            var i = 0;
            var rbIndex = 0;
            foreach (var boid in boids)
            {
                var tamp = boid.position;
                posList[i].x = tamp.x;
                posList[i].y = tamp.z;
                if (_hasRb[i])
                {
                    angleList[i] = _boidsRb[rbIndex].rotation.eulerAngles.y;
                    ++rbIndex;
                }
                else
                    angleList[i] = boid.rotation.eulerAngles.y;
                ++i;
            }
            
            rbIndex = 0;
            for (var j = 0; j < boids.Length; ++j)
            {
                var birdV = posList[j];

                var (n, near) = LookForNearest(j, posList);
                if (n == null) return;

                var newAngle = BoidRuleApply(birdV, angleList[j], near, posList[n??0], angleList[n??0], Time.deltaTime*6);


                
                if (_hasRb[j])
                {
                    var rot = _boidsRb[rbIndex].rotation.eulerAngles;
                    rot.y = newAngle;
                    _boidsRb[rbIndex].rotation = Quaternion.Euler(rot);
                    ++rbIndex;
                }
                else
                {
                    var rot = boids[j].rotation.eulerAngles;
                    rot.y = newAngle;
                    boids[j].rotation = Quaternion.Euler(rot);
                }
            }
        }
    }
}
