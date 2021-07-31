using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public float timeBetweenPotions;
    float xOffset = -8.6579f;

    public GameObject[] potions = new GameObject[4];


    public void StartSpawningPotions()
    {
        StartCoroutine("SpawningTimer");
    }

    public void StopSpawningPotions()
    {
        StopCoroutine("SpawningTimer");
    }

    IEnumerator SpawningTimer()
    {
        while (true)
        {
            //get tile from enemySpawnControllers static list
            GameObject spawnTile = EnemySpawnController.allTiles[Random.Range(0, EnemySpawnController.allTiles.Count)];

            if(spawnTile != null) SpawnPotion(spawnTile);

            yield return new WaitForSeconds(timeBetweenPotions);
        }
    }
    void SpawnPotion(GameObject tile)
    {
        Vector3 spawnLoc = Vector3.zero;
        Vector3 randomPoint;

        //Find a random point inside circle with the same radius of tile, then add it the tiles position to make sure it's in the right location
        //positions z and y are switched to account for the rotation of the tiles
        randomPoint = ((Vector3)Random.insideUnitCircle * Mathf.Abs(xOffset)) + new Vector3(tile.transform.position.x, tile.transform.position.z, tile.transform.position.y);
        //Since points were found using a 2d circle with the wrong rotation, the y value was put inplace of the z value.
        //Actual y value is adjusted so potion spawns ontop of tile
        spawnLoc = new Vector3(randomPoint.x, 11f + tile.transform.position.y, randomPoint.y);

        GameObject curPotion = Instantiate(potions[Random.Range(0, potions.Length)], spawnLoc, Quaternion.identity);
        curPotion.transform.parent = tile.transform;
    }
}
