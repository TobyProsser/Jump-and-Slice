using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnSkinScript : MonoBehaviour
{
    public int thisSkinNum;

    public TextMeshProUGUI buyText;
    public TextMeshProUGUI costText;

    public GameObject thisButton;

    public Image backImage;
    public Color usingSkinColor;
    public Color normalColor;

    public bool inSupportersPackage;
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
        if (thisSkinNum == PlayerInfoScript.playerInfo.playerSkin)
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
        //check this skin num in the bought skins list
        if (StoreSaveInfo.storeInfo.boughtSkins[thisSkinNum] == 1 || thisSkinNum == StoreSaveInfo.storeInfo.firstSelectedSkin)
        {
            buyText.text = "USE";
            costText.text = "";
        }
    }
}
