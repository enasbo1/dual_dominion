using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace script
{
    public class KeepStandManager : MonoBehaviour
    {
        struct StandingObject
        {
            public Transform transform;
            public bool hasRigidbody;
            public Rigidbody rigidbody;
        }
        
        public float angleTolerance = 10.0f;

        public List<GameObject> standingGameObjects = new List<GameObject>();
        
        private List<StandingObject> standingObjects = new List<StandingObject>();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (var standing in from standingObject in standingGameObjects 
                     let rBody = standingObject.GetComponent<Rigidbody>() 
                     select new StandingObject
                     {
                         transform = standingObject.transform,
                         hasRigidbody = rBody != null,
                         rigidbody = rBody
                     })
            {
                standingObjects.Add(standing);
            }
        }
        
        
        static float angleDistanceTo_0(float angle)
        {
            return Math.Abs((angle + 180) % 360 - 180);
        }
        
        // Update is called once per frame
        void FixedUpdate()
        {
            foreach (StandingObject standingObject in standingObjects)
            {
                var transform = standingObject.transform;
                var rotation = transform.localRotation.eulerAngles;
                var change = false;
                if (angleDistanceTo_0(rotation.x) > angleTolerance)
                {
                    rotation.x = rotation.x < 180 ? angleTolerance : -angleTolerance;
                    change = true;
                }

                if (angleDistanceTo_0(rotation.z) > angleTolerance)
                {
                    rotation.z = rotation.z < 180 ? angleTolerance : -angleTolerance;
                    change = true;
                }

                if (change)
                {
                    transform.localRotation = Quaternion.Euler(rotation);
                    if (standingObject.hasRigidbody)
                        standingObject.rigidbody.rotation = Quaternion.Euler(Vector3.zero);
                }
            }
        }
    }
}
