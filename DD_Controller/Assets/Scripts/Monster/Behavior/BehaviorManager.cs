using System.Collections.Generic;
using initScene;
using Mage;
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
            { MonsterBehaviorEnum.Regroup, new Regroup() },
            { MonsterBehaviorEnum.Target, new Target() },
            { MonsterBehaviorEnum.MeleeAttack, new MeleeAttack()},
            { MonsterBehaviorEnum.Slash, new Slash()},
            { MonsterBehaviorEnum.WyvernCharge, new WyvernCharge()}
        };
    }

    public class BehaviorManager : Manager<MonsterDealer, WalkerEnum, MonsterVariants>, IPlayerUser<PlayerDealer>
    {
        [SerializeField] public Material regroupMaterial;
        [SerializeField] public Material targetMaterial;
        [SerializeField] public Material attackMaterial;
        [SerializeField] public PlayerBearer<PlayerDealer> playerBearer;
        public PlayerDealer MainPlayer { get; set; }

        protected TableArray<MonsterBehaviorEnum> ActivesBehaviors = new(0);
        protected bool[] Available = new bool[5];
        public TableArray<float> BehaviorEnd = new(0);

        [DoNotSerialize] public TableArray<Material[]> DefaultMaterials = new(0);
        [DoNotSerialize] public TableList<Transform> Transforms = new(0);
        [DoNotSerialize] public TableList<Rigidbody> Bodies = new(0);
        protected TableArray<MonsterBehaviorEnum[]> KnownBehaviors = new(0);

        private void Start()
        {
            playerBearer.Subscribe(this);
        }

        private void FixedUpdate()
        {
            float time = Time.time;
            for (int i = 0; i < Size; ++i)
                if (Active[i])
                {
                    IMonsterBehavior currentBehavior = MonsterBehaviors.BehaviorMap[ActivesBehaviors[i]];
                    if (BehaviorEnd[i] > time && !currentBehavior.Step(i, this)) continue;

                    float delay = currentBehavior.Stop(i, this);

                    if (delay != 0f)
                    {
                        BehaviorEnd[i] = time + delay;
                        ActivesBehaviors[i] = MonsterBehaviorEnum.Idle;
                        continue;
                    }

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
            Transforms.AddChunk(size);
            Bodies.AddChunk(size);
        }

        protected override void AddElementInChunk(MonsterDealer element)
        {
            ActivesBehaviors[Size] = MonsterBehaviorEnum.Start;
            KnownBehaviors[Size] = element.behaviors;
            BehaviorEnd[Size] = 0f;
            DefaultMaterials[Size] = new Material[element.witnessBlessing?.Length ?? 0];
            Transforms[Size] = element.mainTransform;
            Bodies[Size] = element.body;
        }

        protected override void AddElementInNew(MonsterDealer element)
        {
            ActivesBehaviors.Add();
            KnownBehaviors.Add(element.behaviors);
            BehaviorEnd.Add();
            DefaultMaterials.Add(new Material[element.witnessBlessing?.Length ?? 0]);
            Transforms.Add(element.mainTransform);
            Bodies.Add(element.body);
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
            MonsterBehaviors.BehaviorMap[ActivesBehaviors[i]].Stop(i, this);
            ActivesBehaviors[i] = MonsterBehaviorEnum.Start;
            BehaviorEnd[i] = 0f;
        }

        public MonsterDealer GetDealer(int index)
        {
            return Elements[index];
        }
        
        public int GetDealerId(MonsterDealer dealer)
        {
            for (int i = 0; i < Size; ++i)
                if (Elements[i] == dealer)
                    return i;
            return -1;
        }

        public void SetMainPlayer(PlayerDealer dealer)
        {
            MainPlayer = dealer;
        }
        
    }

    public interface IMonsterBehavior
    {
        public bool IsAvailable(int index, BehaviorManager behaviorManager);
        public float Start(int index, BehaviorManager behaviorManager);
        public bool Step(int index, BehaviorManager behaviorManager);
        public float Stop(int index, BehaviorManager behaviorManager);
    }

    public enum MonsterBehaviorEnum
    {
        Start,
        Idle,
        Target,
        Regroup,
        MovesWhileReady,
        MeleeAttack,
        Slash,
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

        public float Stop(int index, BehaviorManager behaviorManager)
        {
            return 0f;
        }
    }
}