using UnityEngine;

namespace script.network.character
{
    public class CharacterOut : NetworkDataOutput<CharacterData>
    {
        public Transform character;
        public Transform directionMain;
        public Transform directionIntent;
        public Animator characterAnimator;

        public override CharacterData OutNetworkData()
        {
            Vector2 inclination = Vector2.zero;
            Vector3 characterInclination = character.rotation.eulerAngles;
            inclination.x = characterInclination.x;
            inclination.y = characterInclination.z;
            return new CharacterData
            {
                characterInclination = inclination,
                headRotation = directionMain.rotation.eulerAngles,
                position = character.position,
                characterTargetDirection = directionIntent.rotation.eulerAngles.y,
                animation = new CharacterAnimation
                {
                    WalkState = characterAnimator.GetInteger("WalkState")
                }
            };
        }
    }
}