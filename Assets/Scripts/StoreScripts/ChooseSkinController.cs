using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseSkinController : MonoBehaviour
{
    public GameObject pointerObject;

    int selectionChoice = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                print(hit.transform.tag);
                if (hit.transform.tag == "OptionOne")
                {
                    selectionChoice = 0;
                    pointerObject.transform.position = new Vector3(-14.9f, 44.8f, 0.7f);
                    AudioManager.instance.Play("Click");
                }

                if (hit.transform.tag == "OptionTwo")
                {
                    selectionChoice = 1;
                    pointerObject.transform.position = new Vector3(14.9f, 44.8f, 0.7f);
                    AudioManager.instance.Play("Click");
                }
            }
        }
    }

    public void SelectButton()
    {
        AudioManager.instance.Play("Click");
        PlayerInfoScript.playerInfo.playerSkin = selectionChoice;

        //Give player starting amount of lives
        PlayerInfoScript.playerInfo.lives = 8;

        StoreSaveInfo.storeInfo.firstSelectedSkin = selectionChoice;
        StoreSaveInfo.storeInfo.Save();

        SceneManager.LoadScene("TutorialScene");
    }
}
