using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text gameOverText;
    public TMP_Text hiScoreText;

    private string pressSpaceToStart = "Tap to Start!";
    private string pressSpaceToFart = "Tap to FART!";

    private MoveLeft moveLeftScript;
    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        moveLeftScript = GameObject.Find("Background").GetComponent<MoveLeft>();
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + moveLeftScript.score; // Display score
        hiScoreText.text = "High Score: " + moveLeftScript.hiScore; // Display high score

        // Display final score on game over
        if (playerControllerScript.gameOver)
        {
            gameOverText.text = "Game Over!\nFinal Score: " + moveLeftScript.score;
        }

        // Prompt player to press Space after walk in
        else if (playerControllerScript.waitingToStart)
        {
            if (playerControllerScript.pressSpaceToggle)
            {
                gameOverText.text = pressSpaceToStart;
            }
            else
            {
                gameOverText.text = pressSpaceToFart;
            }
        }

        // Remove text on game start
        else
        {
            gameOverText.text = "";
        }
    }
}
