using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting;
using Random = System.Random;

public class EnemyHandler : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Animator animator;
    public Animator rigLayerAnimator;
    public Transform shootPoint;
    public Transform[] weaponSlots;
    public CharacterController playerController;
    public float shootXOffsetFactor = 2f;
    public float shootYOffsetFactor = 2f;
    public float shootZOffsetFactor = 2f;
    public float moveIntervalMin = 0f;
    public float moveIntervalMax = 10f;
    public float stopDistanceFromPlayerMin = 10f;
    public float stopDistanceFromPlayerMax = 60f;
    public float shootIntervalMin = 0f;
    public float shootIntervalMax = 10f;
    public float health = 50f;
    public HearingSense hearingSense;
    public WatchingSense watchingSense;
    public bool isDead = false;




    public Transform playeeeeeeeeee;

    private float timeElapsedSinceLastSawPlayer = 0f;
    private Vector3 randomDirection;
    private NavMeshHit navMeshHit;

    private float timeToWaitToMove = 0f;

    private bool isSetShootingInterval = false;
    private float shootingInterval;
    private float elapsedSecondsSinceShoot;

    private bool isSetAimingInterval = false;
    private float aimingInterval;
    private float elapsedSecondsSinceAim;

    private float elapsedSecondsSinceDead;

    private Weapon weapon;
    private float elapsedSecondsSinceMoved;

    
    private bool isDoneAiming = false;
    private Vector3 targetShootingPosition;
    private Vector3 possiblePlayerPosition;
    private bool sensedPlayer = false;
    private bool troublePlayed = false;
    public bool shouldSet = true;
    private bool stoppingDistanceSet = false;


    public void takeBulletDamage(float damage)
    {
        //Debug.Log("took dadamage: " + damage);
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.enabled = false;
        foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = false;
        }
    }

    private void Start()
    {
        foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = true;
        }

        targetShootingPosition = shootPoint.position;
        weapon = GetComponentInChildren<Weapon>();
        if (weapon)
        {
            weapon.bulletDestination = shootPoint;
            // weapon.transform.SetParent(weaponSlots[(int) weapon.weaponType], false);
        }

        Invoke("randomCrouch", UnityEngine.Random.Range(0.3f, 4f));

    }

    private void randomCrouch()
    {
        animator.SetBool("isCrouching", !animator.GetBool("isCrouching"));
        Invoke("randomCrouch", UnityEngine.Random.Range(0.3f, 4f));
    }

    private void Update()
    {
        if (!isDead)
        {
            checkSenses();
            checkPossiblePlayerPosition();
            //faceThePlayer();
            aimWeapon();
            shootThePlayer();
            //runToPlayer();

        }
        else
        {
            elapsedSecondsSinceDead += Time.deltaTime;
            if (elapsedSecondsSinceDead > 5f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void checkPossiblePlayerPosition()
    {
        if (sensedPlayer)
        {

            navAgent.SetDestination(possiblePlayerPosition);

            animator.SetFloat("InputX", navAgent.velocity.x);
            animator.SetFloat("InputY", navAgent.velocity.z);
            //Debug.Log("distance: " + Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(possiblePlayerPosition.x, possiblePlayerPosition.z)));

            //if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(possiblePlayerPosition.x, possiblePlayerPosition.z)) < 3.5f)
            //{
            //    Debug.Log("innnnn");

            if (watchingSense.canSee)
            {
                sensedPlayer = true;
                timeElapsedSinceLastSawPlayer = 0f;
            }
            else
            {
                timeElapsedSinceLastSawPlayer += Time.deltaTime;
                if (timeElapsedSinceLastSawPlayer > 1f)
                {
                    sensedPlayer = false;
                }
            }

            //}

        }
        else
        {
            randomDirection = UnityEngine.Random.insideUnitSphere * 7f;
            NavMesh.SamplePosition(randomDirection, out navMeshHit, 7f, NavMesh.AllAreas);
            possiblePlayerPosition = navMeshHit.position;
            sensedPlayer = true;
            timeElapsedSinceLastSawPlayer = 0f;
        }
    }

    private void shootThePlayer()
    {
        elapsedSecondsSinceShoot += Time.deltaTime;
        if (!isSetShootingInterval)
        {
            shootingInterval = UnityEngine.Random.Range(shootIntervalMin, shootIntervalMax);
            isSetShootingInterval = true;
        }
        if (elapsedSecondsSinceShoot > shootingInterval && watchingSense.canSee)
        {
            weapon.fireBullet();
            //Debug.Log("fire");
            isSetShootingInterval = false;
            elapsedSecondsSinceShoot = 0f;
        }
    }

    private void aimWeapon()
    {
        elapsedSecondsSinceAim += Time.deltaTime;
        if (!isSetAimingInterval)
        {
            aimingInterval = UnityEngine.Random.Range(0.2f, 1f);
            isSetAimingInterval = true;
        }

        if (watchingSense.canSee)
        {

            var bounds = playerController.bounds;
            shootPoint.position = Vector3.Lerp(shootPoint.position, targetShootingPosition, 0.5f);
            if (Vector3.Distance(shootPoint.position, targetShootingPosition) < 4f)
            {
                isDoneAiming = true;
            }
            else
            {
                isDoneAiming = false;
            }

            if (elapsedSecondsSinceAim > aimingInterval)
            {
                isSetAimingInterval = false;
                elapsedSecondsSinceAim = 0f;
                if (isDoneAiming)
                {
                    var randomShootOffset = new Vector3(
                        UnityEngine.Random.Range(-bounds.extents.x * shootXOffsetFactor, bounds.extents.x * shootXOffsetFactor),
                        UnityEngine.Random.Range(-bounds.extents.y * shootYOffsetFactor, bounds.extents.y * shootYOffsetFactor),
                        UnityEngine.Random.Range(-bounds.extents.z * shootZOffsetFactor,
                            bounds.extents.z * shootZOffsetFactor));

                    targetShootingPosition = bounds.center + randomShootOffset;
                    Debug.DrawLine(weapon.bulletOrigin.position, targetShootingPosition, Color.red, 5f);
                }
            }

        }


    }


    private void checkSenses()
    {
        if (watchingSense.canSee)
        {
            stareAt(watchingSense.playerFiringPosition);
            sensedPlayer = true;
            hearingSense.hearedPlayer = false;
            possiblePlayerPosition = watchingSense.playerFiringPosition;
            hearingSense.playerFiringPosition = possiblePlayerPosition;
        }
        else if (hearingSense.hearedPlayer)
        {
            stareAt(hearingSense.playerFiringPosition);
            shootPoint.position = hearingSense.playerFiringPosition;
            sensedPlayer = true;
            possiblePlayerPosition = hearingSense.playerFiringPosition;
        }
    }



    private void stareAt(Vector3 target)
    {
        var relativePosition = target - transform.position;
        var targetRotation = Quaternion.LookRotation(relativePosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        playeeeeeeeeee.position = target;
        //shootPoint.position = target;

    }

    void runToPlayer()
    {
        elapsedSecondsSinceMoved += Time.deltaTime;
        if (!stoppingDistanceSet)
        {
            stoppingDistanceSet = true;
            navAgent.stoppingDistance = UnityEngine.Random.Range(stopDistanceFromPlayerMin, stopDistanceFromPlayerMax);
        }
        // Debug.Log("moving elapsed: " + elapsedSecondsSinceMoved + " wait: " + timeToWaitToMove + " mindis: " + navAgent.stoppingDistance + " dis: " + Vector3.Distance(transform.position , target.position));

        if (navAgent.velocity.magnitude >= -0.02f && navAgent.velocity.magnitude <= 0.02f &&
            elapsedSecondsSinceMoved > timeToWaitToMove)
        {
            navAgent.stoppingDistance = UnityEngine.Random.Range(stopDistanceFromPlayerMin, stopDistanceFromPlayerMax);
            timeToWaitToMove = UnityEngine.Random.Range(moveIntervalMin, moveIntervalMax);
            elapsedSecondsSinceMoved = 0f;
        }

        animator.SetFloat("InputX", navAgent.velocity.x);
        animator.SetFloat("InputY", navAgent.velocity.z);
    }
}