using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuButtons : MonoBehaviour
{
    public GameController gameController;

    public void TryAgainB()
    {
        AudioManager.instance.Play("Click");
        gameController.LevelLostB();
    }

    public void LevelWonB()
    {
        AudioManager.instance.Play("Click");
        gameController.LevelWonB();
    }

    public void MainMenuB()
    {
        AudioManager.instance.Play("Click");
        SceneManager.LoadScene("MainMenu");
    }

    public void ShopB()
    {
        AudioManager.instance.Play("Click");
        SceneManager.LoadScene("StoreScene");
    }

    public void WatchAdButton()
    {
        AudioManager.instance.Play("Click");
        if (!PlayerInfoScript.playerInfo.supporterPackage) AdController.AdInstance.ShowAd("rewardedVideo");
        else
        {
            PlayerInfoScript.playerInfo.lives = 5;
            SceneManager.LoadScene("GameScene");
        }
    }
}
