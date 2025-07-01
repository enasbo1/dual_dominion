using UnityEngine;

namespace Monster.Variants
{
    public abstract class VariantApplier : MonoBehaviour
    {
        public abstract void ApplyVariant(MonsterDealer dealer, MonsterVariants variant);
        
        public abstract void RestoreDefault(MonsterDealer dealer);
    }
}