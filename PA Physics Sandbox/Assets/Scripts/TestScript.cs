using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform objectTransform;
    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        var movementIntent = new Vector3(
            speed,
            0f,
            0f
        ).normalized;
        
        objectTransform.position += movementIntent * Time.deltaTime;
    }
}
