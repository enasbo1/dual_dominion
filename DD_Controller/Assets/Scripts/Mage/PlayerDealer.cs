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
        public float lifePoints;
        [SerializeField] private GameEnd gameEnd;
        
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private TextMeshProUGUI textHp;
        [SerializeField] private Image damageEffect;

        private string _stringMaxHp;
        
        private void Start()
        {
            lifePoints = maxHealth;
            _stringMaxHp = lifePoints.ToString(CultureInfo.CurrentCulture);
            textHp.text = $"{lifePoints.ToString(CultureInfo.CurrentCulture)} / {_stringMaxHp}";
            
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

        private void Update()
        {
            if (damageEffect.color.a <= 0f) return;
            
            Color damageEffectColor = damageEffect.color;
            damageEffectColor.a -= Time.deltaTime;
            damageEffect.color = damageEffectColor;
        }
        
        private void UpdateHealthUI()
        {
            healthBarSlider.value = lifePoints / maxHealth;
            textHp.text = $"{lifePoints.ToString(CultureInfo.CurrentCulture)} / {_stringMaxHp}";
            
            Color damageEffectColor = damageEffect.color;
            damageEffectColor.a = 1f;
            damageEffect.color = damageEffectColor;
        }
    }
}