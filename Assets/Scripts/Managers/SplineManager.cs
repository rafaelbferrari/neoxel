using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplineManager : MonoBehaviour
{
    [SerializeField]
    public WalkerConfig[] m_walkers;
    public GameObject _playerTarget;

    private int _wave;
    private List<WalkerSplineBase> _walkers;
    private bool _stopWalkers;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _wave = 0;
        StartCoroutine(NextWave(m_walkers[_wave]));
    }

    private void NextWave()
    {
        StopAllCoroutines();

        _wave++;

        if (_wave >= m_walkers.Length) _wave = 0;

        StartCoroutine(NextWave(m_walkers[_wave]));

    }

    IEnumerator NextWave(WalkerConfig config)
    {
        _walkers = new List<WalkerSplineBase>();
        for (int i = 0; i < config.m_wave.m_waveEnemies; i++)
        {
            _walkers.Add(CreateBaseEnemy(ref config));
            yield return new WaitForSeconds(config.m_wave.m_timeToCreateEnemy);
        }
        StartCoroutine(StopAndShoot(config, config.m_wave.m_shootAfterCreating));
    }
    
    IEnumerator StopAndShoot(WalkerConfig config, bool m_shootAfterCreating)
    {
        if(!m_shootAfterCreating)
            yield return new WaitForSeconds(config.m_wave.m_delayToShoot);

        _stopWalkers = true;

        yield return new WaitForSeconds(config.m_wave.m_timeToShootAfterStop);

        for (int i = 0; i < _walkers.Count; i++)
        {
            _walkers[i].StartShoot(config.m_wave.m_timeToPrepareShoot, config.m_ball, _playerTarget.transform);
            yield return new WaitForSeconds(config.m_wave.m_timeToPrepareShoot);
            yield return new WaitForSeconds(config.m_wave.m_timeToShootAfterEnemy);
        }
        _stopWalkers = false;
        StartCoroutine(StopAndShoot(config, false));
    }

    private WalkerSplineBase CreateBaseEnemy(ref WalkerConfig config)
    {
        GameObject g = Instantiate(config.m_wave.m_walker) as GameObject;
        WalkerSplineBase splineBase = g.GetComponent<WalkerSplineBase>();
        splineBase.Setup(config.m_spline.GetSplinePositions().ToArray(), config.m_wave.m_enemySpeed, this);
        return splineBase;
    }

    public void RemoveReference(WalkerSplineBase walker)
    {
        _walkers.Remove(walker);
        if(_walkers.Count == 0)
        {
            NextWave();
        }
    }

    void LateUpdate()
    {
        if(_walkers != null && !_stopWalkers)
        {
            for(int i = 0; i < _walkers.Count; i++)
            {
                if (_walkers[i] != null)
                    _walkers[i].UpdatePosition();
                else
                    _walkers.RemoveAt(i);
            }
        }
    }
}

[System.Serializable]
public class WalkerConfig
{
    public CatmullRomSpline m_spline;
    public Wave m_wave;
    public GameObject m_ball;
    /*public GameObject m_walker;
    public int m_waveEnemies;
    public float m_enemySpeed;
    public float m_timeToCreateEnemy;
    public bool m_shootAfterCreating;
    public float m_delayToShoot;
    public float m_timeToShootAfterStop;
    public float m_timeToPrepareShoot;
    public float m_timeToShootAfterEnemy;*/
}