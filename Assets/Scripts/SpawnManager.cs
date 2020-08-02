using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{    
    private float startDelay = 2;
    private float repeatRate = 2;

    private float spawnPosX = 30f;
    private float spawnPosY = 0f;
    private float spawnPosZ = 0f;
    private Vector3 spawnPos;

    public GameObject[] obstaclePrefab;
    private int obstacleIndex;

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
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

        // spawn obstacles
        if (!playerControllerScript.gameOver && playerControllerScript.gameHasStarted)
        {
            Instantiate(obstaclePrefab[obstacleIndex], spawnPos, obstaclePrefab[obstacleIndex].transform.rotation);
            // stack boxes and barrels
            if (obstacleIndex != 0)
            {
                float obstacleHeight;
                obstacleHeight = obstaclePrefab[obstacleIndex].GetComponent<BoxCollider>().size.y;
                Instantiate(obstaclePrefab[obstacleIndex], new Vector3(spawnPosX, spawnPosY + obstacleHeight, spawnPosZ),
                    obstaclePrefab[obstacleIndex].transform.rotation);
                // stack barrels higher
                if (obstacleIndex == 2)
                {
                    Instantiate(obstaclePrefab[obstacleIndex], new Vector3(spawnPosX, spawnPosY + obstacleHeight * 2, spawnPosZ),
                    obstaclePrefab[obstacleIndex].transform.rotation);
                }
            }
        }
    }
}
