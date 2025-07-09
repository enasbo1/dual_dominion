using Globals;
using Unity.Netcode;
using UnityEngine;

namespace Monster.Variants
{
    public class WarriorVariants : VariantApplier<MonsterDealer>
    {
        
        [SerializeField] private Material[] skinMaterialsI;
        [SerializeField] private Material[] skinMaterialsII;
        [SerializeField] private Material[] skinMaterialsIII;
        [SerializeField] private Renderer skin;
        
        
        private Material[] skinMaterialsD;
        private float _defaultMoveSpeed;
        private float _defaultMaxHealth;
        public override void ApplyVariant(MonsterDealer dealer, MonsterVariants variant)
        {
            if ((SceneObjectReferencer.MainInstance?.isNetworkScene ?? false) && dealer == Dealer)
                ApplyVariantRpc((int)variant);
            else
                ApplyOneVariant(dealer, variant);
 
        }
        
        [Rpc(SendTo.Everyone)]
        private void ApplyVariantRpc(int dealedVariant)
        {
            ApplyOneVariant(Dealer, (MonsterVariants)dealedVariant);
        }

        private void ApplyOneVariant(MonsterDealer dealer, MonsterVariants variant)
        {
            switch (variant)
            {
                case MonsterVariants.DWarrior:
                    if (dealer.moveScript) dealer.moveScript.movementSpeed = _defaultMoveSpeed * 1.3f;
                    skin.materials = skinMaterialsI;
                    break;
                case MonsterVariants.DDefendor:
                    dealer.maxHealth = _defaultMaxHealth * 1.5f;
                    skin.materials = skinMaterialsII;
                    break;
                case MonsterVariants.DProtector:
                    dealer.maxHealth = _defaultMaxHealth * 1.5f;
                    if (dealer.body) dealer.body.mass *= 1.3f;
                    skin.materials = skinMaterialsIII;
                    break;
            }
        }

        public override void RestoreDefault(MonsterDealer dealer)
        {
            if (skinMaterialsD == null) skinMaterialsD = skin.materials;
            if (_defaultMoveSpeed == 0f)
                _defaultMoveSpeed = dealer.moveScript?.movementSpeed ?? 1f;
            else if (dealer.moveScript) dealer.moveScript.movementSpeed = _defaultMoveSpeed;
            if (_defaultMaxHealth == 0f)
                _defaultMaxHealth = dealer.maxHealth;
            else dealer.maxHealth = _defaultMaxHealth;
            
            if (dealer.body) dealer.body.mass = 1f;        }
    }
}