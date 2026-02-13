using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform playerModel;

    [Header("Movement Settings")]
    public float speed = 7.0f;
    public float sprintSpeed = 10.0f;
    public float jumpForce = 5.0f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float detectionRadius = 0.3f;
    public Vector3 groundCheckOffset;

    [Header("Sprint Visual Effects")]
    public float sprintTiltAngle = -60f;
    public float tiltSpeed = 8f;
    public float normalFOV = 75f;
    public float sprintFOV = 90f;

    private Rigidbody rb;
    private bool isGrounded;
    private float horizontalInput;
    private float forwardInput;
    private float currentSpeed;
    
    private bool gameStarted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        currentSpeed = speed;
    }

    public void startGame(){
        gameStarted = true;
    }

    public void stopGame(){
        gameStarted = false;
    }

    void Update()
    {
        if (!gameStarted) return;
        // INPUT
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // GROUND CHECK
        isGrounded = Physics.CheckSphere(
            transform.position + groundCheckOffset,
            detectionRadius,
            groundLayer
        );

        // JUMP
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // SPRINT
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && forwardInput > 0 && !isGrounded;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && forwardInput > 0;
        
        if (isSprinting)
        {
            if (isGrounded)
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = speed;
            }
            Camera.main.fieldOfView = Mathf.Lerp(
                Camera.main.fieldOfView,
                sprintFOV,
                Time.deltaTime * 6f
            );
        }
        else
        {
            currentSpeed = speed;
            Camera.main.fieldOfView = Mathf.Lerp(
                Camera.main.fieldOfView,
                normalFOV,
                Time.deltaTime * 6f
            );
        }

        // TILT DO PERSONAGEM
        float targetTilt = isSprinting ? sprintTiltAngle : -90f;

        Quaternion targetRotation = Quaternion.Euler(
            targetTilt,
            playerModel.localEulerAngles.y,
            playerModel.localEulerAngles.z
        );

        playerModel.localRotation = Quaternion.Lerp(
            playerModel.localRotation,
            targetRotation,
            Time.deltaTime * tiltSpeed
        );
    }

    void FixedUpdate()
    {
        Vector3 moveDirection =
            (transform.forward * forwardInput + transform.right * horizontalInput).normalized;

        rb.MovePosition(
            rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime
        );
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(
            transform.position + groundCheckOffset,
            detectionRadius
        );
    }
}