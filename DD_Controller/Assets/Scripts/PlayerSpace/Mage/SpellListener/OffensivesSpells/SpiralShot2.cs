using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class SpiralShot2 : MonoBehaviour
    {
        public Transform characterTransform;
        public SpellManager spellManager;
        public Transform castedGroupsPosition;
        public Transform defaultGroupsPosition;
        public float timeBetweenBullet = 0.2f;

        private readonly List<GroupOfTheSpell> _groupsOfTheSpell = new List<GroupOfTheSpell>();
        private int _groupCount;
        private int _groupToCast;

        private Spell _spiralShot2;
        
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
            _spiralShot2 = spellManager.GetSpellByName("Spiral Shot 2");
            
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

            _spiralShot2.AddSpellListener(_ => OnSpellCast());
            _spiralShot2.AddSpellFailureListener(_ => OnSpellCastedAsFailure());
        }
        
        private void GroupReset(GroupOfTheSpell group)
        {
            group.objectTransform.parent = defaultGroupsPosition;
            group.objectTransform.localPosition = Vector3.zero;
            group.isTriggerable = true;
                        
            foreach (GroupBullet bullet in group.bullets.Where(
                         bullet => !bullet.bulletAnimation.isPlaying))
            {
                bullet.gameObject.SetActive(false);
                bullet.transform.localPosition = Vector3.zero;
                bullet.bulletCollider.enabled = false;
            }
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
            
            groupOfTheSpellToCast.bullets.ForEach(bullet => bullet.gameObject.SetActive(spellManager.spellToCast.id == _spiralShot2.id));
        }

        private void OnSpellCast()
        {
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            if (!groupOfTheSpellToCast.isTriggerable) return;

            groupOfTheSpellToCast.objectTransform.parent = castedGroupsPosition;
            groupOfTheSpellToCast.bulletToMove = 0;
            groupOfTheSpellToCast.timeSinceLastBullet = -1f;
            groupOfTheSpellToCast.isTriggerable = false;

            _groupToCast = (_groupToCast + 1) % _groupCount;
        }
        
        private void OnSpellCastedAsFailure()
        {
            GroupOfTheSpell groupOfTheSpellToCast = _groupsOfTheSpell[_groupToCast];
            GroupReset(groupOfTheSpellToCast);
        }
    }
}