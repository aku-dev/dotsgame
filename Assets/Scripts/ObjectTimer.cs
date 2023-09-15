using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimer : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float m_TimeSeconds = 1f;

    [Header("Events")]
    [SerializeField] private GameObject m_ObjectOn = null;
    [SerializeField] private GameObject m_ObjectOff = null;

    private void OnEnable()
    {
        StartCoroutine(CRun());
    }

    private void OnDisable()
    {
        if (m_ObjectOn != null) m_ObjectOn.SetActive(false);
        if (m_ObjectOff != null) m_ObjectOff.SetActive(true);

        StopAllCoroutines();
    }

    private IEnumerator CRun()
    {
        yield return new WaitForSecondsRealtime(m_TimeSeconds);

        if (m_ObjectOn != null) m_ObjectOn.SetActive(true);
        if (m_ObjectOff != null) m_ObjectOff.SetActive(false);
    }
}
