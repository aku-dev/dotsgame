using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject m_Object = null;

    /// <summary>
    /// Показать объект
    /// </summary>
    public void ShowObject()
    {
        if (m_Object != null)
        {
            m_Object.SetActive(true);
        }
    }

    /// <summary>
    /// Скрыть объект
    /// </summary>
    public void HideObject()
    {
        if (m_Object != null)
        {
            m_Object.SetActive(false);
        }
    }
}
