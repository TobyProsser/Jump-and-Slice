using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class AdController : MonoBehaviour, IUnityAdsListener
{
    public static AdController AdInstance;

    private string AppleStore_ID = "3918264";
    private string GoogleStore_ID = "3918265";

    private string video_ad = "video";
    private string rewarded_video_ad = "rewardedVideo";
    private string banner_ad = "banner";

    private bool TestMode = false;

    private void Awake()
    {
        if (AdInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            AdInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        if (Application.platform != RuntimePlatform.IPhonePlayer &&
            Application.platform != RuntimePlatform.OSXPlayer)
        {
            Advertisement.Initialize(GoogleStore_ID, TestMode); //Turn to false when not testing
        }
        else
        {
            Advertisement.Initialize(AppleStore_ID, TestMode); //Turn to false when not testing
        }

        Advertisement.AddListener(this);
    }

    public void ShowAd(string p)
    {
        Advertisement.Show(p);
    }

    public void ShowBannerAd(string p)
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Show(p);
    }

    public void OnUnityAdsReady(string placementId)
    {
        
    }

    public void OnUnityAdsDidError(string message)
    {
        
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            PlayerInfoScript.playerInfo.lives = 5;
            SceneManager.LoadScene("GameScene");
        }
    }
}
