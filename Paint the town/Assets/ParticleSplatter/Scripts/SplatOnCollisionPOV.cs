using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatOnCollisionPOV : MonoBehaviour {

    public ParticleSystem particleLauncher;
    // public Gradient particleColorGradient;
    public ParticleDecalPoolPOV dropletDecalPool;

    List<ParticleCollisionEvent> collisionEvents;


    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        //print("Got into the the collision callback in the splatOnCollision script");
        ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            dropletDecalPool.particleHit(collisionEvents[i]);
        }

    }

}
