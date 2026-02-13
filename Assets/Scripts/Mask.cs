using UnityEngine;

public class Mask : MonoBehaviour
{

    public GameObject player;

    void Start()
    {
        player = GameManager.instance.player.gameObject;   
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 1.0f)
        {
            GameManager.instance.Addmasks(1);
            Destroy(gameObject);
        }
    } 
}
