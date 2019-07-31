using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectUser_EventListener_Impl_OnePlayer : KinectUser_EventListener_Impl_default {

    public Vector3 m_centerPoint = new Vector3(0, 0, 1.9f);

    private List<KinectUser> m_kuser = new List<KinectUser>();
	private KinectUser[] m_kuser_ary = null;

    public override KinectUser[] getCurrentUser()
    {
        return m_kuser_ary;
    }
    // Update is called once per frame
    void Update () {


        float dis_min = float.MaxValue;
        KinectUser kuser_min = null;

        Hashtable h = KinectListenerManager.m_skeletonWrapper.getKinectUserTable();
		IDictionaryEnumerator ide = h.GetEnumerator();
		while (ide.MoveNext())
		{
			KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);

            float dis = Vector3.Distance(m_centerPoint, kuser.m_bone[0]);
            if (dis < dis_min)
            {
                dis_min = dis;
                kuser_min = kuser;
            }
		}

        if (null != kuser_min)
        {
            m_kuser.Add(kuser_min);
        }

		m_kuser_ary = m_kuser.ToArray();
	}
	
}
