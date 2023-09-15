/* =======================================================================================================
 * AK Studio
 * 
 * Version 1.0 by Alexandr Kuznecov
 * 21.10.2022
 * =======================================================================================================
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class Dot : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private Image m_SubImage = null;

    /// <summary>
    /// Код цвета
    /// </summary>
    public int ColorCode = 0;

    /// <summary>
    /// Позиция в матрице
    /// </summary>
    public V2 Position = V2.Zero;

    /// <summary>
    /// Движение точки вниз
    /// </summary>
    public bool OnMove = false;
    
    /// <summary>
    /// Цвет в RGB
    /// </summary>
    public Color RGB {
        get { return image.color; }
        set
        {
            image.color = value;
            if (m_SubImage != null)
            {
                m_SubImage.color = value;
            }
        }
    }

    /// <summary>
    /// Класс RectTransform
    /// </summary>
    public RectTransform rectTransform = null;

    private new Animation animation = null;
    private Image image = null;
    
    private float lastInteract = -1;

    /// <summary>
    /// Активировать точку
    /// </summary>
    /// <param name="col">код цвета</param>
    /// <param name="c">цвет в RGB</param>
    /// <param name="x">позиция в матрице</param>
    /// <param name="y">позиция в матрице</param>
    public void Init(int col, Color c, int x, int y)
    {
        ColorCode = col;
        RGB = c;
        Position = new V2(y, x);
        OnMove = false;        
        lastInteract = -1;

        if (animation != null) animation.Stop();
        transform.localScale = Vector3.one;
        m_SubImage.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Скрыть точку
    /// </summary>
    public void OnHide()
    {
        if (animation != null) animation.Play("Hide_Animation");        
    }

    /// <summary>
    /// Показать точку
    /// </summary>
    /// <param name="animate">Анимировать появление</param>
    public void OnShow()
    {
        if (animation != null) animation.Play("Show_Animation");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.onBusy) return;

        if (Time.timeScale == 0) GameManager.UnPause();

        if (!GameManager.Game.last.Contains(this))
        {
            if (animation != null) animation.Play("Click_Animation");

            GameManager.StartLine(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.onBusy || GameManager.Game.last.Count < 1) return;

        if (Time.timeScale == 0) GameManager.UnPause();

        float t = Time.time;

        // Если водим мышкой с нажатой кнопкой то проверим что точка не занята уже в линии
        // Находиться ли по горизонтали или вертикали и совпадает ли цвет
        if (t - lastInteract > 0.5f && Input.GetButton("Fire1"))
        {
            bool busy = GameManager.Game.last.Contains(this);

            if (busy && GameManager.Game.last.Count > 1 && GameManager.Game.last.Last() == this)
            {
                // Открепляем точку
                GameManager.RemovePoint();
                lastInteract = t;
                return;
            }

            // Прикрепляем точку                    
            V2 p1 = GameManager.Game.last.Last().Position; //!!!!
            V2 p2 = Position;

            bool coords = (p1.y == p2.y && (p1.x == p2.x - 1 || p1.x == p2.x + 1))
                       || (p1.x == p2.x && (p1.y == p2.y - 1 || p1.y == p2.y + 1));

            if (!coords || GameManager.Game.last.Count <= 0 || GameManager.Game.last.Last().ColorCode != ColorCode)
            {
                return;
            }

            if (!busy)
            {
                GameManager.AddPoint(this);

                // Мигнуть
                if (animation != null) animation.Play("Click_Animation");


                lastInteract = t;
            }
            else if (GameManager.Game.last.First() == this
                && GameManager.Game.last.Count > 3
                && GameManager.ColorCloseLine == 0)
            {
                // Замкнуть точки
                GameManager.ClosePoint(this);

                // Мигнуть все точки.
                foreach (List<Dot> arr in GameManager.Game.matrix)
                    foreach (Dot a in arr)
                    {
                        if (a.ColorCode == ColorCode) a.animation.Play("Click_Animation");
                    }
            }
        }
    }

    private void Awake()
    {
        animation = GetComponent<Animation>();
        image = GetComponent<Image>();        
        rectTransform = GetComponent<RectTransform>();

        m_SubImage.color = Color.red;
    }
}

[System.Serializable]
public class V2
{
    public int x;
    public int y;

    public V2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static V2 Zero => new(0, 0);
    public static V2 One => new(1, 1);
}