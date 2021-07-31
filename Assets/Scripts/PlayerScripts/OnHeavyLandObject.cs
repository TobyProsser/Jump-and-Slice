using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHeavyLandObject : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.transform.parent.gameObject, 4);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") Destroy(other.gameObject);
    }
}
