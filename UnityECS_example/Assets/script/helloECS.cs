using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace script
{
    public class HelloEcs : MonoBehaviour
    {
        
        private Entity _entity;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            for (int i = 0; i < 10000; i++)
            {
                _entity = entityManager.CreateEntity();
                entityManager.SetName(_entity, "Hello");
                entityManager.AddComponentData(_entity, new HelloEcsComponent { HitPoints = 600 }); 
            }

            
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                _entity = entityManager.CreateEntity();
                entityManager.SetName(_entity, "Hello_x");
                entityManager.AddComponentData(_entity, new HelloEcsComponent { HitPoints = 600 });
            }
        }
    }

    public partial struct DamageOverTimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }
        
        public void OnUpdate(ref SystemState state)
        {
            foreach (var hitPointComponent in SystemAPI.Query<RefRW<HelloEcsComponent>>())
            {
                hitPointComponent.ValueRW.HitPoints -= 1;
            }
        }

        public void OnDestroy(ref SystemState state)
        {
        }
    }
    public partial struct DiesIf0PvSystem : ISystem
    {

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (hitPointComponent, ent) in SystemAPI.Query<RefRO<HelloEcsComponent>>().WithEntityAccess())
            {
                if (hitPointComponent.ValueRO.HitPoints == 0)
                {
                    ecb.DestroyEntity(ent);
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
    public struct HelloEcsComponent : IComponentData
    {
        public int HitPoints;
    }
}
