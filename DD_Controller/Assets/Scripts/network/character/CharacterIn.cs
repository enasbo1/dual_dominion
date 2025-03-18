using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace script.network.character
{
    public class CharacterIn : NetworkDataInput<CharacterData>
    {
        public Transform character;
        public Transform directionMain;
        public Transform directionIntent;
        public Animator characterAnimator;
        
        public override void InNetworkData(CharacterData networkData)
        {
            var rot = character.rotation.eulerAngles;
            rot.x  = networkData.characterInclination.x ;
            rot.z = networkData.characterInclination.y;
            character.rotation = Quaternion.Euler(rot);

            directionMain.rotation = Quaternion.Euler(networkData.headRotation);
            
            rot = directionIntent.rotation.eulerAngles;
            rot.y  = networkData.characterTargetDirection;
            directionIntent.rotation = Quaternion.Euler(rot);
            
            characterAnimator.SetInteger("WalkState", networkData.animation.WalkState);

        }
    }
}