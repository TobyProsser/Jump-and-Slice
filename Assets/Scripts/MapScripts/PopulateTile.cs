using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateTile : MonoBehaviour
{
    //which enemy to spawn
    public List<int> enemyNum = new List<int>();
    public int tileType;
    public int treeChance;                  //1 is equal to 100%, the higher the number the lower the chance
    float xOffset = -8.6579f;

    public List<GameObject> bigObjects = new List<GameObject>();

    void Start()
    {
        Populate();
    }

    void Populate()
    {
        //Chance for trees to spawn on tile
        if (Random.Range(0, treeChance) == 0)
        {
            //Random amount of trees to spawn on tiles
            int treeAmount;
            if (tileType == 3) treeAmount = 1;
            else treeAmount = Random.Range(1, 5);

            Vector3 spawnLoc = Vector3.zero;
            Vector3 randomPoint;
            //Loop through amount of requested trees and place them
            for (int i = 0; i < treeAmount; i++)
            {
                //Find a random point inside circle with the same radius of tile, then add it the tiles position to make sure it's in the right location
                //positions z and y are switched to account for the rotation of the tiles
                randomPoint = ((Vector3)Random.insideUnitCircle * Mathf.Abs(xOffset)) + new Vector3(transform.position.x, transform.position.z, transform.position.y);
                //Since points were found using a 2d circle with the wrong rotation, the y value was put inplace of the z value.
                //Actual y value is adjusted so tree spawns ontop of tile
                if(tileType == 0) spawnLoc = new Vector3(randomPoint.x, 11f + transform.position.y, randomPoint.y);
                else if(tileType == 1) spawnLoc = new Vector3(randomPoint.x, 11f + transform.position.y, randomPoint.y);
                else if(tileType == 3) spawnLoc = new Vector3(randomPoint.x, 11f + transform.position.y, randomPoint.y);

                GameObject curTree = Instantiate(bigObjects[Random.Range(0, bigObjects.Count)], spawnLoc, Quaternion.identity);
                curTree.transform.parent = transform;
            }
        }
    }
}
