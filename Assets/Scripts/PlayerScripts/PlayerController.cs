using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 3;
    public int health;

    public int shieldTime;

    public bool tutorial;

    int curHealth;
    //read by playerMovementController
    [HideInInspector]
    public GameObject curTile;

    PlayerMovementController playerMovementController;
    PlayerAnimationScript playerAnimationScript;
    public GameController gameController;

    public CameraController camControl;

    ParticleSystem landingParticles;

    public bool hasDied = false;

    public bool heavy = false;

    public GameObject shield;
    bool shieldActive = false;

    public List<GameObject> hearts = new List<GameObject>();

    public GameObject potionShowPart;
    public Material potionShowMat;

    public GameObject heavyLandObject;

    bool sounds;

    //read by playerMovementController
    [HideInInspector]
    public bool enemiesOnTile;

    // Start is called before the first frame update
    void Start()
    {
        enemiesOnTile = false;
        playerMovementController = this.GetComponent<PlayerMovementController>();
        playerAnimationScript = this.GetComponent<PlayerAnimationScript>();
        landingParticles = this.transform.GetChild(0).GetComponent<ParticleSystem>();

        //set up hearts with current health information
        UpdateHealth();

        health = maxHealth;
        shield.SetActive(false);
        potionShowPart.SetActive(false);

        if(!tutorial) StartCoroutine(CheckIfEnemiesOnTile());

        sounds = MenuController.soundEffects;
    }

    void EnableShield()
    {
        shieldActive = true;
        shield.SetActive(true);
        StartCoroutine(StopShield());
    }

    IEnumerator StopShield()
    {
        yield return new WaitForSeconds(shieldTime);
        shield.SetActive(false);
        shieldActive = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Tile" && collision.gameObject != curTile)
        {
            curTile = collision.gameObject;
            //Tell EnemySpawnController which tile player is currently on
            EnemySpawnController.curPlayerTile = curTile;
            //Tell parabola jump to stop
            playerMovementController.jump = false;
            playerAnimationScript.jump = false;
            playerAnimationScript.land = true;

            landingParticles.Play();
            if (CheckTileForEnemies(curTile) && !tutorial) camControl.enemiesOnTile = true;

            if (!tutorial) camControl.stopZoom = true;

            //if heavy potion was taken, spawn heavy land object on next land
            if (heavy)
            {
                Instantiate(heavyLandObject, this.transform.position, Quaternion.identity);
                heavy = false;
                StartCoroutine(TurnOffPotionParts(2));

                if(sounds) AudioManager.instance.Play("HeavyLand");
            }
            else
            {
                if (sounds) AudioManager.instance.Play("NormalLand");
            }

            if (!tutorial && MenuController.shake) CameraShaker.Instance.ShakeOnce(2f, 2f, .1f, 1f);
        }

        //If hit by bullet, decrease health and display it on slider
        if (collision.transform.tag == "Bullet")
        {
            //if player is not spinning fast enough, take damage
            if (PlayerSwordController.curSwordSpeed < 300 && PlayerSwordController.curSwordSpeed > -300)
            {
                //if not dead and shield isnt active take damage
                if(!hasDied && !shieldActive)
                {
                    health -= 1;
                    if (sounds) AudioManager.instance.Play("LoseHeart");
                }
                UpdateHealth();
                if (health <= 0 && !hasDied)
                {
                    //keeps from calling this more than once
                    hasDied = true;
                    health = maxHealth;
                    gameController.playerDied();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HealthPotion")
        {
            if (sounds) AudioManager.instance.Play("PotionBreak");

            Destroy(other.gameObject);
            health = maxHealth;
            UpdateHealth();

            potionShowPart.SetActive(true);
            potionShowMat.color = new Color(255, 0, 0, 255);
            StartCoroutine(TurnOffPotionParts(10));
        }

        if (other.tag == "HeavyPotion")
        {
            if (sounds) AudioManager.instance.Play("PotionBreak");

            Destroy(other.gameObject);
            heavy = true;

            potionShowPart.SetActive(true);
            potionShowMat.color = new Color(207, 0, 200, 255);
        }

        if (other.tag == "ShieldPotion" && !shieldActive)
        {
            if (sounds) AudioManager.instance.Play("PotionBreak");

            Destroy(other.gameObject);
            EnableShield();
        }

        if (other.tag == "TimePotion")
        {
            if (sounds) AudioManager.instance.Play("PotionBreak");

            Destroy(other.gameObject);
            //Add Time to countDownTimer
            gameController.extraTime += 8;

            potionShowPart.SetActive(true);
            potionShowMat.color = new Color(255, 186, 0, 255);
            StartCoroutine(TurnOffPotionParts(10));
        }
        if (other.tag == "Coin")
        {
            if (sounds) AudioManager.instance.Play("Coin");
            Destroy(other.gameObject);

            PlayerInfoScript.playerInfo.coins++;
        }
    }

    IEnumerator CheckIfEnemiesOnTile()
    {
        while (true)
        {
            if (curTile != false)
            {
                //Check if there are enemies on player tile, if there are then tell camera to zoom in
                //else tell camera to zoom back out to normal zoom
                if (CheckTileForEnemies(curTile))
                {
                    print("Enemy on tile!");
                    camControl.enemiesOnTile = true;
                    enemiesOnTile = true;
                }
                else
                {
                    camControl.enemiesOnTile = false;
                    enemiesOnTile = false;
                }
                //only check every one second to save no performance
                yield return new WaitForSeconds(.3f);
            }

            yield return null;
        }
    }

    bool CheckTileForEnemies(GameObject tile)
    {
        //Loops through tile children
        foreach (Transform child in tile.transform.parent)
        {
            //If child's tag is Enemy, return true
            if (child.tag == "Enemy")
            {
                return true;
            }
        }
        //if loop finishes without finding Enemy tag, return false
        return false;
    }

    //Also ran by gameController
    public void UpdateHealth()
    {
        //hide all hearts
        for (int i = 0; i < maxHealth; i++)
        {
            hearts[i].SetActive(false);
        }

        //For each health player has, turn on heart
        for (int i = 0; i < health; i++)
        {
            hearts[i].SetActive(true);
        }
    }

    IEnumerator TurnOffPotionParts(float time)
    {
        yield return new WaitForSeconds(time);

        potionShowPart.SetActive(false);
    }
}
