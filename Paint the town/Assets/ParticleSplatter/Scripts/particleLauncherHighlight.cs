using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on https://unity3d.com/learn/tutorials/topics/scripting/droplet-decals?playlist=17117


public class particleLauncherHighlight : MonoBehaviour {

    public ParticleSystem pLauncher;
    public ParticleSystem splatterParticles;
    public ParticleDecalPool PDP;

    List<ParticleCollisionEvent> cEvents;

    public string bID;
    public string owner;
    public string playerColor;

    // Use this for initialization
    void Start () {
        cEvents = new List<ParticleCollisionEvent>();
        bID = PlayerPrefs.GetString("bid");
        playerColor = PlayerPrefs.GetString("color");

        ParticleSystem.MainModule mm = pLauncher.main;


        if (playerColor == "red")
        {
            pLauncher.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", Color.red);
            mm.startColor = new ParticleSystem.MinMaxGradient(Color.red);
        }

        if (playerColor == "blue")
        {
            pLauncher.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", Color.blue);
            mm.startColor = new ParticleSystem.MinMaxGradient(Color.blue);
        }

    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(pLauncher, other, cEvents);

        for(int i = 0; i < cEvents.Count; i++)
        {
            PDP.particleHit(cEvents[i]);
            EmitAtLocation(cEvents[i]);
        }
    }

    void EmitAtLocation(ParticleCollisionEvent pce)
    {
        splatterParticles.transform.position = pce.intersection;
        splatterParticles.transform.rotation = Quaternion.LookRotation(pce.normal);
        splatterParticles.Emit(1);
    }

    // Update is called once per frame
    void Update () {

	}
}
