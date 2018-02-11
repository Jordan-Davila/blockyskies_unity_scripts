using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using GoogleMobileAds.Api;

public class MasterManager : MonoBehaviour
{

    //Initialize: Game Options
    public float score;
    public int highScore;
    public int gold;  
    public int currentThemeId;

    // Debug: Lets use the game's title t show logs
    public Text gameTitle;

    //Initialize: Game States
    public bool gameOver = true;
    public bool inGame = false; //Initial Game Mode is menu
    public int numberOfLost;
    private bool isFirstGame = true;

    // Game Services
    private string leaderobardID = "leaderboard_i_can_fly";

    // Admob
    private InterstitialAd interstitial;
    private BannerView bannerView;

    // This variable is to solve inconsistancies on unlocking achievements
    // CurrentAchievementID helps us set the unlocked achievement once. Next time if
    // it matches with itselft, UnlockAchievement won't run because it already ran once.
    private string currentAchievementID = "";

    //Instanciate yourself
    private static MasterManager s_Instance = null;

    #region Instantiation

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static MasterManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(MasterManager)) as MasterManager;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("MasterManager");
                s_Instance = obj.AddComponent(typeof(MasterManager)) as MasterManager;
                Debug.Log("Could not locate a MasterManager object. MasterManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
       s_Instance = null;
    }

    #endregion

    //IMPORTANT! Dont destroy this object when loading a scene
    private void Awake()
    {
        //this state that if the gameobject to which this script is attached , if it is present in scene then destroy the new one , and if its not present
        //then create new
        if (s_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        SignIn();

        AdmobInit();
    }

	private void OnEnable()
	{
		// Registering for event
		CloudServices.KeyValueStoreDidSynchroniseEvent += OnKeyValueStoreDidSynchronise;
		CloudServices.KeyValueStoreDidChangeExternallyEvent += OnKeyValueStoreChanged;
	}

	private void OnDisable()
	{
		// Unregistering event
		CloudServices.KeyValueStoreDidSynchroniseEvent -= OnKeyValueStoreDidSynchronise;
		CloudServices.KeyValueStoreDidChangeExternallyEvent -= OnKeyValueStoreChanged;
	}

    #region Game Options

    public void GameInit()
    {
        // Check if this is the player's first game

        if (IsAuthenticated())
        {
			Debug.Log("isFirstGame: " + NPBinding.CloudServices.GetBool("isFirstGame"));

			if (!NPBinding.CloudServices.GetBool("isFirstGame"))
			{
				// Make player a regular user. This means this isn't the first time he's played
				NPBinding.CloudServices.SetBool("isFirstGame", true);

                isFirstGame = true;

				// Give the player a reward for trying out the game
				AddGold(10);

			}

            else
            {
                isFirstGame = false;
            }
        }
        else
        {
			// Check is this is the first time the player plays this game.
			if (PlayerPrefs.HasKey("isFirstGame"))
			{
				// Make this the first Game for user
				isFirstGame = true;

				// Set to Local. Next time it won't be his first game.
				PlayerPrefs.SetString("isFirstGame", "1");

				AddGold(10);
			}
			else
			{
				isFirstGame = false;
			}
        }

        // If this isn't the first game than get his gold and score local or cloud

        if (!isFirstGame)
        {
			//Get Gold
			GetSavedGold();

			//Get highScore
			GetSavedHighScore();
        }

    }

    public void GameStart()
    {
        //Reset Score

        // TODO: If user payed 10 gold or watch video ad, Dont restart score.
        score = 0;

        //Start Game
        gameOver = false;

        //Enable GUI
        ToggleObject(GUIManager.Instance.scoreGUI, true);

        //Disable GUI
        ToggleObject(GUIManager.Instance.gameOverGUI, false);
        ToggleObject(GUIManager.Instance.gameStartGUI, false);
    }

    public void GameRestart()
    {
        //Start Game
        gameOver = true;

        //This Reload Scene
        LoadScene(currentThemeId);
    }

    public void GameOver()
    {
        //End Game
        gameOver = true;

        //Store HighScore
        SaveHighScore();

        //Enable GUI
        ToggleObject(GUIManager.Instance.gameOverGUI, true);

        //Disable GUI
        ToggleObject(GUIManager.Instance.scoreGUI, false);

        // Keep track of how many times the player has lost
        numberOfLost++;

        // Show ad every 2 losts
        if (numberOfLost % 2 == 0)
        {
            // Show Interstitial
            ShowInterstitial();

            // After showing ad request another one.
            RequestInterstitial();
        }

        // Display Game Messages
        if (score > highScore)
        {
            InGameGUI.Instance.gameOverGUIMessage.text = "Great Job!";
            InGameGUI.Instance.gameOverGUIHighScore.text = "New HighScore";
            InGameGUI.Instance.gameOverGUIScore.text = Mathf.RoundToInt(score).ToString();
            InGameGUI.Instance.gameOverGUIGold.text = gold.ToString();
        }
        else
        {
			InGameGUI.Instance.gameOverGUIMessage.text = "Game Over";
            InGameGUI.Instance.gameOverGUIHighScore.text = "TOP " + highScore.ToString();
            InGameGUI.Instance.gameOverGUIScore.text = Mathf.RoundToInt(score).ToString();
            InGameGUI.Instance.gameOverGUIGold.text = gold.ToString();
        }
    }

    // HighScores

    public void AddScore()
    {
        score += 10.0f * Time.deltaTime;
        int pointInt = Mathf.RoundToInt(score);
        // GUIManager.Instance.scoreText.text = pointInt.ToString();
        InGameGUI.Instance.scoreGUIScore.text = pointInt.ToString();

        //Display HighScore
        if (score > highScore)
        {
            // GUIManager.Instance.highScoreText.text = "TOP " + pointInt.ToString();
            InGameGUI.Instance.scoreGUIHighScore.text = "TOP " + pointInt.ToString();
        }

        // Unlock achievements
        if (pointInt == 100) { LoadAchievementOnce("achievement001", 100.0f); }
        else if (pointInt == 200) { LoadAchievementOnce("achievement002", 100.0f); }
        else if (pointInt == 400) { LoadAchievementOnce("achievement003", 100.0f); }
        else if (pointInt == 600) { LoadAchievementOnce("achievement004", 100.0f); }
        else if (pointInt == 1000) { LoadAchievementOnce("achievement005", 100.0f); }

    }

    public void GetSavedHighScore()
    {
        // Check if user is signed in
        if (IsAuthenticated())
        {
            highScore = int.Parse(NPBinding.CloudServices.GetString("highScore"));
            Debug.Log("Get HighScore Cloud: " + NPBinding.CloudServices.GetString("highScore"));
        }
        else
        {
            highScore = int.Parse(PlayerPrefs.GetString("highScore"));
            Debug.Log("Get HighScore Local: " + PlayerPrefs.GetString("highScore"));
        }

        // Display highscore to GUI
        InMenuGUI.Instance.highScore.text = "TOP " + highScore.ToString(); 
    }

    public void SaveHighScore()
    {
        if (score >= highScore)
        {
            // Round highScore
            highScore = Mathf.RoundToInt(score);

            // Display highscore to GUI
            InGameGUI.Instance.scoreGUIHighScore.text = "TOP " + highScore.ToString();

			// Add Score to Google Leaderboard
			AddScoreToLeaderboard(highScore);

            // Store highscore to local
            if (IsAuthenticated())
            {
                NPBinding.CloudServices.SetString("highScore", highScore.ToString());
                Debug.Log("Save HighScore Cloud: " + highScore);
            }
            else
            {
                PlayerPrefs.SetString("highScore", highScore.ToString());
                Debug.Log("Save HighScore Local: " + highScore);
            }
        }
    }

    // Gold

    public void AddGold(int goldAmount)
    {
        // Add Gold
        gold += goldAmount;

        // Display to user
        InGameGUI.Instance.scoreGUIGold.text = gold.ToString();

        // Save Gold to local or cloud
        SaveGold(gold.ToString());
    }

    public void GetSavedGold()
    {

        // Check if user is signed in
        if (IsAuthenticated())
        {
            // Convert String to int
            gold = int.Parse(NPBinding.CloudServices.GetString("gold"));
            Debug.Log("Get Gold Cloud: " + NPBinding.CloudServices.GetString("gold"));
        }
        else
        {
            gold = int.Parse(PlayerPrefs.GetString("gold"));
			Debug.Log("Get Gold Local: " + PlayerPrefs.GetString("gold"));
        }

		// Display
        InMenuGUI.Instance.gold.text = gold.ToString();

	}

    public void SaveGold(string goldAmount)
    {
        // Amount to be stored

        if (IsAuthenticated())
        {
			NPBinding.CloudServices.SetString("gold", goldAmount);
            Debug.Log("Save Gold Cloud: " + goldAmount);
        }
        else
        {
            PlayerPrefs.SetString("gold", goldAmount);
			Debug.Log("Save Gold Local: " + goldAmount);
        }
    }

    public void PayWithGold(int goldAmount)
    {
        gold -= goldAmount;

        InGameGUI.Instance.scoreGUIGold.text = gold.ToString();


        // Check if the user is signed in
        if (IsAuthenticated())
        {
            //StoreGoldToCloud();
        }

        else
        {
            //StoreGoldToLocal();
        }
    }

    #endregion

    #region Scene Options

    public void LoadScene(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ToggleObject(GameObject game_object, bool enable_disable)
    {
        game_object.SetActive(enable_disable);
    }

    public void GoToGameMode()
    {
        LoadScene(currentThemeId);
        inGame = true;
    }

    public void GoToMenuMode()
    {
        // Scene #1 is the Main Menu Scene
        // Check Build Settigns to get the id for each scene
        LoadScene(1);
        inGame = false;
    }

    #endregion

    #region Game Services

    public void SignIn()
    {
        // Check is Gameservices are available
        bool _isAvailable = NPBinding.GameServices.IsAvailable();

        if (_isAvailable && !IsAuthenticated())
        {
            // Sign User In
            NPBinding.GameServices.LocalUser.Authenticate((bool _success, string _error) =>
            {

                if (_success)
                {
                    Debug.Log("Sign-In Successfully");
                    Debug.Log("Local User Details : " + NPBinding.GameServices.LocalUser.ToString());
					
                    // Init Cloud
					CloudInit();

					// Init Game
					GameInit();

                }
                else
                {
                    Debug.Log("Sign-In Failed with error " + _error);

					// Init Game
					GameInit();
                }
            });
        }

    }

    public void SignOut()
    {
		NPBinding.GameServices.LocalUser.SignOut((bool _success, string _error) => {

			if (_success)
			{
				Debug.Log("Local user is signed out successfully!");
			}
			else
			{
				Debug.Log("Request to signout local user failed.");
				Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
			}
		});
    }
   
    public bool IsAuthenticated()
    {
        return NPBinding.GameServices.LocalUser.IsAuthenticated;
    }

    public void ShowAchievementsUI()
    {
        if (IsAuthenticated())
        {
            Debug.Log("Sending request to show achievements view.");

            NPBinding.GameServices.ShowAchievementsUI((string _error) =>
            {
                Debug.Log("Achievements view dismissed.");
                Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
            });
        }

        else
        {
            SignIn();
        }

    }

    public void ShowLeaderboardUIWithGlobalID()
    {
        if (IsAuthenticated())
        {
            Debug.Log("Sending request to show leaderboard view.");

            NPBinding.GameServices.ShowLeaderboardUIWithGlobalID(leaderobardID, eLeaderboardTimeScope.ALL_TIME, (string _error) =>
            {
                Debug.Log("Leaderboard view dismissed.");
                Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
            });
        }
        else
        {
            SignIn();
        }

    }

    public void AddScoreToLeaderboard(long score)
    {
        NPBinding.GameServices.ReportScoreWithGlobalID(leaderobardID, score, (bool _success, string _error) =>
        {

            if (_success)
            {
                Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} finished successfully.", leaderobardID));
                Debug.Log(string.Format("New score= {0}.", score));
            }
            else
            {
                Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} failed.", leaderobardID));
                Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
            }
        });
    }

    public void UnlockAchievements(string achievementID, double percentINT)
    {
        NPBinding.GameServices.ReportProgressWithGlobalID(achievementID, percentINT, (bool _success, string _error) =>
        {

            if (_success)
            {
                Debug.Log(string.Format("Request to report progress of achievement with GID= {0} finished successfully.", achievementID));
                Debug.Log(string.Format("Percentage completed= {0}.", percentINT));
            }
            else
            {
                Debug.Log(string.Format("Request to report progress of achievement with GID= {0} failed.", achievementID));
                Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
            }

        });

        // Don't repeat this again
        currentAchievementID = achievementID;
    }

    public void LoadAchievementOnce(string achievementID, double percentINT)
    {
        if (currentAchievementID != achievementID)
        {
            UnlockAchievements(achievementID, percentINT);
        }
    }

    private void CloudInit()
    {
        if (IsAuthenticated())
        {
            NPBinding.CloudServices.Initialise();
            Debug.Log("Cloud service initialize");
        }
        else
        {
            Debug.Log("Cloud service not initialize: User not authenticated");
        }
    }

	private void OnKeyValueStoreDidSynchronise(bool _success)
	{
		if (_success)
		{
			Debug.Log("Successfully synchronised in-memory keys and values.");
		}
		else
		{
			Debug.Log("Failed to synchronise in-memory keys and values.");
		}
	}

	private void OnKeyValueStoreChanged(eCloudDataStoreValueChangeReason _reason, string[] _changedKeys)
	{
		Debug.Log("Cloud key-value store has been changed.");

		if (_changedKeys != null)
		{
			Debug.Log(string.Format("Total keys changed: {0}.", _changedKeys.Length));
			Debug.Log(string.Format("Pick a value from old and new and set the value to cloud for resolving conflict."));
		}
	}


	#endregion

	#region AdMob

	private void AdmobInit()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-3940256099942544~1458002511";
#else
        string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Request Interstitial on Init
        RequestInterstitial();
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();

    }

    private void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);

        // Load a banner ad.
        this.bannerView.LoadAd(this.CreateAdRequest());
    }

    private void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(adUnitId);

        // Load an interstitial ad.
        this.interstitial.LoadAd(this.CreateAdRequest());
    }

    private void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            MonoBehaviour.print("Interstitial is not ready yet");
        }
    }

    #endregion
}
