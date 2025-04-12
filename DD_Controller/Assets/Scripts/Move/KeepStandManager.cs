using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Move
{
    public class KeepStandManager : MonoBehaviour
    {
        public float angleTolerance = 10.0f;

        public List<GameObject> standingGameObjects = new();

        private readonly List<StandingObject> _standingObjects = new();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            foreach (StandingObject standing in from standingObject in standingGameObjects
                     let rBody = standingObject.GetComponent<Rigidbody>()
                     select new StandingObject
                     {
                         Transform = standingObject.transform,
                         HasRigidBody = rBody,
                         RigidBody = rBody
                     })
                _standingObjects.Add(standing);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            foreach (StandingObject standingObject in _standingObjects)
            {
                Transform objectTransform = standingObject.Transform;
                Vector3 rotation = objectTransform.localRotation.eulerAngles;
                bool change = false;
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

                if (!change) return;

                objectTransform.localRotation = Quaternion.Euler(rotation);

                if (!standingObject.HasRigidBody) return;

                standingObject.RigidBody.rotation = Quaternion.Euler(Vector3.zero);
                standingObject.RigidBody.angularVelocity = Vector3.zero;
            }
        }


        private static float angleDistanceTo_0(float angle)
        {
            return Math.Abs((angle + 180) % 360 - 180);
        }

        private struct StandingObject
        {
            public Transform Transform;
            public bool HasRigidBody;
            public Rigidbody RigidBody;
        }
    }
}