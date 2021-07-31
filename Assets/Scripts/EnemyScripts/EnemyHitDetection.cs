using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    EnemyController enemyController;

    private void Awake()
    {
        enemyController = this.transform.parent.GetComponent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword")
        {
            int curDamage = other.transform.parent.GetComponent<PlayerSwordController>().damage;

            enemyController.health -= curDamage;

            if (MenuController.soundEffects) AudioManager.instance.Play("EnemyHit");
        }
    }
}
