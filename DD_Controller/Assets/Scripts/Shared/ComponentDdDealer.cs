using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared
{
    public class ComponentDdDealer :MonoBehaviour
    {
        [SerializeField] [CanBeNull] public Rigidbody Body;
        [SerializeField] [CanBeNull] public Transform MainTransform;
        [SerializeField] [CanBeNull] public Transform HeadTransform;
        [SerializeField] [CanBeNull] public Animator Anim;
        [SerializeField] [CanBeNull] public List<Renderer> WitnessBlessing;
    }
}