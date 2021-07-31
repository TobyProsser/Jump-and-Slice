using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementController : MonoBehaviour
{
    public float jumpHeight;
    //jumpTime will be the same time as jump animation in the future
    public float jumpSpeed = 5;
    public float jumpTime;
    public float walkSpeed;

    //Turned on in this script, turned off in playerController
    public GameObject jumpLineRenderer;

    //changed by playerController
    [HideInInspector]
    public bool jump;
    bool runningJump;
    //changed by playerSwordController
    [HideInInspector]
    public bool swinging;
    bool walk;
    Vector3 endPoint;
    Vector3 playerPos;
    public GameObject testJumpObject;

    PlayerController playerController;
    PlayerAnimationScript playerAnimScript;

    public GameObject mainCamera;
    public GameObject cameraHolder;

    void Awake()
    {
        playerController = this.GetComponent<PlayerController>();
        playerAnimScript = this.GetComponent<PlayerAnimationScript>();

        jumpLineRenderer.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            //Was Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //if player clicked on tile and didnt click on UI and player is on tile (meaning player cant move anywhere if they are not on tile)
                // && !IsPointerOverUIObject()
                if (hit.transform.tag == "Tile" && !IsPointerOverUIObject() && playerController.curTile != null)
                {
                    //If player clicked on tile that they are not standing on, jump, else walk
                    //And if the player isnt currently jumping
                    if (hit.transform.gameObject != playerController.curTile && !jump && !runningJump && !playerController.enemiesOnTile)
                    {
                        runningJump = true;

                        playerPos = this.transform.position;
                        endPoint = hit.point;

                        jumpTime = 0;
                        StartCoroutine(CalculateJumpHeight(playerPos, endPoint));
                    }
                    else if(!jump && !runningJump && hit.transform.gameObject == playerController.curTile)
                    {
                        playerPos = this.transform.position;
                        endPoint = hit.point;
                        //turn off line renderer when walking, so trail lasts after jump until player walks
                        jumpLineRenderer.SetActive(false);

                        endPoint = new Vector3(endPoint.x, endPoint.y + 1.47f, endPoint.z);
                        walk = true;

                        //tell animator to play walking animation
                        playerAnimScript.land = false;
                        playerAnimScript.walk = true;

                        if (MenuController.soundEffects) AudioManager.instance.Play("Walking");
                    }
                }
            }
        }

        //jump turned on when player clicks on tile, turned off by PlayerController
        if (jump)
        {
            jumpTime += Time.deltaTime;
            jumpTime = jumpTime % jumpSpeed;
            this.transform.position = MathParabola.Parabola(playerPos, endPoint, jumpHeight, jumpTime/ jumpSpeed);

            //if player is swinging dont look at end direction, let them spin
            if(!swinging) transform.LookAt(new Vector3(endPoint.x, transform.position.y, endPoint.z));

            //if jumping tell Animator to stop walking animation
            playerAnimScript.walk = false;
            walk = false;
            runningJump = false;
        }
        else if (walk && !jump)
        {
            float step = walkSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, endPoint, step);

            if (Vector3.Distance(transform.position, endPoint) < .001f)
            {
                //tell Animator to stop walking animation
                playerAnimScript.walk = false;
                walk = false;

                if (MenuController.soundEffects) AudioManager.instance.Stop("Walking");
            }
            //if player is swinging dont look at end direction, let them spin
            if (!swinging) transform.LookAt(new Vector3(endPoint.x, transform.position.y, endPoint.z));
        }
    }

    float SetJumpHeight(Transform clickedTile)
    {
        //get absolute value of the difference tile player is standing on, and the one they clicked
        float tileHeightDifference = Mathf.Abs(playerController.curTile.transform.position.y - clickedTile.position.y);

        //if the height difference multiplied is greater than 20, return 20. Else return the heigh difference times 2
        if (tileHeightDifference * 2 >= 20) return 20;
        else if (tileHeightDifference * 2 <= 3) return 6 + tileHeightDifference;
        else return tileHeightDifference * 2;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //Called by gameController when next level starts
    //stops player from moving through new map
    public void StopMovement()
    {
        jump = false;
        walk = false;
    }

    IEnumerator CalculateJumpHeight(Vector3 startPos, Vector3 endPos)
    {
        float height = 10f;
        bool jumpPassed = false;
        //While jump is not passed, keep passing testObjects through parabola to see if they hit anything
        float jumpTime1 = 0;
        int tries = 0;

        endPos = new Vector3(endPos.x, endPos.y + 2, endPos.z);
        while (!jumpPassed)
        {
            //Create new testobject at startPos
            GameObject curTestObject = Instantiate(testJumpObject, startPos, Quaternion.identity);
            jumpTime1 = 0;
            while (curTestObject.transform.position != endPos)
            {
                jumpTime1 += Time.deltaTime;
                jumpTime1 = jumpTime1 % jumpSpeed;

                curTestObject.transform.position = MathParabola.Parabola(startPos, endPos, height, jumpTime1 / .1f);
                if (Vector3.Distance(curTestObject.transform.position, endPos) < 1.5f)
                {
                    break;
                }
                else if (Vector3.Distance(curTestObject.transform.position, endPos) > 100f) break;
                yield return null;
            }
            //set jumpPassed to if the test object hit more than two surfaces(the start and end surfaces)
            int hits = curTestObject.GetComponent<OnTestJumpScript>().amountOfHits;
            print("Hits: " + hits);

            Destroy(curTestObject);
            
            //break loop if jump passed with only hitting starting point and ending point
            if (hits == 0)
            {
                jumpPassed = true;
                break;
            }
            else
            {
                print("Jump Failed, increasing height");
                height += 10f;
            }

            tries++;
            if (tries >= 5) break;
            yield return null;
        }
        jumpHeight = height;
        //start jump after height has been calculated
        startJump();

        yield return null;
    }
    void startJump()
    {
        jump = true;
        jumpLineRenderer.SetActive(true);
        playerAnimScript.jump = true;

        if (MenuController.soundEffects) AudioManager.instance.Play("Jump");

        cameraHolder.transform.GetComponent<CameraController>().target = this.transform;
        cameraHolder.GetComponent<CameraController>().RunWatchPlayer();
    }
}
