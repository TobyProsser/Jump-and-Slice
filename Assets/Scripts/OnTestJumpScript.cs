using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTestJumpScript : MonoBehaviour
{
    public int amountOfHits = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TileBottom")
        {
            amountOfHits++;
        }
    }
}
