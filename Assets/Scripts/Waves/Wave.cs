using UnityEngine;
using System.Collections;

public class Wave : ScriptableObject
{
    public GameObject m_walker;
    public int m_waveEnemies;
    public float m_enemySpeed;
    public float m_timeToCreateEnemy;
    public bool m_shootAfterCreating;
    public float m_delayToShoot;
    public float m_timeToShootAfterStop;
    public float m_timeToPrepareShoot;
    public float m_timeToShootAfterEnemy;
}
