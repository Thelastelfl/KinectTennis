using UnityEngine;
using System.Collections;

public class SkeletonWrapper : MonoBehaviour {
	
	public DeviceOrEmulator devOrEmu;
	private Kinect.KinectInterface kinect;



	private bool updatedSkeleton = false;
	private bool newSkeleton = false;
	
	[HideInInspector]
	public Kinect.NuiSkeletonTrackingState[] players;
	[HideInInspector]
	public int[] m_trackedPlayers = null;
	private int m_trackedCount = 0;

	[HideInInspector]
	public Vector3[,] bonePos;
	[HideInInspector]
	public Vector3[,] boneVel;
	[HideInInspector]
	public Kinect.NuiSkeletonPositionTrackingState[,] boneState;
    [HideInInspector]
    public bool[,] handState;
	
	private System.Int64 ticks;
	private float deltaTime;

	//Kinect User: <user_id, KinectUser>
	private Hashtable m_kuserTable = new Hashtable();
	private static int m_kuserId_Next = 0;

	public Hashtable getKinectUserTable()
	{
		return m_kuserTable;
	}

	// Use this for initialization
	void Start () {
		kinect = devOrEmu.getKinect();
		players = new Kinect.NuiSkeletonTrackingState[Kinect.Constants.NuiSkeletonCount];
		m_trackedPlayers = new int[Kinect.Constants.NuiSkeletonMaxTracked];
        for (int i = 0; i < m_trackedPlayers.Length; ++i)
        {
            m_trackedPlayers[i] = -1;
        }

		bonePos = new Vector3[Kinect.Constants.NuiSkeletonCount,(int)Kinect.NuiSkeletonPositionIndex.Count];
		boneVel = new Vector3[Kinect.Constants.NuiSkeletonCount,(int)Kinect.NuiSkeletonPositionIndex.Count];
		boneState = new Kinect.NuiSkeletonPositionTrackingState[Kinect.Constants.NuiSkeletonCount,(int)Kinect.NuiSkeletonPositionIndex.Count];
		handState = new bool[Kinect.Constants.NuiSkeletonCount, 2];
		for (int i=0; i<Kinect.Constants.NuiSkeletonCount; ++i)
		{
			for (int j=0; j<2; ++j)
			{
				handState[i, j] = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate () {
		updatedSkeleton = false;
		newSkeleton = false;
	}
	

	public bool pollSkeleton (IKinectUser_EventListener kuserMgr = null) {
		m_trackedCount= 0;
		if (!updatedSkeleton)
		{
			updatedSkeleton = true;
			if (kinect.pollSkeleton())
			{
				newSkeleton = true;
				System.Int64 cur = kinect.getSkeleton().liTimeStamp;
				System.Int64 diff = cur - ticks;
				ticks = cur;
				deltaTime = diff / (float)1000;
								
				processSkeleton(kuserMgr);
			}
			else if (! kinect.hasUserIn())
			{//has NO user
				if (m_kuserTable.Count > 0)
				{//onUserLeaver notify

					Debug.Log("There is NO user");

					IDictionaryEnumerator ide = m_kuserTable.GetEnumerator();
					while (ide.MoveNext())
					{
						KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);
						
						kuserMgr._onKinectUserLeave(kuser);
					}

					//remove all user
					m_kuserTable.Clear();
				}
			}

		}

		return newSkeleton;
	}

	private void processSkeleton(IKinectUser_EventListener kuserMgr = null)
    {
		for (int i = 0; i < m_trackedPlayers.Length; ++i)
        {
			m_trackedPlayers[i] = -1;
        }

        m_trackedCount = 0;

        //update players
        {//search all tracked players
            for (int ii = 0; ii < Kinect.Constants.NuiSkeletonCount; ii++)
            {
				//Debug.Log("ii = " + ii + " -> dwUserIndex = " + kinect.getSkeleton().SkeletonData[ii].dwUserIndex + ", dwTrackingID = " + kinect.getSkeleton().SkeletonData[ii].dwTrackingID);

                players[ii] = kinect.getSkeleton().SkeletonData[ii].eTrackingState;
                if (players[ii] == Kinect.NuiSkeletonTrackingState.SkeletonTracked)
                {
					m_trackedPlayers[m_trackedCount] = ii;
                    m_trackedCount++;
                }
            }
        }

		{//update the bone positions, velocities, and tracking states)
			int player_index = -1;
			for (int i = 0; i < m_trackedCount; i++)
	        {
				player_index = m_trackedPlayers[i];

				if (player_index < 0)
				{//用户id不合法
					continue;
				}

	            {//有效用户
	                for (int bone = 0; bone < (int)Kinect.NuiSkeletonPositionIndex.Count; bone++)
	                {
	                    Vector3 oldpos = bonePos[i, bone];
	                    bonePos[i, bone] = kinect.getSkeleton().SkeletonData[player_index].SkeletonPositions[bone];
	                    boneVel[i, bone] = (bonePos[i, bone] - oldpos) / deltaTime;
	                    boneState[i, bone] = kinect.getSkeleton().SkeletonData[player_index].eSkeletonPositionTrackingState[bone];
	                }

	                //update hand state
	                handState[i, 0] = kinect.getIsLeftHandGrip(player_index);
	                handState[i, 1] = kinect.getIsRightHandGrip(player_index);
	            }
	        }
		}

		{//combine user

			//select user into two category: ok/failed
			Hashtable okTable = new Hashtable();
			Hashtable failedTable = new Hashtable();
			Hashtable newTable = new Hashtable();

			while ((okTable.Count + failedTable.Count) < m_kuserTable.Count)
			{//combine user: find same user
				IDictionaryEnumerator ide = m_kuserTable.GetEnumerator();
				while (ide.MoveNext())
				{
					KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);

					if (okTable.ContainsKey(kuser.m_id)
					    || failedTable.ContainsKey(kuser.m_id)
					    )
					{//skip
						continue;
					}

					/////////////////////////
					/// search nearest user
					int player_index = -1;
					float dis_min = float.MaxValue;
					int tracked_index_min = -1;

					for (int i = 0; i < m_trackedCount; i++)
					{
						player_index = m_trackedPlayers[i];
						if (player_index >= 0)
						{
							float dis = Vector3.Distance(kuser.m_bone[0], bonePos[i, 0]);
							if (dis < dis_min)
							{
								dis_min = dis;
								tracked_index_min = i;
							}
						}
					}

					if (tracked_index_min >= 0
					    && dis_min < 0.5f)
					{//maybe same user

						//search other old user
						float dis_min2 = dis_min;
						KinectUser kuser_min = kuser;

						IDictionaryEnumerator ide2 = m_kuserTable.GetEnumerator();
						while (ide2.MoveNext())
						{
							KinectUser kuser2 = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);

							if (okTable.ContainsKey(kuser2.m_id)
							    || failedTable.ContainsKey(kuser.m_id)
								)
							{//skip
								continue;
							}

							float dis = Vector3.Distance(kuser2.m_bone[0], bonePos[tracked_index_min, 0]);
							if (dis < dis_min2)
							{
								dis_min2 = dis;
								kuser_min = kuser2;
							}
						}

						//ok, got the same user, copy data
						for (int i=0; i<kuser_min.m_bone.Length; ++i)
						{
							kuser_min.m_bone[i] = bonePos[tracked_index_min, i];
						}

                        kuser_min.m_handState_left = handState[tracked_index_min, 0] ? KinectUser.KUserHandState.HAND_STATE_GRIP : KinectUser.KUserHandState.HAND_STATE_RELEASE;
                        kuser_min.m_handState_right = handState[tracked_index_min, 1] ? KinectUser.KUserHandState.HAND_STATE_GRIP : KinectUser.KUserHandState.HAND_STATE_RELEASE;

						kuser_min.m_state = KinectUser.KUserState.KUSER_STATE_USERIN;

						//remove this user
						m_trackedPlayers[tracked_index_min] = -1;

						//add to okTable
						okTable.Add(kuser_min.m_id, kuser_min);
					}
					else
					{//add to failed
						failedTable.Add(kuser.m_id, kuser);
					}
				}
			}

			{//add new user into okTable
				int player_index = -1;

				for (int i = 0; i < m_trackedCount; i++)
				{
					player_index = m_trackedPlayers[i];
					if (player_index < 0)
					{//不合法的userId
						continue;
					}

					{//合法的userId
						KinectUser kuser = new KinectUser();

						kuser.m_id = m_kuserId_Next++;
						kuser.m_state = KinectUser.KUserState.KUSER_STATE_USERIN;

						for (int bone=0; bone<((int)(Kinect.NuiSkeletonPositionIndex.Count)); ++bone)
						{
							kuser.m_bone[bone] = bonePos[i, bone];
						}

                        kuser.m_handState_left = handState[i, 0] ? KinectUser.KUserHandState.HAND_STATE_GRIP : KinectUser.KUserHandState.HAND_STATE_RELEASE;
                        kuser.m_handState_right = handState[i, 1] ? KinectUser.KUserHandState.HAND_STATE_GRIP : KinectUser.KUserHandState.HAND_STATE_RELEASE;

						okTable.Add(kuser.m_id, kuser);
						newTable.Add(kuser.m_id, kuser);
					}
				}
			}

			//switch kuserTable
			m_kuserTable = okTable;

			if (null != kuserMgr)
			{//fire EVENT

				{//onUserLeaver
					IDictionaryEnumerator ide = failedTable.GetEnumerator();
					while (ide.MoveNext())
					{
						KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);

						kuserMgr._onKinectUserLeave(kuser);
					}
				}

				{//onUserIn
					IDictionaryEnumerator ide = newTable.GetEnumerator();
					while (ide.MoveNext())
					{
						KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);
						
						kuserMgr._onKinectUserIn(kuser);
					}
				}

				{//onUserUpdate
					IDictionaryEnumerator ide = okTable.GetEnumerator();
					while (ide.MoveNext())
					{
						KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);

						if (newTable.ContainsKey(kuser.m_id))
						{
							continue;
						}

						kuserMgr._onKinectUserUpdate(kuser);
					}
				}
			}
		}
    }
}











