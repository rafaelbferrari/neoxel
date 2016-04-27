using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private LevelStep[] _levelSteps;
    private Dictionary<int, int> _levelConfig;
    private Dictionary<int, int> _currentLevelConfig;
    private int _combo = 0;

    public float m_startRangeDelayToThrow = 1f;
    public float m_endRangeDelayToThrow = 3f;

    public float m_ligthTimeToThrow = 1f;

    public GameObject m_combo;
    
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    void Start()
    {
        LoadLevelSteps();
    }

    public bool IsMyStep(int step)
    {
        return currentStep == step;
    }
    
    public void AddDestroyStep(int step)
    {
        if (!_currentLevelConfig.ContainsKey(step))
        {
            _currentLevelConfig.Add(step, 0);
        }
        _currentLevelConfig[step]++;

        if(_currentLevelConfig[step] >= _levelConfig[step])
        {
            currentStep++;
        }
    }

    public int currentStep = 1;

    private void LoadLevelSteps()
    {
        _levelSteps = GameObject.FindObjectsOfType<LevelStep>();
        _levelConfig = new Dictionary<int, int>();

        foreach(LevelStep l in _levelSteps)
        {
            if(_levelConfig.ContainsKey(l.levelStep))
            {
                _levelConfig[l.levelStep]++;
            }
            else
            {
                _levelConfig.Add(l.levelStep, 1);
            }
        }
    }
    
    public void AddCombo(Vector3 v)
    {
        _combo++;
        GameObject g = Instantiate(m_combo) as GameObject;
        g.transform.position = v;
        g.GetComponent<UIArrowUp>().m_text = _combo.ToString();
    }
}
