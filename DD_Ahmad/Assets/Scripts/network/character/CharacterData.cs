using UnityEngine;

namespace script.network.character
{
    public struct CharacterAnimation
    {
        public int WalkState;
    }
    public struct CharacterData
    {
        public Vector3 position;
        public Vector3 headRotation;
        public Vector2 characterInclination;
        public float characterTargetDirection;
        public CharacterAnimation animation;

        public CharacterData(Vector3 position, Vector3 headRotation, Vector2 characterInclination,
            float characterTargetDirection)
        {
            this.position = position;
            this.headRotation = headRotation;
            this.characterInclination = characterInclination;
            this.characterTargetDirection = characterTargetDirection;
            animation = new CharacterAnimation();
        }
    }
}