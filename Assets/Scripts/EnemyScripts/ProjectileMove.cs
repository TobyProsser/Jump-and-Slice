using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    //Given by EnemyFireScript
    public float speed = 8;

    public GameObject Explosion;

    public Color Color;

    public bool DeathByTime = true;
    public bool Ribbons;

    Renderer Render;

    void Start()
    {
        Destroy(this.gameObject, 4);
        Render = this.GetComponent<Renderer>();
    }

    void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void OnDestroy()
    {   
        //Only play explosition if seen by camera. Helps with performence
        if (OnScreen())
        {
            GameObject CurExplosion = Instantiate(Explosion, this.transform.position, Quaternion.identity);

            ParticleSystem.TrailModule trails = CurExplosion.GetComponent<ParticleSystem>().trails;
            trails.colorOverTrail = Color;
            trails.colorOverLifetime = Color;

            ParticleSystem.MainModule ps = CurExplosion.GetComponent<ParticleSystem>().main;
            ps.startColor = Color;

            if (Ribbons)
            {
                trails.mode = ParticleSystemTrailMode.Ribbon;
            }

            if (DeathByTime)
            {
                ps.startSpeed = 1;
                var emission = CurExplosion.GetComponent<ParticleSystem>().emission;
                var shape = CurExplosion.GetComponent<ParticleSystem>().shape;
                emission.rateOverTime = 10;
                shape.radius = .45f;
            }

            Destroy(CurExplosion, 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //As long as the enemy isnt hitting itself, its bullet should explode on all impacts
        if (collision.transform.tag != "Enemy" && collision.transform.tag != "EnemyHitCol")
        {
            DeathByTime = false;
            Destroy(this.gameObject);
        }
    }

    private bool OnScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);
        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
