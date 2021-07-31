using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
    public GameObject player;
    Vector3 playerSpawnPos = new Vector3(0, 12.4f, 0);

    int curLevel;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI countDownText;

    //read by MapGenerator
    public static int rows;
    //read by enemySpawnController
    public static int enemyAmount;
    //read by enemyController
    public static int enemyHealth;
    //amount of times enemySpawnScript spawns a group of enemies on tile, read by enemySpawnController
    public static int amountOfSpawns;
    //time inbetween spawning enemies, read by enemySpawnController
    public static float spawnTime;

    //given by PlayerController when picked up time potion
    [HideInInspector]
    public int extraTime = 0;
    //Added to by enemySpawnController
    public static int curAmountOfSpawns;

    MapGenerator mapGenerator;
    EnemySpawnController enemySpawner;
    PotionSpawner potionSpawner;

    public GameObject wonPanel;
    public GameObject lostPanel;
    public GameObject noLivesPanel;

    //Given by Map Generator after map has been created
    [HideInInspector]
    public static GameObject tileHolder;

    void Awake()
    {
        wonPanel.SetActive(false);
        lostPanel.SetActive(false);
        noLivesPanel.SetActive(false);
        countDownText.enabled = false;
        curLevel = PlayerInfoScript.playerInfo.level;

        mapGenerator = this.GetComponent<MapGenerator>();
        enemySpawner = this.GetComponent<EnemySpawnController>();
        potionSpawner = this.GetComponent<PotionSpawner>();

        //load past level information
        //this is nessessary because we are just adding to the previous stats when running NextLevel();
        Load();
    }

    private void Start()
    {
        if (PlayerInfoScript.playerInfo.lives > 0) NextLevel();
        else noLivesPanel.SetActive(true);
        levelText.text = curLevel.ToString();
        livesText.text = PlayerInfoScript.playerInfo.lives.ToString();
    }

    void Update()
    {
        //When the current amount of spawns exceeds the amount of spawns for this level
        if (curAmountOfSpawns > amountOfSpawns)
        {
            curAmountOfSpawns = 0;
            //stop spawning enemies
            enemySpawner.StopSpawningEnemies();
            //stop spawning potions
            potionSpawner.StopSpawningPotions();
            //run timer that gives player a certain amount of time to kill remaining enemies
            //If they kill all enemies within this time they pass, else they lose
            StartCoroutine(AllEnemiesSpawned());
        }
    }

    //Also Called by player controller if player dies
    public void NextLevel()
    {
        //Just incase coroutines are still running when next map is spawned, end them
        StopAllCoroutines();
        //Destroy unNeed scene objects from old map
        DestroyLeftOvers();
        //Stop player from moving, and move him to the correct position
        player.GetComponent<PlayerMovementController>().StopMovement();
        player.transform.position = playerSpawnPos;

        countDownText.enabled = false;

        extraTime = 0;

        //Remove the last tiles from enemySpawnController
        EnemySpawnController.allTiles.Clear();
        //First three levels should have these stats
        if (curLevel <= 3)
        {
            rows = 2;
            enemyAmount = 1;
            enemyHealth = 30;
            amountOfSpawns = 1;
            spawnTime = 7;
        }
        else if (curLevel == 5)
        {
            rows = 3;
            enemyAmount = 2;
            enemyHealth = 10;
            //Add to the amount of times that a group of enemies spawn on a tile
            amountOfSpawns = 3;
            spawnTime = 6f;
        }
        else if (curLevel == 15)
        {
            rows = 4;
            enemyHealth = 15;
            enemyAmount = 3;
            amountOfSpawns = 5;
            spawnTime = 6f;
        }
        else if(curLevel == 30)
        {
            rows = 5;
            enemyHealth = 20;
            enemyAmount = 4;
            amountOfSpawns = 7;
            spawnTime = 6;
        }
        //After level 40 increase variables every ten levels
        if (curLevel > 40 && curLevel % 20 == 0)
        {
            amountOfSpawns += 1;
            spawnTime += 2f;
        }

        mapGenerator.ReGenerateMap();
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPotions());
        
        player.GetComponent<PlayerController>().hasDied = false;
        //reset players health to the max health
        player.GetComponent<PlayerController>().health = player.GetComponent<PlayerController>().maxHealth;
        player.GetComponent<PlayerController>().UpdateHealth();
        print("Max Health: " + player.GetComponent<PlayerController>().maxHealth);
    }

    IEnumerator AllEnemiesSpawned()
    {
        //Enable count down timer
        countDownText.enabled = true;
        //set amount of time to count down
        float timeLeft = spawnTime + 8 + extraTime;

        if (MenuController.soundEffects) AudioManager.instance.Play("TimerStart");

        //count down until timeleft = 0 or there are no enemies left
        while (true)
        {
            timeLeft -= .1f;
            //Update count down text
            countDownText.text = Math.Round(timeLeft, 2).ToString();
            //countDownText.text = Mathf.RoundToInt(timeLeft).ToString();

            //if timer equals zero, end loop
            if (timeLeft < 0) break;
            //if there are no enemies left, stop countdown
            else if (CheckIfAllEnemiesDead()) break;

            yield return new WaitForSeconds(.1f);
        }

        countDownText.enabled = false;

        //If all enemies were killed add 1 to level and spawn next map
        //if they lose dont increase level, just spawn new map
        if (CheckIfAllEnemiesDead())
        {
            //After level is passed, save the map stats
            Save();
            PlayerInfoScript.playerInfo.Save();
            CloudOnceServices.instance.SubmitScoreToLeaderboard(PlayerInfoScript.playerInfo.level);

            if (MenuController.soundEffects) AudioManager.instance.Play("BeatLevel");
            if (PlayerInfoScript.playerInfo.lives > 0) wonPanel.SetActive(true);
            else noLivesPanel.SetActive(true);
        }
        else
        {
            if (MenuController.soundEffects) AudioManager.instance.Play("LevelLost");
            if (PlayerInfoScript.playerInfo.lives > 0) lostPanel.SetActive(true);
            else noLivesPanel.SetActive(true);

            KillAllEnemies();

            PlayerInfoScript.playerInfo.lives--;

            PlayerInfoScript.playerInfo.Save();
        }

        PlayerInfoScript.playerInfo.timesPlayed++;
        CheckTimesPlayed();
        yield return null;
    }

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

    //checks all tiles to see if any enemies are alive
    bool CheckIfAllEnemiesDead()
    {
        foreach (Transform child in tileHolder.transform)
        {
            //if tile does contain enemy, return false
            if (CheckTileForEnemies(child.gameObject))
            {
                return false;
            }
        }
        //if no enemies were found, return true
        return true;
    }

    IEnumerator SpawnEnemies()
    {
        //let navMeshBuild before spawning enemies
        yield return new WaitForSeconds(.5f);

        enemySpawner.StartSpawningEnemies();
    }

    IEnumerator SpawnPotions()
    {
        yield return new WaitForSeconds(4f);
        //start spawning potions
        potionSpawner.StartSpawningPotions();
    }

    void DestroyLeftOvers()
    {
        //destories all death particles from last map
        foreach (GameObject particle in GameObject.FindGameObjectsWithTag("DeathParticle")) Destroy(particle);
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/levelInfo.dat")) file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
        else file = File.Create(Application.persistentDataPath + "/levelInfo.dat");

        LevelData data = new LevelData();

        data.rows = rows;
        data.enemyAmount = enemyAmount;

        data.enemyHealth = enemyHealth;
        data.amountOfSpawns = amountOfSpawns;
        data.spawnTime = spawnTime;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/levelInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
            LevelData data = (LevelData)bf.Deserialize(file);
            file.Close();

            rows = data.rows;
            enemyAmount = data.enemyAmount;

            enemyHealth = data.enemyHealth;
            amountOfSpawns = data.amountOfSpawns;
            spawnTime = data.spawnTime;
        }
    }
    //accessed by GameMenuButtons
    public void LevelWonB()
    {
        if (MenuController.soundEffects) AudioManager.instance.Play("BeatLevel2");
        curLevel++;
        PlayerInfoScript.playerInfo.level++;
        levelText.text = curLevel.ToString();
        livesText.text = PlayerInfoScript.playerInfo.lives.ToString();
        wonPanel.SetActive(false);
        NextLevel();
    }
    //accessed by GameMenuButtons
    public void LevelLostB()
    {
        livesText.text = PlayerInfoScript.playerInfo.lives.ToString();
        NextLevel();
        lostPanel.SetActive(false);
    }

    public void playerDied()
    {
        KillAllEnemies();
        PlayerInfoScript.playerInfo.lives--;

        countDownText.enabled = false;
        if (PlayerInfoScript.playerInfo.lives > 0) lostPanel.SetActive(true);
        else noLivesPanel.SetActive(true);
        //These need to be ran again incase NextLevel was called because of player death
        curAmountOfSpawns = 0;
        //stop spawning enemies
        enemySpawner.StopSpawningEnemies();
        //stop spawning potions
        potionSpawner.StopSpawningPotions();
        //Just incase coroutines are still running when next map is spawned, end them
        StopAllCoroutines();

        if (MenuController.soundEffects) AudioManager.instance.Play("LevelLost");

        PlayerInfoScript.playerInfo.timesPlayed++;
        CheckTimesPlayed();

        //After player dies, save
        Save();
        PlayerInfoScript.playerInfo.Save();
    }

    void KillAllEnemies()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }

    void CheckTimesPlayed()
    {
        //play ad every 8 times, on the fifth time an add play a rewarded video
        if (PlayerInfoScript.playerInfo.timesPlayed == 8 && !PlayerInfoScript.playerInfo.supporterPackage)
        {
            PlayerInfoScript.playerInfo.timesPlayed = 0;
            AdController.AdInstance.ShowAd("video");
        }
    }

    [Serializable]
    class LevelData
    {
        public int rows;
        public int enemyAmount;
        public int enemyHealth;
        public int amountOfSpawns;
        public float spawnTime;
    }
}
