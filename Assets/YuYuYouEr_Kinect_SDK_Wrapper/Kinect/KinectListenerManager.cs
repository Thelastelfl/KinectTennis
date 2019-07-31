using UnityEngine;
using System.Collections;


public class KinectListenerManager : MonoBehaviour, IKinectUser_EventListener {

	public SkeletonWrapper __m_skeletonWrapper = null;

    [HideInInspector]
    public static SkeletonWrapper m_skeletonWrapper = null;

	////////////////////////////////////////////
	private static Hashtable m_kinectUser_EventListener = new Hashtable();

	public static void addListener(IKinectUser_EventListener kl)
	{
		if (! m_kinectUser_EventListener.ContainsKey(kl))
		{
			m_kinectUser_EventListener.Add(kl, true);
		}
	}

	public static void removeListener(IKinectUser_EventListener kl)
	{
		if (m_kinectUser_EventListener.ContainsKey(kl))
		{
			m_kinectUser_EventListener.Remove(kl);
		}
	}

	// Use this for initialization
	void Start () {
        m_skeletonWrapper = __m_skeletonWrapper;
	}
	
	// Update is called once per frame
	void Update () {
		if (null != __m_skeletonWrapper)
		{
			if (__m_skeletonWrapper.pollSkeleton(this))
			{
				//example code: get all KinectUser, DO NOT include user which has been leaved
				/*
				Hashtable h = m_sw.getKinectUserTable();
				IDictionaryEnumerator ide = h.GetEnumerator();
				while (ide.MoveNext())
				{
					KinectUser kuser = (KinectUser)(((DictionaryEntry)(ide.Current)).Value);
				}
				*/
			}
		}
	}

	#region Do Notify

	void IKinectUser_EventListener._onKinectUserIn(KinectUser kuser)
	{
		IDictionaryEnumerator ide = m_kinectUser_EventListener.GetEnumerator();
		while (ide.MoveNext())
		{
			IKinectUser_EventListener kl = (IKinectUser_EventListener)(((DictionaryEntry)(ide.Current)).Key);
			kl._onKinectUserIn(kuser);
		}
	}

	void IKinectUser_EventListener._onKinectUserLeave(KinectUser kuser)
	{
		IDictionaryEnumerator ide = m_kinectUser_EventListener.GetEnumerator();
		while (ide.MoveNext())
		{
			IKinectUser_EventListener kl = (IKinectUser_EventListener)(((DictionaryEntry)(ide.Current)).Key);
			kl._onKinectUserLeave(kuser);
		}
	}

	void IKinectUser_EventListener._onKinectUserUpdate(KinectUser kuser)
	{
		IDictionaryEnumerator ide = m_kinectUser_EventListener.GetEnumerator();
		while (ide.MoveNext())
		{
			IKinectUser_EventListener kl = (IKinectUser_EventListener)(((DictionaryEntry)(ide.Current)).Key);
			kl._onKinectUserUpdate(kuser);
		}
	}

	#endregion

}
