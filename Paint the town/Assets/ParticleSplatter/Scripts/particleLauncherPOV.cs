using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on https://unity3d.com/learn/tutorials/topics/scripting/droplet-decals?playlist=17117


public class particleLauncherPOV : MonoBehaviour {

    public ParticleSystem pLauncher;
    public ParticleSystem splatterParticles;
    public ParticleDecalPool PDP;

    List<ParticleCollisionEvent> cEvents;

    public string bID;
    public string owner;
    public string playerColor;
    private ParticleSystem.MainModule psMain;
    private ParticleSystem.MainModule psSplat;

    public ParticleSystem.EmitParams e_params;
    public Camera setCam;

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
        //print("You're capturing a building");

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
            // print(www.text);
            // print("building captured!");
            // print("THIS IS COLOR " + PlayerPrefs.GetString("color", "no color"));
        }
    }

    // Use this for initialization
    void Start () {
        cEvents = new List<ParticleCollisionEvent>();
        bID = PlayerPrefs.GetString("bid");
        playerColor = PlayerPrefs.GetString("color");
        InvokeRepeating("startOwnershipCheck", 5.0f, 2.0f);
        psMain = pLauncher.main;
        psSplat = splatterParticles.main;

        // set color
        if (playerColor == "blue")
        {
            psMain.startColor = Color.blue;
            psSplat.startColor = Color.blue;
        }

        if (playerColor == "red")
        {
            psMain.startColor = Color.red;
            psSplat.startColor = Color.red;
        }

        if (playerColor == "green")
        {
            psMain.startColor = Color.green;
            psSplat.startColor = Color.green;
        }

        if (playerColor == "orange")
        {
            psMain.startColor = new Color(1.0F, 165.0F / 255.0F, 0.0F);
            psSplat.startColor = new Color(1.0F, 165.0F / 255.0F, 0.0F);
        }

        if (playerColor == "yellow")
        {
            psMain.startColor = Color.yellow;
            psSplat.startColor = Color.yellow;
        }

        if (playerColor == "purple")
        {
            psMain.startColor = new Color(160.0F / 255F, 32.0F / 255F, 240.0F / 255F);
            psSplat.startColor = new Color(160.0F / 255F, 32.0F / 255F, 240.0F / 255F);
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

        pLauncher.transform.SetPositionAndRotation(setCam.transform.position, setCam.transform.rotation);

        // emit one particle, if the firebutton is held down
        if (Input.GetButton("Fire1")) {
           // print("we in that");

            // FIX IT FIX IT FIX IT
            var screenPos = new Vector3
            {
                x = Input.mousePosition.x,
                y = Input.mousePosition.y,
                z = setCam.transform.position.z
            };

            var ray = setCam.ScreenPointToRay(screenPos);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var viewportPoint = setCam.WorldToViewportPoint(hit.point);
                var tempPosition = hit.point;
                e_params.position = tempPosition;
                pLauncher.Emit(e_params, 1);
            }
        }
	}

    void startOwnershipCheck ()
    {
        StartCoroutine("checkOwnership");
    }
}
