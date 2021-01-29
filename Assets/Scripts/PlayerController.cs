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

    private float gameStartPos = 2f;
    private float playerPreGameSpeed = 3f;

    private float fartForce = 300f;
    private float resetTimer;
    private float resetTimerInit = 4f;
    private float fartTimer;
    private float fartTimerInit = .19f;
    private float pressSpaceTimer;
    private float pressSpaceTimerInit = 1f;

    public bool gameOver = false;
    public bool gameHasStarted = false;
    public bool isFarting = false;
    public bool isOnGround;
    public bool pressSpaceToggle = false;
    public bool waitingToStart = false;

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
        if (transform.position.x < gameStartPos)
        {
            transform.position += new Vector3(playerPreGameSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            if (!gameHasStarted)
            {
                waitingToStart = true;
                playerAnim.SetBool("Walk", false);
                // Cycle between start texts (called in UIManager)
                pressSpaceTimer -= Time.deltaTime;
                if (pressSpaceTimer < 0)
                {
                    if (pressSpaceToggle)
                    {
                        pressSpaceToggle = false;
                        playerAnim.SetBool("Eat", true);
                    }
                    else
                    {
                        pressSpaceToggle = true;
                        playerAnim.SetBool("Eat", false);
                    }
                    pressSpaceTimer = pressSpaceTimerInit;
                }

                // Start game when Spacebar is pressed or screen is tapped
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    waitingToStart = false;
                    gameHasStarted = true;
                    playerAnim.SetBool("Eat", false);
                }
            }

            // Reset game on game over
            else if (gameOver)
            {
                resetTimer -= Time.deltaTime;
                dirtParticle.Stop();
                if (resetTimer < 0)
                {
                    SceneManager.LoadScene("Prototype 3");
                }
            }

            // Allow player to fart jump while holding space or touching screen
            else if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
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
            if (!waitingToStart)
            {
                playerAnim.SetBool("Walk", true);
                playerAnim.SetBool("Run", false);
            }
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
