using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cursor : MonoBehaviour {

    public GameObject cursor;

    Color originalColor;
    Color enemyHighlightColor;

    Vector3 zeroedScreenVector;
    Vector3 screenMaxSizeVector;

    Image[] uiImages;
    Text[] uiText;

    float uiBarHeight;

    Vector3 cursorScreenPosition;
    Vector3 cursorPosition;

    bool isUICursor;

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

        uiImages = FindObjectsOfType<Image>();
        uiText = FindObjectsOfType<Text>();

        uiBarHeight = uiImages[1].rectTransform.rect.height;

        //if (SceneManager.GetActiveScene().name != "Main Menu")
        //{
        //    isUICursor = false;
            
        //    cursor[0].SetActive(true);
        //    cursor[1].SetActive(false);
        //}
        //else
        //{
        //    isUICursor = true;

        //    cursor[1].SetActive(true);
        //    cursor[0].SetActive(false);
        //}
    }

    void FixedUpdate()
    {
        cursorScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(cursor.transform.position.x + Input.GetAxis("Mouse X"), 0.01f, cursor.transform.position.z + Input.GetAxis("Mouse Y")));

        cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Clamp(cursorScreenPosition.x, zeroedScreenVector.x, screenMaxSizeVector.x),
                Mathf.Clamp(cursorScreenPosition.y, zeroedScreenVector.y, screenMaxSizeVector.y),
                Mathf.Clamp(cursorScreenPosition.z, zeroedScreenVector.z, screenMaxSizeVector.z)));

        cursor.transform.position = new Vector3(cursorPosition.x, 0.01f, cursorPosition.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && other is CapsuleCollider)
        {
            cursor.GetComponent<SpriteRenderer>().color = enemyHighlightColor;

            uiImages[0].enabled = true;

            uiImages[2].enabled = true;

            uiText[0].enabled = true;
            uiText[0].text = other.gameObject.name;
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

            uiImages[0].enabled = false;

            uiImages[2].enabled = false;

            uiText[0].enabled = false;
            uiText[0].text = string.Empty;
        }
        else if (other.gameObject.tag == "Pickup")
        {
            
        }
    }
}
