using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New LevelCommonData", menuName = "Level Common Data", order = 51)]
public class LevelCommonData : ScriptableObject
{
    [SerializeField] private LevelData[] _DATA = null;
    public LevelData[] DATA => _DATA;
}

[System.Serializable]
public class LevelData
{
    [SerializeField] private string _Name = "level_1";
    public string Name => _Name;

    [Header("������� ����")]
    [Tooltip("������ ����. �� 2 �� 8")]
    [SerializeField] private int _Width = 4;
    public int Width => _Width;
    [Tooltip("������ ����.  �� 2 �� 8")]
    [SerializeField] private int _Height = 4;
    public int Height => _Height;

    [Header("������ ���������� ����� ��� �����")]
    [Tooltip("�������.")]
    [SerializeField] private int _Red = 0;
    public int Red => _Red;
    [Tooltip("����������.")]
    [SerializeField] private int _Orange = 0;
    public int Orange => _Orange;
    [Tooltip("�������.")]
    [SerializeField] private int _Green = 0;
    public int Green => _Green;
    [Tooltip("������.")]
    [SerializeField] private int _Yellow = 0;
    public int Yellow => _Yellow;
    [Tooltip("����������.")]
    [SerializeField] private int _Violet = 0;
    public int Violet => _Violet;
    [Tooltip("�������.")]
    [SerializeField] private int _Blue = 5;
    public int Blue => _Blue;

    [Header("���������")]
    [Tooltip("���������� �����.")]
    [SerializeField] private int _Steps = 10;
    public int Steps => _Steps;
    [Tooltip("��������� ����� ������� �����. �� 0 �� 5")]
    [SerializeField] private int _Add = 0;
    public int Add => _Add;

    public void Generate(int level)
    {
        // ���������
        int add  = level % 5;
        int add2 = (int)(level % 100 * 0.1f);

        _Width = 8;
        _Height = 8;

        _Red = 50 + add;
        _Orange = 50 + add;
        _Green = 50 + add;
        _Yellow = 50 + add;
        _Violet = 50 + add;
        _Blue = 50 + add;

        _Steps = 50 - add2;
        _Add = 0;
    }
}