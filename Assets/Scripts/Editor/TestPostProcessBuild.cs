using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ARPeerToPeerSample.Editor
{
    public class TestPostProcessBuild : MonoBehaviour
    {
        [MenuItem("ARPeerToPeer/MovePlugins")]
        public static void MovePlugins()
        {
            PostProcessBuild.OnPostProcessBuild(BuildTarget.iOS, "/Users/blakegross/builds/ar-peer-to-peer-sample-build");
        }
    }
}