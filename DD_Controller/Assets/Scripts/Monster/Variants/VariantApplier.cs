using Unity.Netcode;

namespace Monster.Variants
{
    public abstract class VariantApplier<TDealer> : NetworkBehaviour
    {
        public TDealer Dealer;
        public abstract void ApplyVariant(MonsterDealer dealer, MonsterVariants variant);
        
        public abstract void RestoreDefault(MonsterDealer dealer);
    }
}