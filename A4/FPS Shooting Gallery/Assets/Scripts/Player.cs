using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float horizontalSensitivity;
    public float verticalSensitivity;
    public float mouseSensitivity;
    public bool mouseInversion;
    private short inversion;

    // Use this for initialization
    void Start () {
        Cursor.visible = false;

        if (mouseInversion == true)
        {
            inversion = 1;
        }
        else
        {
            inversion = -1;
        }
	}
	
	// Update is called once per frame
	void Update () {

        float mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseVertical = inversion * Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0, mouseHorizontal, 0, Space.Self);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        Camera.main.transform.Rotate(mouseVertical, 0, 0);
        Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * verticalSensitivity);// , Space.Self);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * Time.deltaTime * verticalSensitivity);// , Space.Self);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * Time.deltaTime * horizontalSensitivity);// , Space.Self);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * Time.deltaTime * horizontalSensitivity);// , Space.Self);
        }
    }
}
