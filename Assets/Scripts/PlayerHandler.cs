using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator rigAnimator;
    public CharacterController characterController;
    public LayerMask environmentMask;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public float gravity = -9.8f;
    public float jumpHeight = 10f;
    public float turnSpeed = 15f;
    public float health = 100f;
    public bool canFollowCameraRotation = true;
    public InGameUI inGameUI;
    public int score;
    public bool isGamePaused = false;
    public bool isDead = false;

    private Vector2 input;
    private Vector3 rootMotion;
    private Vector3 velocity;
    private Camera mainCamera;
    
    private bool isGrounded = false;



    private void Start()
    {
        foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = true;
        }

        mainCamera = Camera.main;
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, environmentMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -0.0001f;
        }

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        playerAnimator.SetFloat("InputX", input.x);
        playerAnimator.SetFloat("InputY", input.y);



        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerAnimator.GetBool("isCrouching"))
            {
                playerAnimator.Play("roll crouching");
            }
            else
            {
                playerAnimator.Play("roll standing");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (playerAnimator.GetBool("isCrouching"))
            {
                playerAnimator.Play("parkour crouching");
            }
            else
            {
                playerAnimator.Play("parkour standing");
            }
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
            velocity = Vector3.zero;
        }

        //characterController.Move(rootMotion);
        //rootMotion = Vector3.zero;

        velocity.y += gravity * Time.deltaTime; //  v = u + at

        characterController.Move(velocity * Time.deltaTime);


        float rotationCam = mainCamera.transform.rotation.eulerAngles.y;
        if (!isDead && canFollowCameraRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotationCam, 0), turnSpeed * Time.deltaTime);
        }

    }

    private void ToggleCrouch()
    {
        playerAnimator.SetBool("isCrouching", !playerAnimator.GetBool("isCrouching"));
    }

    public void takeBulletDamage(float damage)
    {
        //Debug.Log("took damaage: " + damage + " health: " + health);
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        playerAnimator.enabled = false;
        rigAnimator.enabled = false;
        foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = false;
        }
    }

    private void Jump()
    {
        playerAnimator.SetTrigger("isJumping");
        velocity = playerAnimator.velocity;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // v^2 = u^2 + 2gh
    }

    //private void OnAnimatorMove()
    //{
    //    rootMotion += playerAnimator.deltaPosition;
    //}

    private void FixedUpdate()
    {
    }
}