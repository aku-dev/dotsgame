using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PanelMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer = null;
    [SerializeField] private Toggle m_Music = null;
    [SerializeField] private Toggle m_Effects = null;
    [SerializeField] private GameObject m_SoundOn = null;
    [SerializeField] private GameObject m_SoundOff = null;


    private void OnEnable()
    {
        if (GameManager.GameSettings.nosave == true) GameManager.GameSettings = SGameSettings.Load();

        if (m_Music != null)
        {
            if (GameManager.GameSettings.music > -50.0f) m_Music.isOn = true;
            else m_Music.isOn = false;
        }

        if (m_Effects != null)
        {
            if (GameManager.GameSettings.effects > -50.0f) m_Effects.isOn = true;
            else m_Effects.isOn = false;
        }

        if (m_SoundOn != null)
        {
            m_SoundOn.SetActive(GameManager.GameSettings.effects > -50.0f);
        }

        if (m_SoundOff != null)
        {
            m_SoundOff.SetActive(!(GameManager.GameSettings.effects > -50.0f));
        }
    }

    private void Start()
    {
        m_AudioMixer.SetFloat("MusicVolume", GameManager.GameSettings.music);
        m_AudioMixer.SetFloat("EffectsVolume", GameManager.GameSettings.effects);
    }


    public void setMusicToggle()
    {
        if (m_Music.isOn) GameManager.GameSettings.music = -10;
        else GameManager.GameSettings.music = -80;

        m_AudioMixer.SetFloat("MusicVolume", GameManager.GameSettings.music); 
        GameSettingSave();
    }

    public void setEffectToggle()
    {
        if (m_Effects.isOn) GameManager.GameSettings.effects = 0;
        else GameManager.GameSettings.effects = -80;

        m_AudioMixer.SetFloat("EffectsVolume", GameManager.GameSettings.effects);
        GameSettingSave();
    }

    public void EventSoundOn()
    {
        GameManager.GameSettings.music = -10;
        m_AudioMixer.SetFloat("MusicVolume", GameManager.GameSettings.music);
        GameManager.GameSettings.effects = 0;
        m_AudioMixer.SetFloat("EffectsVolume", GameManager.GameSettings.effects);
        GameSettingSave();
    }

    public void EventSoundOff()
    {
        GameManager.GameSettings.music = -80;
        m_AudioMixer.SetFloat("MusicVolume", GameManager.GameSettings.music);
        GameManager.GameSettings.effects = -80;
        m_AudioMixer.SetFloat("EffectsVolume", GameManager.GameSettings.effects);
        GameSettingSave();
    }

    private void GameSettingSave()
    {
        if(GameManager.Instance != null) 
            GameManager.GameSettings.Save();        
    }
}
