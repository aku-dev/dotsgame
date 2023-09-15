using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioMixer m_mixer = null;
    private bool m_sound = false;

    private void OnEnable()
    {
        float effects = 0;
        m_mixer.GetFloat("MusicVolume", out effects);        
        if(effects > 0)
        {
            m_sound = true;
        } else {
            m_sound = false;
        }
        UpdateText();
    }

    private void UpdateText()
    {        
        Text t = GetComponentInChildren<Text>();
        if (m_sound)
        {
            t.text = Translator.GetLangString("menu_sound_on");
        }
        else
        {
            t.text = Translator.GetLangString("menu_sound_off");
        }
    }

    public void OnButtonClick()
    {
        m_sound = !m_sound;
        UpdateText();
        if (m_sound)
        {
            m_mixer.SetFloat("EffectsVolume", 1.0f);
            m_mixer.SetFloat("MusicVolume", 1.0f);
        } else {
            m_mixer.SetFloat("EffectsVolume", -80.0f);
            m_mixer.SetFloat("MusicVolume", -80.0f);
        }
    }
}
