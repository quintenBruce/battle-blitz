using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject bullet;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 rotation = mousePos - transform.position;

        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        if (!canFire )
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring )
            {
                canFire = true;
                timer = 0;
            }
        }
        transform.rotation = Quaternion.Euler(0, 0, rotz);
        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            var instance = Instantiate(bullet, bulletTransform.position, Quaternion.identity);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();


        }
    }
}
