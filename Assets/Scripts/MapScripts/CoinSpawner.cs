using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public float timeBetweenCoins;
    float xOffset = -8.6579f;

    public GameObject coin;

    private void Start()
    {
        StartSpawningCoins();
    }

    public void StartSpawningCoins()
    {
        StartCoroutine("SpawningTimer");
    }

    IEnumerator SpawningTimer()
    {
        while (true)
        {
            //get tile from enemySpawnControllers static list
            GameObject spawnTile = EnemySpawnController.allTiles[Random.Range(0, EnemySpawnController.allTiles.Count)];

            //If the spawn tile is avaliable, spawn coin then wait time, else keep looping until tile is avaliable
            if (spawnTile != null)
            {
                SpawnCoin(spawnTile);
                yield return new WaitForSeconds(Random.Range(timeBetweenCoins, timeBetweenCoins + 10));
            }
            else yield return null;
        }
    }
    void SpawnCoin(GameObject tile)
    {
        Vector3 spawnLoc = Vector3.zero;
        Vector3 randomPoint;

        //Find a random point inside circle with the same radius of tile, then add it the tiles position to make sure it's in the right location
        //positions z and y are switched to account for the rotation of the tiles
        randomPoint = ((Vector3)Random.insideUnitCircle * Mathf.Abs(xOffset)) + new Vector3(tile.transform.position.x, tile.transform.position.z, tile.transform.position.y);
        //Since points were found using a 2d circle with the wrong rotation, the y value was put inplace of the z value.
        //Actual y value is adjusted so potion spawns ontop of tile
        spawnLoc = new Vector3(randomPoint.x, 11f + tile.transform.position.y, randomPoint.y);

        GameObject curPotion = Instantiate(coin, spawnLoc, Quaternion.identity);
        curPotion.transform.parent = tile.transform;
    }
}
