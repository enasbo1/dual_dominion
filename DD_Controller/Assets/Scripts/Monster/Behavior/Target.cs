using Move;
using Shared;
using UnityEngine;

namespace Monster.Behavior
{
    public class Target : IMonsterBehavior
    {
        private const float END_DIST = 4 * 4;
        
        public bool IsAvailable(int index, BehaviorManager behaviorManager)
        {
            Vector3 position = behaviorManager.Transforms[index].position;
            Vector3? target = behaviorManager.MainPlayer.mainTransform?.position;
            return target != null && (position - target.Value).sqrMagnitude > END_DIST;

        }

        public float Start(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);

            if (dealer.witnessBlessing == null) return 3;

            foreach (Renderer renderer in dealer.witnessBlessing) renderer.material = behaviorManager.targetMaterial;
            return 4;
        }

        public bool Step(int index, BehaviorManager behaviorManager)
        {
            Vector3 angles = behaviorManager.Bodies[index] ? 
                behaviorManager.Bodies[index].rotation.eulerAngles : 
                behaviorManager.Transforms[index].rotation.eulerAngles;
            Vector2 position = Vector2Extension.FromV3(behaviorManager.Transforms[index].position);
            Vector2 target = Vector2Extension.FromV3(behaviorManager.MainPlayer.mainTransform?.position);

            if (target == Vector2.zero)
            {
                Debug.LogWarning("Main player is null");
                return true;
            }
            
            float dist = (position - target).sqrMagnitude;

            angles.y += BoidsManager.BoidRuleApply(position, angles.y, dist, target, charge:true, fact:Time.deltaTime*9);
            
            if (behaviorManager.Bodies[index]) 
                behaviorManager.Bodies[index].rotation = Quaternion.Euler(angles);
            else
                behaviorManager.Transforms[index].rotation = Quaternion.Euler(angles);
            
            return dist < END_DIST;
        }

        public float Stop(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);

            if (dealer.witnessBlessing == null) return 0f;
            for (int i = 0; i < dealer.witnessBlessing.Length; i++)
                dealer.witnessBlessing[i].material = behaviorManager.DefaultMaterials[index][i];
            return 0f;
        }
    }
}