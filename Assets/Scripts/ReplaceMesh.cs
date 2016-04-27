using UnityEngine;
using System.Collections;

public class ReplaceMesh : MonoBehaviour
{
    public GameObject m_instanceToCreate;
    private ArrayList _messhes;

    public Material[] m_materials;

	// Use this for initialization
	void Start ()
    {
        Init();
	}


    private void Init()
    {
        GetMeshes();
        ReplaceMeshes();
    }

    private void GetMeshes()
    {
        _messhes = new ArrayList();
        foreach(MeshRenderer m in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            _messhes.Add(m);
        }
    }

    private void ReplaceMeshes()
    {
        GameObject container = GameObject.Find("ExportedObjects_" + gameObject.name);

        if(container == null)
        {
            container = new GameObject("ExportedObjects_" + gameObject.name);
        }

        foreach (MeshRenderer m in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            GameObject g = Instantiate(m_instanceToCreate) as GameObject;
            g.transform.SetParent(container.transform);
            g.transform.position = m.transform.position;
            g.transform.localScale = m.transform.localScale;
            g.gameObject.name = m.gameObject.name;
            m.enabled = false;
            CreateMaterial(m,g);
        }
    }

    private void CreateMaterial(MeshRenderer m, GameObject g)
    {
        Color c = m.sharedMaterial.GetColor("_Color");
        Debug.Log(c);
        //m.sharedMaterials[0].SetColor("_Color", ColorCompare(c));
        g.GetComponent<MeshRenderer>().material = ColorCompare(c);
    }
	
    private Material ColorCompare(Color a)
    {
        float val = 0f;

        float finalVal = -1f;
        Material finalColor = null;

        foreach (Material mat in m_materials)
        {
            Color colorBase = mat.GetColor("_Color");

            float r = Mathf.Abs(a.r - colorBase.r);
            float g = Mathf.Abs(a.g - colorBase.g);
            float b = Mathf.Abs(a.b - colorBase.b);
            
            val = r + g + b;
            
            if(finalVal != -1)
            {
                if(val < finalVal)
                {
                    finalVal = val;
                    finalColor = mat;
                }
            }
            else
            {
                finalVal = val;
                finalColor = mat;
            }
        }


        return finalColor;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
