using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisBat : MonoBehaviour {

    public bool canHit;
    public Vector3 startPosition;
    private GameObject hand;
    private float p_x;
    private float p_y;
    private float p_z;

    void Start () {
        hand = GameObject.Find("Bip01 R Hand");
        p_x = hand.transform.rotation.x - this.transform.rotation.x;
        p_y = hand.transform.rotation.y - this.transform.rotation.y;
        p_z = hand.transform.rotation.z - this.transform.rotation.z;
    }
	
	
	void Update () {
        Quaternion a = new Quaternion(hand.transform.rotation.x + p_x, hand.transform.rotation.y+p_y, hand.transform.rotation.z+p_z, hand.transform.rotation.w);
        //this.transform.position = hand.transform.position +new Vector3(p_x,p_y
        //    ,p_z);
        this.transform.rotation =a;
	}
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.tag.ToString());
        if (collision.transform.tag == "Tennis")
        {
            startPosition = collision.transform.position;
            canHit = true;
        }
    }

}
