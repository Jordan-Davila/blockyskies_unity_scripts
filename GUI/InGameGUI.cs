using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameGUI : MonoBehaviour {

	public Text scoreGUIScore;
	public Text scoreGUIGold;
	public Text scoreGUIHighScore;

	public Text gameOverGUIScore;
	public Text gameOverGUIGold;
	public Text gameOverGUIHighScore;
	public Text gameOverGUIMessage;

	//Instanciate yourself
	private static InGameGUI s_Instance = null;

	#region Instantiation

	// This defines a static instance property that attempts to find the manager object in the scene and returns it to the caller.
	public static InGameGUI Instance
	{
		get
		{
			if (s_Instance == null)
			{
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance = FindObjectOfType(typeof(InGameGUI)) as InGameGUI;
			}

			// If it is still null, create a new instance
			if (s_Instance == null)
			{
				GameObject obj = new GameObject("GUIManager");
				s_Instance = obj.AddComponent(typeof(InGameGUI)) as InGameGUI;
				Debug.Log("Could not locate a GUIManager object. GUIManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}
	#endregion

}
