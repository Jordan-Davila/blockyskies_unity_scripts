using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{

    //Load Buttons
    public GameObject leftBtn;
    public GameObject rightBtn;

    //Load Camera
    public GameObject cameraView;
    private Vector3 move = new Vector3(30, 0, 0);

    //Number of Themes
    public int themeTotal;

    //What theme am I currently viewing
    public int onTheme = 2;
    public int themePrice = 100;

    //GUIs To Enable or Disable
    public GameObject mainMenu;
    public GameObject themeMenu;

    //Players to Enable and Disable
    public GameObject[] playersMainMenu;

    // Buttons to disable and disable
    public GameObject buyBtn;
    public GameObject selectBtn;

    public void Awake()
    {
        // This is for initial purposes
        // Here we set all themes to false
        // TODO: Use a database to store this values for each individual account
        InitialThemeValues();
        CheckLastSelectedTheme();
    }

    public void SwitchThemesSelector(string whereTo)
    {
        if (whereTo == "left")
        {

            // If the selector is on the first theme, then go to the last theme
            if (onTheme == 2)
            {
                cameraView.transform.position = new Vector3(30 * themeTotal, -3, -25);
                onTheme = themeTotal + 1;
            }

            else
            {
                cameraView.transform.position -= move;
                onTheme -= 1;
            }

        }
        else if (whereTo == "right")
        {
            // If your on the last theme in the selector, go back to the first theme
            if (onTheme == themeTotal + 1)
            {
                cameraView.transform.localPosition = new Vector3(-40, -3, -25);

                // onThemeId = 2. 2 is the key identifier for the classic scene.
                onTheme = 2;

            }

            else
            {
                cameraView.transform.position += move;
                onTheme += 1;
            }
        }

        // Display Buy or Select Buttons *This depends if they bought it with gold*
        CheckThemes();

    }

    public void BuyTheme()
    {
        // Check if user has 100 gold, else show him a popup to buy coins
        if (MasterManager.Instance.gold >= themePrice)
        {

            // Subtract Gold Counter by the theme's price
            MasterManager.Instance.PayWithGold(themePrice);

            // Set the current theme id to the selected theme
            SelectTheme();

            // Now make this theme available to the user at all times. Save the data
            PlayerPrefs.SetString("themeId" + onTheme, "true");

        }
        else
        {
            // Play Beep Button
            // Load Gold Store
            Debug.Log("Not enough gold. Please buy some gold");
        }
    }

    public void SelectTheme()
    {
        MasterManager.Instance.currentThemeId = onTheme;
        MasterManager.Instance.GoToMenuMode();

		// Change Player Object in Main Menu
		DisplayPlayerMenu();

        // Save it in PlayerPref
        PlayerPrefs.SetInt("lastSelectedTheme", MasterManager.Instance.currentThemeId);
    }

    public void SwitchToThemeCamera()
    {
        if (themeMenu.activeInHierarchy == false)
        {
            themeMenu.SetActive(true);
            mainMenu.SetActive(false);
        }
    }

    public void SwitchToMainCamera()
    {
        if (mainMenu.activeInHierarchy == false)
        {
            mainMenu.SetActive(true);
            themeMenu.SetActive(false);
        }
    }

    public void InitialThemeValues()
    {
        for (int i = 2; i <= themeTotal + 1; i++)
        {
            if (!PlayerPrefs.HasKey("themeId" + i))
            {
                // Make our classic theme available *DUH*
                if (i == 2)
                {
                    PlayerPrefs.SetString("themeId" + i, "true");
                }

                else
                {
                    PlayerPrefs.SetString("themeId" + i, "false");
                }
            }

        }
    }

    public void CheckThemes()
    {

        // Disable or enable buy and select buttons
        if (PlayerPrefs.GetString("themeId" + onTheme) == "true")
        {
            // The player has bough this theme. Show them the select button;
            buyBtn.SetActive(false);
            selectBtn.SetActive(true);
        }

        else if (PlayerPrefs.GetString("themeId" + onTheme) == "false")
        {
            // Make it available to buy;
            buyBtn.SetActive(true);
            selectBtn.SetActive(false);
        }
    }

    public void CheckLastSelectedTheme()
    {
        if (PlayerPrefs.HasKey("lastSelectedTheme"))
        {
            MasterManager.Instance.currentThemeId = PlayerPrefs.GetInt("lastSelectedTheme");
			for (int i = 0; i < themeTotal; i++)
			{
				playersMainMenu[i].SetActive(false);
			}
        }
        else
        {
            MasterManager.Instance.currentThemeId = 2;
            PlayerPrefs.SetInt("lastSelectedTheme", 2);
        }

        playersMainMenu[MasterManager.Instance.currentThemeId - 2].SetActive(true);
    }

    public void DisplayPlayerMenu()
    {
        for (int i = 0; i < themeTotal; i++)
        {
            playersMainMenu[i].SetActive(false);
        }

        playersMainMenu[MasterManager.Instance.currentThemeId - 2].SetActive(true);
    }
}
