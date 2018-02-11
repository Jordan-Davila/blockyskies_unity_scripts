using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {

    public GameObject gameOverGUI;
    public GameObject scoreGUI;
    public GameObject gameStartGUI;
    public GameObject menuGUI;
    public GameObject settingsGUI;
    public GameObject storeGUI;


    
    //Instanciate yourself
    private static GUIManager s_Instance = null;

    #region Instantiation

    // This defines a static instance property that attempts to find the manager object in the scene and returns it to the caller.
    public static GUIManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(GUIManager)) as GUIManager;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("GUIManager");
                s_Instance = obj.AddComponent(typeof(GUIManager)) as GUIManager;
                Debug.Log("Could not locate a GUIManager object. GUIManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }
    #endregion
   

    // Extensions 
    public bool DoesTagExist(string tagName)
    {
        try
        {
            GameObject aObj = GameObject.FindGameObjectWithTag(tagName);
            aObj.GetComponent<Text>();

            Debug.Log(tagName + " : true");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.Log(tagName + " : false");
            return false;
        }
    }

    // Use this for initialization
    private void Start () {

        //Assign Buttons for Menu or Game Mode
        //Are we in game mode? or Menu mode


	}

	public void ShowSettings()
    {
        MasterManager.Instance.ToggleObject(menuGUI, false);
        MasterManager.Instance.ToggleObject(settingsGUI, true);
    }

	public void ExitSettings()
	{
		MasterManager.Instance.ToggleObject(menuGUI, true);
		MasterManager.Instance.ToggleObject(settingsGUI, false);
	}

    public void GameStart()
    {
        MasterManager.Instance.GameStart();
    }

	public void GameRestart()
	{
		MasterManager.Instance.GameRestart();
	}

	public void GoToGame()
	{
		MasterManager.Instance.GoToGameMode();
	}

    public void GoToMenu()
    {
        MasterManager.Instance.GoToMenuMode();
    }

    public void ShowLeaderboard()
    {
        MasterManager.Instance.ShowLeaderboardUIWithGlobalID();
    }

    public void ShowAchievements()
    {
        MasterManager.Instance.ShowAchievementsUI();
    }

    public void SignIn()
    {
        MasterManager.Instance.SignIn();
    }

	public void SignOut()
	{
		MasterManager.Instance.SignOut();
	}
}