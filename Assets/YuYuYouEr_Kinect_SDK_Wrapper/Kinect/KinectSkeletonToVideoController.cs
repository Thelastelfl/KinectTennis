using UnityEngine;
using System.Collections;

public class KinectSkeletonToVideoController : MonoBehaviour {

    public KinectUser_EventListener_Impl_default m_sw;

    public Transform m_cornor_left_top = null;
    public Transform m_cornor_right_bottom = null;

	public Transform m_skeletonContainer = null;

    public GameObject Hip_Center = null;
    public GameObject Spine = null;
    public GameObject Shoulder_Center = null;
    public GameObject Head = null;
    public GameObject Shoulder_Left = null;
    public GameObject Elbow_Left = null;
    public GameObject Wrist_Left = null;
    public GameObject Hand_Left = null;
    public GameObject Shoulder_Right = null;
    public GameObject Elbow_Right = null;
    public GameObject Wrist_Right = null;
    public GameObject Hand_Right = null;
    public GameObject Hip_Left = null;
    public GameObject Knee_Left = null;
    public GameObject Ankle_Left = null;
    public GameObject Foot_Left = null;
    public GameObject Hip_Right = null;
    public GameObject Knee_Right = null;
    public GameObject Ankle_Right = null;
    public GameObject Foot_Right = null;

	public int m_userIndex = 0;

	private GameObject[] _bones = null; //internal handle for the bones of the model
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

        _bones = new GameObject[(int)Kinect.NuiSkeletonPositionIndex.Count] {Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right};

        m_skl_InColorPoint = new Vector3[(int)Kinect.NuiSkeletonPositionIndex.Count];
	}
	
	// Update is called once per frame
	void Update () {
		KinectUser[] kuser = m_sw.getCurrentUser();
		if (null != kuser
			&& kuser.Length > 0
			&& m_userIndex < kuser.Length
			&& null != kuser[m_userIndex]
			)
		{
			m_skeletonContainer.gameObject.SetActive(true);

			float videoWidth = Kinect.NativeMethods.qfKinectGetVideoWidth();
            float videoHeight = Kinect.NativeMethods.qfKinectGetVideoHeight();

            Vector3 corner_range = m_cornor_right_bottom.localPosition - m_cornor_left_top.localPosition;

			Vector3[] skldata = kuser[m_userIndex].m_bone;

            for (int ii = 0; ii < (int)Kinect.NuiSkeletonPositionIndex.Count; ii++)
            {
                if (((uint)Mask & (uint)(1 << ii)) > 0)
                {
                    positionXYZW[0] = skldata[ii].x;
                    positionXYZW[1] = skldata[ii].y;
                    positionXYZW[2] = skldata[ii].z;
					positionXYZW[3] = 1.0f;// skldata[ii].w;

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
		else
		{
			m_skeletonContainer.gameObject.SetActive(false);
		}
	}
}

