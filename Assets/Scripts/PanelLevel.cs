/* =======================================================================================================
 * AK Studio
 * 
 * Version 1.0 by Alexandr Kuznecov
 * 04.12.2022
 * =======================================================================================================
 */

using UnityEngine;
using UnityEngine.UI;

public class PanelLevel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject m_SceneLoader = null;
    [SerializeField] private Text m_TextTotalScore = null;
    [SerializeField] private GameObject m_ButtonPrefab = null;
    [SerializeField] private GameObject m_ButtonsPanel = null;
    [SerializeField] private LevelCommonData LEVEL_DATA = null;

    public static SGameSettings GameSettings = new();

    private void UpdatePanel()
    {
        m_TextTotalScore.text = Translator.GetLangString("text_score").Replace("%num%", GameSettings.score.ToString());

        // Удалим все кнопки уровней
        foreach (Transform child in m_ButtonsPanel.transform)
        {
            Destroy(child.gameObject, 0);
        }

        // Добавим кнопки уровней
        for (int i = 0; i < LEVEL_DATA.DATA.Length; i++)
        {
            if (LEVEL_DATA.DATA[i] != null)
            {
                LevelData data = LEVEL_DATA.DATA[i];
                GameObject o = Instantiate(m_ButtonPrefab, Vector3.zero, Quaternion.identity);
                int level = i + 1;
                o.GetComponentInChildren<Text>().text = level.ToString();

                if (i == 0 || GameSettings.maxlevel >= level)
                {
                    o.transform.Find("Lock_Image").gameObject.SetActive(false);
                    
                    o.transform.Find("Stars").GetChild(GameSettings.levelstars[i + 1]).gameObject.SetActive(true); // Номер звезды

                    Button b = o.GetComponent<Button>();
                    b.onClick.AddListener(delegate () { LevelButtonClick(level); });
                }

                o.transform.SetParent(m_ButtonsPanel.transform, false);
            }
        }
    }

    private void OnEnable()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Time.timeScale = 1;

        if (GameManager.Instance != null)
        {
            GameSettings = GameManager.GameSettings;
        }
        else
        {
            GameSettings = SGameSettings.Load();
        }

        UpdatePanel();
    }

    public void LevelButtonClick(int num_level)
    {
        GameSettings.runbonuslevel = false;
        GameSettings.level = num_level;
        GameSettings.Save();
        m_SceneLoader.SetActive(true);

        AdsManager.Instance.FirebaseEvent($"click_lvl_{num_level}");
    }

    public void DebugUnLockAll()
    {
        GameSettings.maxlevel = LEVEL_DATA.DATA.Length;
        UpdatePanel();
    }

    public void DebugAddScore(int add)
    {
        GameSettings.score += add;
        GameSettings.Save();
        UpdatePanel();
    }

}
