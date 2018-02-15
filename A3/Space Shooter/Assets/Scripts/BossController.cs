using UnityEngine;
using System.Collections;

[System.Serializable]
public class BossBoundary
{
    public float xMin, xMax, zMin, zMax;
}

public class BossController : MonoBehaviour
{
    public float speed;
    public float tilt;
    public EnemyBoundary boundary;
    public float nextMove;
    private float nextMoveTime = 0.0f;
    public ushort bossHP;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    private float moveHorizontal;
    private float moveVertical;

    private float nextFire;

    void Update()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            GetComponent<AudioSource>().Play();
        }
    }

    void FixedUpdate()
    {
        if (nextMoveTime == 0.0f)
        {
            moveHorizontal = Random.Range(-3.0f, 3.0f);
            moveVertical = Random.Range(-3.0f, 3.0f);

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            GetComponent<Rigidbody>().velocity = movement * Random.Range(0.25f, speed);
        }

        if (nextMoveTime < nextMove)
        {
            GetComponent<Rigidbody>().position = new Vector3
            (
                Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
                0.0f,
                Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
            );

            GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 180.0f, GetComponent<Rigidbody>().velocity.x * -tilt);

            nextMoveTime += Time.time;
        }
        else
        {
            nextMoveTime = 0.0f;
        }
    }
}