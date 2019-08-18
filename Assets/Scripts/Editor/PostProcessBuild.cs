using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ARPeerToPeerSample.Editor
{
    public static class PostProcessBuild
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                string libraryPath = buildPath + "/Libraries/";
                string libraryMultipeerSourcePath = libraryPath + "/Addons/UnityARKitMultipeerConnectivity/UnityMultipeerConnectivity/";
                string libraryUnitySwiftSourcePath = libraryPath + "/Addons/UnityARKitMultipeerConnectivity/UnitySwift/";
                string multipeerDestPath = libraryPath + "UnityMultipeerConnectivity/";
                string swiftDestPath = libraryPath + "UnitySwift/";

                try
                {
                    //FileUtil.MoveFileOrDirectory(libraryMultipeerSourcePath, multipeerDestPath);
                    FileUtil.MoveFileOrDirectory(libraryUnitySwiftSourcePath, swiftDestPath);
                }
                catch(Exception e)
                {
                    Debug.Log("Failed to write file: " + e.ToString());
                }
            }
        }
    }
}
