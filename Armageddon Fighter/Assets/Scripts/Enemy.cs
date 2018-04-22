using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public Enemy enemy;

    GameObject cursor;
    Color originalCursorColor;
    Color enemyHighlightCursorColor;

    Image nameBar;
    Image healthBar;
    Text nameText;

    float originalHealthBarWidth;

    Animation animation;

    public AnimationClip move;
    public AnimationClip attack;
    public AnimationClip die1;
    public AnimationClip die2;
    public AnimationClip die3;

    uint moveSpeedMultiplier;
    uint attackSpeed;
    uint attackRating;
    uint strength;
    uint defense;
    uint health;
    uint magic;
    uint experience;

    uint fullLife;
    int life;

    Player hero;

    Vector3 moveToPosition;
    float moveSpeed;
    bool isMoving;
    bool attackAnimating;
    bool isAlive;
    bool isDead;

    bool moveTriggered = false;

    // Use this for initialization
    void Start()
    {
        isMoving = false;
        isAlive = true;
        isDead = false;

        cursor = GameObject.FindGameObjectWithTag("Cursor");
        originalCursorColor = new Color(0.632f, 1, 1, 1);
        enemyHighlightCursorColor = Color.white;

        hero = FindObjectOfType<Player>();
        animation = enemy.GetComponent<Animation>();

        Image[] uiImages = FindObjectsOfType<Image>();
        Text[] uiText = FindObjectsOfType<Text>();

        if (enemy.name.Contains("Zombie"))
        {
            if (SceneManager.GetActiveScene().name == "Level 1")
            {
                moveSpeedMultiplier = (uint)Random.Range(0, 1);
                attackSpeed = (uint)Random.Range(0, 1);
                attackRating = (uint)Random.Range(0, 5);
                strength = (uint)Random.Range(0, 5);
                defense = (uint)Random.Range(0, 10);
                health = (uint)Random.Range(10, 30);
                magic = 0;

                experience = (uint)((moveSpeedMultiplier * 0.25) + (attackSpeed * 0.5) + (attackRating + 0.1) + strength + (defense * 0.2) + (health * 0.35));

                for (int i = 0; i < uiImages.Length; i++)
                {
                    if (uiImages[i].name == "EnemyNameBar")
                    {
                        nameBar = uiImages[i];
                    }
                    else if (uiImages[i].name == "EnemyHealthBar")
                    {
                        healthBar = uiImages[i];
                    }
                }
                for (int i = 0; i < uiText.Length; i++)
                {
                    if (uiText[i].name == "EnemyNameText")
                    {
                        nameText = uiText[i];
                        break;
                    }
                }

                fullLife = health;
            }
        }

        moveSpeed = 0.02f + (0.001f * moveSpeedMultiplier);
        life = (int)fullLife;
        //originalHealthBarWidth = healthBar.rectTransform.rect.width;

        attackAnimating = false;

        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(394.96f, 9.7f);
        originalHealthBarWidth = 394.96f;

        enemy.name = System.Text.RegularExpressions.Regex.Replace(enemy.name, " " + System.Text.RegularExpressions.Regex.Escape("(") + ".*$", "");
    }

    void FixedUpdate()
    {
        if (isAlive == true)
        {
            if (moveTriggered == true)
            {
                enemy.transform.LookAt(new Vector3(hero.transform.position.x, 0, hero.transform.position.z));
                moveToPosition = new Vector3(hero.transform.position.x, 0, hero.transform.position.z);
            }

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

                if (didMove == true)
                {
                    animation.Play(move.name);
                }

                enemy.transform.position = enemyPosition;
            }
        }
        else if (isDead == false)
        {
            isDead = true;

            animation.Play(die1.name);
            hero.AddExperience(ref experience);
            cursor.GetComponent<SpriteRenderer>().color = originalCursorColor;

            nameBar.enabled = false;

            healthBar.enabled = false;

            nameText.enabled = false;
            nameText.text = string.Empty;

            StartCoroutine(EnemyDeath());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && moveTriggered == false)
        {
            enemy.GetComponent<SphereCollider>().enabled = false;
            //animation.Play(move.name);
            moveTriggered = true;
            isMoving = true;
        }
        else if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = false;
            animation.Play(attack.name);
        }
        //else if (other is BoxCollider)
        //{
        //    CollisionLock(other);
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = false;

            animation[attack.name].speed = 1 + (0.01f * attackSpeed);
            if (attackAnimating == false)
            {
                StartCoroutine(MeleeAttack(other.gameObject));
            }
        }
        //else if (other is BoxCollider)
        //{
        //    CollisionLock(other);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(CheckIfStuck(enemy.transform.position, other));

        if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = true;
        }
    }

    private IEnumerator MeleeAttack(GameObject other)
    {
        attackAnimating = true;
        animation.Play(attack.name);

        yield return new WaitForSeconds((animation[attack.name].length * (animation[attack.name].length / (animation[attack.name].speed * animation[attack.name].length))) / 2);

        uint hitChance = (uint)Random.Range(1, 100);
        animation[attack.name].speed = 1;

        if (hitChance <= 50)
        {
            Player hero = other.GetComponent<Player>();
            hero.Damage(ref attackRating, ref strength, false);
        }

        attackAnimating = false;
    }

    private void CollisionLock(Collider other)
    {
        /*Got this code from tow sources. This was really hard for med to figure out, and these pieces work like a charm :)*/
        /*Sources:
         * https://answers.unity.com/questions/1010169/how-to-know-if-an-object-is-looking-at-an-other.html  for object looking at another
         * https://forum.unity.com/threads/stopping-movement-in-trigger-events.79638/  for not allowing an object to enter another */

        Vector3 dirFromAtoB = (other.transform.position - enemy.transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromAtoB, enemy.transform.forward);

        if (dotProd < 0.3)
        {
            Vector3 newPos = transform.position;
            newPos = other.ClosestPointOnBounds(newPos);

            Vector3 collisionDir = transform.position - newPos;
            collisionDir.Normalize();
            collisionDir *= 1.1f;
            collisionDir.x *= other.bounds.extents.x;
            collisionDir.z *= other.bounds.extents.z;

            enemy.transform.position = new Vector3(newPos.x + collisionDir.x, 0.01f, newPos.z + collisionDir.z);

            StartCoroutine(CheckIfStuck(enemy.transform.position, other));
        }
    }

    public void Damage(ref uint heroRating, ref uint heroStrength, bool attackIsMagic)
    {
        uint damage = 0;

        if (attackIsMagic == false)
        {
            damage = heroStrength - (defense + 5);

            
        }
        else
        {
            damage = heroStrength;
        }

        attackAnimating = false;
        animation.Stop();

        if (damage < life && damage < fullLife)
        {
            life -= (int)damage;
            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(originalHealthBarWidth * ((float)life / (float)fullLife), healthBar.rectTransform.rect.height);
        }
        else
        {
            life = 0;
            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, healthBar.rectTransform.rect.height);
        }

        if (life <= 0)
        {
            enemy.GetComponent<CapsuleCollider>().enabled = false;
            isAlive = false;
        }
    }

    private IEnumerator EnemyDeath()
    {
        yield return new WaitForSeconds(5);
        Destroy(enemy.gameObject);
    }

    private IEnumerator CheckIfStuck(Vector3 previousPosition, Collider other)
    {
        yield return new WaitForSeconds(0.25f);

        if (enemy.transform.position == previousPosition && isMoving == true)
        {
            Vector3 differingPositions = previousPosition - hero.transform.position;
            Bounds colliderBounds = other.bounds;

            float maxXposition = Mathf.Abs(colliderBounds.max.x - previousPosition.x);
            float minXposition = Mathf.Abs(colliderBounds.min.x - previousPosition.x);
            float maxZposition = Mathf.Abs(colliderBounds.max.z - previousPosition.z);
            float minZposition = Mathf.Abs(colliderBounds.min.z - previousPosition.z);

            if (Mathf.Abs(differingPositions.x) > 0.1 )
            {
                if (maxZposition <= minZposition)
                {
                    previousPosition.z += maxZposition + 0.01f;
                }
                else
                {
                    previousPosition.z += minZposition + 0.01f;
                }
            }

            if (Mathf.Abs(differingPositions.z) > 0.1)
            {
                if (maxXposition <= minXposition)
                {
                    previousPosition.x += maxXposition + 0.01f;
                }
                else
                {
                    previousPosition.x += minXposition + 0.01f;
                }
            }

            enemy.transform.position = previousPosition;
        }
    }

    public void showUI()
    {
        nameBar.enabled = true;

        healthBar.enabled = true;

        nameText.enabled = true;

        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(originalHealthBarWidth * ((float)life / (float)fullLife), healthBar.rectTransform.rect.height);

        nameText.text = enemy.name;
    }
}
