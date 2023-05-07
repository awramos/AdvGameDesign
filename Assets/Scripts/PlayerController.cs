using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody rb;

    [SerializeField] private Animator animator;

    public HealthBar healthBar;
    private int currentHealth, maxHealth;

    public int playerSpeed;
    public int jumpSpeed;
    private bool isJumping;
    private bool isGrounded;

    bool canTakeDamage = true;

    private int metalCount;
    private int batteryCount;

    public AudioSource healSound;
    public AudioSource hurtSound;
    public AudioSource collectSound;
    public AudioSource backgroundMusic;

    public Text metalScore;
    public Text batteryScore;
    public GameObject incomplete;
    public GameObject complete;
    public GameObject dead;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        maxHealth = 100;
        currentHealth = 75;
        ChangeHealth(0);

        isGrounded = true;

        currentHealth = 75;
        healthBar.SetMaxHealth(maxHealth);

        metalCount = 0;
        batteryCount = 0;
        metalScore.text = "0/4";
        batteryScore.text = "0/1";
        incomplete.SetActive(false);
        complete.SetActive(false);
        dead.SetActive(false);

        GameManager.instance.player = this;
    }

    private void Update()
    {
        healthBar.SetHealth(currentHealth);
        #region movement
        Vector3 movementVector = Vector3.zero;
        
        float xMovement = 0;
        float zMovement = 0;

        #region keys
        if (Input.GetKey(KeyCode.W))
        {
            xMovement = playerSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            xMovement = -playerSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            zMovement = playerSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            zMovement = -playerSpeed;
        }
        #endregion

        if (xMovement != 0 && zMovement != 0)
        {
            ///cut movement so speed doesn't increase on diagonal
            xMovement = (xMovement / 3)*2;
            zMovement = (zMovement / 3)*2;
        }

        //movementVector = new Vector3(xMovement, rb.velocity.y, zMovement);
        var lookDirection = new Vector3(xMovement, 0, zMovement);

        if (lookDirection != Vector3.zero)
        {
            rb.velocity = new Vector3(xMovement, rb.velocity.y, zMovement);
            playerTransform.forward = lookDirection;
        }     
        
        #endregion
        #region jumping
        if (Input.GetKey(KeyCode.Space) && isGrounded == true)
        {
            if (isJumping == false)
            {
                isJumping = true;
                animator.SetTrigger("isJumping");
                animator.SetBool("isGrounded", false);
                rb.AddForce(Vector3.up * jumpSpeed);
            }
        }
        float speedValue = Mathf.Max(Mathf.Abs(xMovement), Mathf.Abs(zMovement));
        animator.SetFloat("Speed", speedValue);
        #endregion 
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeHealth(-15);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeHealth(10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (isJumping && collision.gameObject.tag == "Ground")
        {
            //print("Collision enter on ground");
            isGrounded = true;
            isJumping = false;            
            animator.SetBool("isGrounded", true);
            animator.SetTrigger("isLanding");
            animator.ResetTrigger("isFalling");
            animator.ResetTrigger("isJumping");
            StartCoroutine(jumpWait());
        }
        else if (collision.gameObject.tag == "Ground")
        {
            //print("Collision enter on ground");
            isGrounded = true;
            animator.SetBool("isGrounded", true);
            animator.SetTrigger("isLanding");
            animator.ResetTrigger("isFalling");
            animator.ResetTrigger("isJumping");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //print("Collision Exit on ground");
            isGrounded = false;
            animator.SetBool("isGrounded", false);
            animator.SetTrigger("isFalling");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("health"))
        {
            healSound.Play();
            ChangeHealth(20);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("metal"))
        {
            collectSound.Play();
            metalCount += 1;
            Destroy(other.gameObject);
            metalScore.text = metalCount + "/4";
        }
        if (other.gameObject.CompareTag("battery"))
        {
            collectSound.Play();
            batteryCount += 1;
            Destroy(other.gameObject);
            batteryScore.text = batteryCount + "/1";
        }
        if (other.gameObject.tag == "goal")
        {
            if (metalCount < 4  && batteryCount < 1)
            {
                incomplete.SetActive(true);
                StartCoroutine(popupWait());
            }
            else
            {
                complete.SetActive(true);
                Time.timeScale = 0;
                StartCoroutine(endGameWait());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("poison"))
        {
            if (canTakeDamage)
            {
                hurtSound.Play();
                ChangeHealth(-5);
                StartCoroutine(damageWait());
            }
        }
        if (other.gameObject.CompareTag("enemy"))
        {
            if (canTakeDamage)
            {
                hurtSound.Play();
                ChangeHealth(-10);
                StartCoroutine(damageWait());
            }
        }
    }

    private void ChangeHealth(int value)
    {
        currentHealth += value;
        if (currentHealth < 1)
        {
            dead.SetActive(true);
            Time.timeScale = 0;
            StartCoroutine(endGameWait());
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if(currentHealth < 40)
        {
            animator.SetBool("isLowHealth", true);
        }
        else
        {
            animator.SetBool("isLowHealth", false);
        }

        healthBar.SetHealth(currentHealth);
    }

    IEnumerator jumpWait()
    {
        yield return new WaitForSecondsRealtime(2);
        animator.ResetTrigger("isLanding");
    }
    IEnumerator popupWait()
    {
        yield return new WaitForSecondsRealtime(4);
        incomplete.SetActive(false);
    }
    IEnumerator endGameWait()
    {
        yield return new WaitForSecondsRealtime(4);
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
    IEnumerator damageWait()
    {
        canTakeDamage = false;
        yield return new WaitForSecondsRealtime(1);
        canTakeDamage = true;
    }
}
