using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ComboManager : MonoBehaviour
{
    public float timeDown = 5f;
    public Image m_image;
    public Text m_text;

    private int count = 0;
    private float time = 0;

    private static ComboManager _instance;
    public static ComboManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ComboManager>();
            }

            return _instance;
        }
    }

    void Start()
    {
        //gameObject.SetActive(false);
        //UpdateText();
        _array = new List<GameObject>();
        _pos = new List<Vector3>();
    }

    private List<GameObject> _array;
    private List<Vector3> _pos;

    public void ShowCombo(Vector3 pos, GameObject g)
    {
        _array.Add(g);
        _pos.Add(pos);
        gameObject.SetActive(true);
        count++;
        time = timeDown;
        m_text.text = count.ToString();
        transform.position = pos;
    }

    void Update()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;
            m_image.fillAmount = time/timeDown;
        }
        else if(count > 0)
        {
            LastStep();
        }
    }

    private void LastStep()
    {
        //_array[_array.Count - 1].SetActive(true);
        transform.position = _pos[_pos.Count - 1];
        //_array.RemoveAt(_array.Count - 1);
        _pos.RemoveAt(_pos.Count - 1);
        count--;
        time = timeDown;
        m_text.text = count.ToString();
    }
}
