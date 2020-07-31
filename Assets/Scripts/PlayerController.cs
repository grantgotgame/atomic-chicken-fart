using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;

    private Animator playerAnim;

    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;

    private AudioSource playerAudio;

    public AudioClip jumpSound;
    public AudioClip crashSound;

    private MoveLeft moveLeftScript;

    private float playerPosXRunning = 0f;
    private float playerPreGameSpeed = 3f;

    private float jumpForce = 1600f;
    private float doubleJumpForce = 2100f;
    private float gravityModifier = 9f;
    private float minDoubleJumpHeight = 1.4f;
    private float resetTimer;
    private float resetTimerInit = 2f;

    public bool isOnGround = true;
    public bool gameOver = false;
    public bool gameHasStarted = false;
    private bool hasDoubleJumped = false;
    private bool isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        moveLeftScript = GameObject.Find("Background").GetComponent<MoveLeft>();
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        Physics.gravity = new Vector3(0, -9.81f * gravityModifier, 0);
        resetTimer = resetTimerInit;
        dirtParticle.Stop();
        isOnGround = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Player enters game from left of screen
        if (transform.position.x < playerPosXRunning && !gameHasStarted)
        {
            transform.position += new Vector3(playerPreGameSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            // Start game
            gameHasStarted = true;
            playerAnim.SetFloat("Speed_f", 1f);            

            // Allow player to jump by pressing space while on the ground
            if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !gameOver)
            {
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isOnGround = false;
                playerAnim.SetTrigger("Jump_trig");
                playerAudio.PlayOneShot(jumpSound, .25f);
            }

            // Allow player to double jump at the top of their jump
            else if (Input.GetKeyDown(KeyCode.Space) && !isOnGround && !gameOver && !hasDoubleJumped
                && playerRb.position.y > minDoubleJumpHeight)
            {
                playerRb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
                hasDoubleJumped = true;
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

            // Trail dirt behind player while on ground and dashing
            else if (Input.GetAxisRaw(moveLeftScript.dashKey) > 0 && isOnGround)
            {
                if (!isDashing)
                {
                    dirtParticle.Play();
                    isDashing = true;
                }
            }
            else
            {
                dirtParticle.Stop();
                isDashing = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Set variables when player lands on ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            hasDoubleJumped = false;
        }

        // Trigger game over when player collides with an obstacle
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
            explosionParticle.Play();
            gameObject.transform.localScale = new Vector3(0, 0, 0);
            playerAudio.PlayOneShot(crashSound, 2f);
        }
    }
}
