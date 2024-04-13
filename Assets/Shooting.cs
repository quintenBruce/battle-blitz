using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UIElements;

public class Shooting : NetworkBehaviour
{
    private NetworkVariable<Vector3> m_pointer = new NetworkVariable<Vector3>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject bullet;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            m_pointer.Value = bulletTransform.position;
        }
        else
        {
            m_pointer.OnValueChanged += OnPointerChanged;
        }
    }
    private void OnPointerChanged(Vector3 previous, Vector3 current)
    {
        bulletTransform.position = current;
    }
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            m_pointer.Value = bulletTransform.position;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {

            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 rotation = mousePos - transform.position;

            float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotz);

            if (!canFire)
            {
                timer += Time.deltaTime;
                if (timer > timeBetweenFiring)
                {
                    canFire = true;
                    timer = 0;
                }
            }
            if (Input.GetMouseButton(0) && canFire)
            {
                canFire = false;
                mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                SpawnBulletServerRPC(bulletTransform.position, Quaternion.identity, mousePos);


            }
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRPC(Vector3 position, Quaternion rotation, Vector3 mousePos)
    {
        var instance = Instantiate(bullet, position, rotation);
        BulletScript script = instance.GetComponent<BulletScript>();
        script.mousePos = mousePos;
        script.upD();
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }
}
