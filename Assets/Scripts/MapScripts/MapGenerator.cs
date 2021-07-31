using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public List<GameObject> tiles = new List<GameObject>();
    public float heightVarient;

    float xOffset = -8.6579f;
    float zOffset = -14.9848f;

    float yOffset = 6.30f;

    public NavMeshSurface navMeshSurface;

    public CameraController camController;

    GameObject curTileHolder;

    public bool tutorial;

    private void Start()
    {
        if (tutorial) ReGenerateMap();
    }

    //Called by GameController to remake map
    public void ReGenerateMap()
    {
        navMeshSurface.RemoveData();
        if(curTileHolder) Destroy(curTileHolder);
        GenerateMap();

        //Change Camera startPos
        if(!tutorial) camController.FindStartPos(GameController.rows);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.UpArrow)) navMeshSurface.BuildNavMesh();

        //if (Input.GetKeyDown(KeyCode.Space)) ReGenerateMap();
    }
    public void GenerateMap()
    {
        //Create object to place tiles in
        GameObject tileHolder = new GameObject();
        tileHolder.name = "TileHolder";

        curTileHolder = tileHolder;

        //instaniate variables
        GameObject curTile;
        Vector3 spawnPoint;
        float yVal = 0;

        //Place first tile
        spawnPoint = new Vector3(0, yVal, 0);
        curTile = Instantiate(tiles[UnityEngine.Random.Range(0, tiles.Count)], spawnPoint, Quaternion.identity);
        curTile.transform.Rotate(90, 0, 0);
        curTile.transform.parent = tileHolder.transform;

        curTile.name = 0 + " " + 0;

        //Loop through rows and columns placing tiles
        for (int row = 1; row < GameController.rows; row++)
        {
            float curXOffset = xOffset * row;
            float curZOffset = zOffset * row;
            for (int col = row + 1; col > 0; col--)
            {
                //curXOffset is increased each time this loop runs, while curZOffset stays the same. Random y value is generated
                spawnPoint = new Vector3(curXOffset, yVal - (yOffset * row) + UnityEngine.Random.Range(-heightVarient, heightVarient), curZOffset);
                curTile = Instantiate(tiles[UnityEngine.Random.Range(0, tiles.Count)], spawnPoint, Quaternion.identity);
                curTile.transform.Rotate(90, 0, 0);
                curTile.transform.parent = tileHolder.transform;

                curTile.name = row + " " + col;

                //Add tile to EnemySpawnController tile list
                EnemySpawnController.allTiles.Add(curTile);

                curXOffset -= (xOffset * 2);
            }
        }
        //Give gameController tileholder
        GameController.tileHolder = tileHolder;

        print("Map remade");

        StartCoroutine(BuildNavMesh());
    }

    IEnumerator BuildNavMesh()
    {
        //wait to let map build before trying to build navMesh
        yield return new WaitForSeconds(.2f);
        navMeshSurface.BuildNavMesh();
    }
}
