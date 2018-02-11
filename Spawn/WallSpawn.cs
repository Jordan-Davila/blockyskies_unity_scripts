using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawn : MonoBehaviour
{

    Vector3 left = new Vector3(-1, 0, 0);
    public float speed = 10.0f;
    public static float lvl = 0.09f;

    public Transform floor; //prefab

    public static bool goingUp;
    public static float t = 0.0f;
    public static float diff = 26.0f; //distace between the two walls

    void Start()
    {
        //Randomly decide if its going to start up or down
        int check = Random.Range(1, 4);
        if (check == 1 || check == 2) { goingUp = true; } else { goingUp = false; }

        //Create a rotation of floor;
        Instantiate(floor, new Vector3(transform.position.x, transform.position.y - diff, 0.0f), Quaternion.identity);
    }

    void FixedUpdate()
    {
        if (MasterManager.Instance.gameOver == false)
        {
            //Manage Levels
            LevelManager();

            //Move to left
            transform.position += left * speed * Time.deltaTime;

            //Reset
            if (transform.position.x <= -15.0f)
            {
                Reset();
            }
        }        
    }

    void Reset()
    {
        if (goingUp) { transform.position = new Vector3(14, transform.position.y + Mathf.Sin(t), 0); }
        else { transform.position = new Vector3(14, transform.position.y - Mathf.Sin(t), 0); }

        t += lvl;
        WallSpawn.t += lvl;

        Instantiate(floor, new Vector3(transform.position.x, transform.position.y - diff, 0.0f), Quaternion.identity);
    }

    public void LevelManager()
    {
        if (MasterManager.Instance.score <= 49)
        {
            lvl = 0.055f;
        }
        else if (MasterManager.Instance.score >= 50 && MasterManager.Instance.score <= 99)
        {
            lvl = 0.070f;
        }
        else if (MasterManager.Instance.score >= 100 && MasterManager.Instance.score <= 199)
        {
            lvl = 0.075f;
        }
        else if (MasterManager.Instance.score >= 200 && MasterManager.Instance.score <= 399)
        {
            lvl = 0.080f;
        }
		else if (MasterManager.Instance.score >= 400 && MasterManager.Instance.score <= 599)
		{
			lvl = 0.085f;
		}
		else if (MasterManager.Instance.score >= 600 && MasterManager.Instance.score <= 799)
		{
			lvl = 0.090f;
		}
        else if (MasterManager.Instance.score >= 800)
        {
            lvl = 0.1f;
        }
    }

}
