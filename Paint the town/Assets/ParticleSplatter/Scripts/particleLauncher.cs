using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on https://unity3d.com/learn/tutorials/topics/scripting/droplet-decals?playlist=17117


public class particleLauncher : MonoBehaviour {

    public ParticleSystem pLauncher;
    public ParticleSystem splatterParticles;
    public ParticleDecalPool PDP;

    List<ParticleCollisionEvent> cEvents;

	// Use this for initialization
	void Start () {
        cEvents = new List<ParticleCollisionEvent>();
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

        // emit one particle, if the firebutton is held down
        if (Input.GetButton("Fire1")) {
            ParticleSystem.MainModule psMain = pLauncher.main;
            pLauncher.Emit(1);
        }

        
        
	}
}
