using UnityEngine;
using System.Collections;

public class handCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        int player2 = col.transform.parent.GetComponent<KinectPointController>().player;
        int player = transform.parent.GetComponent<KinectPointController>().player;

        if (player != player2)
        {
            GameObject.Find("Plane_KinectBackgroundRemoval_bg")
                .GetComponent<changeColor>().OnEnter();
        }
    }

    void OnTriggerStay(Collider col)
    {
        int player2 = col.transform.parent.GetComponent<KinectPointController>().player;
        int player = transform.parent.GetComponent<KinectPointController>().player;

        if (player != player2)
        {
            GameObject.Find("Plane_KinectBackgroundRemoval_bg")
                .GetComponent<changeColor>().OnEnter();
        }
    }

    void OnTriggerExit(Collider col)
    {
        int player2 = col.transform.parent.GetComponent<KinectPointController>().player;
        int player = transform.parent.GetComponent<KinectPointController>().player;

        if (player != player2)
        {
            GameObject.Find("Plane_KinectBackgroundRemoval_bg")
                .GetComponent<changeColor>().OnExit();
        }
    }
}
