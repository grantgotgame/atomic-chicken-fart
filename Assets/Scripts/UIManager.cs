using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text gameOverText;

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
        scoreText.text = "Score: " + moveLeftScript.score;
        if (playerControllerScript.gameOver == true)
        {
            gameOverText.text = "Game Over!\nFinal Score: " + moveLeftScript.score;
        }
    }
}
