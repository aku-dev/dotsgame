using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class UButton : MonoBehaviour
{

    [SerializeField] private string m_KeyName = "";

    private Button button = null;
    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(m_KeyName))
        {
            button.onClick.Invoke();
        }
    }

    public void Click()
    {
        button = GetComponent<Button>();
        button.onClick.Invoke();
    }
}
