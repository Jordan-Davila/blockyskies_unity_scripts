using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSpawn : MonoBehaviour
{
    public float speed = 10.0f;
    Vector3 left = new Vector3(-1, 0, 0);

    private void Update()
    {
        if (MasterManager.Instance.gameOver == false)
        {
            transform.position += left * speed * Time.deltaTime;

            if (transform.position.x <= -16.0f)
            {
                Destroy(gameObject);
            }
        }
    }

}
