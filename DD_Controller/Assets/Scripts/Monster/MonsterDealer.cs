using Monster.Behavior;
using Shared;
using UnityEngine;

namespace Monster
{
    public class MonsterDealer : WalkerDdDealer
    {
        [SerializeField] public MonsterBehaviorEnum[] behaviors;
        [SerializeField] public MonsterVariants[] variants;

        public override void ApplyVariant(MonsterVariants variant)
        {
            if (!mainTransform) return;
            mainTransform.localScale = Vector3.one * (variant == MonsterVariants.Big ? 2 : 1);
            if (body)
                body.mass = variant == MonsterVariants.Big ? 8 : 1;
        }
    }
}