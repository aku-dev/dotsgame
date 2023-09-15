using UnityEngine;
using UnityEngine.UI;

public class PanelBonus : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject m_SceneLoader = null;
    [SerializeField] private GameObject[] m_Flowers = null;
    [SerializeField] private Button[] m_Buttons = null;
    [SerializeField] private Text m_TextTotalScore = null;

    public static SGameSettings GameSettings = new();
    private const int MAX_SCORE = 5000;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameSettings = GameManager.GameSettings;
        }
        else
        {
            GameSettings = SGameSettings.Load();
        }

        m_TextTotalScore.text = Translator.GetLangString("text_score").Replace("%num%", GameSettings.score.ToString());


        for (int i = 0; i < m_Flowers.Length; i++)
        {
            m_Flowers[i].SetActive(GameSettings.score > MAX_SCORE / m_Flowers.Length * (i + 1));
        }

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            if (GameSettings.bonuscomp[i] == 1)
            {
                m_Buttons[i].interactable = false;
                m_Buttons[i].transform.Find("Lock_Image").gameObject.SetActive(false);
                m_Buttons[i].transform.Find("Complete").gameObject.SetActive(true);
            }
            else
            {
                if (GameSettings.score > MAX_SCORE / m_Buttons.Length * (i + 1))
                {
                    m_Buttons[i].interactable = true;
                    m_Buttons[i].transform.Find("Lock_Image").gameObject.SetActive(false);

                    int level = i + 1;
                    m_Buttons[i].onClick.AddListener(delegate () { LevelButtonClick(level); });
                }
            }
        }
    }

    public void LevelButtonClick(int num_level)
    {
        GameSettings.runbonuslevel = true;
        GameSettings.level = num_level;
        GameSettings.Save();

        AdsManager.Instance.FirebaseEvent($"click_bns_{num_level}");
        m_SceneLoader.SetActive(true);
    }
}
