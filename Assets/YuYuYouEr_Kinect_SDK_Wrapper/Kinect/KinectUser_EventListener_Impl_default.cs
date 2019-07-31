using UnityEngine;
using System.Collections;

public class KinectUser_EventListener_Impl_default : MonoBehaviour, IKinectUser_EventListener {

	// Use this for initialization
	void Start () {
		KinectListenerManager.addListener(this);
	}

	void OnEnable()
	{
		KinectListenerManager.addListener(this);
	}

	void OnDisable()
	{
		KinectListenerManager.removeListener(this);
	}

	void OnDestory()
	{
		KinectListenerManager.removeListener(this);
	}


    public virtual KinectUser[] getCurrentUser()
    {
        return null;
    }

	public virtual void _onKinectUserIn(KinectUser kuser)
	{
		SendMessage("OnKinectUserIn", kuser, SendMessageOptions.DontRequireReceiver);
	}

	public virtual void _onKinectUserLeave(KinectUser kuser)
	{
        SendMessage("OnKinectUserLeave", kuser, SendMessageOptions.DontRequireReceiver);
	}

	public virtual void _onKinectUserUpdate(KinectUser kuser)
	{
        SendMessage("OnKinectUserUpdate", kuser, SendMessageOptions.DontRequireReceiver);
	}

}
