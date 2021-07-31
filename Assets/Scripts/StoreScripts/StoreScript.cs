using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StoreScript : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI livesText;

    public GameObject[] panels = new GameObject[5];

    public GameObject notEnoughCoinsPanel;
    public GameObject supportPackage;
    public GameObject supportPurchaseButton;

    public GameObject skinsHolder;
    List<GameObject> skins = new List<GameObject>();
    int curSkinViewNum = 0;

    public GameObject songsHolder;
    List<GameObject> songs = new List<GameObject>();
    int curSongViewNum = 0;

    public GameObject colorsHolder;
    List<GameObject> colors = new List<GameObject>();
    int curColorViewNum = 0;

    int cost;

    void Awake()
    {
        ChangePanel(0);
        notEnoughCoinsPanel.SetActive(false);
        coinsText.text = PlayerInfoScript.playerInfo.coins.ToString();
        livesText.text = PlayerInfoScript.playerInfo.lives.ToString();

        //fill skin list with skins
        foreach (Transform skin in skinsHolder.transform)
        {
            skins.Add(skin.gameObject);
        }

        ChangeViewSkin();

        foreach (Transform song in songsHolder.transform)
        {
            songs.Add(song.gameObject);
        }

        ChangeViewSong();

        foreach (Transform color in colorsHolder.transform)
        {
            colors.Add(color.gameObject);
        }

        ChangeViewColor();

        UpdateStoresInfo();
    }

    private void Start()
    {
        if (PlayerInfoScript.playerInfo.supporterPackage) supportPurchaseButton.SetActive(false);
    }

    public void SelectButtons(int panel)
    {
        AudioManager.instance.Play("Click");
        ChangePanel(panel);
    }

    public void BackButton()
    {
        AudioManager.instance.Play("Click");
        ChangePanel(0);
    }

    public void MenuButton()
    {
        AudioManager.instance.Play("Click");
        SceneManager.LoadScene("MainMenu");
    }

    public void SetCost(int inputcost) { cost = inputcost; }

    public void CoinsForLives(int lives)
    {
        AudioManager.instance.Play("Click");
        print(PlayerInfoScript.playerInfo.coins);
        if (PlayerInfoScript.playerInfo.coins >= cost)
        {
            PlayerInfoScript.playerInfo.coins -= cost;
            PlayerInfoScript.playerInfo.lives += lives;

            livesText.text = PlayerInfoScript.playerInfo.lives.ToString();
            coinsText.text = PlayerInfoScript.playerInfo.coins.ToString();

            //Save lives in player;
            PlayerInfoScript.playerInfo.Save();
        }
        else
        {
            notEnoughCoinsPanel.SetActive(true);
        }
    }

    public void NextSkin()
    {
        AudioManager.instance.Play("Click");
        if (curSkinViewNum >= skins.Count -1) curSkinViewNum = 0;
        else curSkinViewNum++;
        ChangeViewSkin();
    }

    public void PreviousSkin()
    {
        AudioManager.instance.Play("Click");
        if (curSkinViewNum <= 0) curSkinViewNum = skins.Count - 1;
        else curSkinViewNum--;
        ChangeViewSkin();
    }

    public void NextSong()
    {
        AudioManager.instance.Play("Click");
        if (curSongViewNum >= songs.Count - 1) curSongViewNum = 0;
        else curSongViewNum++;
        ChangeViewSong();
    }

    public void PreviousSong()
    {
        AudioManager.instance.Play("Click");
        if (curSongViewNum <= 0) curSongViewNum = songs.Count - 1;
        else curSongViewNum--;
        ChangeViewSong();
    }

    public void NextColor()
    {
        AudioManager.instance.Play("Click");
        if (curColorViewNum >= colors.Count - 1) curColorViewNum = 0;
        else curColorViewNum++;
        ChangeViewColor();
    }

    public void PreviousColor()
    {
        AudioManager.instance.Play("Click");
        if (curColorViewNum <= 0) curColorViewNum = colors.Count - 1;
        else curColorViewNum--;
        ChangeViewColor();
    }

    void ChangeViewSkin()
    {
        for (int i = 0; i < skins.Count; i++)
        {
            skins[i].SetActive(false);
        }

        skins[curSkinViewNum].SetActive(true);
    }

    void ChangeViewSong()
    {
        for (int i = 0; i < songs.Count; i++)
        {
            songs[i].SetActive(false);
        }

        songs[curSongViewNum].SetActive(true);
    }

    void ChangeViewColor()
    {
        for (int i = 0; i < colors.Count; i++)
        {
            colors[i].SetActive(false);
        }

        colors[curColorViewNum].SetActive(true);
    }

    void ChangePanel(int panel)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        panels[panel].SetActive(true);
    }

    public void CloseNotEnoughCoinsPanel()
    {
        AudioManager.instance.Play("Click");
        notEnoughCoinsPanel.SetActive(false);
    }

    void UpdateStoresInfo()
    {
        //If skins were added since the last save, add extra zeros to the end of saved skins list
        if (StoreSaveInfo.storeInfo.boughtSkins.Count < skins.Count)
        {
            int missingSlots = skins.Count - StoreSaveInfo.storeInfo.boughtSkins.Count;

            for (int i = 0; i < missingSlots; i++)
            {
                StoreSaveInfo.storeInfo.boughtSkins.Add(0);
            }
        }

        //If skins were added since the last save, add extra zeros to the end of saved skins list
        if (StoreSaveInfo.storeInfo.boughtMusic.Count < songs.Count)
        {
            int missingSlots = songs.Count - StoreSaveInfo.storeInfo.boughtMusic.Count;

            for (int i = 0; i < missingSlots; i++)
            {
                StoreSaveInfo.storeInfo.boughtMusic.Add(0);
            }
        }

        //If skins were added since the last save, add extra zeros to the end of saved skins list
        if (StoreSaveInfo.storeInfo.boughtColors.Count < colors.Count)
        {
            int missingSlots = colors.Count - StoreSaveInfo.storeInfo.boughtColors.Count;

            for (int i = 0; i < missingSlots; i++)
            {
                StoreSaveInfo.storeInfo.boughtColors.Add(0);
            }
        }

        print(StoreSaveInfo.storeInfo.boughtSkins.Count);
        print(StoreSaveInfo.storeInfo.boughtMusic.Count);
        print(StoreSaveInfo.storeInfo.boughtColors.Count);
    }

    public void CoinsForSkin(int skinNum)
    {
        AudioManager.instance.Play("Click");
        //check if this skin number has already been bought, if it has make it free
        //or if the skinNum is equal to the first selected skin, make it free
        if (StoreSaveInfo.storeInfo.boughtSkins[skinNum] == 1 || skinNum == StoreSaveInfo.storeInfo.firstSelectedSkin)
        {
            cost = 0;
        }

        //check if player has more coins than the cost
        if (PlayerInfoScript.playerInfo.coins >= cost)
        {
            //substract cost from coins
            PlayerInfoScript.playerInfo.coins -= cost;
            //set the skin num in save list to show it has been bought
            StoreSaveInfo.storeInfo.boughtSkins[skinNum] = 1;

            //set new skin in the player save info
            PlayerInfoScript.playerInfo.playerSkin = skinNum;

            //Save skinnumber in player;
            PlayerInfoScript.playerInfo.Save();
            //Save bought skin info in storeSave
            StoreSaveInfo.storeInfo.Save();

            //Update visuals on skin Tile
            skins[skinNum].GetComponent<OnSkinScript>().UpdateInfo();

            print("Num: " + skinNum);
        }
        else
        {
            notEnoughCoinsPanel.SetActive(true);
        }

        coinsText.text = PlayerInfoScript.playerInfo.coins.ToString();
    }

    public void CoinsForColor(int colorNum)
    {
        AudioManager.instance.Play("Click");
        print(PlayerInfoScript.playerInfo.coins);
        //check if this skin number has already been bought, if it has make it free
        //or if the skinNum is equal to the first selected skin, make it free
        if (StoreSaveInfo.storeInfo.boughtColors[colorNum] == 1 || colorNum == 0)
        {
            cost = 0;
        }

        //check if player has more coins than the cost
        if (PlayerInfoScript.playerInfo.coins >= cost)
        {
            //substract cost from coins
            PlayerInfoScript.playerInfo.coins -= cost;
            //set the skin num in save list to show it has been bought
            StoreSaveInfo.storeInfo.boughtColors[colorNum] = 1;

            //set new skin in the player save info
            PlayerInfoScript.playerInfo.heartColor = colorNum;

            //Save skinnumber in player;
            PlayerInfoScript.playerInfo.Save();
            //Save bought skin info in storeSave
            StoreSaveInfo.storeInfo.Save();

            //Update visuals on skin Tile
            colors[colorNum].GetComponent<OnColorScript>().UpdateInfo();
        }
        else
        {
            notEnoughCoinsPanel.SetActive(true);
        }

        coinsText.text = PlayerInfoScript.playerInfo.coins.ToString();
    }

    public void CoinsForSong(int songNum)
    {
        AudioManager.instance.Play("Click");
        //check if this skin number has already been bought, if it has make it free
        //or if the skinNum is equal to the first selected skin, make it free
        if (StoreSaveInfo.storeInfo.boughtMusic[songNum] == 1 || songNum == 0)
        {
            cost = 0;
        }

        //check if player has more coins than the cost
        if (PlayerInfoScript.playerInfo.coins >= cost)
        {
            //substract cost from coins
            PlayerInfoScript.playerInfo.coins -= cost;
            //set the skin num in save list to show it has been bought
            StoreSaveInfo.storeInfo.boughtMusic[songNum] = 1;

            //set new skin in the player save info
            PlayerInfoScript.playerInfo.playerSong = songNum;

            //Save skinnumber in player;
            PlayerInfoScript.playerInfo.Save();
            //Save bought skin info in storeSave
            StoreSaveInfo.storeInfo.Save();

            //Update visuals on skin Tile
            songs[songNum].GetComponent<OnSongScript>().UpdateInfo();
        }
        else
        {
            notEnoughCoinsPanel.SetActive(true);
        }

        coinsText.text = PlayerInfoScript.playerInfo.coins.ToString();
    }

    public void OpenSupportersPanel()
    {
        AudioManager.instance.Play("Click");
        supportPackage.SetActive(true);
    }

    public void CloseSupportersPanel()
    {
        AudioManager.instance.Play("Click");
        supportPackage.SetActive(false);
    }
}
