using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerInfoScript : MonoBehaviour
{
    public static PlayerInfoScript playerInfo;
    public int playerSkin = -1;
    public int heartColor = 0;
    public int playerSong = 0;
    public int coins;

    public int level;
    public int lives;

    public float spinMultiplier;

    public bool supporterPackage = false;
    public int timesPlayed = 0;


    private void Awake()
    {
        if (playerInfo == null)
        {
            playerInfo = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (playerInfo != this)
        {
            Destroy(gameObject);
        }
        if (MenuController.reset && File.Exists(Application.persistentDataPath + "/playerInfo.dat")) File.Delete(Application.persistentDataPath + "/playerInfo.dat");
        //load information as soon as possible
        Load();
    }

    private void OnDisable()
    {
        //Save player info whenever disabled
        Save();
    }
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")) file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
        else file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.playerSkin = playerSkin;
        data.heartColor = heartColor;
        data.playerSong = playerSong;
        data.coins = coins;

        data.level = level;
        data.lives = lives;

        data.spinMultiplier = spinMultiplier;

        data.supporterPackage = supporterPackage;
        data.timesPlayed = timesPlayed;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            //File.Delete(Application.persistentDataPath + "/playerInfo.dat");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            playerSkin = data.playerSkin;
            heartColor = data.heartColor;
            playerSong = data.playerSong;
            coins = data.coins;

            level = data.level;
            lives = data.lives;

            spinMultiplier = data.spinMultiplier;

            supporterPackage = data.supporterPackage;
            timesPlayed = data.timesPlayed;
        }
    }

    [Serializable]
    class PlayerData
    {
        public int playerSkin;
        public int heartColor;
        public int playerSong;
        public int coins;

        public int level;
        public int lives;

        public float spinMultiplier;

        public bool supporterPackage;
        public int timesPlayed;
    }
}

