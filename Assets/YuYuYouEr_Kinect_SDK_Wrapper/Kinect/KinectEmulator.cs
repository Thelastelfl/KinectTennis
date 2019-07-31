using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Kinect;

public class KinectEmulator : MonoBehaviour, KinectInterface {

    private string inputFile = "/../YuYuYouEr_Kinect_Data/Recordings/playback";
    private string inputFileDefault = "/YuYuYouEr_Kinect_Data/Recordings/playbackDefault";
	private float playbackSpeed = 0.0333f;
	private float timer = 0;
	private bool isDefault = true;

    private bool m_hasUserIn = false;
    private float m_hasUserIn_timeout_start = 0.0f;
    private float m_hasUserIn_timeout_MAX = 0.5f;

	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	private bool newSkeleton = false;
	private int curFrame = 0;
	private NuiSkeletonFrame[] skeletonFrame;
	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	//private bool updatedColor = false;
	//private bool newColor = false;
	//private Color32[] colorImage;
	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	//private bool updatedDepth = false;
	//private bool newDepth = false;
	//private short[] depthPlayerData;
	
	
	// Use this for initialization
	void Start () {
        inputFile = Application.dataPath + inputFile;
        inputFileDefault = Application.dataPath + inputFileDefault;

		LoadPlaybackFile(inputFileDefault);
	}
	
	void Update () {
		timer += Time.deltaTime;
		if(Input.GetKeyUp(KeyCode.F12)) {
			if(isDefault) {
				isDefault = false;
				LoadPlaybackFile(inputFile);
			}
			else {
				isDefault = true;
				LoadPlaybackFile(inputFileDefault);
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		newSkeleton = false;
	}
	
	void LoadPlaybackFile(string filePath)  {
		FileStream input = new FileStream(@filePath, FileMode.Open);
		BinaryFormatter bf = new BinaryFormatter();
		SerialSkeletonFrame[] serialSkeleton = (SerialSkeletonFrame[])bf.Deserialize(input);
		skeletonFrame = new NuiSkeletonFrame[serialSkeleton.Length];
		for(int ii = 0; ii < serialSkeleton.Length; ii++){
			skeletonFrame[ii] = serialSkeleton[ii].deserialize();
		}
		input.Close();
		timer = 0;
		Debug.Log("Simulating "+@filePath);
	}

	bool KinectInterface.hasUserIn()
	{
        return m_hasUserIn;
	}

	bool KinectInterface.pollSkeleton() {
		int frame = Mathf.FloorToInt(Time.realtimeSinceStartup / playbackSpeed);
		if(frame > curFrame){
			curFrame = frame;
			newSkeleton = true;
		}

        if (newSkeleton)
        {//got user
            m_hasUserIn_timeout_start = Time.time;

            if (!m_hasUserIn)
            {//last frame has NO user
                m_hasUserIn = true;
            }
        }
        else if (m_hasUserIn)
        {//last frame has user
            if ((Time.time - m_hasUserIn_timeout_start) > m_hasUserIn_timeout_MAX)
            {
                m_hasUserIn = false;
            }
        }

        return newSkeleton;
	}
	
	NuiSkeletonFrame KinectInterface.getSkeleton() {
		return skeletonFrame[curFrame % skeletonFrame.Length];
	}

    bool KinectInterface.getIsLeftHandGrip(int i)
    {
		return false;
    }

    bool KinectInterface.getIsRightHandGrip(int i)
    {
		return false;
    }

	Vector4[] KinectInterface.getSkeleton_defaultUser()
	{
		return null;
	}
	
	bool KinectInterface.getIsLeftHandGrip_defaultUser()
	{
		return false;
	}
	
	bool KinectInterface.getIsRightHandGrip_defaultUser()
	{
		return false;
	}

	bool KinectInterface.pollBackgroundRemoval() {
		return false;
	}
	
	Color32[] KinectInterface.getBackgroundRemoval() {
		return null;
	}
	
	Color32[] KinectInterface.getBackgroundRemovalTexture() {
		return null;
	}
	
	bool KinectInterface.pollColor() {
		return false;
	}
	
	Color32[] KinectInterface.getColor() {
		return null;
	}
	
	bool KinectInterface.pollDepth() {
		return false;
	}
	
	short[] KinectInterface.getDepth() {
		return null;
	}
}
