using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on https://unity3d.com/learn/tutorials/topics/scripting/droplet-decals?playlist=17117


public class particleLauncher : MonoBehaviour {

    public ParticleSystem pLauncher;
    public ParticleSystem splatterParticles;
    public ParticleDecalPool PDP;

    List<ParticleCollisionEvent> cEvents;

    public string bID;
    public string owner;
    public string playerColor;

    IEnumerator checkOwnership()
    {
        int red = 0;
        int blu = 0;

        foreach(ParticleDecalData pdd in PDP.pd)
        {
            if (pdd.color == Color.blue)
            {
                blu += 1;
            }

            else
            {
                red += 1;
            }
        }

        if (blu > red)
        {
            owner = "blue";
        }

        else
        {
            owner = "red";
        }

        if (owner.Equals(playerColor))
        {
            StartCoroutine("captureBuilding");
        }

        yield return null;
    }

    IEnumerator captureBuilding()
    {
        print("You're capturing a building");

        WWWForm captureform = new WWWForm();

        captureform.AddField("building", bID);
        captureform.AddField("team", PlayerPrefs.GetString("teamID", "no teamID"));

        Hashtable headers = new Hashtable();
        headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));

        WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings/updateTeam", captureform.data, headers);
        yield return www;
        if (www.error != null)
        {
            print("Error downloading: " + www.error);
        }
        else
        {
            print(www.text);
            print("building captured!");
            print("THIS IS COLOR " + PlayerPrefs.GetString("color", "no color"));
        }
    }

    // Use this for initialization
    void Start () {
        cEvents = new List<ParticleCollisionEvent>();
        bID = PlayerPrefs.GetString("bid");
        playerColor = PlayerPrefs.GetString("color");
        InvokeRepeating("startOwnershipCheck", 5.0f, 2.0f);
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

    void startOwnershipCheck ()
    {
        StartCoroutine("checkOwnership");
    }
}
