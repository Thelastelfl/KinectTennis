/*
 * KinectModelController.cs - Handles rotating the bones of a model to match 
 * 			rotations derived from the bone positions given by the kinect
 * 
 * 		Developed by Peter Kinney -- 6/30/2011
 * 
 */

using UnityEngine;
using System;
using System.Collections;

public class KinectModelControllerV2 : MonoBehaviour {

    //Assignments for a bitmask to control which bones to look at and which to ignore
    public enum BoneMask
    {
        None = 0x0,
        //EMPTY = 0x1,
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
        Hips = 0x1000,
        Knee_Left = 0x2000,
        Ankle_Left = 0x4000,
        Foot_Left = 0x8000,
        //EMPTY = 0x10000,
        Knee_Right = 0x20000,
        Ankle_Right = 0x40000,
        Foot_Right = 0x80000,
        All = 0xEFFFE,
        Torso = 0x1000000 | Spine | Shoulder_Center | Head, //the leading bit is used to force the ordering in the editor
        Left_Arm = 0x1000000 | Shoulder_Left | Elbow_Left | Wrist_Left | Hand_Left,
        Right_Arm = 0x1000000 | Shoulder_Right | Elbow_Right | Wrist_Right | Hand_Right,
        Left_Leg = 0x1000000 | Hips | Knee_Left | Ankle_Left | Foot_Left,
        Right_Leg = 0x1000000 | Hips | Knee_Right | Ankle_Right | Foot_Right,
        R_Arm_Chest = Right_Arm | Spine,
        No_Feet = All & ~(Foot_Left | Foot_Right),
        Upper_Body = Torso | Left_Arm | Right_Arm
    }

    public KinectUser_EventListener_Impl_default sw;

    public GameObject Hip_Center;
    public GameObject Spine;
    public GameObject Shoulder_Center;
    public GameObject Head;
    public GameObject Collar_Left;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Fingers_Left; //unused
    public GameObject Collar_Right;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Fingers_Right; //unused
    public GameObject Hip_Override;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;
    private Animation animation;

    public int player = 0;
    public BoneMask Mask = BoneMask.All;
    public bool animated = false;
    public float blendWeight = 1;

    public bool m_faceToFace = true;

    private GameObject[] _bones; //internal handle for the bones of the model
    private uint _nullMask = 0x0;

    private Quaternion[] _baseRotation; //starting orientation of the joints
    private Vector3[] _boneDir; //in the bone's local space, the direction of the bones
    private Vector3[] _boneUp; //in the bone's local space, the up vector of the bone
    private Vector3 _hipRight; //right vector of the hips
    private Vector3 _chestRight; //right vectory of the chest

    private KinectUser m_kuser = new KinectUser();
    // Use this for initialization
    void Start() {
        animation = this.GetComponent<Animation>();
        //store bones in a list for easier access, everything except Hip_Center will be one
        //higher than the corresponding Kinect.NuiSkeletonPositionIndex (because of the hip_override)
        _bones = new GameObject[(int)Kinect.NuiSkeletonPositionIndex.Count + 5] {
            null, Hip_Center, Spine, Shoulder_Center,
            Collar_Left, Shoulder_Left, Elbow_Left, Wrist_Left,
            Collar_Right, Shoulder_Right, Elbow_Right, Wrist_Right,
            Hip_Override, Hip_Left, Knee_Left, Ankle_Left,
            null, Hip_Right, Knee_Right, Ankle_Right,
			//extra joints to determine the direction of some bones
			Head, Hand_Left, Hand_Right, Foot_Left, Foot_Right};

        //determine which bones are not available
        for (int ii = 0; ii < _bones.Length; ii++)
        {
            if (_bones[ii] == null)
            {
                _nullMask |= (uint)(1 << ii);
            }
        }

        //store the base rotations and bone directions (in bone-local space)
        _baseRotation = new Quaternion[(int)Kinect.NuiSkeletonPositionIndex.Count];
        _boneDir = new Vector3[(int)Kinect.NuiSkeletonPositionIndex.Count];

        //first save the special rotations for the hip and spine
        _hipRight = Hip_Right.transform.position - Hip_Left.transform.position;
        _hipRight = Hip_Override.transform.InverseTransformDirection(_hipRight);

        _chestRight = Shoulder_Right.transform.position - Shoulder_Left.transform.position;
        _chestRight = Spine.transform.InverseTransformDirection(_chestRight);

        //get direction of all other bones
        for (int ii = 0; ii < (int)Kinect.NuiSkeletonPositionIndex.Count; ii++)
        {
            if ((_nullMask & (uint)(1 << ii)) <= 0)
            {
                //save initial rotation
                _baseRotation[ii] = _bones[ii].transform.localRotation;

                //if the bone is the end of a limb, get direction from this bone to one of the extras (hand or foot).
                if (ii % 4 == 3 && ((_nullMask & (uint)(1 << (ii / 4) + (int)Kinect.NuiSkeletonPositionIndex.Count)) <= 0))
                {
                    _boneDir[ii] = _bones[(ii / 4) + (int)Kinect.NuiSkeletonPositionIndex.Count].transform.position - _bones[ii].transform.position;
                }
                //if the bone is the hip_override (at boneindex Hip_Left, get direction from average of left and right hips
                else if (ii == (int)Kinect.NuiSkeletonPositionIndex.HipLeft && Hip_Left != null && Hip_Right != null)
                {
                    _boneDir[ii] = ((Hip_Right.transform.position + Hip_Left.transform.position) / 2F) - Hip_Override.transform.position;
                }
                //otherwise, get the vector from this bone to the next.
                else if ((_nullMask & (uint)(1 << ii + 1)) <= 0)
                {
                    _boneDir[ii] = _bones[ii + 1].transform.position - _bones[ii].transform.position;
                }
                else
                {
                    continue;
                }
                //Since the spine of the kinect data is ~40 degrees back from the hip,
                //check what angle the spine is at and rotate the saved direction back to match the data
                if (ii == (int)Kinect.NuiSkeletonPositionIndex.Spine)
                {
                    float angle = Vector3.Angle(transform.up, _boneDir[ii]);
                    {//YuYuYouEr Modified for Kinect V2
                     //_boneDir[ii] = Quaternion.AngleAxis(-40 + angle,transform.right) * _boneDir[ii];
                        _boneDir[ii] = Quaternion.AngleAxis(0 + angle, transform.right) * _boneDir[ii];
                    }
                }
                //transform the direction into local space.
                _boneDir[ii] = _bones[ii].transform.InverseTransformDirection(_boneDir[ii]);
            }
        }
        //make _chestRight orthogonal to the direction of the spine.
        _chestRight -= Vector3.Project(_chestRight, _boneDir[(int)Kinect.NuiSkeletonPositionIndex.Spine]);
        //make _hipRight orthogonal to the direction of the hip override
        Vector3.OrthoNormalize(ref _boneDir[(int)Kinect.NuiSkeletonPositionIndex.HipLeft], ref _hipRight);

        if (m_faceToFace)
        {
            Vector3 s = transform.localScale;
            s.z = -s.z;
            transform.localScale = s;
        }
    }
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
    Vector3 newss;
    Vector3 t;
    void Update ()
    {
        Vector3 mid = new Vector3(Spine.transform.position.x, 1, Spine.transform.position.z);
        if (Angle(Spine.transform.position, Shoulder_Center.transform.position, Head.transform.position) >2
            && Head.transform.position.x < Shoulder_Center.transform.position.x && Head.transform.position.z -
            Shoulder_Center.transform.position.z < 0.1)
        {
            transform.Translate(1 * Vector3.left * Time.deltaTime,Space.World);
        }
        if (Angle(Spine.transform.position, Shoulder_Center.transform.position, Head.transform.position) > 2
            && Head.transform.position.x > Shoulder_Center.transform.position.x && Head.transform.position.z -
            Shoulder_Center.transform.position.z < 0.1)
        {
            transform.Translate(1 * Vector3.right * Time.deltaTime, Space.World);
        }
        //update the data from the kinect if necessary
        KinectUser[] kuser = sw.getCurrentUser();
		if (null != kuser
			&& kuser.Length > 0
			&& player < kuser.Length
			&& null != kuser[player]
			)
		{

			//整理数据
			m_kuser.copyValues(kuser[player]);
			for (int i=0; i<m_kuser.m_bone.Length; ++i)
			{
				m_kuser.m_bone[i].z = -m_kuser.m_bone[i].z;
			}

			//应用数据
			for( int ii = 0; ii < (int)Kinect.NuiSkeletonPositionIndex.Count; ii++)
			{
                if ( ((uint)Mask & (uint)(1 << ii) ) > 0 && (_nullMask & (uint)(1 << ii)) <= 0 )
				{
                    //m_kuser.m_bone[ii] = new Vector3(
                    //    sw.transform.position.x * 1,
                    //    sw.transform.position.y * 1,
                    //    sw.transform.position.z * 1);
                    RotateJoint(m_kuser, ii);
                }
			}
		}
	}
	
	void RotateJoint(KinectUser kuser, int bone) {
		//if blendWeight is 0 there is no need to compute the rotations
		if( blendWeight <= 0 ){ return; }
		Vector3 upDir = new Vector3();
		Vector3 rightDir = new Vector3();
		if(bone == (int)Kinect.NuiSkeletonPositionIndex.Spine)
		{
			upDir = ((Hip_Left.transform.position + Hip_Right.transform.position) / 2F) - Hip_Override.transform.position;
			rightDir = Hip_Right.transform.position - Hip_Left.transform.position;
		}
		
		//if the model is not animated, reset rotations to fix twisted joints
		if(!animated){_bones[bone].transform.localRotation = _baseRotation[bone];}

		//if the required bone data from the kinect isn't available, return
		//if( kuser.m_bone[bone] == Kinect.NuiSkeletonPositionTrackingState.NotTracked)
		//{
		//	return;
		//}
		
		//get the target direction of the bone in world space
		//for the majority of bone it's bone - 1 to bone, but Hip_Override and the outside
		//shoulders are determined differently.
		
		Vector3 dir = _boneDir[bone];
		Vector3 target;
		
		//if bone % 4 == 0 then it is either an outside shoulder or the hip override
		if(bone % 4 == 0)
		{
			//hip override is at Hip_Left
			if(bone == (int)Kinect.NuiSkeletonPositionIndex.HipLeft)
			{
				//target = vector from hip_center to average of hips left and right
				target = ((kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.HipLeft]
				           + kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.HipRight]) / 2F)
					- kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.HipCenter];
			}
			//otherwise it is one of the shoulders
			else
			{
				//target = vector from shoulder_center to bone
				target = kuser.m_bone[bone] - kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.ShoulderCenter];
			}
		}
		else
		{
			//target = vector from previous bone to bone
			target = kuser.m_bone[bone] - kuser.m_bone[bone-1];
		}
		//transform it into bone-local space (independant of the transform of the controller)
		target = transform.TransformDirection(target);
        //m_kuser.m_bone = target;
		target = _bones[bone].transform.InverseTransformDirection(target);
		//create a rotation that rotates dir into target
		Quaternion quat = Quaternion.FromToRotation(dir,target);
		//if bone is the spine, add in the rotation along the spine
		if(bone == (int)Kinect.NuiSkeletonPositionIndex.Spine)
		{
			//rotate the chest so that it faces forward (determined by the shoulders)
			dir = _chestRight;
			target = kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.ShoulderRight]
			- kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.ShoulderLeft];
			
			target = transform.TransformDirection(target);
			target = _bones[bone].transform.InverseTransformDirection(target);
			target -= Vector3.Project(target,_boneDir[bone]);
			
			quat *= Quaternion.FromToRotation(dir,target);
			
		}
		//if bone is the hip override, add in the rotation along the hips
		else if(bone == (int)Kinect.NuiSkeletonPositionIndex.HipLeft)
		{
			//rotate the hips so they face forward (determined by the hips)
			dir = _hipRight;
			target = kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.HipRight]
			- kuser.m_bone[(int)Kinect.NuiSkeletonPositionIndex.HipLeft];
			
			target = transform.TransformDirection(target);
			target = _bones[bone].transform.InverseTransformDirection(target);
			target -= Vector3.Project(target,_boneDir[bone]);
			
			quat *= Quaternion.FromToRotation(dir,target);
		}
		
		//reduce the effect of the rotation using the blend parameter
		quat = Quaternion.Lerp(Quaternion.identity, quat, blendWeight);
		//apply the rotation to the local rotation of the bone
		_bones[bone].transform.localRotation = _bones[bone].transform.localRotation  * quat;
		
		if(bone == (int)Kinect.NuiSkeletonPositionIndex.Spine)
		{
			restoreBone(_bones[(int)Kinect.NuiSkeletonPositionIndex.HipLeft],_boneDir[(int)Kinect.NuiSkeletonPositionIndex.HipLeft],upDir);
			restoreBone(_bones[(int)Kinect.NuiSkeletonPositionIndex.HipLeft],_hipRight,rightDir);
		}
		
		return;
	}
	
	void restoreBone(GameObject bone,Vector3 dir, Vector3 target)
	{
		//transform target into bone-local space (independant of the transform of the controller)
		//target = transform.TransformDirection(target);
		target = bone.transform.InverseTransformDirection(target);
		//create a rotation that rotates dir into target
		Quaternion quat = Quaternion.FromToRotation(dir,target);
		bone.transform.localRotation *= quat;
	}
}


