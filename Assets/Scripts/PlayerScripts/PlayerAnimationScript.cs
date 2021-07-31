using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    //default set to 0.1f
    public float walkAnimationSpeed;
    public float idleAnimationSpeed;
    //player model object that will change mesh for animation
    public GameObject playerMeshObject;

    Mesh[] walkingFrames;
    Mesh[] idleFrames;
    Mesh spinningFrame;
    Mesh jumpFrame;
    Mesh landingFrame;

    //Changed by playerMovementController and read by PlayerSwordController
    [HideInInspector]
    public bool walk;
    //Changed by PlayerSwordController
    [HideInInspector]
    public bool swing;
    //Changed by playerMovementController and playerController
    [HideInInspector]
    public bool jump;
    //set tp true by player contoller, Changed by playerMovementController
    [HideInInspector]
    public bool land;

    // Start is called before the first frame update
    void Start()
    {
        SetUpSkin();

        StartCoroutine(Animation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Animation()
    {
        int frame = 0;
        while (true)
        {
            //Swinging comes first so that player can move but still be swinging
            if (swing)
            {
                playerMeshObject.GetComponent<MeshFilter>().mesh = spinningFrame;
            }
            else if (walk)
            {
                //When frame reaches end of list, start back at beginning of list
                //placed at start because if animation goes from walking to idle, walking has more frames so index will be out of range
                if (frame >= walkingFrames.Length - 1)
                {
                    frame = 0;
                }

                //Select next walking frame(Mesh) from list
                playerMeshObject.GetComponent<MeshFilter>().mesh = walkingFrames[frame];
                yield return new WaitForSeconds(walkAnimationSpeed);
                frame++;
            }
            else if (jump)
            {
                playerMeshObject.GetComponent<MeshFilter>().mesh = jumpFrame;
            }
            else if (land)
            {
                playerMeshObject.GetComponent<MeshFilter>().mesh = landingFrame;
            }
            //if not walking, swinging, or jumping, play idle animation
            else
            {
                //When frame reaches end of list, start back at beginning of list
                //placed at start because if animation goes from walking to idle, walking has more frames so index will be out of range
                if (frame >= idleFrames.Length - 1)
                {
                    frame = 0;
                }
                //Select next walking frame(Mesh) from list
                playerMeshObject.GetComponent<MeshFilter>().mesh = idleFrames[frame];
                yield return new WaitForSeconds(idleAnimationSpeed);
                frame++;
            }

            yield return null;
        }
    }

    void SetUpSkin()
    {
        //Get the saved skin in PlayerInfoScript
        int skinNum = PlayerInfoScript.playerInfo.playerSkin;
        //If no skin is selected, default to first skin
        if (skinNum == -1) skinNum = 0;
        //Use that number to pull meshes from a list in SkinsHolder
        Skin curSkin = SkinsHolder.instance.skins[skinNum];
        walkingFrames = curSkin.walkingFrames;
        idleFrames = curSkin.idleFrames;
        spinningFrame = curSkin.spinningFrame;
        jumpFrame = curSkin.jumpFrame;
        landingFrame = curSkin.landingFrame;

        playerMeshObject.GetComponent<Renderer>().material = curSkin.material;
    }
}
