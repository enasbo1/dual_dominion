using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoD
{
    public class MonsterSpawner
    {
        public readonly int id;
        public readonly string name;
        public readonly float respawnDelay;

        public bool canBeSpawn;
        public float cooldown;
        public bool isActive;
        public GameObject Prefab;
        public Button trigger;

        public MonsterSpawner(int id, string name, GameObject prefab, Button trigger, float recastDelay,
            bool enableByDefault)
        {
            this.id = id;
            this.name = name;
            this.trigger = trigger;
            this.Prefab = prefab;
            respawnDelay = recastDelay;
            isActive = enableByDefault;
        }
    }

    public class GodManagerScript : MonoBehaviour
    {
        public double karmaPoint;
        [SerializeField] private TextMeshProUGUI _karmaCounter;
        [SerializeField] private List<GameObject> spawnerList = new List<GameObject>();
        [SerializeField] private List<Button> spawnerButtonList = new List<Button>();
        private readonly List<MonsterSpawner> _monsterSpawnerList;
        public readonly List<MonsterSpawner> monsterSpawnerAvailable;

        public GodManagerScript()
        {
            if (spawnerList.Count > spawnerButtonList.Count) return;

            List<MonsterSpawner> test = new();
            for (int i = 0; i < spawnerList.Count; i++)
                test.Add(new MonsterSpawner(
                    i,
                    "MonsterName",
                    spawnerList[i],
                    spawnerButtonList[i],
                    5,
                    true
                ));

            _monsterSpawnerList = test;
            monsterSpawnerAvailable = _monsterSpawnerList;
        }

        // Update is called once per frame
        private void Update()
        {
            karmaPoint += Time.deltaTime;
            _karmaCounter.text = (Math.Round(karmaPoint * 100) / 100).ToString(CultureInfo.CurrentCulture);
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
            monsterSpawnerAvailable.Clear();
            monsterSpawnerAvailable.AddRange(spellsAvailable);
        }

        public void ResetSpellsAvailable()
        {
            monsterSpawnerAvailable.Clear();
            monsterSpawnerAvailable.AddRange(_monsterSpawnerList.Where(spell => spell.isActive));
        }
    }
}