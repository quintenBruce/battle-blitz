using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    private Animator animator;
    public GameObject[] spawnPoints;
    private static int lastSpawnIndex = -1;
    public Image healthBar;
    private float healthAmount = 100f;


    public void Start()
    {
        animator = GetComponent<Animator>();
        //get canvas and search for healthbar Image
        Canvas c = gameObject.GetComponentInChildren<Canvas>();
        foreach (Image im in c.GetComponentsInChildren<Image>())
        {
            if (im.CompareTag("HealthBar")) healthBar = im;

        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Check if spawnPoints array is not empty
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn points are not assigned.");
            return;
        }

        // Choose the next spawn point index
        int nextSpawnIndex = (lastSpawnIndex + 1) % spawnPoints.Length;
        lastSpawnIndex = nextSpawnIndex;

        // Get the transform of the chosen spawn point
        Transform chosenSpawnPoint = spawnPoints[nextSpawnIndex].transform;

        // Move the spawned object to the chosen spawn point position
        transform.position = chosenSpawnPoint.position;
        transform.rotation = chosenSpawnPoint.rotation;
    }


    // Called once per frame
    private void Update()
    {
        if (!IsOwner) { return; }
        // Get the input for horizontal and vertical movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Normalize the movement vector to ensure consistent speed in all directions
        movement = new Vector2(horizontalInput, verticalInput).normalized;

        // Update the animator parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.magnitude);
    }

    // Called on a fixed schedule regardless of fps. Good for physics
    private void FixedUpdate()
    {
        // Move the Rigidbody using the normalized movement vector
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)" || collision.gameObject.name.Equals("Bullet(Clone)"))
        {
            takeDamage(10);
        }
    }

    private void takeDamage(int damage)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / 100f;
    }


}
