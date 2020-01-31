using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    private float horizontalMove = 0f;
    public float speed = 40f;
    bool jump = false;
    public Animator animator;

    public Text timerText;
    private float currentTime = 0f;
    public float levelTime = 15f;

    public int health;
    public int numOfHearts;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public int collectibles;
    public int numOfKamen;
    public Image[] kamens;
    public Sprite blueKamen;
    public Sprite grayKamen;

    private GameMaster gm;
    // Start is called before the first frame update
    void Start()
    {
        UpdateLives();
        UpdateKamen();
        currentTime = levelTime;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        gm.lastCheckpointPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping", true);
        }

        currentTime -= 1 * Time.deltaTime;
        Invoke("setTimerText",0.5f);
        if (CheckTimer())
        {
            Debug.Log("GameOver");
            timerText.text = "";
            CancelInvoke("setTimerText");
            timerText.text = "Game Over!";
            Invoke("RestartGame", 3);
        }
        if (!CheckRetries())
        {
            CancelInvoke("setTimerText");
            timerText.text = "Game Over!";
            Invoke("RestartGame", 3);
        }
    }
    void setTimerText()
    {
        timerText.text = currentTime.ToString("0");
    }
    public void onLand()
    {
        animator.SetBool("isJumping", false);
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pit"))
        {
            health -= 1;
            // SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            UpdateLives();
            if (CheckRetries())
            {
                transform.position = gm.lastCheckpointPos;
            }
            else
            {
                transform.position = new Vector3(-9.94f, -1.57f, 0);
            }
        }
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            gm.lastCheckpointPos = transform.position;
        }
        if (collision.gameObject.CompareTag("Kamen"))
        {
            collectibles -= 1;
            UpdateKamen();
            collision.gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Spikes"))
        {
            Debug.Log("You Dies");
            health -= 1;
            UpdateLives();
            if (CheckRetries())
            {
                transform.position = gm.lastCheckpointPos;
            }
            else
            {
                Debug.Log("No health");
                //UpdateLives();
                CancelInvoke("setTimerText");
                timerText.text = "Game Over!";
                Invoke("RestartGame", 3);
            }
            
        }
        if (other.gameObject.CompareTag("Nini"))
        {
            Debug.Log("You Win");
        }
        
    }

    bool CheckTimer()
    {
        if(currentTime <= 0)
        {
            Debug.Log("You Lost!");
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CheckRetries()
    {
        if (health >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void UpdateLives()
    {
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
    void UpdateKamen()
    {
        if(collectibles > numOfKamen)
        {
            collectibles = numOfHearts;
        }
        for (int i = 0; i < kamens.Length; i++)
        {
            if (i < collectibles)
            {
                kamens[i].sprite = grayKamen;
            }
            else
            {
                kamens[i].sprite = blueKamen;
            }
            if (i< numOfKamen)
            {
                kamens[i].enabled = true;
            }
            else
            {
                kamens[i].enabled = false;
            }
        }
    }
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}

