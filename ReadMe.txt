Introduction
=========
This is a project for computer vision class. Use finger detection in Kinect to simulate touch interaction with Windows 8. 

Prerequisites 
=============
-Kinect for XBOX360
-Visual Studio 11
-.Net Framework 4.5

Instructions
============
-use Visual Studio 11
-open CCT.NUI.sln
-set CCT.NUI.WPFSamples as startup project
-run (F5)

Notes
=====
-This worked well on Kinect for XBOX360. If you have Kinect for Window device, you may want to turn on the Near mode.
  -go to CCT.NUI.KinectSDK\KinectSensorAdapter.cs
  -add the following line to the constructor method 
    
    this.sensor.DepthStream.Range = DepthRange.Near;

Help
====
For more information, please go to:

Windows 8 Interaction
  Source: https://github.com/an83/KinectTouch2

Kinect Finger Detection
  Source: http://candescentnui.codeplex.com
  Blog: http://blog.candescent.ch