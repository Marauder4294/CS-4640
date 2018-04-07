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
        originalHealthBarWidth = healthBar.rectTransform.rect.width;
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
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = false;
            animation[attack.name].speed = 1 + (0.01f * attackSpeed);
            animation.Play(attack.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && moveTriggered == true)
        {
            isMoving = true;
        }
    }

    public void Damage(ref uint heroRating, ref uint heroStrength, bool attackIsMagic)
    {
        if (attackIsMagic == false)
        {
            uint damage = heroStrength - (defense + 5);

            life -= (int)damage;
            animation.Stop();

            if (damage < life && damage < fullLife)
            {
                healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBar.rectTransform.rect.width * (fullLife / life), healthBar.rectTransform.rect.height);
            }
            else
            {
                healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBar.rectTransform.rect.width * 0, healthBar.rectTransform.rect.height);
            }
        }
        else
        {

        }

        if (life <= 0)
        {
            enemy.GetComponent<CapsuleCollider>().enabled = false;
            animation.Stop();
            isAlive = false;
        }
    }

    private IEnumerator EnemyDeath()
    {
        yield return new WaitForSeconds(5);
        Destroy(enemy.gameObject);
    }
}
