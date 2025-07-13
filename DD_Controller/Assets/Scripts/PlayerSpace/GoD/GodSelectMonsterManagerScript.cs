using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Monster;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GoD
{
    public class Monster
    {
        public readonly int id;
        public readonly WalkerEnum type;
        public readonly MonsterVariants variant;
        public readonly float respawnDelay;
        public readonly float cost;

        public bool canBeSpawn;
        public float cooldown;
        public bool isActive;

        public Monster(
            int id,
            WalkerEnum type,
            MonsterVariants variant,
            float recastDelay,
            float cost,
            bool enableByDefault
            )
        {
            this.id = id;
            this.type = type;
            this.variant = variant;
            this.respawnDelay = recastDelay;
            this.cost = cost;
            isActive = enableByDefault;
        }
    }

    public class GodSelectMonsterManagerScript : MonoBehaviour
    {
        public GodManagerScript godManager;
        public MonsterSpawnScript monsterSpawnScript;
        [SerializeField] public Transform monsterSpawnPoint;
        [FormerlySerializedAs("_karmaCounter")] [SerializeField] private TextMeshProUGUI karmaCounter;
        [SerializeField] private Sprite[] monsterIcons;
        [SerializeField] private WalkerEnum[] monsterTypes;
        [SerializeField] private MonsterVariants[] monsterVariants;
        [SerializeField] private List<float> monsterCosts = new List<float>();
        [SerializeField] private Transform[] buttonList;
        
        private readonly List<Image> _buttonsBackground = new List<Image>();
        
        private readonly List<Monster> _monsterSpawnButtonList = new List<Monster>();
        private List<Monster> _monsterSpawnButtonAvailable;

        private void Awake()
        {
            int i = 0;
            foreach (Transform button in buttonList)
            {
                if (i >= monsterIcons.Length && i >= monsterTypes.Length && i >= monsterVariants.Length)
                {
                    button.gameObject.SetActive(false);
                    continue;
                }
                
                _buttonsBackground.Add(button.GetComponent<Image>());
                i++;
            }

            i = 0;
            
            foreach (Transform button in buttonList)
            {
                if (i >= monsterIcons.Length && i >= monsterTypes.Length && i >= monsterVariants.Length) break;
                
                Sprite monsterIcon = monsterIcons[i];
                WalkerEnum monsterType = monsterTypes[i];
                MonsterVariants monsterVariant = monsterVariants[i];
                float monsterCost = monsterCosts[i];
                
                button.GetChild(0).GetComponent<Image>().sprite = monsterIcon;
                button.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = monsterCost.ToString(CultureInfo.CurrentCulture);
                button.GetComponent<Button>().onClick.AddListener(() => {
                    monsterSpawnScript.type = monsterType;
                    monsterSpawnScript.variant = monsterVariant;
                    _buttonsBackground.ForEach(background => background.color = Color.black);
                    button.GetComponent<Image>().color = Color.gray;
                });
                
                _monsterSpawnButtonList.Add(new Monster(
                    i,
                    monsterType,
                    monsterVariants[i],
                    5,
                    monsterCost,
                    true
                ));

                i++;
            }

            //_monsterSpawnButtonAvailable = _monsterSpawnButtonList;
        }

        private void Update()
        {
            karmaCounter.text = (Math.Round(godManager.karmaPoint * 100) / 100).ToString(CultureInfo.CurrentCulture);
        }

        private void FixedUpdate()
        {
            float timeIncrement = Time.deltaTime;
            /*
            for (int i = _monsterSpawnButtonAvailable.Count - 1; i >= 0; i--)
            {
                MonsterSpawnButton monsterSpawnButton = _monsterSpawnButtonAvailable[i];

                if (monsterSpawnButton.cooldown < monsterSpawnButton.respawnDelay) monsterSpawnButton.cooldown += timeIncrement;
                monsterSpawnButton.canBeSpawn = monsterSpawnButton.cooldown >= monsterSpawnButton.respawnDelay;

                if (!monsterSpawnButton.isActive) _monsterSpawnButtonAvailable.RemoveAt(i);
            }
            */
        }

        public Monster GetMonsterSpawnerByType(WalkerEnum type, MonsterVariants variant)
        {
            return _monsterSpawnButtonList.Find(x => x.type == type && x.variant == variant);
        }

        public List<Monster> GetMonsterSpawners()
        {
            return _monsterSpawnButtonList ?? new List<Monster>();
        }

        public void SetMonsterSpawnersAvailable(List<Monster> spellsAvailable)
        {
            _monsterSpawnButtonAvailable.Clear();
            _monsterSpawnButtonAvailable.AddRange(spellsAvailable);
        }

        public void ResetMonstersAvailable()
        {
            _monsterSpawnButtonAvailable.Clear();
            _monsterSpawnButtonAvailable.AddRange(_monsterSpawnButtonList.Where(spawner => spawner.isActive));
        }
    }
}