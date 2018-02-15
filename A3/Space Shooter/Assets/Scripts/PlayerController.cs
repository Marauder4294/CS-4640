using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Boundary boundary;

    public GameObject shot;
    public GameObject megaShot;
    public Transform shotSpawn;
    public Transform shotSpawn2;
    public Transform shotSpawn3;
    public float fireRate;
    public bool isMultiShot = false;
    private ushort megaForeCount = 0;

    private float nextFire;
    private float nextMegaFire;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            if (isMultiShot == true)
            {
                Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation);
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                Instantiate(shot, shotSpawn3.position, shotSpawn3.rotation);
            }
            else
            {
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            }
            GetComponent<AudioSource>().Play();
        }
        else if (Input.GetButtonDown("Fire2") && SceneManager.GetActiveScene().name == "Boss" && megaForeCount < 2)
        {
            nextMegaFire = Time.time + 3;
        }
        else if (Input.GetButtonUp("Fire2") && Time.time > nextMegaFire && SceneManager.GetActiveScene().name == "Boss" && megaForeCount < 2)
        {
            Instantiate(megaShot, shotSpawn.position, shotSpawn.rotation);
            megaForeCount++;
            nextMegaFire = Time.time + 15;
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        GetComponent<Rigidbody>().velocity = movement * speed;

        GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
        );

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }
}