using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnColorScript : MonoBehaviour
{
    public int thisColorNum;

    public TextMeshProUGUI buyText;
    public TextMeshProUGUI costText;

    public GameObject thisButton;

    public Image backImage;
    public Color usingSkinColor;
    public Color normalColor;

    public bool inSupportersPackage = false;
    public GameObject supporterImage;

    private void Start()
    {
        //If this item is in the supporters package, and the player has not purchased the supportersPackage, disable the button
        if (inSupportersPackage && !StoreSaveInfo.storeInfo.supportersPackage)
        {
            thisButton.SetActive(false);
            supporterImage.SetActive(true);
        }
        else if (inSupportersPackage && StoreSaveInfo.storeInfo.supportersPackage)
        {
            thisButton.SetActive(true);
            supporterImage.SetActive(false);
        }
    }

    void OnEnable()
    {
        UpdateInfo();
    }

    void LateUpdate()
    {
        //if this skin is players current skin
        if (thisColorNum == PlayerInfoScript.playerInfo.heartColor)
        {
            backImage.color = usingSkinColor;
            thisButton.SetActive(false);
        }
        else
        {
            backImage.color = normalColor;
            //if item is in the supporters package and player has purchased it enable button when required
            if (inSupportersPackage && StoreSaveInfo.storeInfo.supportersPackage) thisButton.SetActive(true);
            //if item is in the supporters package and player has not purchased it disable button even when required
            else if (inSupportersPackage && !StoreSaveInfo.storeInfo.supportersPackage) thisButton.SetActive(false);
            //if item is not in the supporters package, enable button when required
            else thisButton.SetActive(true);
        }
    }

    //Updated by StoreScript when skin is purchased
    public void UpdateInfo()
    {
        //check this color num in the bought colors list
        //Also check if this tile is the first hearts, which are given for free
        if (StoreSaveInfo.storeInfo.boughtColors[thisColorNum] == 1 || thisColorNum == 0)
        {
            buyText.text = "USE";
            costText.text = "";
        }
    }
}
