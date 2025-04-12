using System.Collections.Generic;
using Shared;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Monster.Behavior
{
    public static class MonsterBehaviors
    {
        public static readonly Dictionary<MonsterBehaviorEnum, IMonsterBehavior> BehaviorMap = new()
        {
            { MonsterBehaviorEnum.Start, new Starts() },
            { MonsterBehaviorEnum.Idle, new Idle() },
            { MonsterBehaviorEnum.Regroup, new Regroup() }
        };
    }

    public class BehaviorManager : Manager<MonsterDealer, WalkerEnum, MonsterVariants>
    {
        [SerializeField] public Material idleMaterial;


        protected TableArray<MonsterBehaviorEnum> ActivesBehaviors = new(0);
        protected bool[] Available = new bool[5];
        protected TableArray<float> BehaviorEnd = new(0);

        [DoNotSerialize] public TableArray<Material[]> DefaultMaterials = new(0);
        protected TableArray<MonsterBehaviorEnum[]> KnownBehaviors = new(0);

        private void FixedUpdate()
        {
            float time = Time.time;
            for (int i = 0; i < Size; ++i)
                if (Active[i])
                {
                    IMonsterBehavior currentBehavior = MonsterBehaviors.BehaviorMap[ActivesBehaviors[i]];
                    if (BehaviorEnd[i] > time && !currentBehavior.Step(i, this)) continue;

                    currentBehavior.Stop(i, this);

                    int size = KnownBehaviors[i].Length;


                    if (size > Available.Length)
                        Available = new bool[size];
                    int nb = 0;

                    for (int j = 0; j < size; ++j)
                    {
                        try
                        {
                            Available[j] = MonsterBehaviors.BehaviorMap[KnownBehaviors[i][j]].IsAvailable(i, this);
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new KeyNotFoundException($"the behavior {KnownBehaviors[i][j]} is not implemented");
                        }

                        if (Available[j]) nb++;
                    }

                    int rand = Random.Range(0, nb);


                    for (int j = 0; j < KnownBehaviors[i].Length; ++j)
                        if (Available[j])
                        {
                            if (rand == 0)
                            {
                                ActivesBehaviors[i] = KnownBehaviors[i][j];
                                break;
                            }

                            --rand;
                        }

                    BehaviorEnd[i] = time + MonsterBehaviors.BehaviorMap[ActivesBehaviors[i]].Start(i, this);
                }
        }

        protected override void AddChunk(int size)
        {
            ActivesBehaviors.AddChunk(size);
            KnownBehaviors.AddChunk(size);
            BehaviorEnd.AddChunk(size);
            DefaultMaterials.AddChunk(size);
        }

        protected override void AddElementInChunk(MonsterDealer element)
        {
            ActivesBehaviors[Size] = MonsterBehaviorEnum.Start;
            KnownBehaviors[Size] = element.behaviors;
            BehaviorEnd[Size] = 0f;
            DefaultMaterials[Size] = new Material[element.witnessBlessing?.Length ?? 0];
        }

        protected override void AddElementInNew(MonsterDealer element)
        {
            ActivesBehaviors.Add();
            KnownBehaviors.Add(element.behaviors);
            BehaviorEnd.Add();
            DefaultMaterials.Add(new Material[element.witnessBlessing?.Length ?? 0]);
        }

        protected override void InitElement(MonsterDealer element)
        {
            if (element.witnessBlessing == null) return;
            for (int i = 0; i < DefaultMaterials[Size].Length; i++)
                DefaultMaterials[Size][i] = element.witnessBlessing[i].material;
        }

        protected override void RestoreElement(int i, MonsterDealer element)
        {
            if (element.witnessBlessing != null)
                for (int j = 0; j < DefaultMaterials[i].Length; j++)
                    element.witnessBlessing[j].material = DefaultMaterials[i][j];
            ActivesBehaviors[i] = MonsterBehaviorEnum.Start;
            BehaviorEnd[i] = 0f;
        }

        public MonsterDealer GetDealer(int index)
        {
            return Elements[index];
        }
    }

    public interface IMonsterBehavior
    {
        public bool IsAvailable(int index, BehaviorManager behaviorManager);
        public float Start(int index, BehaviorManager behaviorManager);
        public bool Step(int index, BehaviorManager behaviorManager);
        public void Stop(int index, BehaviorManager behaviorManager);
    }

    public enum MonsterBehaviorEnum
    {
        Start,
        Idle,
        Regroup,
        MovesWhileReady,
        MeleeAttack,
        Thrust,
        Guard,
        Parry,
        Heal,
        Invoke,
        NormalCharge,
        WyvernCharge,
        BallCharge,
        FlyOut,
        DirectShot,
        BallisticShot,
        AirShot,
        Slam,
        Slam3,
        Pause,
        DrakeideCrysalis,
        FireBurst
    }

    public class Starts : IMonsterBehavior
    {
        public bool IsAvailable(int index, BehaviorManager behaviorManager)
        {
            return false;
        }

        public float Start(int index, BehaviorManager behaviorManager)
        {
            return 0f;
        }

        public bool Step(int index, BehaviorManager behaviorManager)
        {
            return true;
        }

        public void Stop(int index, BehaviorManager behaviorManager)
        {
        }
    }
}