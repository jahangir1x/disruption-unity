using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Weapon.weaponType currentWeaponType;
    public Transform crosshairTarget;
    public Animator rigAnimator;
    public Weapon currentWeapon;
    public Weapon existingWeapon;
    public PlayerHandler playerHandler;

    private bool canReload = true;

    private void Awake()
    {
        if (existingWeapon != null)
        {
            EquipWeapon(existingWeapon);
        }
    }

    private void EquipWeapon(Weapon weapon)
    {
        if (currentWeapon != null)
        {
            HolsterWeapon(weapon);
        }

        rigAnimator.SetBool("isHolstered", false);
        rigAnimator.Play(weapon.weaponName + "_equip");
        currentWeapon = weapon;
        Debug.Log("lol");

    }

    private void HolsterWeapon(Weapon weapon)
    {
        rigAnimator.SetBool("isHolstered", true);
    }

    private void Update()
    {
        if (currentWeapon != null)
        {
            
            if (currentWeapon.magazineBullets <= 0 && currentWeapon.pocketBullets > 0 && canReload)
            {
                Debug.Log("inside reload: ");
                if (currentWeapon.pocketBullets > currentWeapon.magazineSize)
                {
                    currentWeapon.magazineBullets = currentWeapon.magazineSize;
                    currentWeapon.pocketBullets -= currentWeapon.magazineSize;
                }
                else
                {
                    currentWeapon.magazineBullets = currentWeapon.pocketBullets;
                    currentWeapon.pocketBullets = 0;
                }

                rigAnimator.SetTrigger("needReload");
                canReload = false;

            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("press x");
                rigAnimator.SetBool("isHolstered", !rigAnimator.GetBool("isHolstered"));
            }

            if (currentWeapon.canFire && currentWeapon.magazineBullets > 0 && !playerHandler.isGamePaused)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    currentWeapon.StartFiring(true);
                    canReload = true;
                }

                else if (Input.GetButtonUp("Fire1"))
                {
                    currentWeapon.StopFiring();
                }

                if (currentWeapon.magazineBullets < currentWeapon.magazineSize && Input.GetKeyDown(KeyCode.R))
                {
                    Debug.Log("press r");
                    currentWeapon.magazineBullets = 0;

                }
                
            }
        }
    }
}