using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpawn : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    GameObject[] robots;
    private int timeSpawned;
    private int healthBonus = 0;
	void Start () {
		
	}

    public void SpawnRobot()
    {
        timeSpawned++;
        healthBonus += 1 * timeSpawned;
        GameObject robot = Instantiate(robots[Random.Range(0, robots.Length)]);
        robot.transform.position = transform.position;
        robot.GetComponent<Robot>().health += healthBonus;
    }
}
