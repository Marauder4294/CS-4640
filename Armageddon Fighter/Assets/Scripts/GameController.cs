using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Image healthGlobe;
    public Text enemyNameText;
    public Image enemyTextBar;
    public Image enemyHealthBar;

    // Use this for initialization
    void Start()
    {
        enemyHealthBar.enabled = false;
        enemyTextBar.enabled = false;
        enemyNameText.enabled = false;
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{

    //}
}
