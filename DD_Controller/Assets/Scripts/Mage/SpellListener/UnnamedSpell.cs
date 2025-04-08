using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mage.SpellListener
{
    public class UnnamedSpell : MonoBehaviour
    {
        private class GroupOfTheSpell
        {
            public List<Animation> bullets;
            public int bulletToMove;
            public float timeSinceLastBullet;
            public bool isTriggerable = true;
        }
        
        public SpellManager spellManager;
        public Transform[] groups;
        public float timeBetweenBullet = 0.2f;
        public int spellId = 6;
        
        private Spell _unnamedSpell;
        
        private readonly List<GroupOfTheSpell> _groupsOfTheSpell = new List<GroupOfTheSpell>();
        private int _groupToCast;
        
        private void SpellCasted()
        {
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            
            if (!groupOfTheSpellToCast.isTriggerable) return;

            groupOfTheSpellToCast.bulletToMove = 0;
            groupOfTheSpellToCast.timeSinceLastBullet = -1f;
            groupOfTheSpellToCast.isTriggerable = false;
            
            _groupToCast = (_groupToCast + 1) % groups.Length;
        }
        
        void Start()
        {
            _unnamedSpell = spellManager.GetSpellById(spellId);
            
            foreach (Transform bulletPivots in groups)
            {
                List<Animation> bullets = new List<Animation>();
                foreach (Transform bulletPivot in bulletPivots)
                {
                    Animation bullet = bulletPivot.GetComponentInChildren<Animation>();
                    bullet.gameObject.SetActive(false);
                    bullets.Add(bullet);
                }

                GroupOfTheSpell groupOfTheSpell = new GroupOfTheSpell
                {
                    bullets = bullets
                };
                
                _groupsOfTheSpell.Add(groupOfTheSpell);
            }

            _unnamedSpell.AddSpellListener(_ => SpellCasted());
        }
        
        void FixedUpdate()
        {
            foreach (GroupOfTheSpell groupOfTheSpell in _groupsOfTheSpell.Where(group => group.bullets.Exists(bullet => !bullet.isPlaying)))
            {
                foreach (Animation bullet in groupOfTheSpell.bullets.Where(bullet => !bullet.isPlaying))
                {
                    bullet.gameObject.SetActive(false);
                }
            }
            
            foreach (GroupOfTheSpell groupOfTheSpell in _groupsOfTheSpell.Where(group => !group.isTriggerable))
            {
                if (groupOfTheSpell.bulletToMove >= groupOfTheSpell.bullets.Count)
                {
                    groupOfTheSpell.isTriggerable = true;
                    break;
                }
                
                groupOfTheSpell.timeSinceLastBullet -= Time.deltaTime;

                if (groupOfTheSpell.timeSinceLastBullet <= 0)
                {
                    groupOfTheSpell.bullets[groupOfTheSpell.bulletToMove].gameObject.SetActive(true);
                    
                    // Good to know: animation won't restart if they are being played
                    groupOfTheSpell.bullets[groupOfTheSpell.bulletToMove].Play();

                    groupOfTheSpell.bulletToMove += 1;
                    groupOfTheSpell.timeSinceLastBullet = timeBetweenBullet;
                }
            }
        }
    }
}