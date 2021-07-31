using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;

public class EnemyController : MonoBehaviour
{
    //read in EnemyHitDetection
    [HideInInspector]
    public float health;
    public float enemySpeed;

    public String enemySound;

    //CurTile given by enemySpawnController on spawn
    [HideInInspector]
    public GameObject curTile;

    NavMeshAgent agent;
    bool agentStopped;

    GameObject player;

    EnemyAnimator enemyAnim;

    public GameObject deathParticle;

    void Awake()
    {
        enemyAnim = this.GetComponent<EnemyAnimator>();
        agent = this.GetComponent<NavMeshAgent>();
        player = null;
    }

    void Start()
    {
        health = GameController.enemyHealth;
        StartCoroutine(Walk());

        this.GetComponent<EnemyFireScript>().curTile = curTile;

        if (enemySound != "NULL") StartCoroutine(EnemySounds());
    }

    private void Update()
    {
        //if player isnt equal to null
        if (player != null)
        {
            //if the distance to player is greater than sum value
            if (Vector3.Distance(this.transform.position, player.transform.position) > 8)
            {
                //stop chasing after player
                StopCoroutine(MoveTowardsPlayerRoutine());
                player = null;
            }
            //if enemy is close enough to player, stop chasing player and stop agent
            else if (Vector3.Distance(this.transform.position, player.transform.position) < 6)
            {
                StopCoroutine(MoveTowardsPlayerRoutine());
                agent.isStopped = true;
                agentStopped = true;

                //If enemy is less than 3 units away from player, look at him
                this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
            }
            else
            {
                //If enemy is less than 7 units away from player, look at him
                this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
            }
        }

        //if enemy had been stopped
        if (agentStopped)
        {
            //check if the player is null, or if the player is far away from enemy and resume its wandering
            if (player == null || Vector3.Distance(this.transform.position, player.transform.position) > 4)
            {
                agentStopped = false;
                agent.isStopped = false;
                StartCoroutine(Walk());
            }
        }

        if (health <= 0)
        {
            //When enemy dies, spawn death particle at their location
            GameObject curParticle = Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(curParticle, 7);
            if(MenuController.shake) CameraShaker.Instance.ShakeOnce(2f, 2f, .1f, 1f);
            Destroy(this.gameObject);
        }

        if (agent.velocity != Vector3.zero) enemyAnim.walking = true;
        else enemyAnim.walking = false;
    }

    IEnumerator Walk()
    {
        while (true)
        {
            try
            {
                //If the player isnt in range, wander
                if(player == null) agent.SetDestination(FindPoint());
            }
            catch (Exception)
            {
                print("could not set destination");
            }
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator MoveTowardsPlayerRoutine()
    {
        while (true)
        {
            try
            {
                agent.SetDestination(player.transform.position);
            }
            catch (Exception)
            {
                //print("could not chase player");
            }
            yield return new WaitForSeconds(3);
        }
    }

    Vector3 FindPoint()
    {
        Vector3 walkToLoc;
        Vector3 randomPoint;
        float tileRadius = -8.6579f;

        int tries = 0;
        //Loop finding random points until point on navmesh is found
        do
        {
            //Find a random point inside circle with the same radius of tile, then add it the tiles position to make sure it's in the right location
            //positions z and y are switched to account for the rotation of the tiles
            randomPoint = ((Vector3)UnityEngine.Random.insideUnitCircle * Mathf.Abs(tileRadius)) + new Vector3(curTile.transform.position.x, curTile.transform.position.z, curTile.transform.position.y);
            //Since points were found using a 2d circle with the wrong rotation, the y value was put inplace of the z value.
            //Actual y value is adjusted so point is on top of tile
            walkToLoc = new Vector3(randomPoint.x, 11f + curTile.transform.position.y, randomPoint.y);

            if (IsPointOnNavMesh(walkToLoc)) break;
            //try to find point 15 times, if tries is higher then break loop
            else if (tries > 15) break;
            else tries++;
        }
        while (!IsPointOnNavMesh(walkToLoc));

        if (tries >= 15) print("Couldn't find point to walk to: " + walkToLoc);

        return walkToLoc;
    }

    bool IsPointOnNavMesh(Vector3 targetDestination)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 1f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject;
            StartCoroutine(MoveTowardsPlayerRoutine());
        }

        //health deducted by enemyHitDetection
    }

    IEnumerator EnemySounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 6));

            if (MenuController.soundEffects) AudioManager.instance.Play(enemySound);
        }
    }
}
