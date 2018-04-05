using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cursor : MonoBehaviour {

    public GameObject cursor;
    public GameObject uiCursor;

    Color originalColor;
    Color enemyHighlightColor;

    Vector3 zeroedScreenVector;
    Vector3 screenMaxSizeVector;

    //Image[] uiImages;
    //Text[] uiText;

    float uiBarHeight;

    Vector3 cursorScreenPosition;
    Vector3 cursorPosition;

    bool isUICursor;

    Image enemyNameBar;
    Image enemyHealthBar;
    Text enemyNameText;

    // Use this for initialization
    void Start () {

        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;

        originalColor = new Color(0.632f, 1, 1, 1);
        enemyHighlightColor = Color.white;

        float xLimit = Screen.width * 0.01302f;
        float yLimit = Screen.height * 0.00926f;

        zeroedScreenVector = new Vector3(xLimit, yLimit, 0);
        screenMaxSizeVector = new Vector3(Screen.width - xLimit, Screen.height - yLimit, 19);

        Image[] uiImages = FindObjectsOfType<Image>();
        Text[]  uiText = FindObjectsOfType<Text>();

        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            isUICursor = false;
            
            cursor.transform.position = new Vector3(0, 0.01f, 0);
            uiCursor.GetComponent<RawImage>().rectTransform.sizeDelta = new Vector2(40, 40);

            for (int i = 0; i < uiImages.Length; i++)
            {
                if (uiImages[i].name == "EnemyNameBar")
                {
                    enemyNameBar = uiImages[i];
                }
                else if (uiImages[i].name == "EnemyHealthBar")
                {
                    enemyHealthBar = uiImages[i];
                }
                else if (uiImages[i].name == "UI Bar")
                {
                    uiBarHeight = uiImages[i].rectTransform.rect.height;
                }
            }
            for (int i = 0; i < uiText.Length; i++)
            {
                if (uiText[i].name == "EnemyNameText")
                {
                    enemyNameText = uiText[i];
                }
            }
        }
        else
        {
            isUICursor = true;
            uiBarHeight = Screen.height;

            uiCursor.GetComponent<RawImage>().rectTransform.sizeDelta = new Vector2(100, 100);
            cursor.transform.position = new Vector3(0, 100, 0);
        }

        uiCursor.GetComponent<RawImage>().enabled = isUICursor;
    }

    void FixedUpdate()
    {

        if (isUICursor == false)
        {
            cursorScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(cursor.transform.position.x + Input.GetAxis("Mouse X"), 0.01f, cursor.transform.position.z + Input.GetAxis("Mouse Y")));

            if (cursorScreenPosition.y > uiBarHeight)
            {
                cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Clamp(cursorScreenPosition.x, zeroedScreenVector.x, screenMaxSizeVector.x),
                    Mathf.Clamp(cursorScreenPosition.y, zeroedScreenVector.y, screenMaxSizeVector.y),
                    Mathf.Clamp(cursorScreenPosition.z, zeroedScreenVector.z, screenMaxSizeVector.z)));

                cursor.transform.position = new Vector3(cursorPosition.x, 0.01f, cursorPosition.z);
            }
            else
            {
                isUICursor = true;
                cursor.transform.position = new Vector3(cursorScreenPosition.x, 100 , cursorScreenPosition.z);
                uiCursor.GetComponent<RawImage>().enabled = isUICursor;

                uiCursor.transform.position = new Vector3(cursorScreenPosition.x - (40 * Input.GetAxis("Mouse X")), uiBarHeight - (40 * Input.GetAxis("Mouse Y")), 0);
            }
        }
        else
        {
            if (uiCursor.transform.position.y <= uiBarHeight)
            {
                uiCursor.transform.position = new Vector3(Mathf.Clamp(uiCursor.transform.position.x - (40 * Input.GetAxis("Mouse X")), zeroedScreenVector.x, screenMaxSizeVector.x),
                Mathf.Clamp(uiCursor.transform.position.y - (40 * Input.GetAxis("Mouse Y")), zeroedScreenVector.y, screenMaxSizeVector.y), 0);
            }
            else
            {
                isUICursor = false;
                uiCursor.GetComponent<RawImage>().enabled = isUICursor;

                cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Clamp(uiCursor.transform.position.x + Input.GetAxis("Mouse X"), zeroedScreenVector.x, screenMaxSizeVector.x),
                    Mathf.Clamp(cursorScreenPosition.y, zeroedScreenVector.y, screenMaxSizeVector.y),
                    Mathf.Clamp(cursorScreenPosition.z + Input.GetAxis("Mouse Y"), zeroedScreenVector.z, screenMaxSizeVector.z)));

                cursor.transform.position = new Vector3(cursorPosition.x, 0.01f, cursorPosition.z);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && other is CapsuleCollider)
        {
            cursor.GetComponent<SpriteRenderer>().color = enemyHighlightColor;

            enemyNameBar.enabled = true;

            enemyHealthBar.enabled = true;

            enemyNameText.enabled = true;
            enemyNameText.text = other.gameObject.name;
        }
        else if (other.gameObject.tag == "Pickup")
        {
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && other is CapsuleCollider)
        {
            cursor.GetComponent<SpriteRenderer>().color = originalColor;

            enemyNameBar.enabled = false;

            enemyHealthBar.enabled = false;

            enemyNameText.enabled = false;
            enemyNameText.text = string.Empty;
        }
        else if (other.gameObject.tag == "Pickup")
        {
            
        }
    }
}
