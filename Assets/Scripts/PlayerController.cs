using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    private Animator animator;
    public GameObject[] spawnPoints;
    private static int lastSpawnIndex = -1;
    public Image healthBar;
    private NetworkVariable<float> healthAmount = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public void Start()
    {
        //ulong id = NetworkManager.Singleton.LocalClientId;
        //var x = NetworkManager.Singleton.ConnectedClientsIds;
        //GetComponent<NetworkObject>().ChangeOwnership(x.Last());
        //ulong id2 = NetworkManager.Singleton.LocalClientId;
        animator = GetComponent<Animator>();
        //get canvas and search for healthbar Image
        Canvas c = gameObject.GetComponentInChildren<Canvas>();
        foreach (Image im in c.GetComponentsInChildren<Image>())
        {
            if (im.CompareTag("HealthBar")) healthBar = im;

        }
        
            healthAmount.Value = 100f;
            healthAmount.OnValueChanged += OnHealthChanged;
    }
    private void OnHealthChanged(float previous, float current)
    {
        healthBar.fillAmount = current / 100f;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.Singleton.ConnectedClientsIds.Last());



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
        //string coll = collision.otherCollider.GetType().ToString();
        //if (coll.Contains("EdgeCollider"))
        //{ //struck the body

        //}
            if (collision.gameObject.name == "Bullet(Clone)" || collision.gameObject.name.Equals("Bullet(Clone)"))
            {
                takeDamage(10);
            }
    }
    private void takeDamage(int damage)
    {
        float temp = healthAmount.Value;
        temp -= damage;
        temp = Mathf.Clamp(temp, 0, 100);
        healthAmount.Value = temp;
        healthBar.fillAmount = temp / 100f;
   }


}
