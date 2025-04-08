using System.Collections.Generic;
using JetBrains.Annotations;
using Move;
using UnityEngine;

namespace Shared
{
    public class ComponentDdDealer :MonoBehaviour
    {
        [SerializeField] [CanBeNull] public Rigidbody body;
        [SerializeField] [CanBeNull] public Transform mainTransform;
        [SerializeField] [CanBeNull] public Transform headTransform;
        [SerializeField] [CanBeNull] public MoveScript moveScript;
        [SerializeField] [CanBeNull] public Animator animator;
        [SerializeField] [CanBeNull] public List<Renderer> witnessBlessing;
    }
}