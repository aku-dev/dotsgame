using UnityEngine;
using UnityEngine.UI;

public class PanelStart : MonoBehaviour
{
    [SerializeField] private Text m_TextLevel = null;
    [SerializeField] private Text m_TextTarget = null;
    [SerializeField] private Text m_TextSteps = null;

    [SerializeField] private Text m_TextBlueCount = null;
    [SerializeField] private Text m_TextGreenCount = null;
    [SerializeField] private Text m_TextOrangeCount = null;
    [SerializeField] private Text m_TextRedCount = null;
    [SerializeField] private Text m_TextVioletCount = null;
    [SerializeField] private Text m_TextYellowCount = null;

    private void OnEnable()
    {
        string tl;
        
        if (GameManager.GameSettings.runbonuslevel)
        {
            tl = Translator.GetLangString("text_bonus_level");
            m_TextLevel.text = $"{tl}";
        }
        else
        {
            tl = Translator.GetLangString("text_level");
            m_TextLevel.text = $"{tl} {GameManager.Game.level}";
        }

        m_TextTarget.text = Translator.GetLangString("text_target");

        string ts = Translator.GetLangString("text_start_steps").Replace("%count%", GameManager.Game.steps.ToString());
        m_TextSteps.text = ts;

        m_TextBlueCount.gameObject.SetActive(GameManager.Game.countBlue > 0);
        m_TextBlueCount.text = $"{GameManager.Game.countBlue}";

        m_TextGreenCount.gameObject.SetActive(GameManager.Game.countGreen > 0);
        m_TextGreenCount.text = $"{GameManager.Game.countGreen}";

        m_TextOrangeCount.gameObject.SetActive(GameManager.Game.countOrange > 0);
        m_TextOrangeCount.text = $"{GameManager.Game.countOrange}";

        m_TextRedCount.gameObject.SetActive(GameManager.Game.countRed > 0);
        m_TextRedCount.text = $"{GameManager.Game.countRed}";

        m_TextVioletCount.gameObject.SetActive(GameManager.Game.countViolet > 0);
        m_TextVioletCount.text = $"{GameManager.Game.countViolet}";

        m_TextYellowCount.gameObject.SetActive(GameManager.Game.countYellow > 0);
        m_TextYellowCount.text = $"{GameManager.Game.countYellow}";
    }
}
