using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
    public Transform elevator;
    private Animator arenaAnimator;
    private SphereCollider sphereCollider;
	void Start () {
        arenaAnimator = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Camera.main.transform.parent.gameObject.GetComponent<CameraMovement>().enabled = false;
        player.transform.parent = elevator.transform;
        player.GetComponent<PlayerController>().enabled = false;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.elevatorArrived);
        arenaAnimator.SetBool("OnElevator", true);
    }

    public void ActivatePlatform()
    {
        sphereCollider.enabled = true;
    }
}
