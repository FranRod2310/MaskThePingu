using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI textComponent;
    public GameObject player;
    public GameObject maskPrefab;
    private int masks = 0;
    public int masksCount = 5;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public LayerMask obstacleLayer;
    public GameObject Storm;
    public GameObject playerGroup;
    public GameObject canvas;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject groundWell;
    private ParticleSystem ps;
    private bool gameStarted = false;
    private float stormSpeed = 0.35f;
    public GameObject[] miniPengus;
    private System.Collections.Generic.List<GameObject> spawnedMasks = new System.Collections.Generic.List<GameObject>();

    // Resets the game state to initial conditions
    private void resetGameState() {
        leftDoor.transform.rotation = Quaternion.Euler(-180, 0, -180);
        rightDoor.transform.rotation = Quaternion.Euler(-180, 0, 0);
        if (ps == null) ps = Storm.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        shape.donutRadius = 3.0f;
        masks = 0;
        playerGroup.transform.position = new Vector3(-0.45f, 1.06f, -0.27f);
        playerGroup.transform.rotation = Quaternion.Euler(0, 50, 0);
        canvas.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        foreach (GameObject miniPengu in miniPengus) {
            if (miniPengu != null)
            miniPengu.SetActive(false);
        }
        foreach (GameObject mask in spawnedMasks) {
            if (mask != null) 
            Destroy(mask);
        }
        spawnedMasks.Clear();
    }

    private void winGame() {
        // Implementation for winning the game
        gameStarted = false;
        winCanvas.SetActive(true);
        PlayerController pc = playerGroup.GetComponent<PlayerController>();
        pc.stopGame();
    }

    private void loseGame() {
        // Implementation for losing the game
        gameStarted = false;
        loseCanvas.SetActive(true);
        PlayerController pc = playerGroup.GetComponent<PlayerController>();
        pc.stopGame();
    }

    public void backToMenu() {
        // Implementation for returning to the main menu
        gameStarted = false;
        canvas.SetActive(true);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        PlayerController pc = playerGroup.GetComponent<PlayerController>();
        pc.stopGame();
    }

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void startGame(){
        resetGameState();
        gameStarted = true;
        PlayerController pc = playerGroup.GetComponent<PlayerController>();
        pc.startGame();
        textComponent.color = Color.black;
        textComponent.text = masks.ToString() + " / " + masksCount.ToString();

        ps = Storm.GetComponent<ParticleSystem>();
        int spawnedCount = 0;
        int maxAttempts = 100; // Prevents an infinite loop if the area is too crowded

        while (spawnedCount < masksCount && maxAttempts > 0){
            maxAttempts--;
            float x = Random.Range(-18.0f, 18.0f);
            float z = Random.Range(-18.0f, 18.0f);
            Vector3 spawnPos = new Vector3(x, 1f, z);

            // Perform the check
            bool isBlocked = Physics.CheckSphere(spawnPos, 0.1f, obstacleLayer);

            if (!isBlocked) {
                GameObject newMask = Instantiate(maskPrefab, spawnPos, Quaternion.identity);
                spawnedMasks.Add(newMask);
                spawnedCount++;
                // Draw a GREEN line pointing up where a mask successfully spawned
                Debug.DrawRay(spawnPos, Vector3.up * 5, Color.green, 10f);
            }
            else {
                // Draw a RED line where the spawn was REJECTED
                Debug.DrawRay(spawnPos, Vector3.up * 2, Color.red, 10f);
            }
        }

        if (maxAttempts <= 0) Debug.LogWarning("Could not find enough empty space for all masks!");
    }

    void Start() {
        resetGameState();
        canvas.SetActive(true);
    }

    public void Addmasks(int amount) {
        masks += amount;
        miniPengus[masks - 1].SetActive(true);
        textComponent.text = masks.ToString() + " / " + masksCount.ToString();
        Debug.Log("Masks left: " + (masksCount - masks));
        if (masks >= masksCount) {
            Debug.Log("-- All Masks collected --");
            leftDoor.transform.Rotate(-90, 0, 0);
            rightDoor.transform.Rotate(-90, 0, 0);
            textComponent.text = "All Masks Collected!";
            textComponent.color = Color.green;
        }
    }

    void Update() {
        if (!gameStarted) return;
        var shape = ps.shape;
        if (shape.donutRadius < shape.radius)
            shape.donutRadius += stormSpeed * Time.deltaTime;

        checkPlayerInStorm(shape.radius, shape.donutRadius);
        checkWinCondition();
    }

    void checkPlayerInStorm(float currentRadius, float currentTubeThickness) {
        float distance = Vector3.Distance(player.transform.position, Storm.transform.position);

        // The tube extends outwards and inwards from the 'radius' line
        float innerDangerEdge = currentRadius - currentTubeThickness;

        if (distance >= innerDangerEdge) {
            loseGame();
            Debug.Log("Player is INSIDE the growing fog wall!");
            // Reduce player health here
        }
    }

    void checkWinCondition() {
        if (masks >= masksCount) {
            if (masks >= masksCount) {
                RaycastHit hit;
                // Use player.transform.position instead of transform.position!
                if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 1.5f))
                {
                    Debug.Log("Raycast hit: " + hit.collider.name);
                    // Using CompareTag is safer and faster than comparing names
                    if (hit.collider.CompareTag("WinCondition"))
                    {
                        winGame();
                        Debug.Log("Player has reached the win condition!");
                    }
                }
            }
        }
    }
}