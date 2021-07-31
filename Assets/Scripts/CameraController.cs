using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 closeOffset;
    public Transform startPos;

    GameObject camera;
    Quaternion startLookRot;
    Quaternion newRot;
    float startFieldOfView = 70;
    float zoomFOV = 30;
    public float zoomSpeed;
    public float lookSpeed;

    //Given by PlayerMovementController
    //[HideInInspector]
    public Transform target;
    Vector3 relPos;
    //Given by playerController
    [HideInInspector]
    public bool enemiesOnTile;

    //Given by playerController
    [HideInInspector]
    public bool stopZoom = false;

    bool watchingPlayer = false;
    bool resetingCamera = false;

    void Start()
    {
        camera = this.transform.GetChild(0).gameObject;
        startLookRot = this.transform.rotation;
        target = startPos;
    }

    void Update()
    {
        if (enemiesOnTile)
        {
            Vector3 relPos1 = target.position - transform.position;
            Quaternion newRot1 = Quaternion.LookRotation(relPos1);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot1, lookSpeed * Time.deltaTime);
            print("Watch");
        }
        else if (!resetingCamera && !watchingPlayer) StartCoroutine(ResetCamRot());
    }

    public void RunWatchPlayer()
    {
        StartCoroutine(WatchPlayer());
    }

    IEnumerator ResetCamRot()
    {
        resetingCamera = true;
        StartCoroutine(CamZoom(startFieldOfView));
        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startLookRot, lookSpeed * Time.deltaTime);

            if (transform.rotation == startLookRot) break;
            yield return null;
        }

        resetingCamera = false;
    }

    IEnumerator WatchPlayer()
    {
        watchingPlayer = true;
        StartCoroutine(CamZoom(zoomFOV));
        stopZoom = false;
        while (true)
        {
            relPos = target.position - transform.position;
            newRot = Quaternion.LookRotation(relPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, lookSpeed * Time.deltaTime);
            if (stopZoom) break;
            yield return null;
        }

        watchingPlayer = false;
        if(!enemiesOnTile) StartCoroutine(ResetCamRot());
    }

    IEnumerator CamZoom(float FOV)
    {
        float tempFOV = camera.GetComponent<Camera>().fieldOfView;
        while (tempFOV != FOV)
        {
            float tempSpeed = zoomSpeed * Time.deltaTime;
            tempFOV = Mathf.Lerp(tempFOV, FOV, tempSpeed);
            camera.GetComponent<Camera>().fieldOfView = tempFOV;

            if (Mathf.Abs(tempFOV - FOV) < .3f) break;
            yield return null;
        }
    }
    //Ran by MapGenerator
    public void FindStartPos(int rows)
    {
        startPos.position = new Vector3(0, rows * 26, rows * -21.5f);
        if(rows == 3) startPos.position = new Vector3(0, rows * 20, rows * -22f);
        else if (rows == 4) startPos.position = new Vector3(0, rows * 18, rows * -23f);
        else if (rows == 5) startPos.position = new Vector3(0, rows * 15, rows * -23f);
        this.transform.position = startPos.position;
        print(startPos.position);
    }
}
