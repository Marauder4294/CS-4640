using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour {

    public Button quit;
    public Button boss;
    public Button start;

    void Start()
    {
        quit.onClick.AddListener(() => { quitButtonClick();  } );
        boss.onClick.AddListener(() => { bossButtonClick(); });
        start.onClick.AddListener(() => { startButtonClick(); });
    }

    void quitButtonClick()
    {
        Application.Quit();
    }

    void bossButtonClick()
    {
        SceneManager.LoadScene("Boss", LoadSceneMode.Single);
    }

    void startButtonClick()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
