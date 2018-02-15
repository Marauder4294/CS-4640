using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;
    private GameController gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            return;
        }
        else if ((other.tag == "MegaShot" && this.tag == "Player") || (this.tag == "MegaShot" && other.tag == "Player"))
        {
            return;
        }
        else if (this.tag == "Asteroid" || this.tag == "Enemy" || other.name == "Enemy-Bolt (Clone)")
        {
            if (this.name == "Boss(Clone)")
            {
                BossController boss = this.GetComponent<BossController>();

                if (other.name == "Bolt(Clone)")
                {
                    boss.bossHP -= 2;
                }
                else if (other.name == "Mega-Bolt(Clone)")
                {
                    boss.bossHP -= 50;
                }

                if (boss.bossHP <= 0)
                {
                    Instantiate(explosion, transform.position, transform.rotation);
                    UpdateScoreAndCurrentObject();
                    gameController.GameWin();
                    gameController.StartCoroutine(checkBoss());
                }

            }
            else if (this.tag != "PowerUp" && other.tag != "PowerUp")
            {
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(other.gameObject);
                UpdateScoreAndCurrentObject();
            }
        }
        else if (this.tag == "PowerUp" && other.tag == "Player")
        {
            Destroy(gameObject);
        }
        else if (this.tag == "Player" && other.tag == "PowerUp")
        {
            if (other.name == "Multi Shot PowerUp(Clone)")
            {
                PlayerController player = this.GetComponent<PlayerController>();
                player.isMultiShot = true;
            }
            else if (other.name == "Blast Shield PowerUp(Clone)")
            {
                Instantiate(gameController.shield, new Vector3(0, 0, 3), new Quaternion(0, 0, 0, 0));
                Instantiate(gameController.shield, new Vector3(0, 0, 3.3f), new Quaternion(0, 0, 0, 0));
                Instantiate(gameController.shield, new Vector3(0, 0, 3.6f), new Quaternion(0, 0, 0, 0));
            }

            Destroy(other.gameObject);
        }
        else if (this.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            gameController.GameOver();
            Destroy(other.gameObject);
            UpdateScoreAndCurrentObject();
            gameController.StartCoroutine(checkBoss());
        }
    }

    private void UpdateScoreAndCurrentObject()
    {
        gameController.AddScore(scoreValue);
        Destroy(gameObject);
    }

    IEnumerator checkBoss()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}