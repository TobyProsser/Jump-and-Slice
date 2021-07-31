using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScript : MonoBehaviour
{
    public List<GameObject> texts = new List<GameObject>();

    public GameObject player;
    Vector3 playerPos;
    bool step1 = false;
    bool step2 = false;
    bool step3 = false;

    void Start()
    {
        foreach (GameObject item in texts)
        {
            item.SetActive(false);
        }

        texts[0].SetActive(true);
        playerPos = player.transform.position;
    }

    private void LateUpdate()
    {
        print(PlayerSwordController.curSwordSpeed);
        if (Vector3.Distance(playerPos, player.transform.position) > 3 && !step1)
        {
            step1 = true;
            texts[0].SetActive(false);
            texts[1].SetActive(true);
        }

        if (step1 && !step2)
        {
            if (PlayerSwordController.curSwordSpeed > 10 || PlayerSwordController.curSwordSpeed < -10)
            {
                texts[1].SetActive(false);
                texts[2].SetActive(true);
                step2 = true;
            }

        }

        if (step1 && step2 && !step3)
        {
            
            if (PlayerSwordController.curSwordSpeed > 250 || PlayerSwordController.curSwordSpeed < -250)
            {
                step3 = true;
                texts[2].SetActive(false);
                texts[3].SetActive(true);
                StartCoroutine(Wait());
            }

        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        texts[4].SetActive(true);
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
