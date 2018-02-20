
using System.Collections;
using Wrld;
using Wrld.Resources.Buildings;
using Wrld.Space;
using UnityEngine;

// based on example code from https://wrld3d.com/unity/latest/docs/examples/picking-buildings/

public class HighlightBuildingOnClick : MonoBehaviour
{
    public Material highlightMaterial;
    private Vector3 mouseDownPosition;


    void OnEnable()
    {
       
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) && Vector3.Distance(mouseDownPosition, Input.mousePosition) < 5.0f)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var viewportPoint = Camera.main.WorldToViewportPoint(hit.point);
                var latLongAlt = Api.Instance.CameraApi.ViewportToGeographicPoint(viewportPoint, Camera.main);
                Api.Instance.BuildingsApi.HighlightBuildingAtLocation(latLongAlt, highlightMaterial, OnHighlightReceived);
            }
        }
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {
            StartCoroutine(ClearHighlight(highlight));
        }
    }

    IEnumerator ClearHighlight(Highlight highlight)
    {
        yield return new WaitForSeconds(3.0f);
        Api.Instance.BuildingsApi.ClearHighlight(highlight);
    }

    // set the desired material for the highlight
    public void OnRedClick()
    {
        //highlightMaterial = Resources.Load("RedMaterial") as Material;
        highlightMaterial.color = Color.red;
    }

    public void OnBlueClick()
    {
        //highlightMaterial = Resources.Load("BlueMaterial") as Material;
        highlightMaterial.color = Color.blue;
    }
}
