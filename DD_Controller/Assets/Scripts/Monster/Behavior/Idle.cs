using UnityEngine;

namespace Monster.Behavior
{
    public class Idle : IMonsterBehavior
    {
        public bool IsAvailable(int index, BehaviorManager behaviorManager)
        {
            return true;
        }

        public float Start(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);

            if (dealer.witnessBlessing == null) return 3;
            
            foreach (Renderer renderer in dealer.witnessBlessing)
            {
                renderer.material = behaviorManager.idleMaterial;
            }

            return 3;
        }

        public bool Step(int index, BehaviorManager behaviorManager)
        {
            return false;
        }

        public void Stop(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);

            if (dealer.witnessBlessing == null) return;
            for(int i = 0; i < dealer.witnessBlessing.Length; i++)
            {
                dealer.witnessBlessing[i].material = behaviorManager.DefaultMaterials[index][i];
            }
        }
    }
}