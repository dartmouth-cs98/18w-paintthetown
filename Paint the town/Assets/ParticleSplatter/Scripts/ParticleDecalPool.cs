using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on https://unity3d.com/learn/tutorials/topics/scripting/droplet-decals?playlist=17117


public class ParticleDecalPool : MonoBehaviour {
    private int particleDataIndex;
    public int maxParticleDecals = 200;
    public ParticleDecalData[] pd;
    public float decalSizeMin = .5f;
    public float decalSizeMax = 1.5f;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem decalPS;
    private ParticleSystem.MainModule decalMain;
    private string colorString;
    private Color plrColor;

	// Use this for initialization
	void Start () {
        particleDataIndex = 0;

        colorString = PlayerPrefs.GetString("color");

        

        decalPS = GetComponent<ParticleSystem>();

        pd = new ParticleDecalData[maxParticleDecals];
        particles = new ParticleSystem.Particle[maxParticleDecals];

        for (int i = 0; i < maxParticleDecals; i++)
        {
            pd[i] = new ParticleDecalData();
        }

        decalMain = decalPS.main;

        // set color
        if (colorString == "blue")
        {
            decalMain.startColor = Color.blue;
            plrColor = Color.blue;
        }

        if (colorString == "red")
        {
            decalMain.startColor = Color.red;
            plrColor = Color.red;
        }

        if (colorString == "green")
        {
            decalMain.startColor = Color.green;
            plrColor = Color.green;
        }

        if (colorString == "orange")
        {
            decalMain.startColor = new Color(0.5f, 0.5f, 0.0f);
            plrColor = new Color(0.5f, 0.5f, 0.0f);
        }

        if (colorString == "yellow")
        {
            decalMain.startColor = Color.yellow;
            plrColor = Color.yellow;
        }

        if (colorString == "purple")
        {
            decalMain.startColor = new Color(0.5f, 0.0f, 0.5f);
            plrColor = new Color(0.5f, 0.0f, 0.5f);
        }

    }

    public void particleHit(ParticleCollisionEvent particleCollisionEvent)
    {
        SetParticleData(particleCollisionEvent);
        DisplayParticles();
    }

    void DisplayParticles()
    {
        for(int i = 0; i < maxParticleDecals; i++)
        {
            particles[i].position = pd[i].pos;
            particles[i].rotation3D = pd[i].rotation;
            particles[i].startSize = pd[i].size;
            particles[i].startColor = pd[i].color;
        }

        decalPS.SetParticles(particles, particles.Length);
    }
	
	void SetParticleData(ParticleCollisionEvent pce)
    {
        // check to make sure we arent going beyond the last index
        if(particleDataIndex >= maxParticleDecals)
        {
            particleDataIndex = 0;
        }

        // record the collision's position, size, and color
        pd[particleDataIndex].pos = pce.intersection;
        pd[particleDataIndex].rotation = Quaternion.LookRotation(pce.normal).eulerAngles;
        pd[particleDataIndex].rotation.z = Random.Range(0, 360);
        pd[particleDataIndex].size = Random.Range(decalSizeMin, decalSizeMax);
        pd[particleDataIndex].color = plrColor;

        particleDataIndex++;
    }
}
