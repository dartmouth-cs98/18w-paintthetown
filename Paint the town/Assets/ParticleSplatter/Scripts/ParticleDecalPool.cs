using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on https://unity3d.com/learn/tutorials/topics/scripting/droplet-decals?playlist=17117

// json helper taken from https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}


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
    public string getBuildingParticlesURL;
    public string setBuildingParticlesURL;
    public string buildingID;



    // Use this for initialization
    void Start () {
        particleDataIndex = 0;

        colorString = PlayerPrefs.GetString("color");

        decalPS = GetComponent<ParticleSystem>();

        buildingID = PlayerPrefs.GetString("bid");

        startGetBuildingParticles();

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

        InvokeRepeating("startSetBuildingParticles", 10.0f, 10.0f);
    }

    //starter fuction to retrieve building data
    public void startGetBuildingParticles()
    {
        getBuildingParticlesURL = "https://paint-the-town.herokuapp.com/api/particles?buildingId=" + buildingID;
        StartCoroutine("getBuildingParticles");
    }

    // starter function for saving building particle data
    public void startSetBuildingParticles()
    {
        setBuildingParticlesURL = "https://paint-the-town.herokuapp.com/api/particles";
        StartCoroutine("setBuildingParticles");
    }

    IEnumerator getBuildingParticles()
    {
        Hashtable headers = new Hashtable();
        headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
        WWW www = new WWW(getBuildingParticlesURL, null, headers);
        yield return www;

        print(www.text);

        if (www.text == "{\"particles\":[]}")
        {
            print("in the empty particle array section");
            //the building has no saved particles
            pd = new ParticleDecalData[maxParticleDecals];
            for (int i = 0; i < maxParticleDecals; i++)
            {
                pd[i] = new ParticleDecalData();
            }
        }
        else
        {
            print("in the saved particle array section");
            pd = JsonHelper.FromJson<ParticleDecalData>(www.text);
            print(www.text);
        }
    }

    IEnumerator setBuildingParticles()
    {
        print("Attempting to upload the array");
        WWWForm updateform = new WWWForm();

        updateform.AddField("buildingId", buildingID);
        string particles = JsonHelper.ToJson<ParticleDecalData>(pd);
        print(particles);
        updateform.AddField("particles", particles);

        Hashtable headers = new Hashtable();
        headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
        WWW www = new WWW(setBuildingParticlesURL, updateform.data, headers);
        yield return www;

        if (www.error != null)
        {
            print("Error uploading: " + www.error);
        }
        else
        {
            print(www.text);
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
