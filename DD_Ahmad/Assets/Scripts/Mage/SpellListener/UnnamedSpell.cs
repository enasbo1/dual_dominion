using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mage.SpellListener
{
    public class UnnamedSpell : MonoBehaviour
    {
        private class GroupOfTheSpell
        {
            public List<Animation> Bullets;
            public int BulletToMove = 0;
            public float TimeSinceLastBullet = 0f;
            public bool IsTriggerable = true;
        }
        
        public SpellManager spellManager;
        public Transform[] groups;
        public float timeBetweenBullet = 0.2f;
        
        private readonly List<GroupOfTheSpell> _groupsOfTheSpell = new List<GroupOfTheSpell>();
        private int _groupToCast;
        
        private void SpellCasted()
        {
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            
            if (!groupOfTheSpellToCast.IsTriggerable) return;

            groupOfTheSpellToCast.BulletToMove = 0;
            groupOfTheSpellToCast.TimeSinceLastBullet = -1f;
            groupOfTheSpellToCast.IsTriggerable = false;
            
            _groupToCast = (_groupToCast + 1) % groups.Length;
        }
        
        void Start()
        {
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
                    Bullets = bullets
                };
                
                _groupsOfTheSpell.Add(groupOfTheSpell);
            }

            // spellManager.GetSpellById(0).AddSpellListener(_ => SpellCasted());
        }
        
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                SpellCasted();
            }
            
            foreach (GroupOfTheSpell groupOfTheSpell in _groupsOfTheSpell.Where(group => group.Bullets.Exists(bullet => !bullet.isPlaying)))
            {
                foreach (Animation bullet in groupOfTheSpell.Bullets.Where(bullet => !bullet.isPlaying))
                {
                    bullet.gameObject.SetActive(false);
                }
            }
            
            foreach (GroupOfTheSpell groupOfTheSpell in _groupsOfTheSpell.Where(group => !group.IsTriggerable))
            {
                if (groupOfTheSpell.BulletToMove >= groupOfTheSpell.Bullets.Count)
                {
                    groupOfTheSpell.IsTriggerable = true;
                    break;
                }
                
                groupOfTheSpell.TimeSinceLastBullet -= Time.deltaTime;

                if (groupOfTheSpell.TimeSinceLastBullet <= 0)
                {
                    groupOfTheSpell.Bullets[groupOfTheSpell.BulletToMove].gameObject.SetActive(true);
                    
                    // Good to know: animation won't restart if they are being played
                    groupOfTheSpell.Bullets[groupOfTheSpell.BulletToMove].Play();

                    groupOfTheSpell.BulletToMove += 1;
                    groupOfTheSpell.TimeSinceLastBullet = timeBetweenBullet;
                }
            }
        }
    }
}