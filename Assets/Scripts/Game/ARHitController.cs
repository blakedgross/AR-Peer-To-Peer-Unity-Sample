using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARPeerToPeerSample.Game
{
    /// <summary>
    /// Detects hits via ARFoundation
    /// </summary>
    public class ARHitController
    {
        [SerializeField]
        private ARRaycastManager _arRaycastManager;

        [SerializeField]
        private ARPlaneManager _arPlaneManager;

        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        
        public bool CheckHitOnPlane(out ARRaycastHit hitInfo, out ARPlane trackedPlane)
        {
            hitInfo = new ARRaycastHit();
            trackedPlane = null;

            if (Input.touchCount != 0)
            {
                Touch touch = Input.GetTouch(0);
                if (_arRaycastManager.Raycast(touch.position, s_Hits, TrackableType.Planes))
                {
                    // take the first since its the closest
                    ARRaycastHit raycastHit = s_Hits[0];
                    trackedPlane = _arPlaneManager.GetPlane(raycastHit.trackableId);
                    return true;
                }
            }

            return false;
        }
    }
}
