using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    public GameObject[] objects;

    public int objectsToExplode = 15;

    public float force = 3000f;

    private static Explosion _instance;
    public static Explosion Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Explosion>();
            }
            return _instance;
        }
    }
    
	
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Explode(transform.position);
        }
    }

    public void Explode(Vector3 position)
    {
        for (int i = 0; i < objectsToExplode; i++)
        {
            GameObject g = Instantiate(GetRandomObject(), position, transform.rotation) as GameObject;
            float rand = Random.Range(0f, 1f);

            if (rand > 0.25f)
            {
                g.GetComponent<Rigidbody>().AddForce(Vector3.up * 3000f * Time.deltaTime);
            }
            else if (rand > 0.5f)
            {
                g.GetComponent<Rigidbody>().AddForce(Vector3.right * 3000f * Time.deltaTime);
            }
            else if (rand > 0.75f)
            {
                g.GetComponent<Rigidbody>().AddForce(-Vector3.right * 3000f * Time.deltaTime);
            }
        }
    }
    
    private GameObject GetRandomObject()
    {
        return objects[Random.Range(0,objects.Length -1)];
    }
}
