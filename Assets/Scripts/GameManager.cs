/* =======================================================================================================
 * AK Studio
 * 
 * Version 1.0 by Alexandr Kuznecov
 * 21.10.2022
 * =======================================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TinyJson;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static SGameSettings GameSettings = new();

    [Header("UI")]
    [SerializeField] private GameObject m_PanelStart = null;
    [SerializeField] private GameObject m_PanelWinner = null;
    [SerializeField] private GameObject m_PanelLose = null;
    [SerializeField] private GameObject m_ButtonLevelNext = null;
    [SerializeField] private GameObject m_ButtonBonusNext = null;

    [SerializeField] private GameObject m_menuButton = null;
    [SerializeField] private RectTransform m_GameMatrix = null;
    [SerializeField] private Image m_globalColor = null;

    [SerializeField] private Text m_TextStepsCount = null;
    [SerializeField] private Text m_TextLevelCount = null;
    [SerializeField] private Text m_TextWinCongratulation = null;
    [SerializeField] private GameObject[] m_WinStars = null;
    [SerializeField] private Text m_TextScore = null;
    [SerializeField] private Text m_TextTotalScore = null;
    [SerializeField] private Button m_ButtonBlend = null;
    [SerializeField] private GameObject m_Panel_Blend = null;

    [Header("Settings")]
    [SerializeField] private LevelCommonData LEVEL_DATA = null;
    [SerializeField] private LevelCommonData BONUS_DATA = null;
    [SerializeField] private AudioMixer m_AudioMixer = null;
    [SerializeField] private AudioSource m_Audio = null;
    [SerializeField] private GameObject m_PrefabDot = null;

    [SerializeField] private Text[] m_TextCounts = null;
    [SerializeField] private GameObject[] m_Checks = null;

    [Header("Colors")]
    [SerializeField] private Canvas rootCanvas = null;
    [SerializeField] private Material lineMaterial = null;
    [SerializeField] private Color[] colorSet = null;
    [SerializeField] private Color colorNormal = Color.gray;
    [SerializeField] private Color colorWarning = Color.red;
    [SerializeField] private Animation animationWarning = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] soundDots = null;
    [SerializeField] private AudioClip soundClosePoint = null;
    [SerializeField] private AudioClip soundShow = null;
    [SerializeField] private AudioClip soundHide = null;

    public static bool onBusy = false;
    public static bool onDown = false;
    public static int ColorCloseLine { get; private set; } = 0; // Цвет замкнувший линию.
    public static SGame Game = new();

    private bool onPause = false;
    private bool onMove = false;    

    private static GameObject currentLineObject = null;
    private static LineRenderer currentLineRenderer = null;
    private static readonly List<Vector3> linePositions = new();    

    #region Public Methods

    public static void StartLine(Dot d)
    {
        if (onDown) return;

        linePositions.Clear();
        Game.last.Clear();
        Game.last.Add(d);
        Vector3 pos = d.transform.position;
        
        currentLineObject = new GameObject();
        currentLineObject.transform.position = pos;
        currentLineRenderer = currentLineObject.AddComponent<LineRenderer>();
        currentLineRenderer.material = Instance.lineMaterial;

        currentLineRenderer.startColor = d.RGB;
        currentLineRenderer.endColor   = d.RGB;

        currentLineRenderer.startWidth = 0.1f;
        currentLineRenderer.endWidth = 0.1f;
        currentLineRenderer.numCornerVertices = 1;
        currentLineRenderer.alignment = LineAlignment.TransformZ;

        pos.z = -2;
        Instance.onMove = true;

        linePositions.Add(pos);
        linePositions.Add(pos);
        ColorCloseLine = 0;

        // Звук
        Instance.m_Audio.PlayOneShot(Instance.soundDots[0], 0.5f);
        onDown = true;
    }

    public static void AddPoint(Dot d)
    {
        Game.last.Add(d);
        Vector3 pos = d.transform.position;
        pos.z = -2;

        currentLineRenderer.positionCount++;
        linePositions[linePositions.Count - 1] = pos;
        linePositions.Add(pos);

        // Звук
        int i = (linePositions.Count > Instance.soundDots.Length) ? Instance.soundDots.Length - 1 : linePositions.Count - 1;
        Instance.m_Audio.PlayOneShot(Instance.soundDots[i], 0.5f);
    }

    public static void ClosePoint(Dot d)
    {
        linePositions[^1] = linePositions[0];
        currentLineRenderer.SetPositions(linePositions.ToArray());
        ColorCloseLine = d.ColorCode;

        // Звук
        Instance.m_Audio.PlayOneShot(Instance.soundClosePoint, 0.5f);

        // Фон
        Color c = d.RGB;
        c.a = 0.4f;
        Instance.m_globalColor.color = c;
    }

    public static void RemovePoint()
    {
        Game.last.RemoveAt(Game.last.Count - 1);
        currentLineRenderer.positionCount--;
        linePositions.RemoveAt(linePositions.Count - 1);
        ColorCloseLine = 0;

        // Фон
        Instance.m_globalColor.color = new Color(1, 1, 1, 0);
    }

    public void GameRestart()
    {
        GameStart();
    }

    public void GameNew()
    {
        GameSettings.level = 1;
        GameSettings.Save();
        GameStart();
    }

    public void GameStart()
    {
        GenerateGameData();

        onBusy = false;
        onMove = false;
        onDown = false;
        Game.last.Clear();
        Game.matrix.Clear();

        foreach(Dot d in m_GameMatrix.GetComponentsInChildren<Dot>())
        {
            Destroy(d.gameObject);
        }

        // Заполним матрицу и отрисуем.
        m_GameMatrix.sizeDelta = new Vector2(Game.width * 128, Game.height * 128);

        for (int y = 0; y < Game.height; y++)
        {
            List<Dot> arr = new();
            for (int x = 0; x < Game.width; x++)
            {
                Vector3 pos  = Vector3.zero;
                GameObject o = Instantiate(m_PrefabDot);
                Dot d        = o.GetComponent<Dot>();

                d.rectTransform.SetParent(m_GameMatrix.transform);
                d.rectTransform.localScale = Vector3.one;
                d.rectTransform.anchoredPosition = new Vector2(x * 128 + 64, -y * 128 - 64);

                int col = Game.GetRandColor();

                d.Init(col, colorSet[col - 1], x, y);
                d.OnShow();
                arr.Add(d);
            }
            Game.matrix.Add(arr);
        }

        DrawCounters();

        // Звук
        m_Audio.PlayOneShot(soundShow, 0.5f);

        m_TextStepsCount.color = colorNormal;
        m_menuButton.SetActive(true);
        m_ButtonBlend.interactable = false;
        if (animationWarning != null)
        {
            animationWarning[animationWarning.clip.name].time = 0.01f;
            animationWarning[animationWarning.clip.name].speed = 0;            
        }

        // Кнопки дальше
        if (GameSettings.runbonuslevel)
        {
            m_ButtonLevelNext.SetActive(false);
            m_ButtonBonusNext.SetActive(true);
        }
        else
        {
            m_ButtonLevelNext.SetActive(true);
            m_ButtonBonusNext.SetActive(false);
        }

        StopAllCoroutines();
    }


    public static void Pause() { Instance.GamePause(); }
    public void GamePause()
    {
        Time.timeScale = 0;
        onPause = true;
        ClearLine();
    }

    public static void UnPause() { Instance.GameUnPause(); }
    public void GameUnPause()
    {
        Time.timeScale = 1;
        onPause = false;
    }

    /// <summary>
    /// Перемешать цвета в матрице.
    /// </summary>
    public void GameBlend()
    {
        m_ButtonBlend.interactable = false;
        for (int y = 0; y < Game.matrix.Count; y++)
        {
            for (int x = 0; x < Game.matrix[y].Count; x++)
            {
                int col = Game.GetRandColor();
                Game.matrix[y][x].Init(col, colorSet[col - 1], x, y);
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// UI счетчиков
    /// </summary>
    private void DrawCounters()
    {
        m_TextStepsCount.text = Game.steps.ToString();
        m_TextLevelCount.text = $"{Game.level}";

        m_TextCounts[0].gameObject.SetActive(Game.countBlue > 0);
        m_TextCounts[0].text = $"{Game.currentBlue} / {Game.countBlue}";
        m_Checks[0].SetActive(Game.currentBlue >= Game.countBlue);

        m_TextCounts[1].gameObject.SetActive(Game.countGreen > 0);
        m_TextCounts[1].text = $"{Game.currentGreen} / {Game.countGreen}";
        m_Checks[1].SetActive(Game.currentGreen >= Game.countGreen);

        m_TextCounts[2].gameObject.SetActive(Game.countOrange > 0);
        m_TextCounts[2].text = $"{Game.currentOrange} / {Game.countOrange}";
        m_Checks[2].SetActive(Game.currentOrange >= Game.countOrange);

        m_TextCounts[3].gameObject.SetActive(Game.countRed > 0);
        m_TextCounts[3].text = $"{Game.currentRed} / {Game.countRed}";
        m_Checks[3].SetActive(Game.currentRed >= Game.countRed);

        m_TextCounts[4].gameObject.SetActive(Game.countViolet > 0);
        m_TextCounts[4].text = $"{Game.currentViolet} / {Game.countViolet}";
        m_Checks[4].SetActive(Game.currentViolet >= Game.countViolet);

        m_TextCounts[5].gameObject.SetActive(Game.countYellow > 0);
        m_TextCounts[5].text = $"{Game.currentYellow} / {Game.countYellow}";
        m_Checks[5].SetActive(Game.currentYellow >= Game.countYellow);
    }

    /// <summary>
    /// Очистить линию и массив выбранныз точек
    /// </summary>
    private void ClearLine()
    {
        linePositions.Clear();
        Game.last.Clear();
        Destroy(currentLineRenderer);
        Destroy(currentLineObject);
        onMove = false;
        onDown = false;

        // Фон
        Instance.m_globalColor.color = new Color(1, 1, 1, 0);
    }

    /// <summary>
    /// Движение точек сверхну вниз
    /// </summary>
    /// <returns>Закончилось движение</returns>
    private bool MoveDotsDown()
    {
        bool retval = false;
        for (int y = 0; y < Game.matrix.Count; y++)
        {
            for (int x = 0; x < Game.matrix[y].Count; x++)
            {
                Dot d = Game.matrix[y][x];
                if (d.OnMove)
                {
                    // Проход вниз до неподвижной точки
                    float dx = d.rectTransform.anchoredPosition.x;
                    float downY = -Game.height * 128 + 64;

                    if (y + 1 < Game.height)
                        downY = Game.matrix[y + 1][x].rectTransform.anchoredPosition.y + 128;

                    d.rectTransform.anchoredPosition = new Vector2(dx, d.rectTransform.anchoredPosition.y - 48.0f);

                    if (d.rectTransform.anchoredPosition.y < downY)
                    {
                        d.rectTransform.anchoredPosition = new Vector2(dx, downY);
                    }
                    else
                    {
                        retval = true;
                    }
                }
            }
        }
        return retval;
    }

    /// <summary>
    /// Генерировать уровень
    /// </summary>
    private void GenerateGameData()
    {
        LevelData lv = new();
        int levelNumer;

        LevelData[] CD = (GameSettings.runbonuslevel) ? BONUS_DATA.DATA : LEVEL_DATA.DATA;
        if (CD.Length >= GameSettings.level)
        {            
            lv = CD[GameSettings.level - 1];
        }
        else
        {
            lv.Generate(GameSettings.level);
        }

        levelNumer = GameSettings.level;

        Debug.Log(lv.Name);

        Game = new SGame
        {
            width = lv.Width,
            height = lv.Height,
            level = levelNumer,
            steps = lv.Steps
        };
        Game.Init();

        Game.totalSteps = lv.Steps;

        Game.countBlue = lv.Blue;
        Game.countGreen = lv.Green;
        Game.countOrange = lv.Orange;
        Game.countRed = lv.Red;
        Game.countViolet = lv.Violet;
        Game.countYellow = lv.Yellow;

        Game.levelAddColors = lv.Add;
    }

    #endregion

    #region Private MonoBehaviour Methods

    private void Awake()
    {
        // Синглтон
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Time.timeScale = 1;
        GameSettings = SGameSettings.Load();

        #region HTML5
        // Язык игры
        if (GameSettings.nosave)
        {
            string lg = GetArg("lang");
            if (lg == "ru" || lg == "en" || lg == "tr")
            {
                GameSettings.lang = lg;
            }
        }
        #endregion


        m_AudioMixer.SetFloat("MusicVolume", GameSettings.music);
        m_AudioMixer.SetFloat("EffectsVolume", GameSettings.effects);

        m_TextTotalScore.text = Translator.GetLangString("text_score").Replace("%num%", GameSettings.score.ToString());

        GameStart();
        m_PanelStart.SetActive(true);
        GamePause();
    }

    
    private void Update()
    {
        if (Time.timeScale != 1) return;

        // Двигаем линию
        if (onMove)
        {
            // Отпустили мышку
            if (Input.GetButtonUp("Fire1"))
            {
                // Если было замыкание то добавим все точки нужного цвета
                if (ColorCloseLine > 0)
                {
                    foreach (List<Dot> arr in Game.matrix)
                        foreach (Dot d in arr)
                            if(!Game.last.Contains(d) && d.ColorCode == ColorCloseLine)
                            {
                                Game.last.Add(d);
                            }
                    Game.maxDotsClosed++;
                }

                // Обработаем соединенные точки.
                if (Game.last.Count > 1)
                {
                    onBusy = true;
                    Game.steps--;

                    foreach (Dot d in Game.last)
                    {
                        // Обновим счетчики
                        switch (d.ColorCode)
                        {
                            case 1: Game.currentBlue++; break;
                            case 2: Game.currentGreen++; break;
                            case 3: Game.currentOrange++; break;
                            case 4: Game.currentRed++; break;
                            case 5: Game.currentViolet++; ; break;
                            case 6: Game.currentYellow++; ; break;
                        }
                        d.ColorCode = 0;
                        d.OnHide();
                    }

                    // Отсортируем массив с точками.
                    Game.Sort();

                    // Подсчет статистики.
                    Game.totalDots += Game.last.Count;
                    if (Game.last.Count > Game.maxDotsLine) Game.maxDotsLine = Game.last.Count;

                    // Освободим все запущенные точки.
                    StartCoroutine(CShowUpDots());

                    // Звук
                    m_Audio.PlayOneShot(soundHide, 0.5f);
                }
                ClearLine();
            }
            else
            {
                // Нажата мышь
                if (Input.GetButton("Fire1"))
                {
                    if (ColorCloseLine == 0)
                    {
                        Vector2 movePos;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                rootCanvas.transform as RectTransform,
                                Input.mousePosition,
                                rootCanvas.worldCamera,
                                out movePos);

                        Vector3 positionMove = rootCanvas.transform.TransformPoint(movePos);

                        positionMove.z = -10;

                        linePositions[^1] = positionMove;
                        currentLineRenderer.SetPositions(linePositions.ToArray());
                    }
                }
                else
                {
                    ClearLine();
                }
            }
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator CShowUpDots()
    {
        yield return new WaitForSeconds(0.2f); // 0.4f

        for (int y = 0; y < Game.matrix.Count; y++)
        {
            for (int x = 0; x < Game.matrix[y].Count; x++)
            {
                if (Game.matrix[y][x].ColorCode == 0)
                {
                    int col = Game.GetRandColor();
                    Game.matrix[y][x].Init(col, colorSet[col - 1], x, y);

                    Game.matrix[y][x].OnMove = true;
                    Game.matrix[y][x].rectTransform.anchoredPosition
                        = new Vector2(Game.matrix[y][x].rectTransform.anchoredPosition.x, 256);
                }
            }
        }        
        StartCoroutine(CWaterfall());
    }

    private IEnumerator CWaterfall()
    {
        // Падать вниз пока не попадешь в точку.
        while (MoveDotsDown())
        {
            yield return new WaitForFixedUpdate();
        }

        DrawCounters();

        onBusy = false;

        // Каждые 15 шагов включаем кнопку перемешивания.
        if(Game.totalSteps != Game.steps && Game.steps % 15 == 0) m_ButtonBlend.interactable = true;

        // Проверим матрицу на наличие ходов.
        bool allowSolution = false;
        for (int y = 0; y < Game.matrix.Count; y++)
        {
            for (int x = 0; x < Game.matrix[y].Count; x++)
            {
                if (   (y + 1 > Game.matrix.Count && Game.matrix[y][x].ColorCode == Game.matrix[y + 1][x].ColorCode)
                    || (y - 1 >= 0 && Game.matrix[y][x].ColorCode == Game.matrix[y - 1][x].ColorCode)
                    || (x + 1 > Game.matrix[y].Count && Game.matrix[y][x].ColorCode == Game.matrix[y][x + 1].ColorCode)
                    || (x - 1 >= 0 && Game.matrix[y][x].ColorCode == Game.matrix[y][x - 1].ColorCode))
                {
                    allowSolution = true;
                    break;
                }
            }
        }

        if(allowSolution == false)
        {
            m_Panel_Blend.SetActive(true);
            yield return new WaitForSeconds(2.0f);

            m_Panel_Blend.SetActive(false);
            GameBlend();
        }

        // Выиграл ли игрок?
        if ((Game.countBlue == 0      || (Game.countBlue > 0 && Game.countBlue <= Game.currentBlue)) 
            && (Game.countGreen == 0  || (Game.countGreen > 0 && Game.countGreen <= Game.currentGreen))
            && (Game.countOrange == 0 || (Game.countOrange > 0 && Game.countOrange <= Game.currentOrange))
            && (Game.countRed == 0    || (Game.countRed > 0 && Game.countRed <= Game.currentRed))
            && (Game.countViolet == 0 || (Game.countViolet > 0 && Game.countViolet <= Game.currentViolet))
            && (Game.countYellow == 0 || (Game.countYellow > 0 && Game.countYellow <= Game.currentYellow)))
        {
            StartCoroutine(CGameWin());
        }
        else
        {            
            if (Game.steps <= 5)
            {
                m_TextStepsCount.color = colorWarning;
                if (animationWarning != null)
                {
                    animationWarning[animationWarning.clip.name].speed = 1;
                    animationWarning.Play();
                }
            }
            else
            {
                m_TextStepsCount.color = colorNormal;
            }

            // Проиграл ли игрок?
            if (Game.steps <= 0)
            {
                Game.steps = 0;
                StartCoroutine(CGameLose());
            }
        }
    }

    private IEnumerator CGameWin()
    {
        yield return new WaitForFixedUpdate();

        // Случайное поздравление
        string congratulation = $"text_congratulation_{Random.Range(1, 5)}";
        m_TextWinCongratulation.text = Translator.GetLangString(congratulation);

        // Считаем количество очков количество точек всего на оставшиеся ходы
        //int score = (Game.currentBlue - Game.countBlue) + (Game.currentGreen - Game.countGreen) + (Game.currentOrange - Game.countOrange)
            //+ (Game.currentRed - Game.countRed) + (Game.currentViolet - Game.countViolet) + (Game.currentYellow - Game.countYellow);
        //score += (Game.steps * 2 + Game.maxDotsLine * 2 + Game.maxDotsClosed * 4);
        int sc = (Game.currentBlue - Game.countBlue) + (Game.currentGreen - Game.countGreen) + (Game.currentOrange - Game.countOrange)
               + (Game.currentRed - Game.countRed) + (Game.currentViolet - Game.countViolet) + (Game.currentYellow - Game.countYellow);
        sc += Game.steps * 5 + Game.maxDotsClosed * 2;

        // Считаем звезды, тупо количество оставшихся шагов. 50% от количества шагов.
        int star = 0;
        if (Game.steps > 0)
        {
            float pr = Game.steps / ((float)Game.totalSteps / 100);
            if (pr > 15) star = 1;
            if (pr > 30) star = 2;
            if (pr > 50) star = 3;
        }

        //Debug.Log(JsonUtility.ToJson(Game));
        //Debug.Log($"PR = {pr}");

        for (int i = 0; i < m_WinStars.Length; i++)
        {
            if (GameSettings.runbonuslevel)
            {
                m_WinStars[i].SetActive(i == m_WinStars.Length - 1);
            }
            else
            {
                m_WinStars[i].SetActive(i == star);
            }
        }

        GameSettings.score += sc;

        // Если не бонусный уровень.
        if (!GameSettings.runbonuslevel)
        {
            if (star > GameSettings.levelstars[GameSettings.level])
                GameSettings.levelstars[GameSettings.level] = star;

            Game.level++;

            if (Game.level > GameSettings.maxlevel)
                GameSettings.maxlevel = Game.level;

            GameSettings.level = Game.level;            
        }
        else
        {
            GameSettings.bonuscomp[GameSettings.level - 1] = 1;
        }
        
        GameSettings.Save();

        // Считаем количество звезд в зависимости от максимума точек за раз.
        m_TextScore.text = Translator.GetLangString("text_score").Replace("%num%", sc.ToString());
        m_TextTotalScore.text = Translator.GetLangString("text_score").Replace("%num%", GameSettings.score.ToString());

        GamePause();
        m_menuButton.SetActive(false);
        m_PanelWinner.SetActive(true);

        AdsManager.Instance.FirebaseEvent("player_win");
    }

    private IEnumerator CGameLose()
    {
        yield return new WaitForFixedUpdate();
        GamePause();
        m_menuButton.SetActive(false);
        m_PanelLose.SetActive(true);

        AdsManager.Instance.FirebaseEvent("player_lose");
    }

    #endregion

    #region HTML5 Events

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            AudioListener.volume = 0;
            GamePause();
        }
        else
        {
            AudioListener.volume = 1;
            if (!onPause) { GameUnPause(); }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            AudioListener.volume = 1;
            if (!onPause) { GameUnPause(); }
        }
        else
        {
            AudioListener.volume = 0;
            GamePause();
        }
    }

    public static string GetArg(string name)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
    #endregion
}

#region Structure Game

[System.Serializable]
public class SGameRoot
{
    public int width = 4;
    public int height = 4;

    public int countRed = 0;
    public int countOrange = 0;
    public int countGreen = 0;
    public int countYellow = 0;
    public int countViolet = 0;
    public int countBlue = 0;

    public int steps = 10;
    public int levelAddColors = 0; // Если 0 цвета будут только те которые должен собрать игрок, 1 добавляет один лишний цвет.
}

public class SGame : SGameRoot
{
    public int currentRed = 0;
    public int currentOrange = 0;
    public int currentGreen = 0;
    public int currentYellow = 0;
    public int currentViolet = 0;
    public int currentBlue = 0;

    public int totalSteps = 0;    
    public int level = 0;

    public int totalDots = 0; // Всего собрали точек за уровень
    public int maxDotsLine = 0; // Максимум точек в линии
    public int maxDotsClosed = 0; // Сколько точек замкнули    

    public List<List<Dot>> matrix = new();
    public List<Dot> last = new();

    /// <summary>
    /// Инициализировать класс игры
    /// </summary>
    public void Init()
    {
        countRed = 0;
        countOrange = 0;
        countGreen = 0;
        countYellow = 0;
        countViolet = 0;
        countBlue = 0;

        currentRed = 0;
        currentOrange = 0;
        currentGreen = 0;
        currentYellow = 0;
        currentViolet = 0;
        currentBlue = 0;

        totalDots = 0;
        maxDotsLine = 0;
        maxDotsClosed = 0;
        totalSteps = steps;
    }

    /// <summary>
    /// Возвращает количество цветов для сбора
    /// </summary>
    /// <returns></returns>
    public int GetRandColor()
    {
        List<int> colors = new();

        if (countBlue > 0) colors.Add(1);
        if (countGreen > 0) colors.Add(2);
        if (countOrange > 0) colors.Add(3);
        if (countRed > 0) colors.Add(4);
        if (countViolet > 0) colors.Add(5);
        if (countYellow > 0) colors.Add(6);

        for(int i = 0; i < levelAddColors; i++)
        {
            if (!colors.Contains(1)) { colors.Add(1); continue; }
            if (!colors.Contains(2)) { colors.Add(2); continue; }
            if (!colors.Contains(3)) { colors.Add(3); continue; }
            if (!colors.Contains(4)) { colors.Add(4); continue; }
            if (!colors.Contains(5)) { colors.Add(5); continue; }
            if (!colors.Contains(6)) { colors.Add(6); continue; }
        }

        return colors[Random.Range(0, colors.Count)];
    }

    /// <summary>
    /// Убранные точки выводим наверх.
    /// </summary>
    public void Sort()
    {
        for (int j = 0; j < matrix.Count; j++)
        {
            for (int y = 0; y < matrix.Count; y++)
            {
                for (int x = 0; x < matrix[y].Count; x++)
                {
                    if (y > 0 && matrix[y][x].ColorCode == 0 && matrix[y - 1][x].ColorCode > 0)
                    {
                        Dot tmp = matrix[y][x];

                        tmp.OnMove              = true;
                        matrix[y - 1][x].OnMove = true;

                        matrix[y][x]     = matrix[y - 1][x];
                        matrix[y - 1][x] = tmp;
                    }

                    matrix[y][x].Position = new V2(y, x);
                }
            }
        }
    }
}

#endregion

#region Structure Settings

/// <summary>
/// Настройки игры
/// </summary>
[System.Serializable]
public class SGameSettings
{
    public bool nosave = true;
    public string lang = "en";
    public float music = -16.5f;
    public float effects = 0.0f;
    public int score = 0;
    public int level = 1;
    public int maxlevel = 1;
    
    public int maxbonuslevel = 0;
    public bool runbonuslevel = true;

    public int[] levelstars = new int[50];
    public int[] bonuscomp = new int[5] { 0, 0, 0, 0, 0 };

    public void Save()
    {
        nosave = false;
        PlayerPrefs.SetString("GAME_DATA", this.ToJson()); 
        PlayerPrefs.Save();

        Debug.Log("== SAVE ==");
        Debug.Log(this.ToJson());
    }

    public static SGameSettings Load()
    {
        string s = PlayerPrefs.GetString("GAME_DATA", "");

        Debug.Log("== LOAD ==");
        Debug.Log(s.ToJson());
        if (s != "" && s != null)
        {
            return s.FromJson<SGameSettings>();
        }
        return new();
    }
}

#endregion