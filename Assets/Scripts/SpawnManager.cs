using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

    private float levelOneMaxScore = 10000f;
    private float levelTwoMaxScore = 30000f;

    private PlayerController playerControllerScript;

    private MoveLeft moveLeftScript;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        moveLeftScript = GameObject.Find("Background").GetComponent<MoveLeft>();
        InvokeRepeating("SpawnObstacle", startDelay, repeatRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObstacle()
    {
        // Select random obstacle to spawn based on score
        if (moveLeftScript.score < levelOneMaxScore)
        {
            obstacleIndex = 0;
        }
        else if (moveLeftScript.score < levelTwoMaxScore)
        {
            obstacleIndex = Random.Range(0, 2);
        }
        else
        {
            obstacleIndex = Random.Range(0, obstaclePrefab.Length);
        }

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
