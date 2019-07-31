@echo OFF

echo =======================================================
echo 正在将Kinect SDK切换到2.0版本（Dev By http://www.YuYuYouEr.cn）

echo =======================================================
echo 注意事项：请先关闭Unity编辑器，否则可能切换不成功
echo .
echo .
echo 如果已经关闭
echo .


pause

echo .
echo .

echo =============================
echo 删除1.0版本相关文件
echo .

rmdir /Q /S Kinect_V1.0
del /Q /F /AH Kinect_V1.0.meta



echo .
echo .
echo =============================
echo 解压2.0版本相关文件
echo .

unzip -o Kinect_V2.0.zip


echo .
echo .
echo .
echo =============================
echo .
echo .
echo .
echo 成功切换到Kinect SDK 2.0版本，请重新启动Unity编辑器
echo .

pause
