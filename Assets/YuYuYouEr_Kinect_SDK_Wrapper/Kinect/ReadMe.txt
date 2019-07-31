==========================================================================
YuYuYouEr_Kinect_SDK_Wrapper v2.0.0.5
社区免费版
-----------------------------------------

Unity3D 分为32位版本和64位版本，
请将YuYuYouEr_Kinect_SDK_Wrapper_Kinect_V1(或V2)_x86(或x64)目录中所有文件复制到系统盘Windows目录中，
注意：请不要放在System32目录，直接丢在Windows目录中。



如果遇到使用问题，请联系官方博客求助：http://www.YuYuYouEr.cn


=========================================

函数说明详见博客页面：

http://www.yuyuyouer.cn/blog/?page_id=369


==============================================

官方网站：http://www.YuYuYouEr.cn

官方QQ群：322609996

--------------------------------
	
功能说明：

本版插件同时支持Kinect一代与二代硬件设备，开发者只需安装好相应设备驱动即可。

插件所需的DLL放在如下目录中：

YuYuYouEr_Kinect_SDK_Wrapper_Kinect_V1_x64：Kinect一代插件DLL 64位版本，封装SDK v1.8
YuYuYouEr_Kinect_SDK_Wrapper_Kinect_V1_x86：Kinect一代插件DLL 32位版本，封装SDK v1.8

YuYuYouEr_Kinect_SDK_Wrapper_Kinect_V2_x64：Kinect二代插件DLL 64位版本，封装SDK v2.0-1409
YuYuYouEr_Kinect_SDK_Wrapper_Kinect_V2_x86：Kinect二代插件DLL 32位版本，封装SDK v2.0-1409

--------------------------------
Kinect二代接口特性：
	对Kinect for windows SDK v2.0-1409进行封装

	支持自定义用户管理器，内置单用户/双用户示例
	支持用户进入/离开事件

	支持骨骼数据录制/重放，方便骨骼动作算法调试

	支持深度图、彩色视频
	支持SDK v2.0背景移除接口

	单人骨骼数据、支持双人骨骼
	支持单人握拳检测、支持双人握拳检测

	支持坐标系转换：骨骼数据 <-> 深度图像 -> 彩色图像

	支持Enable开关，运行时暂停某些特性

	提供32位、64位两份DLL

--------------------------------
Kinect一代接口特性：
	对Kinect for windows SDK v1.8进行封装

	支持自定义用户管理器，内置单用户/双用户示例
	支持用户进入/离开事件

	支持骨骼数据录制/重放，方便骨骼动作算法调试

	支持深度图、彩色视频
	支持SDK v1.8背景移除接口

	单人骨骼数据、支持双人骨骼
	支持单人握拳检测、支持双人握拳检测

	支持Kinect马达调整仰视、俯视角度

	支持人脸检测（缩放、位置、旋转）
	支持语音识别
	支持坐标系转换：骨骼数据 <-> 深度图像 -> 彩色图像

	支持Enable开关，运行时暂停某些特性

	提供32位、64位两份DLL
