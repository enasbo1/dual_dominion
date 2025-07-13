using UnityEngine;

namespace PlayerSpace.Mage.UI
{
    public class MageUIRendererScript : MonoBehaviour
    {
        [Header("Arrows color when a spell can be cast")]
        public Color upArrowColor = new Color(255, 182, 0, 255);

        public Color rightArrowColor = new Color(0, 255, 21, 255);
        public Color downArrowColor = new Color(0, 157, 255, 255);
        public Color leftArrowColor = new Color(255, 0, 128, 255);

        [Header("Arrows color when a spell cannot be cast")]
        public Color upDisabledArrowColor = new Color(110, 101, 78, 255);

        public Color rightDisabledArrowColor = new Color(81, 106, 83, 255);
        public Color downDisabledArrowColor = new Color(77, 97, 110, 255);
        public Color leftDisabledArrowColor = new Color(106, 83, 95, 255);

        [Header("Spell background color when it can't be cast")]
        public Color spellBackgroundColorOnCooldown = new Color(77, 77, 77, 255);

        [Header("Spell background color on cast")]
        public Color spellBackgroundColorInCast = new Color(255, 255, 255, 255);

        [Header("Spell name color when it can't be cast")]
        public Color spellNameColorOnCooldown = new Color(136, 136, 136, 255);

        [Header("Spell name color on cast")] public Color spellNameColorOnCast = new Color(255, 255, 255, 255);
    }
}