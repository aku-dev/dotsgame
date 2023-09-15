using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class AdsManager : MonoBehaviour
{
    [SerializeField] private Button m_RewardedButton = null;
    [SerializeField] private UnityEvent m_RewardedEvents = new UnityEvent();
    public static AdsManager Instance = null;

    private bool showInterstitialAd = false;

    // Синглтон
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }        
    }

    private void OnEnable()
    {
        Debug.Log("AdsManager.OnEnable()");

    }

    private void OnDisable()
    {
        StopAllCoroutines();      
    }

    private void Start()
    {

    }

    public void FirebaseEvent(string str)
    {
        str = str.ToLower();
        Debug.Log($"gm_{str}");
        //Firebase.Analytics.FirebaseAnalytics.LogEvent($"gm_{str}");
    }

    private void InterstitialLoaded()
    {

    }

    private void RewardedLoaded()
    {
        m_RewardedButton.interactable = true;
    }

    public void ShowAds()
    {
        Debug.Log("ShowAds()");
    }

    public void ShowRewardedAds()
    {
        Debug.Log("ShowRewardedAds()");
    }

    public void RewardedSuccessful()
    {
        m_RewardedEvents.Invoke();
    }

    #region HTML5 Events

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            AudioListener.volume = 0;

        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }
    }


    public static string GetArg(string name)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
    #endregion

}
