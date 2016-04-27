using UnityEngine;
using System.Collections;

public class AutoDestroyBase : MonoBehaviour
{
    public float timeToDestroy = 2f;

    // Use this for initialization
    void Start()
    {
        Invoke("Destroy", timeToDestroy);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

}
