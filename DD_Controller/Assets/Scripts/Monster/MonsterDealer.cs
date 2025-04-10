using JetBrains.Annotations;
using Shared;
using UnityEngine;

namespace Monster
{
    public class MonsterDealer : WalkerDdDealer
    {
        [SerializeField][CanBeNull] private Material shameMaterial;
        
        public new void Reset(bool respawn)
        {
            base.Reset(respawn);
            if (respawn && shameMaterial)
                witnessBlessing?.ForEach(rend => rend.material = shameMaterial);
        }
    }
}