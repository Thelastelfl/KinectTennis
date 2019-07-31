using UnityEngine;
using System.Collections;

public class changeColor : MonoBehaviour {
    public Color m_color_enter;
    public Color m_color_exit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnEnter()
    {
        GameObject.Find("Plane_KinectBackgroundRemoval_bg").GetComponent<Renderer>().material.color = m_color_enter;
    }

    public void OnExit()
    {
        GameObject.Find("Plane_KinectBackgroundRemoval_bg").GetComponent<Renderer>().material.color = m_color_exit;
    }
}
