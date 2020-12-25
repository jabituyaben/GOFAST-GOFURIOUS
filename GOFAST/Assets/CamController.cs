using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public GameObject tictac;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.LookAt(tictac.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
