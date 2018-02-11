using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

    public string loadLevel;

	// Use this for initialization
	void Start () {
        StartCoroutine("CountDown");
	}

    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(loadLevel);
    }
}
