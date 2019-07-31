/*
 * KinectModelController.cs - Moves every 'bone' given to match
 * 				the position of the corresponding bone given by
 * 				the kinect. Useful for viewing the point tracking
 * 				in 3D.
 * 
 * 		Developed by Peter Kinney -- 6/30/2011
 * 
 */

using UnityEngine;
using System;
using System.Collections;

public class KinectPointController : MonoBehaviour {
	
	//Assignments for a bitmask to control which bones to look at and which to ignore
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

    public KinectUser_EventListener_Impl_default sw;

    public Vector3 m_scale = new Vector3(1.0f, 1.0f, 1.0f);
    public bool m_isLeftHandGrip = false;
    public bool m_isRightHandGrip = false;

    private bool m_isUserIn = false;

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
	//private Vector4[] _bonePos; //internal handle for the bone positions from the kinect
	
	public int player;
	public BoneMask Mask = BoneMask.All;
	
	// Use this for initialization
	void Start () {
		//store bones in a list for easier access
		_bones = new GameObject[(int)Kinect.NuiSkeletonPositionIndex.Count] {Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right};
		//_bonePos = new Vector4[(int)BoneIndex.Num_Bones];
		
	}

	// Update is called once per frame
	void Update () {
		//update all of the bones positions
        KinectUser[] kuser = sw.getCurrentUser();
        if (null != kuser
            && kuser.Length > 0
            && player < kuser.Length
            && null != kuser[player]
            )
        {
            m_isUserIn = true;

            Vector3 vpos;
            for (int ii = 0; ii < (int)Kinect.NuiSkeletonPositionIndex.Count; ii++)
            {
                //_bonePos[ii] = sw.getBonePos(ii);
                if (((uint)Mask & (uint)(1 << ii)) > 0)
                {

                    vpos = kuser[player].m_bone[ii];
                    vpos.Scale(m_scale);
                    _bones[ii].transform.localPosition = vpos;

                    if (ii == (int)Kinect.NuiSkeletonPositionIndex.HandLeft)
                    {
                        if (kuser[player].m_handState_left != KinectUser.KUserHandState.HAND_STATE_UNKNOWN)
                        {
                            m_isLeftHandGrip = (kuser[player].m_handState_left == KinectUser.KUserHandState.HAND_STATE_GRIP);
                        }

                        _bones[ii].transform.localScale = m_isLeftHandGrip
                                        ? new Vector3(0.2f, 0.2f, 0.2f) : new Vector3(0.1f, 0.1f, 0.1f);
                    }
                    else if (ii == (int)Kinect.NuiSkeletonPositionIndex.HandRight)
                    {
                        if (kuser[player].m_handState_right != KinectUser.KUserHandState.HAND_STATE_UNKNOWN)
                        {
                            m_isRightHandGrip = (kuser[player].m_handState_right == KinectUser.KUserHandState.HAND_STATE_GRIP);
                        }

                        _bones[ii].transform.localScale = m_isRightHandGrip
										? new Vector3(0.2f, 0.2f, 0.2f) : new Vector3(0.1f, 0.1f, 0.1f);
					}
				}
            }
        }
        else
        {
            m_isUserIn = false;
            m_isLeftHandGrip = false;
            m_isRightHandGrip = false;
        }
	}

    public bool get_isUserIn()
    {
        return m_isUserIn;
    }
    /// <summary>
    /// 计算两点间的距离
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    private double Distance(GameObject p1, GameObject p2)
    {
        double sqrt = System.Math.Sqrt(System.Math.Pow(p1.transform.position.x - p2.transform.position.x, 2) +
            System.Math.Pow(p1.transform.position.y - p2.transform.position.y, 2) +
            System.Math.Pow(p1.transform.position.z - p2.transform.position.z, 2));

        return System.Math.Abs(sqrt);
    }
    public bool Squats()
    {
        //下蹲动作检测
        if (Angle(Knee_Left.transform.position, Hip_Left.transform.position, Ankle_Left.transform.position) < 160.0f
            && Angle(Knee_Right.transform.position, Hip_Right.transform.position, Ankle_Right.transform.position) < 160.0f)
        {
            Debug.Log("dun");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 第一种挥拍判定，正手挥拍。
    /// </summary>
    /// <returns></returns>
    public bool OneHit()
    {
        bool t = false;
        if (Angle(Elbow_Left.transform.position, Wrist_Left.transform.position, Shoulder_Left.transform.position) > 160)
        {
            t = true;
            if (Hand_Right.transform.position.x < Shoulder_Right.transform.position.x)
            {
                t = false;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 第二种挥拍判定，首先计算手，胳膊肘，肩膀之间的夹角。正手挥拍。
    /// </summary>
    /// <returns></returns>
    public bool TwoWave()
    {
        bool t = false;
        if (Angle(Elbow_Right.transform.position, Wrist_Right.transform.position, Shoulder_Right.transform.position) > 160)
        {
            t = true;
        }
        if (t == true)
        {
            if (Hand_Right.transform.position.x < Shoulder_Right.transform.position.x)
            {
                Debug.Log("zheng hit");
                t = false;
                return true;
            }
        }
        return false;
    }
    bool ts = false;
    /// <summary>
    /// 第三种挥拍判定，首先计算手，胳膊肘，肩膀之间的夹角。反手挥拍
    /// </summary>
    /// <returns></returns>
    public bool ThreeWave()
    {
        if (Angle(Elbow_Right.transform.position, Wrist_Right.transform.position, Shoulder_Right.transform.position) < 100
            && Hand_Right.transform.position.x < Shoulder_Right.transform.position.x)
        {
            ts = true;
            Debug.Log("true");
        }
        if (ts == true)
        {
            if (Hand_Right.transform.position.x > Shoulder_Right.transform.position.x)
            {
                Debug.Log("fan hit");
                ts = false;
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 举手,发球
    /// </summary>
    public bool HandUp()
    {
        if (Elbow_Right.transform.position.y > Shoulder_Right.transform.position.y)
        {
            Debug.Log("ju you shou");
            return true;
        }
        return false;
    }
    /// <summary>
    /// 暂停
    /// </summary>
    public bool Stop()
    {
        if (Wrist_Left.transform.position.y > Shoulder_Center.transform.position.y && Elbow_Left.transform.position.y
            > Shoulder_Center.transform.position.y && Mathf.Abs(Wrist_Left.transform.position.y - Elbow_Left.transform.position.y) < 1)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 抬腿
    /// </summary>
    public bool RaiseLeftLeg()
    {
        if (Knee_Left.transform.position.y > Hip_Right.transform.position.y)
        {
            Debug.Log("raise left leg");
            return true;
        }

        return false;
    }
    public bool RaiseRightLeg()
    {
        if (Knee_Right.transform.position.y > Hip_Left.transform.position.y)
        {
            Debug.Log("raise right leg");
            return true;
        }
        return false;
    }
    public void test()
    {
        //Debug.Log(Angle(Shoulder_Right.transform.position, Wrist_Right.transform.position, Elbow_Right.transform.position));
    }
    /// <summary>
    /// 计算三点之间的夹角,cen点为构成夹角的点,也就是第一个点作为夹角点
    /// </summary>
    /// <param name="cen"></param>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static double Angle(Vector3 cen, Vector3 first, Vector3 second)
    {
        const double M_PI = 3.1415926535897;

        double ma_x = first.x - cen.x;
        double ma_y = first.y - cen.y;
        double mb_x = second.x - cen.x;
        double mb_y = second.y - cen.y;
        double v1 = (ma_x * mb_x) + (ma_y * mb_y);
        double ma_val = Math.Sqrt(ma_x * ma_x + ma_y * ma_y); // 模
        double mb_val = Math.Sqrt(mb_x * mb_x + mb_y * mb_y); // 模
        double cosM = v1 / (ma_val * mb_val);   // a·b = a模 · b模 * cos值
        double angleAMB = Math.Acos(cosM) * 180 / M_PI;  //角度转换成弧度，进行计算

        return angleAMB;
    }

}
