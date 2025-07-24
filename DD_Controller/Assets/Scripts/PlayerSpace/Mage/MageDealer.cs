using System;
using System.Globalization;
using EndGame;
using Globals;
using Menu;
using Shared;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSpace.Mage
{
    public class MageDealer : WalkerDdDealer
    {
        [Header("MageDealer Specific")]
        [SerializeField] private GameEnd gameEnd;
        [SerializeField] private GameObject levelUpGrimoireObject;
        [SerializeField] private SpellManager spellManager;
        
        [Header("Health")]
        [SerializeField] public float lifePoints = 1000;
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private TextMeshProUGUI textHp;
        [SerializeField] private Image damageEffect;
        [SerializeField] private bool isInvincible;
        
        [Header("XP Bar")]
        [SerializeField] public float levelXpPoints = 850;
        [SerializeField] private float currentXP;
        [SerializeField] private Slider xpBarSlider;
        [SerializeField] private TextMeshProUGUI textXp;        
        [SerializeField] private Image scoreEffect;
        public int skillsToUnlock;
        
        private TimeScaleController _timeScaleController;
        private PauseMenuScript _pauseMenu;
        private Color _originalXpColor;
        private float _effectTimer;
        
        private void Start()
        {
            SceneObjectReferencer.WaitingInit += sor =>
            {
                _timeScaleController = sor.timeScaleController;

                if (!_timeScaleController) {
                    Debug.LogError("TimeScaleController instance not found in scene.");
                    enabled = false;
                    return;
                }

                
                _pauseMenu  = sor.pauseMenu;

                if (!_pauseMenu) {
                    Debug.LogError("PauseMenuScript instance not found in scene.");
                    enabled = false;
                    return;
                }
                
                if (!gameEnd) gameEnd = sor.gameEnd;
            };
            
            lifePoints = maxHealth;
            textHp.text = $"{lifePoints.ToString(CultureInfo.CurrentCulture)} / {maxHealth.ToString(CultureInfo.CurrentCulture)}";
            UpdateXpUI();

            _originalXpColor = scoreEffect.color;
            
            Color damageEffectColor = damageEffect.color;
            damageEffectColor.a = 0f;
            damageEffect.color = damageEffectColor;
        }

        public void Aie(float damage)
        {
            if (isInvincible) return;
            lifePoints -= damage;
            animator?.SetTrigger(PlayerAnimP.Hurt);
            UpdateHealthUI();
            
            // Mage Defeat conditions
            if (lifePoints <= 0)
            {
                isInvincible = true;
                gameEnd.EndGame(false);
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
                levelXpPoints += 150;

                // Mage Victory conditions
                if (skillsToUnlock > 0 && spellManager.spellsToUnlock.Count == 0)
                {
                    isInvincible = true;
                    gameEnd.EndGame(true);
                }
            }
        }

        private void Update()
        {
            bool isLevelUpTime = Math.Abs(Time.timeScale - _timeScaleController.timeFlowLevelUp) < 0.001f;
            if (!_pauseMenu.isPauseActive && skillsToUnlock > 0 && !isLevelUpTime)
                _timeScaleController.SetTimeScale(_timeScaleController.timeFlowLevelUp);
            if (skillsToUnlock <= 0 && isLevelUpTime)
                _timeScaleController.SetTimeScale(_timeScaleController.timeFlowNeutral);
            
            if (networkObject != null && networkObject.isActiveAndEnabled && !networkObject.IsOwner) return;
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
        
        public void UpdateHealthUI()
        {
            healthBarSlider.value = lifePoints / maxHealth;
            textHp.text = $"{math.round(lifePoints).ToString(CultureInfo.CurrentCulture)} / {maxHealth.ToString(CultureInfo.CurrentCulture)}";
            
            Color damageEffectColor = damageEffect.color;
            damageEffectColor.a = 1f;
            damageEffect.color = damageEffectColor;
        }
        
        private void UpdateXpUI()
        {
            xpBarSlider.value = currentXP / levelXpPoints;
            textXp.text = $"{currentXP.ToString(CultureInfo.CurrentCulture)} / {levelXpPoints.ToString(CultureInfo.CurrentCulture)}";

            _effectTimer = 1f;
        }
    }
}