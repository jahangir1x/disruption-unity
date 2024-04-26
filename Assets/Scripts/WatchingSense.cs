using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchingSense : MonoBehaviour
{
    public Collider playerCollider;
    public Transform eyePosition;
    
    public bool canSee = false;
    public Vector3 playerFiringPosition;
    public EnemyHandler enemyHandler;

    public Transform playeeeee;

    private float playerHeadPosition = 1.6f;
    private RaycastHit raycastHit;
    private Ray ray;
    private bool canSetPossibleLocation = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCollider = other;
            enemyHandler.playerController = playerCollider.gameObject.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (playerCollider != null)
        {

            for (float offset = playerHeadPosition; offset > 0f; offset -= 0.2f)
            {
                ray.origin = eyePosition.position;
                ray.direction = playerCollider.gameObject.transform.position + new Vector3(0f, offset, 0f) - eyePosition.position;

                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (raycastHit.collider.CompareTag("PlayerHit"))
                    {
                        //Debug.Log("raycast collider: " + raycastHit.collider.name);
                        Debug.DrawLine(ray.origin, playerCollider.gameObject.transform.position + new Vector3(0f, offset, 0f), Color.green, 10f);
                        playerFiringPosition = playerCollider.gameObject.transform.position;
                        //enemyHandler.playerController = playerCollider.gameObject.get
                        canSee = true;
                    }
                }
            }

            if (!canSee)
            {
                Invoke("StorePossibleLocation", 0.9f);
                canSetPossibleLocation = false;
                canSee = false;
            }

            playeeeee.position = playerFiringPosition;


        }
        else
        {
            canSee = false;
        }

    }

    private void StorePossibleLocation()
    {
        if (playerCollider != null)
        {
            playerFiringPosition = playerCollider.gameObject.transform.position;
            playeeeee.position = playerFiringPosition;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCollider = null;
        }
    }
}