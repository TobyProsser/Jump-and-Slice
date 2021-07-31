using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GG.Infrastructure.Utils.Swipe;

public class MoveOnSwipe_EightDirections : MonoBehaviour
{
    public PlayerSwordController swordController;

    [Header("Available movements:")]

    [SerializeField] private bool _left = true;
    [SerializeField] private bool _right = true;

    
    public void OnSwipeHandler(string id)
    {
        switch (id)
        {
            case DirectionId.ID_LEFT:
                MoveLeft();
                break;

            case DirectionId.ID_RIGHT:
                MoveRight();
                break;
        }
    }

   
    private void MoveRight()
    {
        if (_right)
        {
            //print("MoveRight");
            swordController.SwordRight();
        }
    }

    private void MoveLeft()
    {
        if (_left)
        {
            //print("MoveLeft");
            swordController.SwordLeft();
        }
    }
}
