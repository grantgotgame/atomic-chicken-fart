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
    private float playerPreGameSpeed = .05f;

    private float fartForce = 800f;
    private float resetTimer;
    private float resetTimerInit = 3f;
    private float fartTimer;
    private float fartTimerInit = .19f;
    private float pressSpaceTimer;
    private float pressSpaceTimerInit = 1f;

    public bool walkingIn = true;
    public bool gameOver = false;
    public bool gameHasStarted = false;
    public bool isFarting = false;
    public bool isOnGround; // called in MoveLeft script
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
        walkingIn = true;
    }

    // Update is called once per frame
    private void Update()
    {
        // Start game when Spacebar is pressed or screen is tapped
        if (waitingToStart)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                waitingToStart = false;
                gameHasStarted = true;
                playerAnim.SetBool("Eat", false);
            }
        }
    }
    void FixedUpdate()
    {
        // Player enters game from left of screen
        if (transform.position.x < gameStartPos)
        {
            transform.position += new Vector3(playerPreGameSpeed, 0, 0);
        }
        else
        {
            walkingIn = false;
        }

        if (!walkingIn && !gameHasStarted)
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
        }

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

        // Allow player to fart jump while holding space or touching screen
        if (gameHasStarted && !gameOver)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
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

            // Stop farting when player releases spacebar or tap
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
