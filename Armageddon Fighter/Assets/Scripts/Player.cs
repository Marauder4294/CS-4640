using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
//using System.IO;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Player hero;

    GameObject cursor;
    Color originalCursorColor;
    Color enemyHighlightCursorColor;

    RawImage uiCursor;
    Image healthGlobe;
    Image ManaGlobe;

    bool isMoving;
    bool isAttacking;
    bool isBlocking;
    bool attackAnimating;
    //bool isEngaged;

    float moveSpeed;
    uint fullLife;
    int life;
    uint mana;
    uint experience;

    bool isInUI;

    uint moveSpeedMultiplier;
    uint attackSpeed;
    uint attackRating;
    uint strength;
    uint defense;
    uint health;
    uint magic;

    Animation animation;

    public AnimationClip idle;
    //AnimationClip walk;
    public AnimationClip run;
    public AnimationClip attack;
    public AnimationClip block;
    public AnimationClip die1;
    public AnimationClip die2;

    Vector3 moveToPosition;

    // Use this for initialization
    void Start()
    {
        // TODO filestream for stats

        if (hero.name.Contains("Slasher"))
        {
            moveSpeedMultiplier = 40;
            attackSpeed = 30;
            attackRating = 40;
            strength = 25;
            defense = 20;
            health = 20;
            magic = 20;
            experience = 0;
        }
        else if (hero.name.Contains("Dasher"))
        {
            moveSpeedMultiplier = 80;
            attackSpeed = 20;
            attackRating = 15;
            strength = 10;
            defense = 10;
            health = 10;
            magic = 50;
            experience = 0;
        }
        else if (hero.name.Contains("Brute"))
        {
            moveSpeedMultiplier = 10;
            attackSpeed = 10;
            attackRating = 30;
            strength = 65;
            defense = 40;
            health = 30;
            magic = 10;
            experience = 0;
        }

        moveSpeed = 0.04f + (0.001f * moveSpeedMultiplier);
        fullLife = 2 * health;
        life = 2 * (int)health;
        mana = 2 * magic;

        isMoving = false;
        isAttacking = false;
        isBlocking = false;
        attackAnimating = false;

        //isEngaged = false;

        cursor = GameObject.FindGameObjectWithTag("Cursor");
        originalCursorColor = new Color(0.632f, 1, 1, 1);
        enemyHighlightCursorColor = Color.white;

        uiCursor = GameObject.FindGameObjectWithTag("UI Cursor").GetComponent<RawImage>();

        animation = hero.GetComponent<Animation>();

        isInUI = false;
    }

    void FixedUpdate()
    {
        if (uiCursor.enabled == true && isInUI == false)
        {
            isInUI = true;
        }
        else if (uiCursor.enabled == false && isInUI == true)
        {
            isInUI = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (isInUI == false && (isAttacking == false || cursor.GetComponent<SpriteRenderer>().color == originalCursorColor))
            {
                isMoving = true;

                moveToPosition = new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z);

                hero.transform.LookAt(moveToPosition);
            }

        }
        else if (Input.GetMouseButton(1))
        {
            if (hero.name.Contains("Slasher") == false)
            {
                isBlocking = true;
                animation.Play(block.name);
            }
            else if(isBlocking == false)
            {
                isBlocking = true;

                hero.transform.LookAt(new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z));

                animation.Stop();
                animation.Play(block.name);

                animation[block.name].time = 0.25f;
                animation[block.name].speed = 0;
            }
            else if (isBlocking == true)
            {
                hero.transform.LookAt(new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z));
            }
        }
        else if (Input.GetMouseButton(1) == false && isBlocking == true)
        {
            isBlocking = false;

            if (hero.name.Contains("Slasher") == false)
            {
                animation.Play(block.name);
            }
            else
            {
                hero.transform.LookAt(new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z));

                animation[block.name].time = 0.25f;
                animation[block.name].speed = -1;

                animation.Play(block.name);
            }
        }

        if (isMoving == true)
        {
            bool didMove = false;

            Vector3 cursorPosition = moveToPosition;
            Vector3 heroPosition = hero.transform.position;

            if (Mathf.Abs(heroPosition.x - cursorPosition.x) > 0.1)
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
            if (Mathf.Abs(heroPosition.z - cursorPosition.z) > 0.1)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other is SphereCollider == false)
        {
            CollisionLock(other);
        }

        if (other.gameObject.tag == "Enemy" && other is CapsuleCollider)
        {
            hero.GetComponent<Rigidbody>().velocity = Vector3.zero;
            hero.transform.LookAt(new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z));
            //isEngaged = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other is SphereCollider == false)
        {
            CollisionLock(other);
        }

        if (other.tag == "Enemy" && other is CapsuleCollider && attackAnimating == false)
        {
            if (Input.GetMouseButton(0) && cursor.GetComponent<SpriteRenderer>().color == enemyHighlightCursorColor)
            {
                isMoving = false;
                isAttacking = true;
                isBlocking = false;

                hero.transform.LookAt(other.transform.position);
                
                animation[attack.name].speed = 1 + (0.01f * attackSpeed);
                StartCoroutine(MeleeAttack(other.gameObject));
            }
            else
            {
                isAttacking = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && other is CapsuleCollider)
        {
            //isEngaged = false;
            isAttacking = false;
        }
    }

    private void CollisionLock(Collider other)
    {
        /*Got this code from tow sources. This was really hard for med to figure out, and these pieces work like a charm :)*/
        /*Sources:
         * https://answers.unity.com/questions/1010169/how-to-know-if-an-object-is-looking-at-an-other.html  for object looking at another
         * https://forum.unity.com/threads/stopping-movement-in-trigger-events.79638/  for not allowing an object to enter another */

        Vector3 dirFromAtoB = (other.transform.position - hero.transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromAtoB, hero.transform.forward);

        if (dotProd > 0.3)
        {
            Vector3 newPos = transform.position;
            newPos = other.ClosestPointOnBounds(newPos);

            Vector3 collisionDir = transform.position - newPos;
            collisionDir.Normalize();
            collisionDir *= 1.1f;
            collisionDir.x *= other.bounds.extents.x;
            collisionDir.z *= other.bounds.extents.z;

            hero.transform.position = new Vector3(newPos.x + collisionDir.x, 0.01f, newPos.z + collisionDir.z);
        }
    }

    private IEnumerator MeleeAttack(GameObject other)
    {
        attackAnimating = true;
        animation.Play(attack.name);

        yield return new WaitForSeconds((animation[attack.name].length * (animation[attack.name].length / (animation[attack.name].speed * animation[attack.name].length))) / 2);

        uint hitChance = (uint)Random.Range(0, 100);
        animation[attack.name].speed = 1;
        attackAnimating = false;

        if (hitChance <= 75)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.Damage(ref attackRating, ref strength, false);
        }
    }

    public void Damage(ref uint enemyRating, ref uint enemyStrength, bool attackIsMagic)
    {
        //if (attackIsMagic == false)
        //{
        //    uint damage = enemyStrength;

        //    animation.Stop();

        //    if (damage < life && damage < fullLife)
        //    {
        //        life -= (int)damage;
        //        healthGlobe.GetComponent<Sprite>().sizeDelta = new Vector2(healthBar.rectTransform.rect.width, originalHealthGlobeHeight * ((float)life / (float)fullLife));
        //    }
        //    else
        //    {
        //        life = 0;
        //        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, healthBar.rectTransform.rect.height);
        //    }
        //}
        //else
        //{

        //}

        //if (life <= 0)
        //{
        //    enemy.GetComponent<CapsuleCollider>().enabled = false;
        //    animation.Stop();
        //    isAlive = false;
        //}
    }

    public void AddExperience(ref uint amount)
    {
        experience += amount;
    }
}
