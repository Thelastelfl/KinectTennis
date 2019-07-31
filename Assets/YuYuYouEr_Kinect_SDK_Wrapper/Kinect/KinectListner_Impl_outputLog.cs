using UnityEngine;
using System.Collections;

public class KinectListner_Impl_outputLog : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnKinectUserIn(KinectUser kuser)
	{
		Debug.Log("KinectListner_Impl_outputLog::OnKinectUserIn -> " + kuser.m_id);
	}
	
	void OnKinectUserLeave(KinectUser kuser)
	{
		Debug.Log("KinectListner_Impl_outputLog::OnKinectUserLeave -> " + kuser.m_id);
	}
	
	void OnKinectUserUpdate(KinectUser kuser)
	{
	}

}
