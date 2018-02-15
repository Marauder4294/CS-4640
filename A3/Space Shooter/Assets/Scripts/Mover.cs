using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
    public float speed;

    void Start()
    {
        if (this.name == "Asteroid(Clone)")
        {
            GetComponent<Rigidbody>().velocity = transform.forward * Random.Range(-0.01f, speed);
        }
        else
        {
            GetComponent<Rigidbody>().velocity = transform.forward * speed;
        }
    }
}