using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public GameObject elon;
    public GameObject tictacGO;

    private bool locked = false;
    private bool freezetracking;
    private Quaternion original_tictac_rotation = new Quaternion();
    private float last_tictac_range;
    public GameObject sea;
    public GameObject cam_gimble;
    private float speed_measurement;
    private Vector3 last_position = new Vector3();
    private Vector3 last_tictact_position = new Vector3();
    private Vector3 tictac_startpoint;
    private Vector3 tictac_endpoint;

    private List<Vector3> tictac_route = new List<Vector3>();
    private List<Vector3> hornet_route = new List<Vector3>();

    public float banking_angle = 0;
    public float debug;
    public GameObject hornet_model;
    public Vector3 hornet_currentpoint = new Vector3();
    public float final_bankangle = 10;
    public float current_bankangle = 1;
    public float ROT;
    public float hornet_speed_ms = 190;

    public Camera cam;
    public GameObject tictac;
    public GameObject hornet;
    public GameObject hornetGO;

    public Text hornet_speed;
    public Text range;
    public Text camera_alt;
    public Text camera_az;
    public Text hornet_altitude;
    public Text CAS;
    public Text tictac_speed;
    public Text Vc;
    public GameObject hornet_rotation;

    private float meter_to_nm = 0.000539957f;
    private float meters_to_feet = 3.28f;
    private float meters_to_knots = 1.94384f;
    private Quaternion display_rotation = new Quaternion();

    public Text timer_display;
    public float timer = 0;

    private float tic_tac_startrange = 8150;

    public positions start_setup = new positions();
    public positions end_setup = new positions();

    public Vector3 hornet_startpoint;
    public Vector3 hornet_endpoint;

    Vector3 cam_rotation_lerp = new Vector3();

    public float t = 0;
    // Start is called before the first frame update

    private Vector3 cam_start;
    private Vector3 cam_end;
    private Vector3 currentAngle = new Vector3();

    private Vector3 midpoint = new Vector3();

    void Start()
    {
        freezetracking = false;
        cam_gimble.transform.position = hornetGO.transform.position;
        tictac.transform.position = cam_gimble.transform.position;
        Quaternion rotation = cam.transform.rotation;
        tictac.transform.rotation = rotation;
        tictac.transform.position = tictac.transform.position + (tictac.transform.forward * tic_tac_startrange);

        start_setup.hornet_x = hornetGO.transform.position.x;
        start_setup.hornet_z = hornetGO.transform.position.z;
        start_setup.cam_x = 26;
        start_setup.cam_y = -43;
        start_setup.hornet_rotation_z = hornet.transform.eulerAngles.z;
        start_setup.tic_tac_range = tic_tac_startrange;

        end_setup.hornet_x = start_setup.hornet_x;
        end_setup.hornet_z = 3948;
        end_setup.cam_x = 35;
        end_setup.cam_y = -58;
        end_setup.hornet_rotation_z = 30;
       // end_setup.tic_tac_range = 6111;
        end_setup.tictac_x = -4870.5f;
        end_setup.tictac_z = 6642.7f;

        hornet_startpoint = new Vector3(start_setup.hornet_x, hornetGO.transform.position.y, start_setup.hornet_z);
        hornet_endpoint = new Vector3(end_setup.hornet_x, hornetGO.transform.position.y, end_setup.hornet_z);

        tictac_startpoint = tictac.transform.position;
        tictac_endpoint = new Vector3(end_setup.tictac_x, tictac.transform.position.y, end_setup.tictac_z);

        //midpoint = tictac_startpoint + ((tictac_startpoint - tictac_endpoint) / 2);

        cam_start = new Vector3(start_setup.cam_x, start_setup.cam_y, 0);
        cam_end = new Vector3(end_setup.cam_x, end_setup.cam_y, 0);
    }

    private void Update()
    {
        if (Input.GetKeyUp("1"))
            ReloadScene();
        if (Input.GetKeyUp("2"))
            toggle_freeze();
        if (Input.GetKeyUp("3"))
            switch_object();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (timer < 21)
        {
            
            t = timer / 21;

            if (timer < 6)
            {
                banking_angle = Mathf.Lerp(banking_angle, final_bankangle, (timer / 6) * Time.deltaTime);               
            }
            else
            {
                banking_angle = final_bankangle;
            }
            bank_hornet(banking_angle);
            hornetGO.transform.Rotate(Vector3.up * -banking_angle * Time.deltaTime);
            hornet_rotation.transform.eulerAngles = new Vector3(0, 0, -1 * banking_angle);
            // hornetGO.transform.position = Vector3.Lerp(hornet_startpoint, hornet_endpoint, t);

            hornetGO.transform.position += hornetGO.transform.forward * hornet_speed_ms * Time.deltaTime;
            
            cam_gimble.transform.position = hornetGO.transform.position;
            hornet_model.transform.position = hornetGO.transform.position;
            
            update_display();

            tictac.transform.position = Vector3.Lerp(tictac_startpoint, tictac_endpoint, t);
            if (!freezetracking)
            {
                tictac.transform.LookAt(tictac_endpoint);
                if(tictac != elon)
                    tictac.transform.Rotate(Vector3.left * 90f);
                cam.transform.LookAt(tictac.transform.position);
            }
            else
            {
                if (!locked)
                {
                    Vector3 freezepoint = Vector3.Lerp(tictac_startpoint, tictac_endpoint, t * 0.7f);
                    cam.transform.LookAt(freezepoint);
                    locked = true;
                }
            }
            hornet_currentpoint = hornetGO.transform.position;
        }
        timer += Time.deltaTime;
        timer_display.text = timer.ToString("F1");

        sea.transform.position += Vector3.forward * 2.2f * Time.deltaTime;

        update_routes();
    }

    private void bank_hornet(float bank_angle)
    {
        //rate of turn, degrees per second
        ROT = (1901 * Mathf.Tan(bank_angle))/369;
    }

    public void update_routes()
    {
        tictac_route.Add(tictac.transform.position);
        Vector3[] tictacroute = tictac_route.ToArray();
        tictac.GetComponent<LineRenderer>().positionCount = tictacroute.Length;
       tictac.GetComponent<LineRenderer>().SetPositions(tictacroute);

        hornet_route.Add(hornetGO.transform.position);
        Vector3[] hornetroute = hornet_route.ToArray();
        hornetGO.GetComponent<LineRenderer>().positionCount = hornetroute.Length;
        hornetGO.GetComponent<LineRenderer>().SetPositions(hornetroute);
    }

    public void update_display()
    {
        hornet_altitude.text = (hornetGO.transform.position.y * meters_to_feet).ToString("F0");

        float tictac_range = ((tictac.transform.position - cam.transform.position).magnitude * meter_to_nm);
        last_tictac_range = tictac_range;
        range.text = tictac_range.ToString("F1");

        camera_alt.text = (-1 * cam.transform.eulerAngles.x).ToString("F0");
        camera_az.text = (360 - cam.transform.eulerAngles.y).ToString("F0");

        speed_measurement = meters_to_knots * (hornetGO.transform.position - last_position).magnitude / Time.deltaTime;
        last_position = hornetGO.transform.position;
        hornet_speed.text = speed_measurement.ToString("F0");

        speed_measurement = meters_to_knots * (tictac.transform.position - last_tictact_position).magnitude / Time.deltaTime;
        last_tictact_position = tictac.transform.position;
        tictac_speed.text = speed_measurement.ToString("F0");

        speed_measurement = Mathf.Abs(last_tictac_range - tictac_range / Time.deltaTime);
        float vc = speed_measurement;
        Vc.text = vc.ToString("F0");
    }

    public void ReloadScene()
    {
        Debug.Log("reloading scene");
        SceneManager.LoadScene(0);
    }

    public void toggle_freeze()
    {
        locked = false;
        if (freezetracking)
            freezetracking = false;
        else
            freezetracking = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tictac.transform.position, tictac_endpoint);
    }

    private void switch_object()
    {
        if (tictac != elon)
        {
            elon.transform.position = tictac.transform.position;
            tictac = elon;
            elon.SetActive(true);
            tictacGO.SetActive(false);
        }
        else
        {
            tictacGO.transform.position = tictac.transform.position;
            tictac = tictacGO;
            tictacGO.SetActive(true);
            elon.SetActive(false);
        }
    }
}
