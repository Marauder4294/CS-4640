using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject hazard;
    public GameObject enemy;
    public GameObject player;
    public GameObject multiShot;
    public GameObject blastShield;
    public GameObject shield;
    public GameObject boss;
    public Vector3 spawnValues;
    public Vector3 spawnEnemyValues;
    public ushort hazardCount;
    public ushort enemyCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;
    public int powerWaitMax;
    private int powerWait;
    public ushort waveCount;
    public bool isMultiShot = false;
    public bool isBlastShield = false; 

    public GUIText scoreText;
    public GUIText restartText;
    public GUIText gameOverText;

    private bool gameOver;
    private bool restart;
    private int score;
    private ushort wave = 0;

    void Start()
    {
        gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;
        UpdateScore();
        Instantiate(player, new Vector3(0,0,0), new Quaternion(0,0,0,0));
        if (SceneManager.GetActiveScene().name == "Main")
        {
            StartCoroutine(SpawnWaves());
        }
        else if (SceneManager.GetActiveScene().name == "Boss")
        {
            Instantiate(boss, new Vector3(0, 0, 12), new Quaternion(0, 0, 0, 0));
        }
        
        StartCoroutine(SpawnPower());
    }

    void Update()
    {
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        powerWait = Random.Range(0, powerWaitMax);
        while (true)
        {
            powerWait = 3;
            if (wave < waveCount)
            {
                if ((wave + 1) % 5 == 0)
                {
                    spawnAsteroids();
                    spawnEnemyShips();
                    yield return new WaitForSeconds(spawnWait);
                }
                else if (wave % 2 == 0)
                {
                    for (ushort i = 0; i < hazardCount; i++)
                    {
                        spawnAsteroids();
                        yield return new WaitForSeconds(spawnWait);
                    }

                    wave++;
                }
                else if (wave % 2 == 1)
                {

                    for (ushort i = 0; i < enemyCount; i++)
                    {
                        spawnEnemyShips();
                        yield return new WaitForSeconds(spawnWait);
                    }

                    wave++;
                }
            }
            else
            {
                SceneManager.LoadScene("Boss", LoadSceneMode.Single);
            }

            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }
        }
    }

    IEnumerator SpawnPower()
    {
        yield return new WaitForSeconds(Random.Range(0, powerWaitMax));

        while (true)
        {
            int chosen = Random.Range(1, 3);

            if (chosen == 1)
            {
                Instantiate(multiShot, new Vector3(Random.Range(-6, 6), 0, Random.Range(-4, 8)), multiShot.transform.rotation);
            }
            else
            {
                Instantiate(blastShield, new Vector3(Random.Range(-6, 6), 0, Random.Range(-4, 8)), blastShield.transform.rotation);
            }

            yield return new WaitForSeconds(Random.Range(0, powerWaitMax));
        }
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    void spawnAsteroids()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(hazard, spawnPosition, spawnRotation);
    }

    void spawnEnemyShips()
    {
        Vector3 spawnEnemyPosition = new Vector3(Random.Range(-spawnEnemyValues.x, spawnEnemyValues.x), spawnEnemyValues.y, Random.Range(spawnValues.z, 12.0f));
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(enemy, spawnEnemyPosition, spawnRotation);
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!\n Final " + scoreText.text;
        gameOver = true;
        hazardCount = 0;
        enemyCount = 0;
    }

    public void GameWin()
    {
        gameOverText.text = "The Boss is defeated!\nYou have saved the galaxy!\nFinal " + scoreText.text;
        gameOver = true;
        hazardCount = 0;
        enemyCount = 0;
    }
}