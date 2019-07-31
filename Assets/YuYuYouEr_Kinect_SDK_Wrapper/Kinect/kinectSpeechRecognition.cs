using UnityEngine;
using System.Collections;

public class kinectSpeechRecognition : MonoBehaviour {

    private string m_strResult = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(60, 40, 200, 30), "Start Speech Recognition"))
        {
            Kinect.NativeMethods.qfKinectInitSpeech(1033);
        }

        if (GUI.Button(new Rect(60, 70, 200, 30), "Stop Speech Recognition"))
        {
            Kinect.NativeMethods.qfKinectUnInitSpeech();
        }

        GUI.Button(new Rect(60, 100, 300, 30), "Speech Result: " + m_strResult);
        {
            byte[] byteResult = new byte[1024];
            if (0 == Kinect.NativeMethods.qfKinectCopySpeechReslut(byteResult))
            {
                m_strResult = System.Text.Encoding.UTF8.GetString(byteResult);
            }
        }
    }
}
