using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawn : MonoBehaviour
{
    public float speed = 10.0f;
    public float intitialPosition;
    public int barrierCounter = 0;
    public GameObject goldPrefab;

    Vector3 left = new Vector3(-1, 0, 0);
    
    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(intitialPosition, Random.Range(-12.0f, -2.0f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (MasterManager.Instance.gameOver == false)
        {
            transform.position += left * speed * Time.deltaTime;

            if (transform.position.x <= -16.0f)
            {
                Reset();
            }
        }
    }

    void Reset()
    {
        barrierCounter += 1;

        if (barrierCounter % 3 == 0)
        {
            Instantiate(goldPrefab, new Vector3(10, Random.Range(-10, 0), 0.0f), Quaternion.identity);
        }

        transform.position = new Vector3(16.0f, Random.Range(-12.0f, -2.0f), 0);
    }
}
