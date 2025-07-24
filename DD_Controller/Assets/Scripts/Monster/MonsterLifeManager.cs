using System;
using GameRule;
using initScene;
using PlayerSpace;
using PlayerSpace.Mage;
using Shared;
using UnityEngine;

namespace Monster
{
    public class MonsterLifeManager : LifeManager<MonsterDealer, WalkerEnum, MonsterVariants>
    {
        [SerializeField] public PlayerBearer<MageDealer> playerBearer;
        public new static MonsterLifeManager MainInstance;

        private new void Start()
        {
            if (!IsMainInstance) return;
            if (MainInstance != null)
                throw new Exception("there is More than one instance of MonsterLifeManager marked as the Main Instance");
            MainInstance = this;
        }

        protected override void onDeath(MonsterDealer dealer)
        {
            playerBearer?.MainPlayer?.Score(dealer.scoreValue);
        }
    }
}