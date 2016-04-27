using UnityEngine;
using System.Collections;

public class WalkerSplineBase : MonoBehaviour {

    private Vector3[] _splinePositions;
    private int _count;
    private float _speed;
    private SplineManager _manager;

    public ThrowBall m_throwBalll;

    public void Setup(Vector3[] positions, float speed, SplineManager manager)
    {
        _manager = manager;
        _speed = speed;
        _splinePositions = positions;
        _count = 0;
        transform.position = _splinePositions[_count];
    }

    public void UpdatePosition()
    {
        if (transform.position == _splinePositions[_count])
        {
            _count++;
            if (_count >= _splinePositions.Length)
            {
                _count = 0;
            }

        }
        
        transform.position = Vector3.MoveTowards(transform.position, _splinePositions[_count], Time.deltaTime * _speed);

        transform.LookAt(Camera.main.transform);
    }
    
    public void StartShoot(float delay, GameObject ball, Transform target)
    {
        StartCoroutine(PrepareToShoot(delay, ball, target));
    }

    protected virtual IEnumerator PrepareToShoot(float delay, GameObject ball, Transform target)
    {
        yield return new WaitForSeconds(delay);
        Shoot(ball, target);
    }

    protected virtual void Shoot(GameObject ball, Transform target)
    {
        m_throwBalll.BallThrow(ball, target);
    }

    public void Destroy()
    {
        _manager.RemoveReference(this);
        Destroy(gameObject);
    }
}
