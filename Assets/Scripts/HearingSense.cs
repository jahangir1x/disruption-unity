using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingSense : MonoBehaviour
{
    
    public Collider playerCollider;
    public Vector3 playerFiringPosition;
    public bool hearedPlayer = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("player: " + other.gameObject.name);
            playerCollider = other;
        }
    }

    private void Update()
    {
        //Debug.Log("playerCol: " + playerCollider);
        if (playerCollider != null)
        {
            if (playerCollider.gameObject.GetComponent<WeaponHandler>().currentWeapon.isFiring)
            {
                playerFiringPosition = playerCollider.transform.position;
                hearedPlayer = true;
            }
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
