using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    public float health = 100f;
    public float damage = 50.0f;

    ParticleSystem smoke;
    ParticleSystem redFire;
    ParticleSystem orangeFire;

    // Start is called before the first frame update
    void Start()
    {
        smoke = this.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        redFire = this.gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
        orangeFire = this.gameObject.transform.GetChild(2).GetComponent<ParticleSystem>();
    }

    public void TakeHit() {
        health -= damage * Time.deltaTime;
        if (health <= 0.0f) Die();
    }

    void Die() {
        smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);  // Maybe you could leave smoke enabled???
        redFire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        orangeFire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }


}
