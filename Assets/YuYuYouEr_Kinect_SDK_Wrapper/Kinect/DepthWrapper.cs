using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Level of indirection for the depth image,
/// provides:
/// -a frames of depth image (no player information),
/// -an array representing which players are detected,
/// -a segmentation image for each player,
/// -bounds for the segmentation of each player.
/// </summary>
public class DepthWrapper: MonoBehaviour {
	
	public DeviceOrEmulator devOrEmu;
	private Kinect.KinectInterface kinect;


	private bool updatedSeqmentation = false;
	private bool newSeqmentation = false;
	

	/// Depth image for the latest frame
	[HideInInspector]
	public short[] depthImg;
	
	// Use this for initialization
	void Start () {
		kinect = devOrEmu.getKinect();
	}

	void LateUpdate()
	{
		updatedSeqmentation = false;
		newSeqmentation = false;
	}

	public bool pollDepth()
	{
		//Debug.Log("" + updatedSeqmentation + " " + newSeqmentation);
		if (!updatedSeqmentation)
		{
			updatedSeqmentation = true;
			if (kinect.pollDepth())
			{
				newSeqmentation = true;

				processDepth();
			}
		}
		return newSeqmentation;
	}
	
	private void processDepth()
	{
		depthImg = kinect.getDepth();
	}

}
