using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    public GameObject spell;
    uint strength;
    uint attackRating;
    GameObject hero;
    Vector3 cursorPosition;

    AudioSource audioSource;

    public AudioClip initial;
    public AudioClip impact;

    // Use this for initialization
    void Start () {
        hero = FindObjectOfType<Player>().gameObject;
        cursorPosition = GameObject.FindGameObjectWithTag("Cursor").transform.position;
        spell.transform.position = new Vector3(hero.transform.position.x, 1, hero.transform.position.z);
        spell.transform.LookAt(cursorPosition);

        if (spell.name.Contains("FireBall"))
        {
            strength = 15;
            attackRating = 1000;
        }

        audioSource = spell.GetComponent<AudioSource>();

        audioSource.PlayOneShot(initial);
	}
	
	// Update is called once per frame
	void Update () {
        spell.transform.Rotate(0, 0, 45f, Space.Self);
        spell.transform.position += new Vector3(spell.transform.forward.x / 3, 0, spell.transform.forward.z / 3);
	}

    private void OnTriggerEnter(Collider other)
    {
        GameObject.Find("Canvas").GetComponent<AudioSource>().PlayOneShot(impact);

        if (other.gameObject.tag == "Enemy" && other is CapsuleCollider)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.Damage(ref attackRating, ref strength, true);

            Destroy(spell);
        }
        else if (other.tag == "Boundary")
        {
            Destroy(spell);
        }
    }
}
