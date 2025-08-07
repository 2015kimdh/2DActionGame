using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 8.0f;
    public float acceleration = 25f;
    public float deceleration = 30f;

    public float dashSpeed = 20.0f;
    public float dashDuration = 0.2f;
    public float doubleTapTime = 0.25f;

    public float dashCooldown = 0.1f;
    private bool canDash = true;

    public float jumpForce = 12f;
    public int maxJump = 0;
    
    private int jumpCount = 0;
    private bool isGrounded = false;

    [SerializeField]
    private Rigidbody2D rb;
    private float moveInput;
    private bool isDashing = false;

    private float lastLeftTapTime = -1f;
    private float lastRightTapTime = -1f;
    private float lastDownTapTime = -1f;
    

    private void Update()
    {
        if (!isDashing)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }

        //좌우 더블탭 대쉬
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Time.time -  lastLeftTapTime < doubleTapTime && canDash)
            {
                StartCoroutine(Dash(Vector2.left));
            }
            lastLeftTapTime = Time.time;
        }
       
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Time.time - lastRightTapTime < doubleTapTime && canDash)
            {
                StartCoroutine(Dash(Vector2.right));
            }
            lastRightTapTime = Time.time;
        }
        //하단 더블탭 대쉬
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Time.time - lastDownTapTime < doubleTapTime)
            {
                StartCoroutine (Dash(Vector2.down));
            }
            lastDownTapTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump)
        {
            Debug.Log("점프! 현재 점프 횟수: " + jumpCount);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // 점프 초기화
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }


    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            float targetSpeed = moveInput * moveSpeed;
            float speedDiff = targetSpeed - rb.linearVelocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, 0.9f) * Mathf.Sign(speedDiff);
            
            rb.AddForce(new Vector2(movement, 0f));
        }
    }
    private IEnumerator Dash(Vector2 dir)
    {
        canDash = false;
        isDashing = true;
        // rb.linearVelocity = new Vector2(dir.x * dashSpeed, rb.linearVelocity.y);
        rb.linearVelocity = dir * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("점프 초기화됨");
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
// 대쉬 후 방향키 게속 누르고 있으면 가속이 계속 이어짐 그리고 이동 느낌 낼려고 수치 딸깍하다보면 대쉬말곤 이동이 안되는 버그가 있음
