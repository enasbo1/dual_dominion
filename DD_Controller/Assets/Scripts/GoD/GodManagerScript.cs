using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GoD
{
    public class MonsterSpawnButton
    {
        public readonly int id;
        public readonly WalkerEnum type;
        public readonly float respawnDelay;
        public readonly float cost;

        public bool canBeSpawn;
        public float cooldown;
        public bool isActive;

        public MonsterSpawnButton(
            int id,
            WalkerEnum type,
            float recastDelay,
            float cost,
            bool enableByDefault)
        {
            this.id = id;
            this.type = type;
            this.respawnDelay = recastDelay;
            this.cost = cost;
            isActive = enableByDefault;
        }
    }

    public class GodManagerScript : MonoBehaviour
    {
        public double karmaPoint;
        public MonsterSpawnScript monsterSpawnScript;
        [FormerlySerializedAs("_karmaCounter")] [SerializeField] private TextMeshProUGUI karmaCounter;
        [SerializeField] private List<Sprite> monsterIcons = new List<Sprite>();
        [SerializeField] private List<WalkerEnum> monsterTypes = new List<WalkerEnum>();
        [SerializeField] private List<float> monsterCosts = new List<float>();
        [SerializeField] private Transform buttonList;
        
        private List<Image> _buttonsBackground = new List<Image>();
        
        private List<MonsterSpawnButton> _monsterSpawnButtonList;
        private List<MonsterSpawnButton> _monsterSpawnButtonAvailable;

        private void Start()
        {
            List<MonsterSpawnButton> test = new List<MonsterSpawnButton>();
            List<WalkerEnum> typeOrder = new List<WalkerEnum>()
            {
                WalkerEnum.DominionArmy,
            };
            
            int i = 0;
            foreach (Transform button in buttonList)
            {
                if (i >= monsterIcons.Count && i >= typeOrder.Count)
                {
                    Destroy(button.gameObject);
                    continue;
                }
                
                _buttonsBackground.Add(button.GetComponent<Image>());
                i++;
            }

            i = 0;
            foreach (Transform button in buttonList)
            {
                if (i >= monsterIcons.Count && i >= typeOrder.Count) break;
                
                Debug.Log("1");
                Sprite monsterIcon = monsterIcons[i];
                WalkerEnum monsterType = monsterTypes[i];
                float monsterCost = monsterCosts[i];
                
                Debug.Log("2");
                button.GetChild(0).GetComponent<Image>().sprite = monsterIcon;
                button.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = monsterCost.ToString(CultureInfo.CurrentCulture);
                button.GetComponent<Button>().onClick.AddListener(() => {
                    monsterSpawnScript.type = monsterType;
                    _buttonsBackground.ForEach(background => background.color = Color.black);
                    button.GetComponent<Image>().color = Color.gray;
                });
                
                Debug.Log("3");
                test.Add(new MonsterSpawnButton(
                    i,
                    monsterType,
                    5,
                    monsterCost,
                    true
                ));

                i++;
            }
            Debug.Log("4");

            _monsterSpawnButtonList = test;
            _monsterSpawnButtonAvailable = _monsterSpawnButtonList;
            Debug.Log(test);
            Debug.Log(_monsterSpawnButtonList);
            
        }

        // Update is called once per frame
        private void Update()
        {
            karmaPoint += Time.deltaTime;
            karmaCounter.text = (Math.Round(karmaPoint * 100) / 100).ToString(CultureInfo.CurrentCulture);
        }

        private void FixedUpdate()
        {
            float timeIncrement = Time.deltaTime;

            for (int i = _monsterSpawnButtonAvailable.Count - 1; i >= 0; i--)
            {
                MonsterSpawnButton monsterSpawnButton = _monsterSpawnButtonAvailable[i];

                if (monsterSpawnButton.cooldown < monsterSpawnButton.respawnDelay) monsterSpawnButton.cooldown += timeIncrement;
                monsterSpawnButton.canBeSpawn = monsterSpawnButton.cooldown >= monsterSpawnButton.respawnDelay;

                if (!monsterSpawnButton.isActive) _monsterSpawnButtonAvailable.RemoveAt(i);
            }
        }

        public MonsterSpawnButton GetMonsterSpawnerByType(WalkerEnum type)
        {
            Debug.Log(_monsterSpawnButtonList);
            return _monsterSpawnButtonList.Find(x => x.type == type);
        }

        public List<MonsterSpawnButton> GetMonsterSpawners()
        {
            return _monsterSpawnButtonList ?? new List<MonsterSpawnButton>();
        }

        public void SetMonsterSpawnersAvailable(List<MonsterSpawnButton> spellsAvailable)
        {
            _monsterSpawnButtonAvailable.Clear();
            _monsterSpawnButtonAvailable.AddRange(spellsAvailable);
        }

        public void ResetSpellsAvailable()
        {
            _monsterSpawnButtonAvailable.Clear();
            _monsterSpawnButtonAvailable.AddRange(_monsterSpawnButtonList.Where(spawner => spawner.isActive));
        }
    }
}