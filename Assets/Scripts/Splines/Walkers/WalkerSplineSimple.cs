using UnityEngine;
using System.Collections;

public class WalkerSplineSimple : WalkerSplineBase
{
    
    private Light _ligth;
    
    void Start()
    {
        _ligth = GetComponentInChildren<Light>();
        _ligth.intensity = 0;
        _ligth.enabled = false;
    }
    
    protected override IEnumerator PrepareToShoot(float delay, GameObject ball, Transform target)
    {
        if (_ligth != null)
        {
            _ligth.enabled = true;
            float time = 0;
            while (time < delay)
            {
                time += Time.deltaTime;
                _ligth.intensity = 8f * (time / delay);
                yield return new WaitForEndOfFrame();
            }
            _ligth.enabled = false;
        }
        Shoot(ball, target);
    }

}
