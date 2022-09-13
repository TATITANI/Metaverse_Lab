using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    Rigidbody2D rigid2D;
    Animator animator;
    float jumpForce = 680.0f;
    public float walkForce = 300.0f;
    public float maxWalkSpeed = 20.0f;
    private string currentSceneName;

    void Start()
    {
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        // jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.animator.SetTrigger("jumpTrigger");
            this.rigid2D.AddForce(transform.up * this.jumpForce);
        }

        // left right
        int key = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow)) key = 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) key = -1;
        // player speed
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);
        if (speedx < this.maxWalkSpeed)
        {
            this.rigid2D.AddForce(transform.right * key * this.walkForce);
        }

        if (key != 0) transform.localScale = new Vector3(key, 1, 1);
        if (this.rigid2D.velocity.y == 0)
        {
            this.animator.speed = speedx / 2;
        }
        else
        {
            this.animator.speed = 1.0f;
        }

        if (transform.position.y < -10)
        {
            SceneManager.LoadScene(currentSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Goal~~");
        if (currentSceneName == "jump1")
        {
            SceneManager.LoadScene("jump2");
        }
        else if (currentSceneName == "jump2")
        {
            SceneManager.LoadScene("clearScene");
        }
    }
}