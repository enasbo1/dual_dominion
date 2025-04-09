using JetBrains.Annotations;
using UnityEngine;

namespace Monster
{
    public class SpawnerDealer : MonoBehaviour
    {
        [SerializeField] [CanBeNull] public Transform mainTransform;
        [SerializeField] [CanBeNull] public GameObject prefabToSpawn;
    }
}
