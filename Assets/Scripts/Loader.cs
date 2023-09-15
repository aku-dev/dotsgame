using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [SerializeField] private int sceneID = 0;

    [Header("Иконка загрузки")]
    [SerializeField] private Image ImageLoader = null;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AsyncLoad());
    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            ImageLoader.fillAmount = operation.progress / 0.9f;

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    } 

    public void SetSceneID(int i)
    {
        sceneID = i;
    }
}
