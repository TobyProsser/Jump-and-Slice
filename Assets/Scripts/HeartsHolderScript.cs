using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsHolderScript : MonoBehaviour
{
    public List<Image> hearts = new List<Image>();

    public List<Sprite> heartSprites = new List<Sprite>();

    private void Start()
    {
        ChangeHearts();
    }

    void ChangeHearts()
    {
        int curHeartColor = PlayerInfoScript.playerInfo.heartColor;
        foreach (Image heart in hearts)
        {
            heart.sprite = heartSprites[curHeartColor];
        }
    }
}
