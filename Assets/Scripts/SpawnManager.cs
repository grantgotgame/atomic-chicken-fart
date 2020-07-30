using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{    
    private float startDelay = 2;
    private float repeatRate = 2;

    private Vector3 spawnPos = new Vector3(25, 0, 0);

    public GameObject[] obstaclePrefab;
    private int obstacleIndex;

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        InvokeRepeating("SpawnObstacle", startDelay, repeatRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObstacle()
    {
        // Select random obstacle
        obstacleIndex = Random.Range(0, obstaclePrefab.Length);

        if (playerControllerScript.gameOver == false)
        {
            Instantiate(obstaclePrefab[obstacleIndex], spawnPos, obstaclePrefab[obstacleIndex].transform.rotation);
        }
    }
}
