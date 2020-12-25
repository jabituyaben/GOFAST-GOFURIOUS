using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endpoint : MonoBehaviour
{
    public Vector3 gizmo_startpoint;
    public Vector3 gizmo_endpoint;
    // Start is called before the first frame update
    void Start()
    {
        //go to the start point
        transform.position = new Vector3(0,7620,0);
        transform.eulerAngles = new Vector3(26, -43, 0);

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            gizmo_startpoint = hit.point;
            Debug.Log("start = " + hit.point);
        }

        transform.position = new Vector3(-535.8372f, 7620, 3934.089f);
        transform.eulerAngles = new Vector3(35,-58,0);

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            Debug.Log(hit.point);
            gizmo_endpoint = hit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gizmo_endpoint,100f);
        Gizmos.DrawSphere(gizmo_startpoint, 100f);
    }

}
