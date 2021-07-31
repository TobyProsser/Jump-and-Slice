using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireScript : MonoBehaviour
{
    public GameObject bullet;
    public Transform shootPoint;

    public float bulletSpeed;

    public float reloadTime;

    GameObject player;

    //Given by enemyController
    [HideInInspector]
    public GameObject curTile;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Shooting());
    }

    IEnumerator Shooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(reloadTime);
            Shoot();
        }
    }
    private void Shoot()
    {
        //Only shoot bullet if player is on the same tile
        if (curTile.name == EnemySpawnController.curPlayerTile.transform.parent.name)
        {
            GameObject CurBullet = Instantiate(bullet, shootPoint.position, this.transform.rotation);
            CurBullet.GetComponent<ProjectileMove>().speed = bulletSpeed;
            CurBullet.tag = "Bullet";
            if(MenuController.soundEffects) AudioManager.instance.Play("Missile");
        }
    }
}
