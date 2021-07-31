using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnController : MonoBehaviour
{
    //list of all spawned tiles, given by MapGenerator
    public static List<GameObject> allTiles = new List<GameObject>();
    //Tile which player is currently on, given by playerController
    public static GameObject curPlayerTile;

    public List<GameObject> enemies = new List<GameObject>();

    private IEnumerator FindTileToSpawn;

    IEnumerator FindTileToSpawnEnemies()
    {
        GameObject curSpawningTile;

        while (true)
        {
            //If player is on tile (stops code from running at start while curplayerTile hasent been assigned yet)
            //and if tileHolder has been created
            if (curPlayerTile != null && GameController.tileHolder != null)
            {
                //create list off all the free tiles
                List<GameObject> freeTileList = GetAllFreeTiles();
                //if the freeList is not empty
                if (freeTileList.Count != 0)
                {
                    //Get a random tile in the freeTileList
                    curSpawningTile = freeTileList[UnityEngine.Random.Range(0, freeTileList.Count)];
                    //Spawn enemies on the tile
                    SpawnEnemies(curSpawningTile);
                }
                else print("No more free tiles, game end");   //GAME WILL PROBABLY END AT THIS POINT

                //Find another tile to spawn enemies on after x time
                yield return new WaitForSeconds(GameController.spawnTime);
            }
            else yield return null;
        }
    }

    //ran by GameController
    public void StartSpawningEnemies()
    {
        StartCoroutine("FindTileToSpawnEnemies");
    }
    //ran by GameController
    public void StopSpawningEnemies()
    {
        //stop stored coroutine
        StopCoroutine("FindTileToSpawnEnemies");
    }

    void SpawnEnemies(GameObject curTile)
    {
        List<int>enemyType = curTile.transform.GetComponent<PopulateTile>().enemyNum;

        for (int i = 0; i < GameController.enemyAmount; i++)
        {
            try
            {
                GameObject curEnemy = Instantiate(enemies[enemyType[UnityEngine.Random.Range(0, enemyType.Count)]], FindPoint(curTile), Quaternion.identity);
                curEnemy.GetComponent<EnemyController>().curTile = curTile;

                curEnemy.transform.parent = curTile.transform;
            }
            catch (Exception)
            {
                print("Could not spawn enemy");
            }
        }

        //increase the amount of times enemies were spawned on GameController
        GameController.curAmountOfSpawns++;
    }

    Vector3 FindPoint(GameObject curTile)
    {
        Vector3 point;
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
            point = new Vector3(randomPoint.x, 11f + curTile.transform.position.y, randomPoint.y);

            if (IsPointOnNavMesh(point))
            {
                //print("POINT FOUND");
                break;
            }
            //try to find point 15 times, if tries is higher then break loop
            else if (tries > 15) break;
            else tries++;
        }
        while (!IsPointOnNavMesh(point));

        if (tries >= 15) print("Couldn't find spawn point: " + point);

        return point;
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

    //checks to see if any enemies are on tile
    bool CheckTileForEnemies(GameObject tile)
    {
        //Loops through tile children
        foreach (Transform child in tile.transform)
        {
            //If child's tag is Enemy, return true
            if (child.tag == "Enemy")
            {
                return true;
            }
        }
        //if loop finishes without finding Enemy tag, return false
        return false;
    }

    //checks all tiles adds tiles that do not currently have enemies and player isnt on to a list
    List<GameObject> GetAllFreeTiles()
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in GameController.tileHolder.transform)
        {
            //if tile does not have enemies and player isnt on it, add it to the list
            if (!CheckTileForEnemies(child.gameObject) && !PlayerOnTile(child.gameObject)) tempList.Add(child.gameObject);
        }

        return tempList;
    }

    //Check if player is on tile
    bool PlayerOnTile(GameObject curTile)
    {
        //just comparing objects doesnt work for some reason, so I compare their names
        if (curTile.name != curPlayerTile.transform.parent.name) return false;
        else return true;
    }
}