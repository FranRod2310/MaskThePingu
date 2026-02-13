using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;
    public Quaternion rotationOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = rotationOffset;
        transform.position = player.transform.position + offset;
    }
}
