using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    // Use this for initialization

    public float fireRate;
    protected float lastFireTime;
    public Ammo ammo;
    public AudioClip liveFire;
    public AudioClip dryFire;
    public float zoomFactor;
    public int range;
    public int damage;

    private float zoomFOV;
    private float zoomSpeed = 6;

    public AudioClip deathSound;
    public AudioClip weakHitSound;

    void Start()
    {
        zoomFOV = Constants.CameraDefaultZoom / zoomFactor;
        lastFireTime = Time.time - 10;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomFOV, zoomSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    Camera.main.fieldOfView = Constants.CameraDefaultZoom;
        //}
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomFOV, zoomSpeed * Time.deltaTime);
        }
        else
        {
            Camera.main.fieldOfView = Constants.CameraDefaultZoom;
        }
    }

    protected void Fire()
    {
        if (ammo.HasAmmo(tag))
        {
            GetComponent<AudioSource>().PlayOneShot(liveFire);
            ammo.ConsumeAmmo(tag);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(dryFire);
        }
        GetComponentInChildren<Animator>().Play("Fire");
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range))
        {
            processHit(hit.collider.gameObject);
        }
    }

    private void processHit(GameObject hitObject)
    {
        if (hitObject.tag == "Bullseye")
        {
            GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");

            for (int i = 0; i <= robots.Length - 1; i++)
            {
                if (robots[i].tag == "Robot")
                {
                    Destroy(robots[i].gameObject);
                    GetComponent<AudioSource>().PlayOneShot(weakHitSound);
                    GetComponent<AudioSource>().PlayOneShot(deathSound);
                }
            }
        }
        else if (hitObject.GetComponent<Player>() != null)
        {
            hitObject.GetComponent<Player>().TakeDamage(damage);
        }
        else if (hitObject.GetComponent<Robot>() != null)
        {
            hitObject.GetComponent<Robot>().TakeDamage(damage);
        }
    }
}
