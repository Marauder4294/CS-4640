using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class Player : MonoBehaviour
{
    public GameObject hero;
    GameObject enemy;

    GameObject cursor;

    bool isMoving;
    bool lockXmovement;
    bool lockZmovement;
    float moveSpeed;

    uint moveSpeedMultiplier;
    uint attackSpeed;
    uint strength;
    uint defense;
    uint health;
    uint magic;

    public AnimationClip idle;
    AnimationClip walk;
    public AnimationClip run;
    public AnimationClip attack;
    public AnimationClip block;
    AnimationClip die;

    Vector3 moveToPosition;

    bool isEngaged;

    // Use this for initialization
    void Start()
    {
        // TODO filestream for stats

        if (hero.name.Contains("Slasher"))
        {
            moveSpeedMultiplier = 40;
            attackSpeed = 30;
            strength = 20;
            defense = 20;
            health = 20;
            magic = 20;
        }
        else if (hero.name.Contains("Dasher"))
        {
            moveSpeedMultiplier = 80;
            attackSpeed = 20;
            strength = 10;
            defense = 10;
            health = 10;
            magic = 50;
        }
        else if (hero.name.Contains("Brute"))
        {
            moveSpeedMultiplier = 10;
            attackSpeed = 10;
            strength = 60;
            defense = 40;
            health = 30;
            magic = 10;
        }

        moveSpeed = 0.04f + (0.001f * moveSpeedMultiplier);

        isMoving = false;
        isEngaged = false;
        lockXmovement = false;
        lockZmovement = false;

        cursor = GameObject.FindGameObjectWithTag("Cursor");
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (isEngaged == true)
            {
                hero.transform.LookAt(enemy.transform.position);
                hero.GetComponent<Animation>().Play(attack.name);
                Animation animation = hero.GetComponent<Animation>();
                animation[attack.name].speed = 1 + (0.01f * attackSpeed);
                animation.Play(attack.name);
            }
            else
            {
                isMoving = true;

                moveToPosition = new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z);

                hero.transform.LookAt(moveToPosition);
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (Regex.IsMatch(hero.name, "Slasher") == false)
            {
                hero.GetComponent<Animation>().Play(block.name);
            }
            else
            {
                hero.transform.LookAt(new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z));

                Animation animation = hero.GetComponent<Animation>();
                animation.Play(block.name);

                StartCoroutine(HoldAnimation(animation, block.name));
            }
        }

        if (isMoving == true)
        {
            bool didMove = false;

            Vector3 cursorPosition = moveToPosition;
            Vector3 heroPosition = hero.transform.position;

            if (Mathf.Abs(heroPosition.x - cursorPosition.x) > 0.1)
            {
                if (lockXmovement == false)
                {
                    didMove = true;

                    float test = cursorPosition.x - heroPosition.x;

                    if (test > 0)
                    {
                        heroPosition.x += moveSpeed;
                    }
                    else if (test < 0)
                    {
                        heroPosition.x -= moveSpeed;
                    }

                    if (Mathf.Abs(test) < moveSpeed)
                    {
                        heroPosition.x = cursorPosition.x;
                    }
                }
            }
            if (Mathf.Abs(heroPosition.z - cursorPosition.z) > 0.1)
            {
                if (lockZmovement == false)
                {
                    didMove = true;

                    float test = cursorPosition.z - heroPosition.z;

                    if (test > 0)
                    {
                        heroPosition.z += moveSpeed;
                    }
                    else if (test < 0)
                    {
                        heroPosition.z -= moveSpeed;
                    }

                    if (Mathf.Abs(test) < moveSpeed)
                    {
                        heroPosition.z = cursorPosition.z;
                    }
                }
            }

            if (didMove == false)
            {
                isMoving = false;
                hero.GetComponent<Animation>().Play(idle.name);
            }
            else
            {
                hero.GetComponent<Animation>().Play(run.name);
            }

            Vector3 cursorScreenPlacement = Camera.main.WorldToScreenPoint(cursor.transform.position);

            hero.transform.position = heroPosition;
            Camera.main.transform.position = new Vector3(heroPosition.x + 2.53f, 7.9f, heroPosition.z + 5.14f);

            cursorScreenPlacement = Camera.main.ScreenToWorldPoint(cursorScreenPlacement);
            cursor.transform.position = new Vector3(cursorScreenPlacement.x, 0.01f, cursorScreenPlacement.z);
        }
    }

    IEnumerator HoldAnimation(Animation animation, string animClip)
    {
        yield return new WaitForSeconds(0.25f);

        animation[block.name].time = 0.25f;
        animation[block.name].speed = 0;
        animation[block.name].weight = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && this.gameObject.tag == "Player" && other is CapsuleCollider)
        {
            enemy = other.gameObject;
            hero.transform.LookAt(new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z));
            isEngaged = true;
        }
        else if (other is BoxCollider)
        {
            Vector3 boundary = other.transform.position;
            Vector3 heroDirection = this.gameObject.transform.position;
            if (Mathf.Abs(heroDirection.x - boundary.x) <= 0.2)
            {
                collisionXlock(this.gameObject, other.gameObject);
            }
            if (Mathf.Abs(heroDirection.z - boundary.z) <= 0.2)
            {
                collisionZlock(this.gameObject, other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other is BoxCollider)
        {
            Vector3 boundary = other.transform.position;
            Vector3 heroDirection = this.gameObject.transform.position;
            if (Mathf.Abs(heroDirection.x - boundary.x) <= 0.2)
            {
                collisionXlock(this.gameObject, other.gameObject);
            }
            else
            {
                lockXmovement = false;
            }
            if (Mathf.Abs(heroDirection.z - boundary.z) <= 0.2)
            {
                collisionZlock(this.gameObject, other.gameObject);
            }
            else
            {
                lockZmovement = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && this.gameObject.tag == "Player" && other is CapsuleCollider)
        {
            enemy = null;
            isEngaged = false;
        }
        else if (other is BoxCollider)
        {
            lockXmovement = false;
            lockZmovement = false;
        }
    }

    private void collisionXlock(GameObject player, GameObject other)
    {
        lockXmovement = true;
        lockZmovement = false;
        Vector3 oppositeMovement = new Vector3(player.transform.position.x, 0, (player.transform.position.z - other.transform.position.z) * -1);

        FindObjectOfType<Cursor>().gameObject.transform.position = oppositeMovement;

        hero.transform.LookAt(FindObjectOfType<Cursor>().gameObject.transform.position);
    }

    private void collisionZlock(GameObject player, GameObject other)
    {
        lockZmovement = true;
        lockXmovement = false;
        Vector3 oppositeMovement = new Vector3((player.transform.position.x - other.transform.position.x) * -1, 0, player.transform.position.z);

        FindObjectOfType<Cursor>().gameObject.transform.position = oppositeMovement;

        hero.transform.LookAt(FindObjectOfType<Cursor>().gameObject.transform.position);
    }
}
