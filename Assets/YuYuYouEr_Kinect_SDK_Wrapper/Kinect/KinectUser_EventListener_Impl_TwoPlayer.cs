using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectUser_EventListener_Impl_TwoPlayer : KinectUser_EventListener_Impl_default {

    public Vector3 m_leftPoint = new Vector3(-0.8f, 0, 1.9f);
    public Vector3 m_rightPoint = new Vector3(0.8f, 0, 1.9f);

    private List<KinectUser> m_kuser = new List<KinectUser>();
	private KinectUser[] m_kuser_ary = null;

    public override KinectUser[] getCurrentUser()
    {/* [0]为leftUser，[1]为rightUser，不存在为null */
		return m_kuser_ary;
    }

	// Update is called once per frame
	void Update () {

        m_kuser.Clear();

        Hashtable h = KinectListenerManager.m_skeletonWrapper.getKinectUserTable();
        if (h.Count == 1)
        {
            IDictionaryEnumerator ide = h.GetEnumerator();
            while (ide.MoveNext())
            {
                KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);

                if (kuser.m_bone[0].x <= 0.0f)
                {
                    m_kuser.Add(kuser);
                    break;
                }
                else
                {
                    m_kuser.Add(null);  //leftUser不存在
                    m_kuser.Add(kuser); //rightUser存在
                    break;
                }
            }
        }
        else if (h.Count > 1)
        {//more than 1

            float dis_min_left = float.MaxValue;
            KinectUser kuser_min_left = null;

            float dis_min_right = float.MaxValue;
            KinectUser kuser_min_right = null;

            IDictionaryEnumerator ide = h.GetEnumerator();
            while (ide.MoveNext())
            {
                KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);

                if (kuser.m_bone[0].x <= 0.0f)
                {//left
                    float dis = Vector3.Distance(m_leftPoint, kuser.m_bone[0]);
                    if (dis < dis_min_left)
                    {
                        dis_min_left = dis;
                        kuser_min_left = kuser;
                    }
                }
                else
                {//right
                    float dis = Vector3.Distance(m_rightPoint, kuser.m_bone[0]);
                    if (dis < dis_min_right)
                    {
                        dis_min_right = dis;
                        kuser_min_right = kuser;
                    }
                }
            }

            if (null != kuser_min_right)
            {
                m_kuser.Add(kuser_min_left);
                m_kuser.Add(kuser_min_right);
            }
            else if (null != kuser_min_left)
            {
                m_kuser.Add(kuser_min_left);
            }
        }

		m_kuser_ary = m_kuser.ToArray();
	}
	
}
