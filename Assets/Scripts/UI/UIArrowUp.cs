using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIArrowUp : MonoBehaviour
{
    public string m_text;

    public Text m_textMesh;

	// Use this for initialization
	void Start () {
        m_textMesh.text = m_text;
	}
	
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
