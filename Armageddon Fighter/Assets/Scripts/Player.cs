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
    public GameObject spell1;
    Color originalCursorColor;
    Color enemyHighlightCursorColor;

    RawImage uiCursor;
    Image healthGlobe;
    Image manaGlobe;
    Image experienceBar;
    Text beltSlot1;
    Text beltSlot2;
    Text beltSlot3;

    bool isMoving;
    bool isAttacking;
    bool isBlocking;
    bool attackAnimating;
    bool isAlive;
    bool isDead;

    float moveSpeed;
    uint fullLife;
    int life;
    uint fullMana;
    int mana;
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

    GameObject minXBoundary;
    GameObject maxXBoundary;

    GameObject minZBoundary;
    GameObject maxZBoundary;

    GameObject minYBoundary;

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

        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");
        Image[] uiImages = FindObjectsOfType<Image>();
        Text[] uiText = FindObjectsOfType<Text>();

        for (int i = 0; i < boundaries.Length; i++)
        {
            if (boundaries[i].name == "BoundaryXMin")
            {
                minXBoundary = boundaries[i];
            }
            else if (boundaries[i].name == "BoundaryXMax")
            {
                maxXBoundary = boundaries[i];
            }
            else if (boundaries[i].name == "BoundaryZMin")
            {
                minZBoundary = boundaries[i];
            }
            else if (boundaries[i].name == "BoundaryZMax")
            {
                maxZBoundary = boundaries[i];
            }
            else if (boundaries[i].name == "BoundaryYMin")
            {
                minYBoundary = boundaries[i];
            }
        }
        for (int i = 0; i < uiImages.Length; i++)
        {
            if (uiImages[i].name == "HealthGlobe")
            {
                healthGlobe = uiImages[i];
            }
            else if (uiImages[i].name == "MagicGlobe")
            {
                manaGlobe = uiImages[i];
            }
            else if (uiImages[i].name == "ExperienceBar")
            {
                experienceBar = uiImages[i];
            }
        }
        for (int i = 0; i < uiText.Length; i++)
        {
            if (uiText[i].name == "BeltSlot1Amount")
            {
                beltSlot1 = uiText[i];
            }
            else if (uiText[i].name == "BeltSlot2Amount")
            {
                beltSlot2 = uiText[i];
            }
            else if (uiText[i].name == "BeltSlot3Amount")
            {
                beltSlot3 = uiText[i];
            }
        }

        moveSpeed = 0.04f + (0.001f * moveSpeedMultiplier);
        fullLife = 2 * health;
        life = (int)fullLife;
        fullMana = 2 * magic;
        mana = (int)fullMana;

        beltSlot1.text = "5";
        beltSlot2.text = "2";
        beltSlot3.text = "1";

        isMoving = false;
        isAttacking = false;
        isBlocking = false;
        attackAnimating = false;

        isAlive = true;
        isDead = false;

        cursor = GameObject.FindGameObjectWithTag("Cursor");
        originalCursorColor = new Color(0.632f, 1, 1, 1);
        enemyHighlightCursorColor = Color.white;

        uiCursor = GameObject.FindGameObjectWithTag("UI Cursor").GetComponent<RawImage>();

        animation = hero.GetComponent<Animation>();

        isInUI = false;
    }

    void FixedUpdate()
    {
        if (isAlive == true)
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

                    moveToPosition = new Vector3(cursor.transform.position.x, 0.1f, cursor.transform.position.z);

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
                else if (isBlocking == false)
                {
                    isBlocking = true;

                    hero.transform.LookAt(new Vector3(cursor.transform.position.x, 0.1f, cursor.transform.position.z));

                    animation.Stop();
                    animation.Play(block.name);

                    animation[block.name].time = 0.25f;
                    animation[block.name].speed = 0;
                }
                else if (isBlocking == true)
                {
                    hero.transform.LookAt(new Vector3(cursor.transform.position.x, 0.1f, cursor.transform.position.z));
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
                    hero.transform.LookAt(new Vector3(cursor.transform.position.x, 0.1f, cursor.transform.position.z));

                    animation[block.name].time = 0.25f;
                    animation[block.name].speed = -1;

                    animation.Play(block.name);
                }
            }
            else if (Input.GetKey(KeyCode.Alpha1) && attackAnimating == false)
            {
                if (attackAnimating == false)
                {
                    int cost = 7;

                    if (mana >= cost)
                    {
                        hero.transform.LookAt(cursor.transform);
                        StartCoroutine(MagicAttack(cost));
                    }
                }
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                int amount = 0;
                int.TryParse(beltSlot1.text, out amount);

                if (amount > 0)
                {
                    life = (int)fullLife;
                    healthGlobe.fillAmount = 1;

                    beltSlot1.text = (amount - 1).ToString();
                }
            }
            else if (Input.GetKey(KeyCode.W))
            {
                int amount = 0;
                int.TryParse(beltSlot2.text, out amount);

                if (amount > 0)
                {
                    mana = (int)fullMana;
                    manaGlobe.fillAmount = 1;

                    beltSlot2.text = (amount - 1).ToString();
                }
            }
            else if (Input.GetKey(KeyCode.E))
            {
                int amount = 0;
                int.TryParse(beltSlot3.text, out amount);

                if (amount > 0)
                {
                    life = (int)fullLife;
                    healthGlobe.fillAmount = 1;
                    mana = (int)fullMana;
                    manaGlobe.fillAmount = 1;

                    beltSlot3.text = (amount - 1).ToString();
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
                cursor.transform.position = new Vector3(Mathf.Clamp(cursorScreenPlacement.x, minXBoundary.transform.position.x + 1, maxXBoundary.transform.position.x - 1), 0.01f,
                    Mathf.Clamp(cursorScreenPlacement.z, minZBoundary.transform.position.z + 1, maxZBoundary.transform.position.z - 1));
            }
        }
        else if (isDead == false)
        {
            isDead = true;

            animation.Stop();

            animation.Play(die1.name);

            animation[die1.name].time = animation[die1.name].length;
            animation[die1.name].speed = 0;
            //StartCoroutine(HeroDeath());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other is SphereCollider == false && other.tag != "Boundary")
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
        if (other is SphereCollider == false && other.tag != "Boundary")
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
                if (attackAnimating == false)
                {
                    StartCoroutine(MeleeAttack(other.gameObject));
                }
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
        /*Got this code from two sources. This was really hard for med to figure out, and these pieces work like a charm :)*/
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

        uint hitChance = (uint)Random.Range(1, 100);
        animation[attack.name].speed = 1;
        attackAnimating = false;

        if (hitChance <= 75)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.Damage(ref attackRating, ref strength, false);
        }
    }
    private IEnumerator MagicAttack(int cost)
    {
        attackAnimating = true;

        animation[block.name].speed = 1 + (0.01f * attackSpeed);
        animation.Play(block.name);

        yield return new WaitForSeconds((animation[block.name].length * (animation[block.name].length / (animation[block.name].speed * animation[block.name].length))) / 2);

        Instantiate(spell1);

        mana -= cost;
        manaGlobe.fillAmount = (float)mana / (float)fullMana;

        animation[block.name].speed = 1;
        attackAnimating = false;
    }

    public void Damage(ref uint enemyRating, ref uint enemyStrength, bool attackIsMagic)
    {
        uint damage = enemyStrength;

        if (damage < life && damage < fullLife)
        {
            life -= (int)damage;
            healthGlobe.fillAmount = (float)life / (float)fullLife;
        }
        else
        {
            life = 0;
            healthGlobe.fillAmount = 0;
        }

        isAttacking = false;
        attackAnimating = false;
        animation[attack.name].time = 0;
        animation.Stop();

        if (life <= 0)
        {
            hero.GetComponent<CapsuleCollider>().enabled = false;
            cursor.SetActive(false);
            isAlive = false;
        }
    }

    private IEnumerator HeroDeath()
    {
        yield return new WaitForSeconds(5);
    }

    public void AddExperience(ref uint amount)
    {
        experience += amount;
    }
}
