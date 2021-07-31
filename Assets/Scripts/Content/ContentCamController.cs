using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentCamController : MonoBehaviour
{
    public float speed;

    public Transform target;
    public bool lookAt;
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += new Vector3(-1, 0, 0) * speed;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += new Vector3(1, 0, 0) * speed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.position += new Vector3(0, 1, 0) * speed;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.position += new Vector3(0, -1, 0) * speed;
        }

        if (Input.GetKey(KeyCode.Period))
        {
            this.transform.position += new Vector3(0, 0, 1) * speed;
        }

        if (Input.GetKey(KeyCode.Comma))
        {
            this.transform.position += new Vector3(0, 0, -1) * speed;
        }

        if (lookAt) transform.LookAt(target);
    }
}
