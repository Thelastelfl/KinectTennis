using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using Kinect;

public class KinectSensor : MonoBehaviour, KinectInterface {
	//make KinectSensor a singleton (sort of)
	private static KinectInterface instance;
    public static KinectInterface Instance
    {
        get
        {
            if (instance == null)
                throw new Exception("There needs to be an active instance of the KinectSensor component.");
            return instance;
        }
        private set
        { instance = value; }
    }


    public bool m_enableMultiUser = false;
    public bool m_enableBackgroundRemoval = false;
    public bool m_enableInteractive = false;
    public bool m_enableFaceTracking = false;
    public bool m_enableSpeechReg = false;


	//场景销毁时不关闭Kinect
	public bool m_doNotShutdownKinect = false;


	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	private bool updatedSkeleton = false;
	private bool newSkeleton = false;
	[HideInInspector]
	private NuiSkeletonFrame skeletonFrame = new NuiSkeletonFrame() { SkeletonData = new NuiSkeletonData[6] };
	private Vector4[] m_skeletonPosition = null;
	private float[] m_skldata = null;
	private float[,] m_skldata_mul = null;
    private int[] m_userID = null;

	/// <summary>
	/// has User In
	/// </summary>
	private bool m_hasUserIn = false;
	private float m_hasUserIn_timeout_start = 0.0f;
	private float m_hasUserIn_timeout_MAX = 0.5f;

    private byte[] handEvent = null;
    private bool m_isLeftHandGrip = false;
    private bool m_isRightHandGrip = false;

    private byte[,] handEvent_Multi = null;
    private bool[] m_isLeftHandGrip_Multi = null;
    private bool[] m_isRightHandGrip_Multi = null;

    /// <summary>
	///variables used for updating and accessing video data 
	/// </summary>
	private bool updatedBackgroundRemoval = false;
	private bool newBackgroundRemoval = false;
	[HideInInspector]
	private Color32[] BackgroundRemovalImage;
	private Color32[] BackgroundRemovalImageTexture;
	byte[] m_BackgroundRemoval_data = null;

	/// <summary>
	///variables used for updating and accessing video data 
	/// </summary>
	private bool updatedColor = false;
	private bool newColor = false;
	private Color32[] colorImageTexture = null;
	private byte[] m_color_data = null;

	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	private bool updatedDepth = false;
	private bool newDepth = false;
	[HideInInspector]
	private short[] depthPlayerData;
	private Color32[] depthImageTexture;
	private byte[] m_depth_data = null;

	/// <summary>
	/// Image size
	/// </summary>
	private int depth_width = 640;
	private int depth_height = 480;
	private int video_width = 640;
	private int video_height = 480;

    private System.Threading.Thread m_th = null;


	void Awake()
	{
		if (KinectSensor.instance != null)
		{
			Debug.Log("There should be only one active instance of the KinectSensor component at at time.");
            throw new Exception("There should be only one active instance of the KinectSensor component at a time.");
		}
		try
        {
            int hr = NativeMethods.qfKinectInit();
            if (hr != 0)
            {
                throw new Exception("NuiInitialize Failed.");
            }

            NativeMethods.qfKinectSetEnableMultiUser(m_enableMultiUser);
            NativeMethods.qfKinectSetEnableBackgroundRemoval(m_enableBackgroundRemoval);
            NativeMethods.qfKinectSetEnableKinectInteractive(m_enableInteractive);

            NativeMethods.qfKinectSetEnableFaceTracking(m_enableFaceTracking);
            NativeMethods.qfKinectSetEnableSpeechRecognition(m_enableSpeechReg);

            ////////////////////////////////////////////////////////
            depth_width = NativeMethods.qfKinectGetDepthWidth();
            depth_height = NativeMethods.qfKinectGetDepthHeight();

            video_width = NativeMethods.qfKinectGetVideoWidth();
            video_height = NativeMethods.qfKinectGetVideoHeight();

            ////////////////////////////////////////////////////////
            //init
            BackgroundRemovalImage = new Color32[video_width * video_height];
            BackgroundRemovalImageTexture = new Color32[video_width * video_height];
            m_BackgroundRemoval_data = new byte[video_width * video_height * 4];

            colorImageTexture = new Color32[video_width * video_height];
            m_color_data = new byte[video_width * video_height * 4];

            //depthImage = new Color32[depth_width * depth_height];
            m_depth_data = new byte[depth_width * depth_height * 4];
            depthPlayerData = new short[depth_width * depth_height];

            m_skldata = new float[20 * 4];
            m_skldata_mul = new float[6, 20 * 4];
            m_userID = new int[6];

            handEvent = new byte[2];
            handEvent_Multi = new byte[6, 2];
            m_isLeftHandGrip_Multi = new bool[6];
            m_isRightHandGrip_Multi = new bool[6];

            for (int i = 0; i < skeletonFrame.SkeletonData.Length; ++i)
            {
				m_userID[i] = 0;

                skeletonFrame.SkeletonData[i].eTrackingState = Kinect.NuiSkeletonTrackingState.NotTracked;

                skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState = new NuiSkeletonPositionTrackingState[20];
                skeletonFrame.SkeletonData[i].SkeletonPositions = new Vector4[20];

                for (int i_skl = 0; i_skl < skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState.Length; ++i_skl)
                {
                    skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState[i_skl] = Kinect.NuiSkeletonPositionTrackingState.NotTracked;
                }
            }
            ////////////////////////////////////////////////////////

            KinectSensor.Instance = this;

            if (null == m_th)
            {
                m_th = new System.Threading.Thread(new System.Threading.ThreadStart(pollThread));
                m_th.Start();
            }
        }
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
	}
	
	void LateUpdate()
	{
		updatedSkeleton = false;
		newSkeleton = false;

		updatedBackgroundRemoval = false;
		updatedColor = false;
		updatedDepth = false;

		m_isLeftHandGrip = false;
		m_isRightHandGrip = false;
		
		m_skeletonPosition = null;
	}

	bool KinectInterface.hasUserIn()
	{
		return m_hasUserIn;
	}

	bool KinectInterface.pollSkeleton()
	{
		bool bRet = false;

		if (Kinect.NativeMethods.qfKinectGetEnableMultiUser())
		{
			bRet = pollSkeleton_multiUser();
		}
		else
		{
			bRet = pollSkeleton_1user();
		}

		if (bRet)
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

		return bRet;
	}
	

	bool pollSkeleton_1user()
	{
		if (!updatedSkeleton)
		{
			updatedSkeleton = true;
			
			float[] skldata = m_skldata;
			if (null != skldata)
			{
				int hr = NativeMethods.qfKinectCopySkeletonData(skldata);
				if(hr == 0)
				{
					newSkeleton = true;
				}
				
				int i_skldata = 0;
				
				for (int i=0; i<Kinect.Constants.NuiSkeletonCount; ++i)
				{
					skeletonFrame.liTimeStamp = (long)(Time.time * 1000);

					if (i == 0)
					{//one user
						skeletonFrame.SkeletonData[i].eTrackingState =
							newSkeleton ? Kinect.NuiSkeletonTrackingState.SkeletonTracked
										: Kinect.NuiSkeletonTrackingState.NotTracked;
						
						for (int i_skl=0
								; i_skl<skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState.Length
								; ++i_skl, i_skldata+=4)
						{
							skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState[i_skl] =
									newSkeleton ? Kinect.NuiSkeletonPositionTrackingState.Tracked
												: Kinect.NuiSkeletonPositionTrackingState.NotTracked;
							
							skeletonFrame.SkeletonData[i].SkeletonPositions[i_skl] =
									new Vector4(skldata[i_skldata+0], skldata[i_skldata+1], skldata[i_skldata+2], skldata[i_skldata+3]);
						}
					}
					else
					{
						skeletonFrame.SkeletonData[i].eTrackingState = Kinect.NuiSkeletonTrackingState.NotTracked;
					}
				}
				
				//default user
				m_skeletonPosition = skeletonFrame.SkeletonData[0].SkeletonPositions;
			}

			if (null != handEvent)
			{
				//handEvent
				for (int i=0; i<handEvent.Length; ++i)
				{
					handEvent[i] = 0;
				}
				
				try {
					NativeMethods.qfKinectCopyHandEventReslut(handEvent);
				} catch (Exception ex) {
					Debug.Log(ex);
				}
				
				m_isLeftHandGrip = (handEvent[0] == 1);
				m_isRightHandGrip = (handEvent[1] == 1);

				/////////////////////////
				/// clear multi handEvent data
				for (int i = 0; i < 6; ++i)
				{
					for (int j=0; j<2; ++j)
					{
						handEvent_Multi[i,j] = 0;
					}
				}
				
				for (int i = 0; i < 6; ++i)
				{
					m_isLeftHandGrip_Multi[i] = (handEvent_Multi[i,0] == 1);
					m_isRightHandGrip_Multi[i] = (handEvent_Multi[i,1] == 1);
				}

				//fill first user of multi
				handEvent_Multi[0, 0] = handEvent[0];
				handEvent_Multi[0, 1] = handEvent[1];
				m_isLeftHandGrip_Multi[0] = m_isLeftHandGrip;
				m_isRightHandGrip_Multi[0] = m_isRightHandGrip;
			}

		}
		
		return newSkeleton;
	}

	bool pollSkeleton_multiUser()
    {
        if (!updatedSkeleton)
        {
            updatedSkeleton = true;

            float[,] skldata = m_skldata_mul;
            int[] userID = m_userID;

            if (null != skldata)
            {
                int hr = NativeMethods.qfKinectCopyMultiSkeletonData(skldata, userID);
                if (hr == 0)
                {
                    newSkeleton = true;
                }


                for (int i = 0; i < Kinect.Constants.NuiSkeletonCount; ++i)
                {
                    skeletonFrame.liTimeStamp = (long)(Time.time * 1000);

                    skeletonFrame.SkeletonData[i].eTrackingState =
                        (newSkeleton && (userID[i]!=0)) ? Kinect.NuiSkeletonTrackingState.SkeletonTracked
                                    : Kinect.NuiSkeletonTrackingState.NotTracked;

					//Debug.Log("userID[ " + i + " ] = " + userID[i]);
					//skeletonFrame.SkeletonData[i].dwTrackingID = (uint)userID[i];
					skeletonFrame.SkeletonData[i].dwUserIndex = (uint)userID[i];

                    int i_skldata = 0;
                    for (int i_skl = 0
                            ; i_skl < skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState.Length
                            ; ++i_skl, i_skldata += 4)
                    {
                        skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState[i_skl] =
                                (newSkeleton && (userID[i] != 0)) ? Kinect.NuiSkeletonPositionTrackingState.Tracked
                                            : Kinect.NuiSkeletonPositionTrackingState.NotTracked;

                        skeletonFrame.SkeletonData[i].SkeletonPositions[i_skl] =
                                new Vector4(skldata[i, i_skldata + 0]
                                                , skldata[i, i_skldata + 1]
                                                , skldata[i, i_skldata + 2]
                                                , skldata[i, i_skldata + 3]
                                            );
                    }
                }

            }

            if (null != handEvent_Multi)
            {
                //handEvent
                for (int i = 0; i < 6; ++i)
                {
                    for (int j=0; j<2; ++j)
                    {
                        handEvent_Multi[i,j] = 0;
                    }
                }

                try
                {
                    NativeMethods.qfKinectCopyMultiHandEventReslut(handEvent_Multi);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                for (int i = 0; i < 6; ++i)
                {
                    m_isLeftHandGrip_Multi[i] = (handEvent_Multi[i,0] == 1);
                    m_isRightHandGrip_Multi[i] = (handEvent_Multi[i,1] == 1);
                }
            }

        }

        return newSkeleton;
    }
    
    NuiSkeletonFrame KinectInterface.getSkeleton()
    {
		return skeletonFrame;
	}
	
	bool KinectInterface.getIsLeftHandGrip(int i)
	{
        if (i >= 0 && i < 6)
        {
            return m_isLeftHandGrip_Multi[i];
        }

        return false;
	}
	
	bool KinectInterface.getIsRightHandGrip(int i)
	{
        if (i >= 0 && i < 6)
        {
            return m_isRightHandGrip_Multi[i];
        }

        return false;
    }

	Vector4[] KinectInterface.getSkeleton_defaultUser()
	{
		return m_skeletonPosition;
	}
	
	bool KinectInterface.getIsLeftHandGrip_defaultUser()
	{
		return m_isLeftHandGrip;
	}
	
	bool KinectInterface.getIsRightHandGrip_defaultUser()
	{
		return m_isRightHandGrip;
	}

	bool KinectInterface.pollColor()
	{
		if (!updatedColor)
		{
			updatedColor = true;

            m_need_pollColor = true;
		}

		return true;
	}
	
	Color32[] KinectInterface.getColor() {
        if (newColor)
        {
            newColor = false;
            return colorImageTexture;
        }

        return null;
	}
	

	bool KinectInterface.pollDepth()
	{
		if (!updatedDepth)
		{
			updatedDepth = true;

            m_need_pollDepth = true;
		}
		
		return true;
	}
	
	short[] KinectInterface.getDepth(){
        if (newDepth)
        {
            newDepth = false;
            return depthPlayerData;
        }

        return null;
	}

	private short[] extractDepthImage(byte[] buf)
	{
		short[] newbuf = depthPlayerData;
		for (int i=0, i_byte=0; i<newbuf.Length; ++i, i_byte+=4)
		{
			newbuf[i] = (short)((buf[i_byte+1] << 8) | buf[i_byte+0]);
		}
		
		return newbuf;
	}
	
	bool KinectInterface.pollBackgroundRemoval()
	{
		if (!updatedBackgroundRemoval)
		{
			updatedBackgroundRemoval = true;

            m_need_pollBackgroundRemoval = true;
		}

		return true;
	}
	
	Color32[] KinectInterface.getBackgroundRemoval(){
		return BackgroundRemovalImage;
	}
	
	Color32[] KinectInterface.getBackgroundRemovalTexture() {
        if (newBackgroundRemoval)
        {
            newBackgroundRemoval = false;
            return BackgroundRemovalImageTexture;
        }

        return null;
	}

    void OnDestroy()
    {
        KinectSensor.instance = null;
        m_pollThread_run = false;

		if (m_doNotShutdownKinect)
		{
			NativeMethods.qfKinectUnInit();
		}
    }

    void OnApplicationQuit()
    {
        m_pollThread_run = false;

        NativeMethods.qfKinectUnInit();
    }

    /////////////////////////////////////////////////
	/////////////////////////////////////////////////
	/////////////////////////////////////////////////
	/////////////////////////////////////////////////
	/////////////////////////////////////////////////
	/////////////////////////////////////////////////
	private bool m_pollThread_run = true;

    private bool m_need_pollColor = false;
    private bool m_need_pollDepth = false;
    private bool m_need_pollBackgroundRemoval = false;
    public void pollThread()
    {
        while (m_pollThread_run)
        {
			System.Threading.Thread.Sleep(30);

            if (m_need_pollColor)
            {
                byte[] data = m_color_data;
                if (null != data)
                {
                    int hr = NativeMethods.qfKinectCopyVideoData(data);
                    if (hr == 0)
                    {
                        qfOpenCV.bgraToColor32(m_color_data, video_width, video_height, 4, colorImageTexture);

                        m_need_pollColor = false;
                        newColor = true;
                    }
                }

            }

            if (m_need_pollDepth)
            {
                byte[] depth_data = m_depth_data;
                if (null != depth_data)
                {
                    int hr = NativeMethods.qfKinectCopyDepthData(depth_data);
                    if (hr == 0)
                    {
                        depthPlayerData = extractDepthImage(depth_data);

                        m_need_pollDepth = false;
                        newDepth = true;
                    }
                }
            }

            if (m_need_pollBackgroundRemoval && NativeMethods.qfKinectGetEnableBackgroundRemoval())
            {
                byte[] data = m_BackgroundRemoval_data;
                if (null != data)
                {
                    int hr = NativeMethods.qfKinectCopyBackgroundRemovalData(data);
                    if (hr == 0)
                    {
                        qfOpenCV.bgraToColor32(data, video_width, video_height, 4, BackgroundRemovalImageTexture);

                        m_need_pollBackgroundRemoval = false;
                        newBackgroundRemoval = true;
                    }
                }
            }
        }

        Debug.Log("pollThread EXIT SUCC");
    }
}

