using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GoD
{
    public class MonsterSpawner
    {
        public readonly int id;
        public readonly string name;
        public Button trigger;
        public GameObject prefab;
        public readonly float respawnDelay;
        public bool isActive;
        
        public bool canBeSpawn;
        public float cooldown;

        public MonsterSpawner(int id, string name, GameObject prefab, Button trigger, float recastDelay, bool enableByDefault)
        {
            this.id = id;
            this.name = name;
            this.trigger = trigger;
            this.prefab = prefab;
            this.respawnDelay = recastDelay;
            isActive = enableByDefault;
        }
    }
    
    public class GodManagerScript : MonoBehaviour
    {
        public double karmaPoint;
        [SerializeField] private List<GameObject> spawnerList = new List<GameObject>();
        [SerializeField] private List<Button> spawnerButtonList = new List<Button>();
        private readonly List<MonsterSpawner> _monsterSpawnerList;
        public readonly List<MonsterSpawner> monsterSpawnerAvailable;
    
        public GodManagerScript()
        {
            if (spawnerList.Count > spawnerButtonList.Count) return;

            List<MonsterSpawner> test = new List<MonsterSpawner>();
            for (int i = 0; i < spawnerList.Count; i++)
            {
                test.Add(new MonsterSpawner(
                    i,
                    "MonsterName",
                    spawnerList[i],
                    spawnerButtonList[i],
                    5,
                    true
                ));
            }

            _monsterSpawnerList = test;
            monsterSpawnerAvailable = _monsterSpawnerList;
        }
        
        public MonsterSpawner GetMonsterSpawnerByName(string name)
        {
            return _monsterSpawnerList.Find(x => x.name == name);
        }

        public List<MonsterSpawner> GetMonsterSpawners()
        {
            return _monsterSpawnerList ?? new List<MonsterSpawner>();
        }
        
        public void SetMonsterSpawnersAvailable(List<MonsterSpawner> spellsAvailable)
        {
            this.monsterSpawnerAvailable.Clear();
            this.monsterSpawnerAvailable.AddRange(spellsAvailable);
        }
        
        public void ResetSpellsAvailable()
        {
            this.monsterSpawnerAvailable.Clear();
            this.monsterSpawnerAvailable.AddRange(_monsterSpawnerList.Where(spell => spell.isActive));
        }
        
        // Update is called once per frame
        void Update()
        {
            karmaPoint += Time.deltaTime;
        }
        
        private void FixedUpdate()
        {
            float timeIncrement = Time.deltaTime;
            
            for (int i = monsterSpawnerAvailable.Count - 1; i >= 0; i--)
            {
                MonsterSpawner monsterSpawner = monsterSpawnerAvailable[i];

                if (monsterSpawner.cooldown < monsterSpawner.respawnDelay) monsterSpawner.cooldown += timeIncrement;
                monsterSpawner.canBeSpawn = monsterSpawner.cooldown >= monsterSpawner.respawnDelay;

                if (!monsterSpawner.isActive) monsterSpawnerAvailable.RemoveAt(i);
            }
        }
    }
}
