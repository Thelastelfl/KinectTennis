using UnityEngine;
using System.Collections;

public class skeletonToVideo : MonoBehaviour {

    public Transform m_cornor_left_top;
    public Transform m_cornor_right_bottom;

    public DeviceOrEmulator devOrEmu;
    private Kinect.KinectInterface kinect;

    public GameObject Hip_Center;
    public GameObject Spine;
    public GameObject Shoulder_Center;
    public GameObject Head;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;

    private GameObject[] _bones; //internal handle for the bones of the model
    private Vector3[] m_skl_InColorPoint = null;
    float[] positionXYZW = new float[4];
    int[] plColorX = new int[1];
    int[] plColorY = new int[1];

    public enum BoneMask
    {
        None = 0x0,
        Hip_Center = 0x1,
        Spine = 0x2,
        Shoulder_Center = 0x4,
        Head = 0x8,
        Shoulder_Left = 0x10,
        Elbow_Left = 0x20,
        Wrist_Left = 0x40,
        Hand_Left = 0x80,
        Shoulder_Right = 0x100,
        Elbow_Right = 0x200,
        Wrist_Right = 0x400,
        Hand_Right = 0x800,
        Hip_Left = 0x1000,
        Knee_Left = 0x2000,
        Ankle_Left = 0x4000,
        Foot_Left = 0x8000,
        Hip_Right = 0x10000,
        Knee_Right = 0x20000,
        Ankle_Right = 0x40000,
        Foot_Right = 0x80000,
        All = 0xFFFFF,
        Torso = 0x10000F, //the leading bit is used to force the ordering in the editor
        Left_Arm = 0x1000F0,
        Right_Arm = 0x100F00,
        Left_Leg = 0x10F000,
        Right_Leg = 0x1F0000,
        R_Arm_Chest = Right_Arm | Spine,
        No_Feet = All & ~(Foot_Left | Foot_Right)
    }

    //public int player;
    public BoneMask Mask = BoneMask.All;

	// Use this for initialization
	void Start () {
        kinect = devOrEmu.getKinect();

        _bones = new GameObject[(int)Kinect.NuiSkeletonPositionIndex.Count] {Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right};

        m_skl_InColorPoint = new Vector3[(int)Kinect.NuiSkeletonPositionIndex.Count];
	}
	
	// Update is called once per frame
	void Update () {
        Vector4[] skldata = kinect.getSkeleton_defaultUser();

        if (null != skldata && skldata.Length >= (int)Kinect.NuiSkeletonPositionIndex.Count)
        {
            float videoWidth = Kinect.NativeMethods.qfKinectGetVideoWidth();
            float videoHeight = Kinect.NativeMethods.qfKinectGetVideoHeight();

            Vector3 corner_range = m_cornor_right_bottom.localPosition - m_cornor_left_top.localPosition;


            for (int ii = 0; ii < (int)Kinect.NuiSkeletonPositionIndex.Count; ii++)
            {
                if (((uint)Mask & (uint)(1 << ii)) > 0)
                {
                    positionXYZW[0] = skldata[ii].x;
                    positionXYZW[1] = skldata[ii].y;
                    positionXYZW[2] = skldata[ii].z;
                    positionXYZW[3] = skldata[ii].w;

                    if (0 <= Kinect.NativeMethods.qfKinectTransformSkeletonToVideoImage(positionXYZW, plColorX, plColorY))
                    {
                        m_skl_InColorPoint[ii].x = (plColorX[0] / videoWidth - 0.5f) * corner_range.x;
                        m_skl_InColorPoint[ii].y = (plColorY[0] / videoHeight - 0.5f) * corner_range.y;
                        m_skl_InColorPoint[ii].z = 0.0f;

                        _bones[ii].transform.localPosition = m_skl_InColorPoint[ii];
                    }

                }
            }
        }

	}
}

