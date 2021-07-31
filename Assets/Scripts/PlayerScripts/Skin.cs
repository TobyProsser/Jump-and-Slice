using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skin
{
    public string name;
    public float spinSpeed;

    public Mesh[] walkingFrames = new Mesh[8];
    public Mesh[] idleFrames = new Mesh[4];
    public Mesh spinningFrame;
    public Mesh jumpFrame;
    public Mesh landingFrame;

    public Material material;
}
