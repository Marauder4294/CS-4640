using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammo : MonoBehaviour
{
    public Text BearAmmoText;
    public Text PingPongBallAmmoText;
    public Text CarAmmoText;

    public GameObject Bear;
    public GameObject PingPong;
    public GameObject Car;

    public Transform GunBarrel;

    public ushort BearAmmo;
    public ushort PingPongBallAmmo;
    public ushort CarAmmo;

    public float BearFireFrequency;
    public float PingPongBallFireFrequency;
    public float CarFireFrequency;

    public float BearProjectileSpeed;
    public float PingPongProjectileSpeed;
    public float CarProjectileSpeed;

    public RawImage BearUnderline;
    public RawImage PingPongUnderline;
    public RawImage CarUnderline;

    public AudioClip BearSound;
    public AudioClip PingPongSound;
    public AudioClip CarSound;
    public AudioClip NoAmmo;
    public AudioClip SwitchAmmo;

    private string equippedAmmo; // "Bear", "PingPong", "Car"
    private float timeToFire;


    // Use this for initialization
    void Start()
    {
        BearAmmoText.text = "x " + BearAmmo;
        PingPongBallAmmoText.text = "x " + PingPongBallAmmo;
        CarAmmoText.text = "x " + CarAmmo;

        PingPongUnderline.color = Color.white;
        CarUnderline.color = Color.white;

        equippedAmmo = "Bear";
        BearUnderline.color = Color.red;
        timeToFire = BearFireFrequency;
    }

    // Update is called once per frame
    void Update()
    {

        timeToFire = timeToFire - Time.deltaTime;

        if (Input.GetButton("Fire1") && timeToFire <= 0)
        {
            if (equippedAmmo == "Bear")
            {
                if (BearAmmo <= 0)
                {
                    NoAmmoSound();
                }
                else
                {
                    GameObject projectile = Instantiate(Bear, new Vector3(GunBarrel.position.x, GunBarrel.position.y, GunBarrel.position.z), GunBarrel.rotation);
                    projectile.GetComponent<Rigidbody>().velocity = -projectile.transform.up * BearProjectileSpeed;

                    BearAmmo--;
                    BearAmmoText.text = "x " + BearAmmo;
                    timeToFire = BearFireFrequency;

                    GetComponent<AudioSource>().PlayOneShot(BearSound);

                    Destroy(projectile, 10.0f);
                }
                
            }
            else if (equippedAmmo == "PingPong")
            {
                if (PingPongBallAmmo <= 0)
                {
                    NoAmmoSound();
                }
                else
                {
                    GameObject projectile = Instantiate(PingPong, new Vector3(GunBarrel.position.x, GunBarrel.position.y, GunBarrel.position.z), GunBarrel.rotation);
                    projectile.GetComponent<Rigidbody>().velocity = -projectile.transform.up * PingPongProjectileSpeed;

                    GetComponent<AudioSource>().PlayOneShot(PingPongSound);

                    PingPongBallAmmo--;
                    PingPongBallAmmoText.text = "x " + PingPongBallAmmo;
                    timeToFire = PingPongBallFireFrequency;

                    Destroy(projectile, 2.0f);
                }
            }
            else if (equippedAmmo == "Car")
            {
                if (CarAmmo <= 0)
                {
                    NoAmmoSound();
                }
                else
                {
                    GameObject projectile = Instantiate(Car, new Vector3(GunBarrel.position.x, GunBarrel.position.y, GunBarrel.position.z), GunBarrel.rotation);
                    projectile.GetComponent<Rigidbody>().velocity = -projectile.transform.up * CarProjectileSpeed;

                    CarAmmo--;
                    CarAmmoText.text = "x " + CarAmmo;
                    timeToFire = CarFireFrequency;

                    GetComponent<AudioSource>().PlayOneShot(CarSound);

                    StartCoroutine(ChangeCarSize(projectile));

                    Destroy(projectile, 10.0f);
                }
            }
        }
        else if (Input.GetKey(KeyCode.Keypad1) && equippedAmmo != "Bear")
        {
            ChangeAmmoSound();

            BearUnderline.color = Color.red;
            PingPongUnderline.color = Color.white;
            CarUnderline.color = Color.white;

            equippedAmmo = "Bear";
            timeToFire = 0;
        }
        else if (Input.GetKey(KeyCode.Keypad2) && equippedAmmo != "PingPong")
        {
            ChangeAmmoSound();

            BearUnderline.color = Color.white;
            PingPongUnderline.color = Color.red;
            CarUnderline.color = Color.white;

            equippedAmmo = "PingPong";
            timeToFire = 0;
        }
        else if (Input.GetKey(KeyCode.Keypad3) && equippedAmmo != "Car")
        {
            ChangeAmmoSound();

            BearUnderline.color = Color.white;
            PingPongUnderline.color = Color.white;
            CarUnderline.color = Color.red;

            equippedAmmo = "Car";
            timeToFire = 0;
        }
    }

    void NoAmmoSound()
    {
        GetComponent<AudioSource>().PlayOneShot(NoAmmo);
    }

    void ChangeAmmoSound()
    {
        GetComponent<AudioSource>().PlayOneShot(SwitchAmmo);
    }

    IEnumerator ChangeCarSize(GameObject projectile)
    {
        yield return new WaitForSeconds(0.25f);
        projectile.transform.localScale = new Vector3(.6f, .6f, .6f);
    }
}
