using UnityEngine;
using System.Collections;

public class KinectUser {

	public enum KUserState {
		KUSER_STATE_USERIN
		, KUSER_STATE_USERLEAVE
		, KUSER_UNKNOWN
	};

    public enum KUserHandState {
        HAND_STATE_UNKNOWN
        , HAND_STATE_GRIP
        , HAND_STATE_RELEASE
    };

	public int m_id = -1;
	//public int m_userIndex = -1;	//打算用Kinect驱动部分提供的userIndex来记录，但是发现不是很稳定，先注释掉
	public Vector3[] m_bone = new Vector3[(int)(Kinect.NuiSkeletonPositionIndex.Count)];
	public KUserState m_state = KUserState.KUSER_STATE_USERLEAVE;
    public KUserHandState m_handState_left = KUserHandState.HAND_STATE_UNKNOWN;
    public KUserHandState m_handState_right = KUserHandState.HAND_STATE_UNKNOWN;

	public KinectUser()
	{
		for (int i = 0; i < m_bone.Length; ++i)
		{
			m_bone[i] = Vector3.zero;
		}
	}
	public KinectUser(KinectUser kuser)
	{
		copyValues(kuser);
	}

	public void copyValues(KinectUser kuser)
	{
		m_id = kuser.m_id;

		for (int i = 0; i < m_bone.Length; ++i)
		{
			m_bone[i] = kuser.m_bone[i];
		}

		m_state = kuser.m_state;
		m_handState_left = kuser.m_handState_left;
		m_handState_right = kuser.m_handState_right;
	}
}
