using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordController : MonoBehaviour
{
    [Header("Upgradable")]
    public int damage;
    public float maxSpinSpeed;

    [Header("Technical")]
    public float startSwordSpeed;
    public float swordSpeedMulti;

    public float swordSpeedDecreaseAmount;

    //read by PlayerController
    public static float curSwordSpeed;

    public GameObject swordHitBox;

    public GameObject spiral;
    public float spiralMultiplyer;
    public GameObject swordTrail;

    PlayerController playerController;
    PlayerAnimationScript playerAnimScript;
    PlayerMovementController playerMoveScript;

    bool soundPlaying = false;

    private void Awake()
    {
        playerController = this.GetComponent<PlayerController>();
        playerAnimScript = this.GetComponent<PlayerAnimationScript>();
        playerMoveScript = this.GetComponent<PlayerMovementController>();

        spiral.SetActive(false);
        swordTrail.SetActive(false);
    }

    private void Start()
    {
        SetUpSwordSpeed();
    }

    private void FixedUpdate()
    {
        //if sword has a speed
        if (curSwordSpeed != 0 && playerController.curTile != null)
        {
            if (!soundPlaying)
            {
                soundPlaying = true;
                StartCoroutine("SwordSound");
            }

            swordHitBox.GetComponent<BoxCollider>().enabled = true;
            //sets land frame to false just incase player swings after landing and doesnt walk first
            playerAnimScript.land = false;
            //ShowSword();
            //tell animator to play swing frame
            playerAnimScript.swing = true;
            playerMoveScript.swinging = true;
            transform.Rotate(0, curSwordSpeed * Time.deltaTime, 0);

            //if sword speed is close to zero, set it to zero
            if (curSwordSpeed > -5 && curSwordSpeed < 5) curSwordSpeed = 0;
            //slowly decrease the speed of the sword over time
            //Means player has to keep swiping to use sword
            else if (curSwordSpeed > 0) curSwordSpeed -= swordSpeedDecreaseAmount;
            else if (curSwordSpeed < 0) curSwordSpeed += swordSpeedDecreaseAmount;
            //print(curSwordSpeed);
            swordTrail.SetActive(true);
        }
        else
        {
            //if sword is not being swung, disable the hit box so it cant deal damage
            swordHitBox.GetComponent<BoxCollider>().enabled = false;
            //tell animator NOT to play swing frame
            playerAnimScript.swing = false;
            playerMoveScript.swinging = false;
            //HideSword();
            curSwordSpeed = 0;
            swordTrail.SetActive(false);
        }

        //if sword is at high enough speed, turn on spiral
        if (curSwordSpeed > 200 || curSwordSpeed < -200)
        {
            spiral.SetActive(true);

            //rotate the spiral at a higher speed
            spiral.transform.Rotate(0, curSwordSpeed * Time.deltaTime * spiralMultiplyer, 0);
        }
        else spiral.SetActive(false);

        if (curSwordSpeed < 50 && curSwordSpeed > -50)
        {
            soundPlaying = false;
            StopCoroutine("SwordSound");
        }
    }

    void HideSword()
    {
        curSwordSpeed = 0;
        //sword.SetActive(false);
    }

    void ShowSword()
    {
        //sword.SetActive(true);
    }

    //each time player swipes increase sword speed. called by MoveOnSwipe_EightDirections
    public void SwordLeft()
    {
        //swordSpeedMulti is saved as part of skin
        curSwordSpeed += startSwordSpeed * swordSpeedMulti;
    }

    public void SwordRight()
    {
        curSwordSpeed -= startSwordSpeed * swordSpeedMulti;
    }

    IEnumerator SwordSound()
    {
        while (true)
        {
            if (curSwordSpeed < 40 && curSwordSpeed > -40) yield return new WaitForSeconds(1f);
            else if (curSwordSpeed < 200 && curSwordSpeed > -200) yield return new WaitForSeconds(.6f);
            else yield return new WaitForSeconds(.45f);
            if(MenuController.soundEffects) AudioManager.instance.Play("SwordSwing");
        }
    }

    void SetUpSwordSpeed()
    {
        //Get the saved skin in PlayerInfoScript
        int skinNum = PlayerInfoScript.playerInfo.playerSkin;
        //If no skin is selected, default to first skin
        if (skinNum == -1) skinNum = 0;
        //Use that number to pull meshes from a list in SkinsHolder
        Skin curSkin = SkinsHolder.instance.skins[skinNum];

        swordSpeedMulti = curSkin.spinSpeed;
    }
}
