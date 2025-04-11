using System;
using JetBrains.Annotations;
using Shared;
using UnityEngine;

namespace Monster
{
    public class SpawnerDealer : Dealer<Enum, Enum>
    {
        [SerializeField] [CanBeNull] public WalkerEnum[] prefabToSpawn;
    }
}
