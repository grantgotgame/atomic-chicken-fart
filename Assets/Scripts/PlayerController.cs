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

    public AudioClip fartSound;
    public AudioClip crashSound;

    private MoveLeft moveLeftScript;

    private float playerPosXRunning = 0f;
    private float playerPreGameSpeed = 3f;

    private float fartForce = 8500f;
    private float gravityModifier = 9f;
    private float resetTimer;
    private float resetTimerInit = 2f;
    private float fartTimer;
    private float fartTimerInit = .3f;

    public bool isOnGround = true;
    public bool gameOver = false;
    public bool gameHasStarted = false;
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

            // Allow player to fart jump
            else if (Input.GetKey(KeyCode.Space))
            {
                playerRb.AddForce(Vector3.up * fartForce);
                fartTimer -= Time.deltaTime;
                if (isOnGround)
                {
                    isOnGround = false;
                    playerAnim.SetTrigger("Jump_trig");
                }                
                if (fartTimer < 0)
                {
                    playerAudio.clip = fartSound;
                    playerAudio.Play();
                    fartTimer = fartTimerInit;
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
