using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class GrowingShot : MonoBehaviour
    {
        public Transform characterTransform;
        public SpellManager spellManager;
        public Transform castedGroupsPosition;
        public Transform defaultGroupsPosition;
        public float timeBetweenBullet = 0.2f;
        public int spellId = 8;
        public float defaultDamageMax = 25f;

        private readonly List<GroupOfTheSpell> _groupsOfTheSpell = new List<GroupOfTheSpell>();
        private int _groupCount;
        private int _groupToCast;

        private Spell _growingShot;
        
        private class GroupOfTheSpell
        {
            public Transform objectTransform;
            public List<GroupBullet> bullets;
            public int bulletToMove;
            public bool isTriggerable = true;
            public float timeSinceLastBullet;
            public float bulletsSizeToAdd;
            public Vector3 bulletsScale;
            public float growMultiplayer = 1;
        }
        
        private class GroupBullet
        {
            public GameObject gameObject;
            public Transform transform;
            public Animation bulletAnimation;
            public Collider bulletCollider;
            public DamageDealerScript damageDealer;
        }

        private void GroupReset(GroupOfTheSpell group)
        {
            group.objectTransform.parent = defaultGroupsPosition;
            group.objectTransform.localPosition = Vector3.zero;
            group.isTriggerable = true;
            group.bulletsScale = Vector3.one;
            group.growMultiplayer = 1;
            
            foreach (GroupBullet bullet in group.bullets)
            {
                bullet.gameObject.SetActive(false);
                bullet.transform.localPosition = Vector3.zero;
                bullet.bulletCollider.enabled = false;
                bullet.damageDealer.damageMax = defaultDamageMax;
            }
        }

        private void Start()
        {
            _growingShot = spellManager.GetSpellById(spellId);
            
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
                        bulletCollider = bulletAnimation.GetComponent<Collider>(),
                        damageDealer = bulletAnimation.GetComponent<DamageDealerScript>()
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
                
                GroupReset(groupOfTheSpell);
                _groupsOfTheSpell.Add(groupOfTheSpell);
            }

            _growingShot.AddSpellListener(_ => SpellCasted());
            _growingShot.AddSpellFailureListener(_ => SpellCastedAsFailure());
        }

        private void FixedUpdate()
        {
            foreach (GroupOfTheSpell groupOfTheSpell in _groupsOfTheSpell.Where(group => !group.isTriggerable))
            {
                // Remove bullet who are at their destination
                foreach (GroupBullet bullet in groupOfTheSpell.bullets.Where(
                             bullet => !bullet.bulletAnimation.isPlaying))
                {
                    bullet.gameObject.SetActive(false);
                }
                
                // Spell reset
                if (groupOfTheSpell.bulletToMove >= groupOfTheSpell.bullets.Count)
                {
                    if (!groupOfTheSpell.bullets[groupOfTheSpell.bulletToMove - 1].bulletAnimation.isPlaying)
                    {
                        GroupReset(groupOfTheSpell);
                        break;
                    }
                    continue;
                }

                // Spell bullet launch
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
            
            foreach (GroupOfTheSpell groupOfTheSpell in _groupsOfTheSpell.Where(group => group.objectTransform.parent == defaultGroupsPosition))
            {
                groupOfTheSpell.objectTransform.rotation = characterTransform.rotation;
            }
            
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            if (!groupOfTheSpellToCast.isTriggerable) return;
            
            bool isSpellToCast = spellManager.spellToCast.id == spellId && _growingShot.canBeCast;

            if (isSpellToCast)
            {
                float sizeToAdd = (0.25f / (groupOfTheSpellToCast.growMultiplayer * groupOfTheSpellToCast.growMultiplayer + 1)) * groupOfTheSpellToCast.growMultiplayer;
                Vector3 bulletsScale = groupOfTheSpellToCast.bulletsScale;
                
                groupOfTheSpellToCast.bulletsSizeToAdd = sizeToAdd;
                groupOfTheSpellToCast.bulletsScale = new Vector3(bulletsScale.x + sizeToAdd, bulletsScale.y + sizeToAdd, bulletsScale.z + sizeToAdd);
                groupOfTheSpellToCast.growMultiplayer += Time.deltaTime;
            }
            
            groupOfTheSpellToCast.bullets.ForEach(bullet =>
            {
                bullet.gameObject.SetActive(isSpellToCast);
                
                if (!isSpellToCast) return;
                
                float sizeToAdd = groupOfTheSpellToCast.bulletsSizeToAdd;
                Vector3 bulletsScalePreCast = groupOfTheSpellToCast.bulletsScale / 2f;
                
                Vector3 bulletPos = bullet.transform.localPosition;
                bulletPos = new Vector3(bulletPos.x, bulletsScalePreCast.y / 2f + 3f, bulletPos.z);
                bullet.transform.localScale = bulletsScalePreCast;
                bullet.transform.localPosition = bulletPos;
                bullet.damageDealer.damageMax += sizeToAdd * 1.2f;
            });
        }

        private void SpellCasted()
        {
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            if (!groupOfTheSpellToCast.isTriggerable) return;

            groupOfTheSpellToCast.objectTransform.parent = castedGroupsPosition;
            groupOfTheSpellToCast.bulletToMove = 0;
            groupOfTheSpellToCast.timeSinceLastBullet = -1f;
            groupOfTheSpellToCast.isTriggerable = false;
            groupOfTheSpellToCast.bullets.ForEach(bullet => bullet.transform.localScale = groupOfTheSpellToCast.bulletsScale);

            _groupToCast = (_groupToCast + 1) % _groupCount;
        }
        
        private void SpellCastedAsFailure()
        {
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            GroupReset(groupOfTheSpellToCast);
        }
    }
}