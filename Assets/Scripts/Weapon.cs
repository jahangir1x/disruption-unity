using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum weaponType
    {
        Primary = 0,
        Secondary = 1
    }

    public string weaponName;
    public int bulletsPerSecond = 5;
    public ParticleSystem muzzleFlash;
    public ParticleSystem environmentHitEffect;
    public ParticleSystem fleshHitEffect;
    public TrailRenderer tracerEffect;
    public Transform bulletOrigin;
    public Transform bulletDestination;
    public float weaponDamage = 9f;
    public int magazineSize = 30;
    public int magazineBullets = 28;
    public int pocketBullets = 40;
    public AudioSource gunshotSound;
    public bool isFiring = false;
    public bool canFire = true;
    public RaycastHit raycastHit;
    public float weaponForce = 30f;
    public PlayerHandler playerHandler;

    public float enemyHitHandDamage = 20;
    public float enemyHitLegDamage = 25;
    public float enemyHitBodyDamage = 34;
    public float enemyHitHeadDamage = 100;

    public int enemyHitHandScore = 2;
    public int enemyHitLegScore = 3;
    public int enemyHitBodyScore = 5;
    public int enemyHitHeadScore = 10;


    private Ray ray;
    private float timeSinceLastBullet;
    private bool isPlayerShooting = false;

    public void StartFiring(bool isPlayerShooting = false)
    {
        this.isPlayerShooting = isPlayerShooting;
        isFiring = true;
        timeSinceLastBullet = 0f;
        fireBullet();
    }

    private void Update()
    {
        timeSinceLastBullet += Time.deltaTime;
        if (isFiring)
        {
            if (timeSinceLastBullet > (1f / bulletsPerSecond))
            {
                timeSinceLastBullet = 0f;
                fireBullet();
            }
        }
    }

    public void fireBullet()
    {
        if (magazineBullets <= 0 || !canFire)
        {
            StopFiring();
            return;
        }
        gunshotSound.Play();
        if (isPlayerShooting)
        {
            magazineBullets--;
        }

        muzzleFlash.Emit(1);
        ray.origin = bulletOrigin.position;
        ray.direction = bulletDestination.position - bulletOrigin.position;
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        tracer.transform.position = bulletDestination.position;

        if (Physics.Raycast(ray, out raycastHit))
        {
            Debug.DrawLine(ray.origin, raycastHit.point, Color.red, 10f);
            //Debug.Log(raycastHit.collider.name);

            //if (raycastHit.collider.CompareTag("EnemyHitHand"))
            //{
            //    ParticleSystem effect = Instantiate(fleshHitEffect, raycastHit.point, Quaternion.identity);
            //    //fleshHitEffect.transform.position = raycastHit.point;
            //    effect.transform.forward = raycastHit.normal;
            //    effect.transform.SetParent(raycastHit.transform, true);
            //    fleshHitEffect.Emit(1);
            //    EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
            //    enemyHandler.takeBulletDamage(enemyHitHandDamage);

            //}
            //else if (raycastHit.collider.CompareTag("EnemyHitLeg"))
            //{

            //    fleshHitEffect.transform.position = raycastHit.point;
            //    fleshHitEffect.transform.forward = raycastHit.normal;
            //    fleshHitEffect.transform.SetParent(raycastHit.transform, true);
            //    fleshHitEffect.Emit(1);
            //    EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
            //    enemyHandler.takeBulletDamage(enemyHitLegDamage);

            //}
            //else if (raycastHit.collider.CompareTag("EnemyHitBody"))
            //{
            //    Debug.Log("hit body");
            //    fleshHitEffect.transform.position = raycastHit.point;
            //    fleshHitEffect.transform.forward = raycastHit.normal;
            //    fleshHitEffect.transform.SetParent(raycastHit.transform, true);
            //    fleshHitEffect.Emit(1);
            //    EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
            //    enemyHandler.takeBulletDamage(enemyHitBodyDamage);

            //}
            //else if (raycastHit.collider.CompareTag("EnemyHitHead"))
            //{

            //    fleshHitEffect.transform.position = raycastHit.point;
            //    fleshHitEffect.transform.forward = raycastHit.normal;
            //    fleshHitEffect.transform.SetParent(raycastHit.transform, true);
            //    fleshHitEffect.Emit(1);
            //    EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
            //    enemyHandler.takeBulletDamage(enemyHitHeadDamage);

            //}
            //else if (raycastHit.collider.CompareTag("PlayerHit"))
            //{
            //    Debug.Log("player hit");
            //    environmentHitEffect.transform.position = raycastHit.point;
            //    environmentHitEffect.transform.forward = raycastHit.normal;
            //    environmentHitEffect.transform.SetParent(raycastHit.transform, true);
            //    environmentHitEffect.Emit(1);
            //    PlayerHandler playerHandler = raycastHit.collider.GetComponentInParent<PlayerHandler>();
            //    playerHandler.takeBulletDamage(weaponDamage);
            //    // }
            //}


            if (raycastHit.collider.CompareTag("EnemyHitHand"))
            {
                ParticleSystem effect = Instantiate(fleshHitEffect, raycastHit.point, Quaternion.identity);
                effect.transform.forward = raycastHit.normal;
                effect.transform.SetParent(raycastHit.transform, true);
                effect.Emit(1);
                EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
                if (!enemyHandler.isDead)
                {
                    playerHandler.score += enemyHitHandScore;
                }
                enemyHandler.takeBulletDamage(enemyHitHandDamage);

            }
            else if (raycastHit.collider.CompareTag("EnemyHitLeg"))
            {

                ParticleSystem effect = Instantiate(fleshHitEffect, raycastHit.point, Quaternion.identity);
                effect.transform.forward = raycastHit.normal;
                effect.transform.SetParent(raycastHit.transform, true);
                effect.Emit(1);
                EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
                
                if (!enemyHandler.isDead)
                {
                    playerHandler.score += enemyHitLegScore;
                }
                enemyHandler.takeBulletDamage(enemyHitLegDamage);
            }
            else if (raycastHit.collider.CompareTag("EnemyHitBody"))
            {
                ParticleSystem effect = Instantiate(fleshHitEffect, raycastHit.point, Quaternion.identity);
                effect.transform.forward = raycastHit.normal;
                effect.transform.SetParent(raycastHit.transform, true);
                effect.Emit(1);
                EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
                
                if (!enemyHandler.isDead)
                {
                    playerHandler.score += enemyHitBodyScore;
                }
                enemyHandler.takeBulletDamage(enemyHitBodyDamage);
            }
            else if (raycastHit.collider.CompareTag("EnemyHitHead"))
            {

                ParticleSystem effect = Instantiate(fleshHitEffect, raycastHit.point, Quaternion.identity);
                effect.transform.forward = raycastHit.normal;
                effect.transform.SetParent(raycastHit.transform, true);
                effect.Emit(1);
                EnemyHandler enemyHandler = raycastHit.collider.GetComponentInParent<EnemyHandler>();
                
                if (!enemyHandler.isDead)
                {
                    playerHandler.score += enemyHitHeadScore;
                }
                enemyHandler.takeBulletDamage(enemyHitHeadDamage);
            }
            else if (raycastHit.collider.CompareTag("PlayerHit"))
            {
                ParticleSystem effect = Instantiate(environmentHitEffect, raycastHit.point, Quaternion.identity);
                effect.transform.forward = raycastHit.normal;
                effect.transform.SetParent(raycastHit.transform, true);
                effect.Emit(1);
                PlayerHandler playerHandler = raycastHit.collider.GetComponentInParent<PlayerHandler>();
                playerHandler.takeBulletDamage(weaponDamage);
                // }
            }



            else
            {

                ParticleSystem effect = Instantiate(environmentHitEffect, raycastHit.point, Quaternion.identity);
                effect.transform.forward = raycastHit.normal;
                effect.transform.SetParent(raycastHit.transform, true);
                effect.Emit(1);
                if (raycastHit.rigidbody != null)
                {
                    raycastHit.rigidbody.AddForceAtPosition(ray.direction * weaponForce, raycastHit.point);
                }
            }
        }
        
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}