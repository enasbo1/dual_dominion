using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mage.SpellListener
{
    public class UnnamedSpell : MonoBehaviour
    {
        public SpellManager spellManager;
        public Transform castedGroupsPosition;
        public Transform defaultGroupsPosition;
        public float timeBetweenBullet = 0.2f;
        public int spellId = 6;

        private readonly List<GroupOfTheSpell> _groupsOfTheSpell = new List<GroupOfTheSpell>();
        private int _groupCount;
        private int _groupToCast;

        private Spell _unnamedSpell;
        
        private class GroupOfTheSpell
        {
            public Transform objectTransform;
            public List<GroupBullet> bullets;
            public int bulletToMove;
            public bool isTriggerable = true;
            public float timeSinceLastBullet;
        }
        
        private struct GroupBullet
        {
            public GameObject gameObject;
            public Transform transform;
            public Collider bulletCollider;
            public Animation bulletAnimation;
        }

        private void Start()
        {
            _unnamedSpell = spellManager.GetSpellById(spellId);
            
            foreach (Transform group in defaultGroupsPosition.transform)
            {
                _groupCount += 1;
                List<GroupBullet> bullets = new List<GroupBullet>();
                foreach (Transform bulletPivot in group)
                {
                    Animation bulletAnimation = bulletPivot.GetComponentInChildren<Animation>();
                    
                    GroupBullet groupBullet = new GroupBullet
                    {
                        gameObject = bulletAnimation.gameObject,
                        transform = bulletAnimation.transform,
                        bulletAnimation = bulletAnimation,
                        bulletCollider = bulletAnimation.GetComponent<Collider>()
                    };

                    groupBullet.gameObject.SetActive(false);
                    groupBullet.bulletCollider.enabled = false;
                    bullets.Add(groupBullet);
                }

                GroupOfTheSpell groupOfTheSpell = new GroupOfTheSpell
                {
                    bullets = bullets,
                    objectTransform = group
                };

                _groupsOfTheSpell.Add(groupOfTheSpell);
            }

            _unnamedSpell.AddSpellListener(_ => SpellCasted());
        }

        private void FixedUpdate()
        {
            foreach (GroupOfTheSpell groupOfTheSpell in _groupsOfTheSpell.Where(group => !group.isTriggerable))
            {
                foreach (GroupBullet bullet in groupOfTheSpell.bullets.Where(
                             bullet => !bullet.bulletAnimation.isPlaying))
                {
                    bullet.gameObject.SetActive(false);
                    bullet.transform.localPosition = Vector3.zero;
                    bullet.bulletCollider.enabled = false;
                }
                
                if (groupOfTheSpell.bulletToMove >= groupOfTheSpell.bullets.Count)
                {
                    if (!groupOfTheSpell.bullets[groupOfTheSpell.bulletToMove - 1].bulletAnimation.isPlaying)
                    {
                        groupOfTheSpell.objectTransform.parent = defaultGroupsPosition;
                        groupOfTheSpell.objectTransform.localPosition = Vector3.zero;
                        groupOfTheSpell.isTriggerable = true;
                        break;
                    }
                    continue;
                }

                groupOfTheSpell.timeSinceLastBullet -= Time.deltaTime;

                if (groupOfTheSpell.timeSinceLastBullet <= 0)
                {
                    GroupBullet bulletToMove = groupOfTheSpell.bullets[groupOfTheSpell.bulletToMove];
                    bulletToMove.gameObject.SetActive(true);
                    bulletToMove.bulletCollider.enabled = true;

                    // Good to know: animation won't restart if they are being played
                    bulletToMove.bulletAnimation.Play();

                    groupOfTheSpell.bulletToMove += 1;
                    groupOfTheSpell.timeSinceLastBullet = timeBetweenBullet;
                }
            }
            
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            if (!groupOfTheSpellToCast.isTriggerable) return;
            
            groupOfTheSpellToCast.bullets.ForEach(bullet => bullet.gameObject.SetActive(spellManager.spellToCast.id == spellId));
        }

        private void SpellCasted()
        {
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            if (!groupOfTheSpellToCast.isTriggerable) return;

            groupOfTheSpellToCast.objectTransform.parent = castedGroupsPosition;
            groupOfTheSpellToCast.bulletToMove = 0;
            groupOfTheSpellToCast.timeSinceLastBullet = -1f;
            groupOfTheSpellToCast.isTriggerable = false;

            _groupToCast = (_groupToCast + 1) % _groupCount;
        }
    }
}