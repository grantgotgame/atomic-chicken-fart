using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    public float speed;
    private float speedInit = 20f;
    private float speedScaler = .03f;
    private float fartSpeedModifier = 1.1f;
    private float leftBound = -10;

    public int score = 0;
    public int hiScore = 0;

    private bool fsmApplied = false;

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        speed = speedInit;
        hiScore = PlayerPrefs.GetInt("hiScore", hiScore);
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

                // Increase score while flying
                if (!playerControllerScript.isOnGround)
                {
                    score += (int)speed;
                    if (score > hiScore)
                    {
                        hiScore = score;
                        PlayerPrefs.SetInt("hiScore", hiScore);
                    }
                }

                // Increase speed and double score while farting
                if (playerControllerScript.isFarting)
                {                    
                    speed += speedScaler; // Increase speed gradually
                    if (!fsmApplied)
                    {
                        speed *= fartSpeedModifier;
                        fsmApplied = true;
                    }
                }

                // Decrease speed while not farting
                else
                {
                    if (fsmApplied)
                    {
                        speed /= fartSpeedModifier;
                        fsmApplied = false;
                    }
                }                
            }
        }
    }
}
