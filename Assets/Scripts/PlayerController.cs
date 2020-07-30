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

    public float jumpForce = 1600f;
    public float doubleJumpForce = 2100f;
    private float gravityModifier = 9f;
    private float minDoubleJumpHeight = 1.4f;
    private float resetTimer;
    private float resetTimerInit = 2f;

    public bool isOnGround = true;
    public bool gameOver = false;
    private bool hasDoubleJumped = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        Physics.gravity = new Vector3(0, -9.81f * gravityModifier, 0);
        resetTimer = resetTimerInit;
    }

    // Update is called once per frame
    void Update()
    {
        // Allow player to jump by pressing space while on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !gameOver)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            playerAnim.SetTrigger("Jump_trig");
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSound, .25f);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isOnGround && !gameOver && !hasDoubleJumped
            && playerRb.position.y > minDoubleJumpHeight && playerRb.velocity.y < 0)
        {
            playerRb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            hasDoubleJumped = true;
        }
        // Reset game on game over
        else if (gameOver)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer < 0)
            {
                SceneManager.LoadScene("Prototype 3");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Trail dirt behind player while on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            hasDoubleJumped = false;
            dirtParticle.Play();
        }
        // Trigger game over when player collides with an obstacle
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over");
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
            explosionParticle.Play();
            gameObject.transform.localScale = new Vector3(0, 0, 0);
            dirtParticle.Stop();
            playerAudio.PlayOneShot(crashSound, 2f);
        }
    }
}
