using Monster.AnimParameter;
using Move;
using Shared;
using UnityEngine;

namespace Monster.Behavior
{
    public class Slash : IMonsterBehavior
    {
        private const float END_DIST = 4 * 4;
        
        private BehaviorManager _behaviorManager;
        
        public bool IsAvailable(int index, BehaviorManager behaviorManager)
        {
            if (!behaviorManager.GetDealer(index).sensor)
            {
                Debug.LogWarning("erreur lors de la récupération du sensor du monstre");
                return false;
            }
            
            Vector3 position = behaviorManager.Transforms[index].position;
            Vector3? target = behaviorManager.MainPlayer.mainTransform?.position;
            return target != null && (position - target.Value).sqrMagnitude < END_DIST;

        }

        public float Start(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);
            
            dealer.animator?.SetTrigger(MonsterAnimP.Attack);

            dealer.HitListener += HitTarget;
            _behaviorManager = behaviorManager;

            if (dealer.witnessBlessing == null) return 0.5f;

            foreach (Renderer renderer in dealer.witnessBlessing) renderer.material = behaviorManager.attackMaterial;
            return 0.5f;
        }

        private void HitTarget(MonsterDealer dealer, Rigidbody targetBody)
        {
            if (_behaviorManager.MainPlayer.body == targetBody)
            {
                int index = _behaviorManager.GetDealerId(dealer);
                if (index == -1) return;
                if (_behaviorManager.BehaviorEnd[index] - 0.25f > Time.time) return;
                _behaviorManager.MainPlayer.Aie(dealer.currentDamages);
            }
            
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
            
            angles.y += BoidsManager.BoidRuleApply(position, angles.y, dist, target, charge:true, fact:Time.deltaTime*24);
            
            Quaternion qAngle = Quaternion.Euler(angles);

            if (behaviorManager.Bodies[index])
                behaviorManager.Bodies[index].rotation = qAngle;
            else
                behaviorManager.Transforms[index].rotation = qAngle;
            
            float fact = (behaviorManager.BehaviorEnd[index] - Time.time) * 4;
            
            if (dist >= END_DIST) return false;
            
            Vector2 dir = target - position;
            
            if (dist > 2.25f)
                behaviorManager.Transforms[index].position += new Vector3(dir.x, 0f, dir.y) * (Time.deltaTime * fact);
            else
                behaviorManager.Transforms[index].position -= new Vector3(dir.x, 0f, dir.y) * (Time.deltaTime * 2.25f * fact / dist);

            return false;
        }

        public float Stop(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);
            
            
            dealer.HitListener -= HitTarget;
            
            if (dealer.witnessBlessing == null) return 1f;
            for (int i = 0; i < dealer.witnessBlessing.Length; i++)
                dealer.witnessBlessing[i].material = behaviorManager.DefaultMaterials[index][i];
            return 1f;
        }
    }
}