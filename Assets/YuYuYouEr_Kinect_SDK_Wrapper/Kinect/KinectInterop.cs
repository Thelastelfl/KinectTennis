using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kinect {
	
	public interface KinectInterface {

		bool hasUserIn();
		bool pollSkeleton();
		NuiSkeletonFrame getSkeleton();
		bool getIsLeftHandGrip(int i);
		bool getIsRightHandGrip(int i);
		
		Vector4[] getSkeleton_defaultUser();
		bool getIsLeftHandGrip_defaultUser();
		bool getIsRightHandGrip_defaultUser();

		bool pollBackgroundRemoval();
		Color32[] getBackgroundRemoval();
		Color32[] getBackgroundRemovalTexture();
		
		bool pollColor();
		Color32[] getColor();
		
		bool pollDepth();
		short[] getDepth();
		
	}
	
	public static class Constants
	{
		public static int NuiSkeletonCount = 6;
    	public static int NuiSkeletonMaxTracked = 6;
	}
	
	/// <summary>
	///Structs and constants for interfacing c# with the c++ kinect dll 
	/// </summary>

    public enum NuiSkeletonPositionIndex : int
    {
        HipCenter = 0,
        Spine,
        ShoulderCenter,
        Head,
        ShoulderLeft,
        ElbowLeft,
        WristLeft,
        HandLeft,
        ShoulderRight,
        ElbowRight,
        WristRight,
        HandRight,
        HipLeft,
        KneeLeft,
        AnkleLeft,
        FootLeft,
        HipRight,
        KneeRight,
        AnkleRight,
        FootRight,
        Count
    }

    public enum NuiSkeletonPositionTrackingState
    {
        NotTracked = 0,
        Inferred,
        Tracked
    }

    public enum NuiSkeletonTrackingState
    {
        NotTracked = 0,
        PositionOnly,
        SkeletonTracked
    }

	public enum NuiImageResolution
	{
		resolutionInvalid = -1,
		resolution80x60 = 0,
		resolution320x240,
		resolution640x480,
		resolution1280x1024                        // for hires color only
	}

    /*[StructLayoutAttribute(LayoutKind.Sequential)]
    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }*/

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct NuiSkeletonData
    {
        public NuiSkeletonTrackingState eTrackingState;
        public uint dwTrackingID;
        public uint dwEnrollmentIndex_NotUsed;
        public uint dwUserIndex;
        public Vector4 Position;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
        public Vector4[] SkeletonPositions;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
        public NuiSkeletonPositionTrackingState[] eSkeletonPositionTrackingState;
        public uint dwQualityFlags;
    }
	
    public struct NuiSkeletonFrame
    {
        public Int64 liTimeStamp;
        public uint dwFrameNumber;
        public uint dwFlags;
        public Vector4 vFloorClipPlane;
        public Vector4 vNormalToGravity;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
        public NuiSkeletonData[] SkeletonData;
    }
	
	public struct NuiTransformSmoothParameters
	{
		public float fSmoothing;
		public float fCorrection;
		public float fPrediction;
		public float fJitterRadius;
		public float fMaxDeviationRadius;
	}
	
	[Serializable]
	public struct SerialVec4 {
		float x,y,z,w;
		
		public SerialVec4(Vector4 vec){
			this.x = vec.x;
			this.y = vec.y;
			this.z = vec.z;
			this.w = vec.w;
		}
		
		public Vector4 deserialize() {
			return new Vector4(x,y,z,w);
		}
	}
	
	[Serializable]
	public struct SerialSkeletonData {
		public NuiSkeletonTrackingState eTrackingState;
        public uint dwTrackingID;
        public uint dwEnrollmentIndex_NotUsed;
        public uint dwUserIndex;
        public SerialVec4 Position;
        public SerialVec4[] SkeletonPositions;
        public NuiSkeletonPositionTrackingState[] eSkeletonPositionTrackingState;
        public uint dwQualityFlags;
		
		public SerialSkeletonData (NuiSkeletonData nui) {
			this.eTrackingState = nui.eTrackingState;
	        this.dwTrackingID = nui.dwTrackingID;
	        this.dwEnrollmentIndex_NotUsed = nui.dwEnrollmentIndex_NotUsed;
	        this.dwUserIndex = nui.dwUserIndex;
	        this.Position = new SerialVec4(nui.Position);
	        this.SkeletonPositions = new SerialVec4[20];
			for(int ii = 0; ii < 20; ii++){
				this.SkeletonPositions[ii] = new SerialVec4(nui.SkeletonPositions[ii]);
			}
	        this.eSkeletonPositionTrackingState = nui.eSkeletonPositionTrackingState;
	        this.dwQualityFlags = nui.dwQualityFlags;
		}
		
		public NuiSkeletonData deserialize() {
			NuiSkeletonData nui = new NuiSkeletonData();
			nui.eTrackingState = this.eTrackingState;
	        nui.dwTrackingID = this.dwTrackingID;
	        nui.dwEnrollmentIndex_NotUsed = this.dwEnrollmentIndex_NotUsed;
	        nui.dwUserIndex = this.dwUserIndex;
	        nui.Position = this.Position.deserialize();
	        nui.SkeletonPositions = new Vector4[20];
			for(int ii = 0; ii < 20; ii++){
				nui.SkeletonPositions[ii] = this.SkeletonPositions[ii].deserialize();
			}
	        nui.eSkeletonPositionTrackingState = this.eSkeletonPositionTrackingState;
	        nui.dwQualityFlags = this.dwQualityFlags;
			return nui;
		}
	}
	
	[Serializable]
	public struct SerialSkeletonFrame
	{
		public Int64 liTimeStamp;
        public uint dwFrameNumber;
        public uint dwFlags;
        public SerialVec4 vFloorClipPlane;
        public SerialVec4 vNormalToGravity;
        public SerialSkeletonData[] SkeletonData;
		
		public SerialSkeletonFrame (NuiSkeletonFrame nui) {
			this.liTimeStamp = nui.liTimeStamp;
			this.dwFrameNumber = nui.dwFrameNumber;
			this.dwFlags = nui.dwFlags;
			this.vFloorClipPlane = new SerialVec4(nui.vFloorClipPlane);
			this.vNormalToGravity = new SerialVec4(nui.vNormalToGravity);
			this.SkeletonData = new SerialSkeletonData[6];
			for(int ii = 0; ii < 6; ii++){
				this.SkeletonData[ii] = new SerialSkeletonData(nui.SkeletonData[ii]);
			}
		}
		
		public NuiSkeletonFrame deserialize() {
			NuiSkeletonFrame nui = new NuiSkeletonFrame();
			nui.liTimeStamp = this.liTimeStamp;
			nui.dwFrameNumber = this.dwFrameNumber;
			nui.dwFlags = this.dwFlags;
			nui.vFloorClipPlane = this.vFloorClipPlane.deserialize();
			nui.vNormalToGravity = this.vNormalToGravity.deserialize();
			nui.SkeletonData = new NuiSkeletonData[6];
			for(int ii = 0; ii < 6; ii++){
				nui.SkeletonData[ii] = this.SkeletonData[ii].deserialize();
			}
			return nui;
		}
	}

	public class NativeMethods
	{
        /*
		 * kinect switch functions
		 */

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern bool qfKinectGetEnableMultiUser();

		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern void qfKinectSetEnableMultiUser(bool bEnable);

		
		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern bool qfKinectGetEnableFaceTracking();

		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern void qfKinectSetEnableFaceTracking(bool bEnable);

		
		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern bool qfKinectGetEnableSpeechRecognition();

		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern void qfKinectSetEnableSpeechRecognition(bool bEnable);

		
		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern bool qfKinectGetEnableKinectInteractive();

		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern void qfKinectSetEnableKinectInteractive(bool bEnable);

		
		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern bool qfKinectGetEnableBackgroundRemoval();

		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
		public static extern void qfKinectSetEnableBackgroundRemoval(bool bEnable);
		


		/* 
		 * kinect NUI (general) functions
		 */
				
		[DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
	    public static extern int qfKinectInit();

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectUnInit();
		
		/* 
		 * kinect image functions
		 */

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopyVideoData(byte[/*640*480*4*/] data);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopyDepthData(byte[/*320*240*4*/] data);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopyBackgroundRemovalData(byte[/*640*480*4*/] data);
		
		/*
		 * kinect skeleton functions
		 */

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopySkeletonData(float[/*20*4*/] data);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopyMultiSkeletonData(float[/*6*/, /*20*4*/] data, int[] userID);

		/*
		 * kinect size functions
		 */
        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectGetDepthWidth();

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectGetDepthHeight();

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectGetVideoWidth();

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectGetVideoHeight();

		/*
		 * kinect face tracking functions
		 */
        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopyFaceTrackResult(float[/*1*/] scale
		                                                     , float[/*3*/] rotationXYZ, float[/*3*/]translationXYZ
		                                                     , long[/*2*/] translationColorXY);


		/*
		 * kinect speech functions
		 */

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectInitSpeech(int languageCode);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectUnInitSpeech();

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopySpeechReslut(byte[/*1024*/] strResult);
		
		/*
		 * kinect interaction functions
		 */
        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopyHandEventReslut(byte[/*2*/] handEvent);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectCopyMultiHandEventReslut(byte[/*6*/, /*2*/] handEvent);

		/*
		 * kinect transform functions
		 */
        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectTransformSkeletonToDepthImage(float[/*4*/] positionXYZW
                                                     , int[/*1*/] plDepthX, int[/*1*/] plDepthY, short[/*1*/] pusDepthValue);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectTransformSkeletonToVideoImage(float[/*4*/] positionXYZW
                                                     , int[/*1*/] plColorX, int[/*1*/] plColorY);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectTransformDepthImageToSkeleton(float[/*4*/] positionXYZW
                                                     , int lDepthX, int lDepthY, short usDepthValue);

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int qfKinectTransformDepthImageToVideoImage(int lDepthX, int lDepthY, short usDepthValue
                                                     , int[/*1*/] plColorX, int[/*1*/] plColorY);

        /*
         * µ÷ÕûÑö¸©½Ç¶È
         */
        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern int getElevationAngle();

        [DllImport("YuYuYouEr_Kinect_SDK_Wrapper")]
        public static extern bool setElevationAngle(int angle);

	}
	
}
