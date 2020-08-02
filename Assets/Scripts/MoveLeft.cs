using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float speed;
    private float speedInit = 16;
    private float dashSpeedModifier = 1.8f;
    private float leftBound = -10;

    public int score = 0;

    public string dashKey = "Fire3";

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerScript.gameHasStarted)
        {
            // Destroy game objects that go off screen
            if (transform.position.x < leftBound && gameObject.CompareTag("Obstacle"))
            {
                Destroy(gameObject);
            }

            if (!playerControllerScript.gameOver)
            {
                // Move stuff left
                transform.Translate(Vector3.left * Time.deltaTime * speed);

                // Increase speed and score (more while dash key is pressed)
                if (playerControllerScript.isDashing)
                {
                    speed = speedInit * dashSpeedModifier;
                    score += 2;
                }
                else
                {
                    speed = speedInit;
                    score++;
                }
            }
        }        
    }
}
