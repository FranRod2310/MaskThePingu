using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private float turnSpeed = 150.0f;
    private float mouseX;
    private float mouseY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + mouseX * turnSpeed * Time.deltaTime, 0);
    }
}
