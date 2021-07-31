using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSwordScript : MonoBehaviour
{
    public GameObject destoryTreePart;
    public GameObject destorySnowPart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tree" || other.tag == "Cactus")
        {
            Destroy(other.gameObject);
            GameObject curPart = Instantiate(destoryTreePart, new Vector3(other.transform.position.x, other.transform.position.y + 5, other.transform.position.z), Quaternion.identity);
            Destroy(curPart, 3);
        }

        if (other.tag == "SnowMan")
        {
            Destroy(other.gameObject);
            GameObject curPart = Instantiate(destorySnowPart, new Vector3(other.transform.position.x, other.transform.position.y + 5, other.transform.position.z), Quaternion.identity);
            Destroy(curPart, 3);
        }
    }
}
