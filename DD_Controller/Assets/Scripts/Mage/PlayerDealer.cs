using System.Globalization;
using end_game;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mage
{
    public class PlayerDealer : WalkerDdDealer
    {
        [SerializeField] private GameEnd gameEnd;
        [SerializeField] private GameObject levelUpGrimoireObject;
        [SerializeField] private SpellManager spellManager;
        
        [Header("Health")]
        [SerializeField] public float lifePoints;
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private TextMeshProUGUI textHp;
        [SerializeField] private Image damageEffect;
        
        [Header("XP Bar")]
        [SerializeField] public float levelXpPoints = 1000;
        [SerializeField] private float currentXP = 0;
        [SerializeField] private Slider xpBarSlider;
        [SerializeField] private TextMeshProUGUI textXp;        
        [SerializeField] private Image scoreEffect;
        public int skillsToUnlock;
        
        
        private string _stringMaxHp;
        private string _stringMaxXp;
        private Color _originalXpColor;
        private float _effectTimer;
        private void Start()
        {
            lifePoints = maxHealth;
            _stringMaxHp = lifePoints.ToString(CultureInfo.CurrentCulture);
            _stringMaxXp = levelXpPoints.ToString(CultureInfo.CurrentCulture);
            textHp.text = $"{lifePoints.ToString(CultureInfo.CurrentCulture)} / {_stringMaxHp}";
            UpdateXpUI();

            _originalXpColor = scoreEffect.color;
            
            Color damageEffectColor = damageEffect.color;
            damageEffectColor.a = 0f;
            damageEffect.color = damageEffectColor;
        }

        public void Aie(float damage)
        {
            lifePoints -= damage;
            animator?.SetTrigger(PlayerAnimP.Hurt);
            UpdateHealthUI();
            if (lifePoints <= 0)
            {
                gameEnd.GameOver();
            }
        }

        public void Score(float score)
        {
            currentXP += score;
            UpdateXpUI();
            if (levelXpPoints <= currentXP)
            {
                currentXP -= levelXpPoints;
                skillsToUnlock += 1;

                if (skillsToUnlock > 0 && spellManager.spellsToUnlock.Count == 0)
                {
                    gameEnd.GameWon();
                }
            }
        }

        private void Update()
        {
            levelUpGrimoireObject.SetActive(skillsToUnlock > 0);
            
            if (!damageEffect) return;
            if (!scoreEffect) return;
            if (damageEffect.color.a > 0f)
            {
                            
                Color damageEffectColor = damageEffect.color;
                damageEffectColor.a -= Time.deltaTime;
                damageEffect.color = damageEffectColor;
            }

            if (!(_effectTimer > 0f)) return;
            
            _effectTimer -= Time.deltaTime / 2;

            if (_effectTimer <= 0f)
                scoreEffect.color = _originalXpColor;
            else
            {
                Color temp = _originalXpColor;
                
                temp.r = temp.r * (1-_effectTimer) + _effectTimer;
                temp.g = temp.g * (1-_effectTimer) + _effectTimer; 
                temp.b = temp.b * (1-_effectTimer) + _effectTimer; 
                    
                scoreEffect.color = temp;
            }
        }
        
        private void UpdateHealthUI()
        {
            healthBarSlider.value = lifePoints / maxHealth;
            textHp.text = $"{lifePoints.ToString(CultureInfo.CurrentCulture)} / {_stringMaxHp}";
            
            Color damageEffectColor = damageEffect.color;
            damageEffectColor.a = 1f;
            damageEffect.color = damageEffectColor;
        }
        
        private void UpdateXpUI()
        {
            xpBarSlider.value = currentXP / levelXpPoints;
            textXp.text = $"{currentXP.ToString(CultureInfo.CurrentCulture)} / {_stringMaxXp}";

            _effectTimer = 1f;
        }
    }
}