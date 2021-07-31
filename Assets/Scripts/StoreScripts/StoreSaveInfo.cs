using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StoreSaveInfo : MonoBehaviour
{
    public static StoreSaveInfo storeInfo;

    public List<int> boughtSkins = new List<int>();
    public List<int> boughtMusic = new List<int>();
    public List<int> boughtColors = new List<int>();

    public int firstSelectedSkin = -1;

    public bool supportersPackage;

    private void Awake()
    {
        if (storeInfo == null)
        {
            storeInfo = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (storeInfo != this)
        {
            Destroy(gameObject);
        }
        if (MenuController.reset && File.Exists(Application.persistentDataPath + "/storeInfo.dat")) File.Delete(Application.persistentDataPath + "/storeInfo.dat");
        //load information as soon as possible
        Load();
    }

    private void OnDisable()
    {
        //Save store info whenever disabled
        Save();
    }
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/storeInfo.dat")) file = File.Open(Application.persistentDataPath + "/storeInfo.dat", FileMode.Open);
        else file = File.Create(Application.persistentDataPath + "/storeInfo.dat");

        PlayerData data = new PlayerData();
        data.boughtSkins = boughtSkins;
        data.boughtMusic = boughtMusic;
        data.boughtColors = boughtColors;
        data.firstSelectedSkin = firstSelectedSkin;

        data.supportersPackage = supportersPackage;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/storeInfo.dat"))
        {
            //File.Delete(Application.persistentDataPath + "/storeInfo.dat");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/storeInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            boughtSkins = data.boughtSkins;
            boughtMusic = data.boughtMusic;
            boughtColors = data.boughtColors;
            firstSelectedSkin = data.firstSelectedSkin;

            supportersPackage = data.supportersPackage;
        }
    }

    [Serializable]
    class PlayerData
    {
        public List<int> boughtSkins;
        public List<int> boughtMusic;
        public List<int> boughtColors;

        public int firstSelectedSkin;

        public bool supportersPackage;
    }
}
