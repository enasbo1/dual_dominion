using Globals;
using Monster.AnimParameter;
using Move;
using Shared;
using UnityEngine;

namespace Monster.Behavior
{
    public class WyvernCharge : IMonsterBehavior
    {
        private BehaviorManager _behaviorManager;
        private const float END_DIST = 8 * 8;
        private static readonly Vector3 FireDirectionVector = new Vector3(0.0f, -1.0f, 0.1f).normalized;
        private static readonly Vector3 OffsetVector = new Vector3(0.0f, 0.5f, 1.75f);
        public bool IsAvailable(int index, BehaviorManager behaviorManager)
        {
            if (!behaviorManager.GetDealer(index).sensor)
            {
                Debug.LogWarning("erreur lors de la récupération du sensor du monstre");
                return false;
            }

            behaviorManager.GetDealer(index).sensor.keepPresent = true;
            
            
            Vector3 position = behaviorManager.Transforms[index].position;
            Vector3? target = behaviorManager.MainPlayer.mainTransform?.position;
            return target != null && (position - target.Value).sqrMagnitude > END_DIST;

        }

        public float Start(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);
            
            dealer.animator?.SetTrigger(WyvernAnimP.Fly);
            _behaviorManager = behaviorManager;
            dealer.sensor.keepPresent = true;
            if (!dealer.mainTransform) return 0;
            Vector3 dir = dealer.mainTransform.rotation * FireDirectionVector;

            dealer.sensor.transform.localScale *= 2f;
            behaviorManager.Bodies[index].ResetInertiaTensor();
            
            if (Physics.Raycast((behaviorManager.Transforms[index].position + OffsetVector) - (dir * 0.2f), dir,
                    out RaycastHit hitInfo, 8f*dealer.size, SceneObjectReferencer.MainInstance.MapLayer))
            {
                dealer.sensor.transform.localPosition = OffsetVector + dir * (hitInfo.distance - 0.2f);
                behaviorManager.Bodies[index].useGravity = false;
                if (dealer.sensor.renderer) dealer.sensor.renderer.enabled = true;
            };

            if (dealer.witnessBlessing == null) return 3f;

            foreach (Renderer renderer in dealer.witnessBlessing) renderer.material = behaviorManager.attackMaterial;
            return 3f;
        }
        private void HitTarget(MonsterDealer dealer, Rigidbody targetBody)
        {
            if (_behaviorManager.MainPlayer.body == targetBody)
            {
                _behaviorManager.MainPlayer.Aie(dealer.currentDamages * Time.deltaTime);
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
            
            MonsterDealer dealer = behaviorManager.GetDealer(index);
            
            Vector3 dir = behaviorManager.Transforms[index].rotation * FireDirectionVector;

            if (Physics.Raycast((behaviorManager.Transforms[index].position + OffsetVector) - (dir * 0.2f), dir,
                    out RaycastHit hitInfo, 8f*dealer.size, SceneObjectReferencer.MainInstance.MapLayer))
            {
                behaviorManager.Bodies[index].linearVelocity *= 0.75f;
                behaviorManager.Bodies[index].angularVelocity *= 0.5f;
                behaviorManager.Bodies[index].useGravity = false;
                
                foreach (Collider element in dealer.sensor.nearby)
                {
                    HitTarget(dealer, element.attachedRigidbody);
                }
                
                dealer.sensor.transform.localPosition = OffsetVector + dir * ((hitInfo.distance-0.5f)/dealer.size);
                


                behaviorManager.Transforms[index].position += Vector3.up * (Time.deltaTime *((6f*dealer.size) / (hitInfo.distance>0.5f ? hitInfo.distance : 0.5f) - 1.5f));
                
                
                if (dealer.sensor.renderer) dealer.sensor.renderer.enabled = true;
            }
            else
            {
                behaviorManager.Bodies[index].useGravity = true;
                if (dealer.sensor.renderer) dealer.sensor.renderer.enabled = false;
            }
            
            behaviorManager.Transforms[index].position += behaviorManager.Transforms[index].rotation * Vector3.forward * ((dealer.moveScript?.movementSpeed ?? 1f) * Time.deltaTime * 12f * dealer.size);


            float change = BoidsManager.BoidRuleApply(position, angles.y, dist, target, charge:true, fact:Time.deltaTime*4);
            angles.y += change;
            
            
            if (behaviorManager.Bodies[index]) 
                behaviorManager.Bodies[index].rotation = Quaternion.Euler(angles);
            else
                behaviorManager.Transforms[index].rotation = Quaternion.Euler(angles);
            
            return false;
        }

        public float Stop(int index, BehaviorManager behaviorManager)
        {
            MonsterDealer dealer = behaviorManager.GetDealer(index);
            dealer.sensor.transform.localPosition = OffsetVector;
            if (dealer.sensor.renderer) dealer.sensor.renderer.enabled = false;
            dealer.sensor.transform.localScale *= 0.5f;
            
            behaviorManager.Bodies[index].useGravity = true;
            behaviorManager.Bodies[index].ResetInertiaTensor();
            if (dealer.witnessBlessing == null) return 1f;
            for (int i = 0; i < dealer.witnessBlessing.Length; i++)
                dealer.witnessBlessing[i].material = behaviorManager.DefaultMaterials[index][i];
            return 1f;
        }
    }
}