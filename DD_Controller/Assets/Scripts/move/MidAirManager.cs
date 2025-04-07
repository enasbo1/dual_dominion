using System.Collections.Generic;
using Actions;
using Shared;
using UnityEngine;

namespace Move
{
    public class MidAirManager : MonoBehaviour
    {
        [SerializeField] private List<ComponentDdDealer> componentDealers;
        
        private readonly List<Animator> _animators = new List<Animator>();
        private readonly List<Transform> _transforms = new List<Transform>();
        private readonly List<Rigidbody> _body = new List<Rigidbody>();
        private readonly List<bool> _isActive = new List<bool>();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private static readonly int Mid_air = Animator.StringToHash("mid-air");
        private void Start()
        {
            componentDealers.ForEach(AddNewObject);
        }

        public void AddNewObject(ComponentDdDealer componentDdDealer)
        {
            _animators.Add(componentDdDealer.Anim);
            _transforms.Add(componentDdDealer.MainTransform);
            _body.Add(componentDdDealer.Body);
            _isActive.Add(true);
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _isActive.Count; i++)
            {
                if (!_isActive[i]) continue;
                
                
                _animators[i].SetBool(Mid_air ,
                    Physics.SphereCast(_transforms[i].position + (Vector3.up * 0.55f),
                    0.45f, 
                    Vector3.down, 
                    out var _, 
                    1.0f));
            }
        }
    }
}
