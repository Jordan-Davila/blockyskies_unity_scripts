using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpawn : MonoBehaviour {

    Vector3 left = new Vector3(-1, 0, 0);

    float speed = 10.0f;
    
    void FixedUpdate()
    {
        if (MasterManager.Instance.gameOver == false)
        {
            transform.position += left * speed * Time.deltaTime;

            if (transform.position.x <= -14.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
