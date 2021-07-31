using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsHolder : MonoBehaviour
{
    public List<Skin> skins = new List<Skin>();

    public static SkinsHolder instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
