using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerX : MonoBehaviour
{
    public bool gameOver = false;

    public float floatForce;
    private float gravityModifier = 1.5f;
    private float topBound = 14.4f;
    private float bounceForce = 700f;
    private float sceneTimer = 5f;

    private Rigidbody playerRb;

    public ParticleSystem explosionParticle;
    public ParticleSystem fireworksParticle;

    private AudioSource playerAudio;
    public AudioClip moneySound;
    public AudioClip explodeSound;
    public AudioClip bounceSound;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity *= gravityModifier;
        playerAudio = GetComponent<AudioSource>();
        playerRb = GetComponent<Rigidbody>();

        // Apply a small upward force at the start of the game
        playerRb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // While space is pressed and player is low enough, float up
        if (Input.GetKey(KeyCode.Space) && !gameOver && transform.position.y < topBound && playerRb.velocity.y < floatForce)
        {
            playerRb.AddForce(Vector3.up * floatForce);
        }
        else if (transform.position.y > topBound && playerRb.velocity.y > 0)
        {
            playerRb.AddForce(Vector3.up * -floatForce);
        }

        // Restart game after scene timer expires on game over
        if (gameOver == true)
        {
            sceneTimer -= Time.deltaTime;
            if (sceneTimer < 0)
            {
                SceneManager.LoadScene("Challenge 3");
            }
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        // if player collides with bomb, explode and set gameOver to true
        if (other.gameObject.CompareTag("Bomb"))
        {
            explosionParticle.Play();
            playerAudio.PlayOneShot(explodeSound, 1.0f);
            gameOver = true;
            Debug.Log("Game Over!");
            Destroy(other.gameObject);
            gameObject.transform.localScale = new Vector3(0, 0, 0);
        }        

        // if player collides with ground, bounce back and play sound
        else if (other.gameObject.CompareTag("Ground") && !gameOver)
        {
            playerRb.AddForce(Vector3.up * bounceForce);
            playerAudio.PlayOneShot(bounceSound, 1.0f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // if player collides with money, fireworks
        if (other.gameObject.CompareTag("Money"))
        {
            fireworksParticle.Play();
            playerAudio.PlayOneShot(moneySound, 1.0f);
            Destroy(other.gameObject);
        }
    }

}
