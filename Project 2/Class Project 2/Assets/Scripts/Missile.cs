using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    // Use this for initialization
    public float speed = 30f;
    public int damage = 10;
    //[SerializeField]
    //GameObject missileprefab;

    void Start () {
        //StartCoroutine("deathTimer");
        deathtimer();
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}
    IEnumerator deathtimer()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.GetComponent<Player>() != null
            && collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponent<Player>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    //public void fire()
    //{
    //    GameObject missile = Instantiate(missileprefab);
    //    missile.transform.position = missileFireSpot.transform.position;
    //    missile.transform.rotation = missileFireSpot.transform.rotation;
    //    robot.Play("Fire");
    //}
}
