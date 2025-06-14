using GameRule;
using initScene;
using Mage;
using Shared;
using UnityEngine;

namespace Monster
{
    public class MonsterLifeManager : LifeManager<MonsterDealer, WalkerEnum, MonsterVariants>
    {
        [SerializeField] public PlayerBearer<PlayerDealer> playerBearer;

        protected override void onDeath(MonsterDealer dealer)
        {
            playerBearer?.MainPlayer?.Score(dealer.scoreValue);
        }
    }
}