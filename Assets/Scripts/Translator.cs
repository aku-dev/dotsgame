using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Translator : MonoBehaviour
{
    public static Translator Instance;


    //public static SGameSettings GameSettings = new();

    [SerializeField] private Text[] m_Texts = null;
    [SerializeField] private GameObject m_LogoRu = null;
    [SerializeField] private GameObject m_LogoEn = null;



    private readonly Dictionary<int, string> textsDictionary = new Dictionary<int, string>(); // Массив данных для перевода id, name
    private LangData langData = null;

    public static string GetLangString(string key)
    {
        if (Instance == null) return "";

        if(Instance.langData == null)
        {
            Instance.langData = Resources.Load<LangData>("Localization/" + GameManager.GameSettings.lang);
        }
        return LangData.GetValue(Instance.langData, key);
    }

    public static void SetLang(string l)
    {
        GameManager.GameSettings.lang = l;
        GameManager.GameSettings.Save();
        Instance.DoTranslate();
    }

    public void SetLangEvent(string l)
    {
        GameManager.GameSettings.lang = l;
        GameManager.GameSettings.Save();
        DoTranslate();
    }

    public void DoTranslate()
    {
        langData = Resources.Load<LangData>("Localization/" + GameManager.GameSettings.lang);

        if (textsDictionary.Count < 0) return;

        foreach (Text t in m_Texts)
        {
            if (t != null && textsDictionary[t.GetInstanceID()] != null)
            {
                t.text = LangData.GetValue(langData, textsDictionary[t.GetInstanceID()]);
            }
        }

        if (m_LogoRu != null) m_LogoRu.gameObject.SetActive(GameManager.GameSettings.lang == "ru");
        if (m_LogoEn != null) m_LogoEn.gameObject.SetActive(GameManager.GameSettings.lang != "ru");
    }

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

    private void LoadDictionary()
    {
        foreach (Text t in m_Texts) if (t != null) textsDictionary.Add(t.GetInstanceID(), t.text);
    }

    private void OnEnable()
    {
        // Язык игры
        if (GameManager.GameSettings.nosave)
        {
            GameManager.GameSettings = SGameSettings.Load();


                string s = GetArg("lang");
                if (s == "ru" || s == "en" || s == "tr")
                {
                    SetLang(s);
                    GameManager.GameSettings.lang = s;
                    GameManager.GameSettings.Save();
                }            
        }

        LoadDictionary();
        DoTranslate();
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
}
