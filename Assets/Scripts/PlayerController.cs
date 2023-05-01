using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
/* TODO
 * MainMenu: Look at play button and why it makes the lighting weird
 * Inventory/PlayerController: Shift inventory to the inventory script
 * EnemyController: Work out enemy movement and animations
 * GUIManager: ("countdownTimer") Add conditional text for game over, goal not met yet, and goal met
 * Combine HealthBar with GUIManager
*/
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

    private int itemCount;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        maxHealth = 100;
        currentHealth = 75;
        ChangeHealth(0);

        isGrounded = true;

        currentHealth = 75;
        healthBar.SetMaxHealth(maxHealth);

        itemCount = 0;

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
            StartCoroutine(WaitForSeconds(2f));
            animator.ResetTrigger("isLanding");
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
            print("health obtained");
            ChangeHealth(20);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("item"))
        {
            print("item obtained");
            itemCount += 1;
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "goal")
        {
            if (itemCount < 2)
            {
                print("Collect all the goal items first!");
            }
            else
            {
                print("Goal met!");
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.freezeRotation = true;
                WaitForSeconds(10f);
                SceneManager.LoadScene("Menu");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("poison"))
        {
            //print("poison collision");
            if (canTakeDamage)
            {
                ChangeHealth(-5);
                StartCoroutine(WaitForSeconds(1f));
            }
        }
        if (other.gameObject.CompareTag("enemy"))
        {
            //print("enemy collision");
            if (canTakeDamage)
            {
                ChangeHealth(-10);
                StartCoroutine(WaitForSeconds(.9f));
            }
        }
    }

    private void ChangeHealth(int value)
    {
        currentHealth += value;
        if (currentHealth < 1)
        {
            print("You Died - Game Over");
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.freezeRotation = true;
            WaitForSeconds(10f);
            SceneManager.LoadScene("Menu");

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

    IEnumerator WaitForSeconds(float f)
    {
        canTakeDamage = false;
        yield return new WaitForSecondsRealtime(f);
        canTakeDamage = true;
    }
}
