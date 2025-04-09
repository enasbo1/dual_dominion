using System;
using JetBrains.Annotations;
using Shared;
using UnityEngine;

namespace Monster
{
    public class SpawnerDealer : Dealer<Enum>
    {
        [SerializeField] [CanBeNull] public GameObject prefabToSpawn;
    }
}
