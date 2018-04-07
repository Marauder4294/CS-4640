using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemy;

    public AnimationClip move;
    public AnimationClip attack;
    public AnimationClip die1;
    public AnimationClip die2;
    public AnimationClip die3;

    GameObject hero;

    Vector3 moveToPosition;
    float moveSpeed;
    bool isMoving;

    bool moveTriggered = false;

    // Use this for initialization
    void Start()
    {
        hero = FindObjectOfType<Player>().gameObject;
        moveSpeed = 0.02f;
        isMoving = false;
    }

    void FixedUpdate()
    {
        if (moveTriggered == true)
        {
            enemy.transform.LookAt(new Vector3(hero.transform.position.x, 0, hero.transform.position.z));
            moveToPosition = new Vector3(hero.transform.position.x, 0, hero.transform.position.z);
        }

        //if (moveTriggered == true)
        //{
        //    enemy.transform.LookAt(new Vector3(hero.transform.position.x, 0, hero.transform.position.z));
        //    moveToPosition = new Vector3(hero.transform.position.x, 0, hero.transform.position.z);
        //    isMoving = true;
        //}

        if (isMoving == true)
        {
            bool didMove = false;

            Vector3 heroPosition = moveToPosition;
            Vector3 enemyPosition = enemy.transform.position;

            if (Mathf.Abs(enemyPosition.x - heroPosition.x) > 0.9)
            {
                didMove = true;

                float test = heroPosition.x - enemyPosition.x;

                if (test > 0)
                {
                    enemyPosition.x += moveSpeed;
                }
                else if (test < 0)
                {
                    enemyPosition.x -= moveSpeed;
                }

                if (Mathf.Abs(test) < moveSpeed)
                {
                    enemyPosition.x = heroPosition.x;
                }
            }
            if (Mathf.Abs(enemyPosition.z - heroPosition.z) > 0.9)
            {
                didMove = true;

                float test = heroPosition.z - enemyPosition.z;

                if (test > 0)
                {
                    enemyPosition.z += moveSpeed;
                }
                else if (test < 0)
                {
                    enemyPosition.z -= moveSpeed;
                }

                if (Mathf.Abs(test) < moveSpeed)
                {
                    enemyPosition.z = heroPosition.z;
                }
            }

            //if (didMove == false)
            //{
            //    enemy.GetComponent<Animation>().Play(attack.name);
            //}

            if (didMove == true)
            {
                enemy.GetComponent<Animation>().Play(move.name);
            }

            enemy.transform.position = enemyPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && moveTriggered == false)
        {
            enemy.GetComponent<SphereCollider>().enabled = false;
            //enemy.GetComponent<Animation>().Play(move.name);
            moveTriggered = true;
            isMoving = true;
        }
        else if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = false;
            enemy.GetComponent<Animation>().Play(attack.name);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = false;
            enemy.GetComponent<Animation>().Play(attack.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = true;
        }
    }
}
