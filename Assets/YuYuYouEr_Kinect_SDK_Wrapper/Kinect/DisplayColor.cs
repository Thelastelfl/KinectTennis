using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class DisplayColor : MonoBehaviour {
	
	public DeviceOrEmulator devOrEmu;
	private Kinect.KinectInterface kinect;
	
	private Texture2D tex = null;

	private int video_width = 640;
	private int video_height = 480;

	// Use this for initialization
	void Start () {
		kinect = devOrEmu.getKinect();

		video_width = Kinect.NativeMethods.qfKinectGetVideoWidth();
		video_height = Kinect.NativeMethods.qfKinectGetVideoHeight();

		tex = new Texture2D(video_width, video_height, TextureFormat.ARGB32, false);
		GetComponent<Renderer>().material.mainTexture = tex;
	}
	
	// Update is called once per frame
	void Update () {
		if (kinect.pollColor())
		{
            Color32[] c = kinect.getColor();
            if (null != c)
            {
                tex.SetPixels32(c);
                tex.Apply(false);
            }
		}
	}
}
