using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmunitionCrate : MonoBehaviour
{
    public Animator crateAnimator;

    private WeaponHandler playerWeaponHandler;
    private bool canEquip = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHandler>() != null)
        {
            crateAnimator.SetBool("isCrateOpen", true);
            playerWeaponHandler = other.gameObject.GetComponent<WeaponHandler>();
            other.gameObject.GetComponent<PlayerHandler>().inGameUI.ShowAmmoFillHintPanel();
            canEquip = true;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHandler>() != null)
        {
            other.gameObject.GetComponent<PlayerHandler>().inGameUI.HideAmmoFillHintPanel();
            crateAnimator.SetBool("isCrateOpen", false);
            canEquip = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canEquip)
        {
            playerWeaponHandler.currentWeapon.pocketBullets = 150;
        }
    }
}
