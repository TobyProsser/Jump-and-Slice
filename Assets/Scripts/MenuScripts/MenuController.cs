using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static bool music;
    public static bool soundEffects;
    public static bool shake;

    public TextMeshProUGUI levelText;
    //to reset saved data
    public static bool reset = false;

    public GameObject soundsButton;
    public GameObject musicButton;
    public GameObject shakeButton;

    void Start()
    {
        levelText.text = PlayerInfoScript.playerInfo.level.ToString();
        InitializeOptions();
    }

    void InitializeOptions()
    {
        music = false;
        ToggleMusic();

        soundEffects = false;
        ToggleSoundEffects();

        shake = false;
        ToggleCameraShake();
    }

    public void PlayButton()
    {
        AudioManager.instance.Play("Click");
        //if this is the first time playing, open skin select scene
        if (PlayerInfoScript.playerInfo.level <= 0) SceneManager.LoadScene("ChooseSkinScene");
        else SceneManager.LoadScene("GameScene");
    }

    public void StoreButton()
    {
        AudioManager.instance.Play("Click");
        SceneManager.LoadScene("StoreScene");
    }

    public void ExitButton()
    {
        AudioManager.instance.Play("Click");
        Application.Quit();
    }

    public void ToggleMusic()
    {
        AudioManager.instance.Play("Click");
        if (music)
        {
            music = false;
            musicButton.GetComponent<Image>().color = Color.gray;
            AudioManager.instance.Stop("Music");
        }
        else
        {
            music = true;
            musicButton.GetComponent<Image>().color = Color.white;
            AudioManager.instance.Play("Music");
        }
    }

    public void ToggleSoundEffects()
    {
        AudioManager.instance.Play("Click");
        if (soundEffects)
        {
            soundEffects = false;
            soundsButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            soundEffects = true;
            soundsButton.GetComponent<Image>().color = Color.white;
        }
    }

    public void ToggleCameraShake()
    {
        AudioManager.instance.Play("Click");
        if (shake)
        {
            shake = false;
            shakeButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            shake = true;
            shakeButton.GetComponent<Image>().color = Color.white;
        }
    }
}
