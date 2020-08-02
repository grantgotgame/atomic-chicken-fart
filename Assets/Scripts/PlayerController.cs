using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;

    private Animator playerAnim;

    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;

    private AudioSource playerAudio;

    public AudioClip fartSound;
    public AudioClip crashSound;

    private float gameStartPos = -1.5f;
    private float playerPreGameSpeed = 3f;

    private float fartForce = 500f;
    private float resetTimer;
    private float resetTimerInit = 3f;
    private float fartTimer;
    private float fartTimerInit = .19f;

    public bool gameOver = false;
    public bool gameHasStarted = false;
    public bool isFarting = false;
    public bool isOnGround;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        resetTimer = resetTimerInit;
        dirtParticle.Stop();
        gameOver = false;
        gameHasStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Player enters game from left of screen
        if (transform.position.x < gameStartPos && !gameHasStarted)
        {
            transform.position += new Vector3(playerPreGameSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            // Start game
            gameHasStarted = true;

            // Reset game on game over
            if (gameOver)
            {
                resetTimer -= Time.deltaTime;
                dirtParticle.Stop();
                if (resetTimer < 0)
                {
                    SceneManager.LoadScene("Prototype 3");
                }
            }

            // Allow player to fart jump while holding space
            else if (Input.GetKey(KeyCode.Space))
            {                
                playerRb.AddForce(Vector3.up * fartForce); // Apply fart force                
                fartTimer -= Time.deltaTime; // Countdown timer to prevent fart sounds from overlapping

                // Flap wings
                isOnGround = false;
                playerAnim.SetBool("Run", true);
                playerAnim.SetBool("Walk", false);

                // Trigger fart sound if key is pressed again rather than held
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    fartTimer = 0;                
                }                

                // Fart particles (farticles)
                if (!isFarting)
                {
                    dirtParticle.Play();
                    isFarting = true;
                }

                // Fart sounds
                if (fartTimer <= 0)
                {
                    playerAudio.clip = fartSound;
                    playerAudio.Play();
                    fartTimer = fartTimerInit;
                }
            }

            // Stop farting when player releases spacebar
            else
            {
                dirtParticle.Stop();
                isFarting = false;
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop flapping wings when landing
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            playerAnim.SetBool("Walk", true);
            playerAnim.SetBool("Run", false);
        }

        // Trigger game over when player collides with an obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver = true;
            explosionParticle.Play();
            gameObject.transform.localScale = new Vector3(0, 0, 0);
            playerAudio.PlayOneShot(crashSound, 2f);
        }
    }
}
