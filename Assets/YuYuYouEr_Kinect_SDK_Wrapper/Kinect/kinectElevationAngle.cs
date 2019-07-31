using UnityEngine;
using System.Collections;

public class kinectElevationAngle : MonoBehaviour {

    private int m_slider_value = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200.0f, 30), "设备仰俯角度:");
        m_slider_value = Mathf.RoundToInt(GUI.VerticalSlider(new Rect(10, 40, 60.0f, Screen.height * 0.3f), (float)m_slider_value, -27.0f, 27.0f));
        if (Input.mousePresent
            && (Input.GetMouseButtonUp(0)
                || Input.GetMouseButtonUp(2)
                )
            )
        {
            Kinect.NativeMethods.setElevationAngle(m_slider_value);
        }
    }
}
