using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentMovementController : MonoBehaviour
{
    public Transform goToPoint;

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
    //changed by playerSwordController
    [HideInInspector]
    public bool swinging;
    bool walk;
    Vector3 endPoint;
    Vector3 playerPos;

    PlayerController playerController;
    PlayerAnimationScript playerAnimScript;
    void Awake()
    {
        playerController = this.GetComponent<PlayerController>();
        playerAnimScript = this.GetComponent<PlayerAnimationScript>();

        jumpLineRenderer.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerPos = this.transform.position;
            endPoint = goToPoint.position;

            jumpTime = 0;
            jump = true;

            jumpLineRenderer.SetActive(true);
            playerAnimScript.jump = true;

            AudioManager.instance.Play("Jump");
        }
        if (Input.GetMouseButtonDown(2))
        {
            playerPos = this.transform.position;
            endPoint = goToPoint.position;
            //turn off line renderer when walking, so trail lasts after jump until player walks
            jumpLineRenderer.SetActive(false);

            endPoint = new Vector3(endPoint.x, endPoint.y + 1.47f, endPoint.z);
            walk = true;

            //tell animator to play walking animation
            playerAnimScript.land = false;
            playerAnimScript.walk = true;

            AudioManager.instance.Play("Walking");
        }

        //jump turned on when player clicks on tile, turned off by PlayerController
        if (jump)
        {
            jumpTime += Time.deltaTime;
            jumpTime = jumpTime % jumpSpeed;
            this.transform.position = MathParabola.Parabola(playerPos, endPoint, jumpHeight, jumpTime / jumpSpeed);

            //if player is swinging dont look at end direction, let them spin
            if (!swinging) transform.LookAt(new Vector3(endPoint.x, transform.position.y, endPoint.z));

            //if jumping tell Animator to stop walking animation
            playerAnimScript.walk = false;
            walk = false;
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

                AudioManager.instance.Stop("Walking");
            }
            //if player is swinging dont look at end direction, let them spin
            if (!swinging) transform.LookAt(new Vector3(endPoint.x, transform.position.y, endPoint.z));
        }
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
}
