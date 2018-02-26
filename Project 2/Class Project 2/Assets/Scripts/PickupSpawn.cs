using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawn : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    private GameObject[] pickups;
    private GameObject pickup;

    private void Start()
    {
        StartCoroutine("respawnPickup");
    }

    void spawnPickup()
    {
        pickup = Instantiate(pickups[Random.Range(0, pickups.Length - 1)]);
        pickup.transform.position = transform.position;
        pickup.transform.parent = transform;
    }

    IEnumerator respawnPickup()
    {
        yield return new WaitForSeconds(20);
        spawnPickup();
    }

    public void PickupWasPickedUp()
    {
        StartCoroutine("respawnPickup");
    }
}
