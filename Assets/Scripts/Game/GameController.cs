using ARPeerToPeerSample.Network;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARPeerToPeerSample.Game
{
    public class GameController : MonoBehaviour
    {
        private enum GameState
        {
            Searching,
            PlaneFound,
            SearchingForSharedPlane
        }

        private NetworkManagerBase _networkManager;

        [SerializeField, Tooltip("Wifi object for Android")]
        private GameObject _androidWifiObject;

        [SerializeField, Tooltip("Menu view logic object")]
        private MenuViewLogic _menuViewLogic;

        [SerializeField, Tooltip("Anchor object")]
        private GameObject _anchorPrefab;

        [SerializeField]
        private ARHitController _arHitController;

        [SerializeField, Tooltip("AR Foundation Session")]
        private ARSession _arSession;

        [SerializeField]
        private ARPlaneManager _planeManager;

        [SerializeField, Tooltip("Relative spawned object prefab")]
        private GameObject _anchoredObjectsToSpawn;

        private GameObject _anchor;
        private ARPlane _arPlane;
        private TrackableId _planeToFind;
        private GameState _gameState;

        private void Awake()
        {
#if UNITY_ANDROID
            GameObject androidNetworkGO = Instantiate(_androidWifiObject);
            _networkManager = new NetworkManagerAndroid(androidNetworkGO.GetComponent<WifiDirectImpl>());
#elif UNITY_IOS
            _networkManager = new NetworkManageriOS(_arSession);
#endif
            _networkManager.ServiceFound += OnServiceFound;
            _networkManager.ConnectionEstablished += OnConnectionEstablished;
            _networkManager.ColorChangeMessageRecieved += OnColorChangeMessageReceived;
            _networkManager.AnchorRecieved += OnAnchorRecieved;
            _networkManager.ObjectSpawned += OnObjectSpawned;
            _networkManager.Start();

            _menuViewLogic.ConnectionButtonPressed += OnConnectionButtonPressed;
            _menuViewLogic.ChangeColorButtonPressed += OnChangeColorAndSendMessage;
            _menuViewLogic.SendWorldMapButtonPressed += OnSendWorldMap;
            _planeManager.planesChanged += OnPlanesChanged;

            _anchor = Instantiate(_anchorPrefab);
            _anchor.SetActive(false);

            _gameState = GameState.Searching;
            // uncomment to unit test packet serialization
            //print("color serialization result: " + _networkManager.TestColorSerialization() + " network package: "  + _networkManager.TestNetworkPacketSerialization());
        }

        private void Update()
        {
            ARRaycastHit hitInfo; ARPlane trackedPlane;
            if (_arHitController.CheckHitOnPlane(out hitInfo, out trackedPlane))
            {
                print("found hit on plane: " + hitInfo.pose.position);
                if (_gameState == GameState.Searching)
                {
                    _gameState = GameState.PlaneFound;
                    _networkManager.SendAnchor(trackedPlane);
                    _arPlane = trackedPlane;
                }
                else if (_gameState == GameState.PlaneFound)
                {
                    GameObject spawnedObject = SpawnObject(hitInfo.pose);

                    // since object is parented to an anchor, we send local info since on recieving end it should also be parented to an anchor
                    _networkManager.SendModelSpawn(spawnedObject.transform.localPosition, spawnedObject.transform.localRotation);
                }
            }
        }

        private void SetAnchorToPlane(ARPlane plane)
        {
            _anchor.SetActive(true);
            _anchor.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            _anchor.transform.SetParent(plane.transform);
        }

        private GameObject SpawnObject(Pose pose)
        {
            GameObject spawnedObject = Instantiate(_anchoredObjectsToSpawn, pose.position, pose.rotation);
            spawnedObject.transform.SetParent(_arPlane.transform, true);
            spawnedObject.transform.localPosition = new Vector3(spawnedObject.transform.localPosition.x, spawnedObject.transform.localPosition.y + .25f, spawnedObject.transform.localPosition.z);
            return spawnedObject;
        }

        private void OnServiceFound(string serviceAddress)
        {
            _menuViewLogic.SetConnectionName(serviceAddress);
        }

        private void OnConnectionButtonPressed()
        {
            _networkManager.Connect();
        }

        private void OnConnectionEstablished()
        {
            _menuViewLogic.SetStateConnectionEstablished();
        }

        private void OnColorChangeMessageReceived(Color color)
        {
            print("received color: " + color.ToString());
            SetColor(_anchor.GetComponent<Renderer>(), color);
        }

        private void OnChangeColorAndSendMessage()
        {
            Color colorToSend;
            int colorToSendNum = UnityEngine.Random.Range(0, 3);
            if (colorToSendNum == 0)
            {
                colorToSend = Color.red;
            }
            else if (colorToSendNum == 1)
            {
                colorToSend = Color.blue;
            }
            else
            {
                colorToSend = Color.green;
            }

            SetColor(_anchor.GetComponent<Renderer>(), colorToSend);
            _networkManager.SendColorMessage(colorToSend);
        }

        private void OnSendWorldMap()
        {
            if (_networkManager is NetworkManageriOS networkManageriOS)
            {
                networkManageriOS.SendWorldMap();
            }
        }

        private void SetColor(Renderer renderer, Color color)
        {
            var block = new MaterialPropertyBlock();

            // You can look up the property by ID instead of the string to be more efficient.
            block.SetColor("_BaseColor", color);

            // You can cache a reference to the renderer to avoid searching for it.
            renderer.SetPropertyBlock(block);
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs aRPlanesChangedEventArgs)
        {
            string[] planes = new string[_planeManager.trackables.count];
            int counter = 0;
            foreach (ARPlane aRPlane in _planeManager.trackables)
            {
                if (_gameState == GameState.SearchingForSharedPlane && _planeToFind.Equals(aRPlane.trackableId))
                {
                    _gameState = GameState.PlaneFound;
                    _arPlane = aRPlane;
                    SetAnchorToPlane(_arPlane);
                }

                planes[counter] = aRPlane.trackableId.ToString();
                ++counter;
            }

            _menuViewLogic.UpdatePlaneList(planes);
        }

        private void OnAnchorRecieved(TrackableId trackableId)
        {
            ARPlane anchorPlane;
            if (_planeManager.trackables.TryGetTrackable(trackableId, out anchorPlane))
            {
                _arPlane = anchorPlane;
                SetAnchorToPlane(_arPlane);
                _gameState = GameState.PlaneFound;
            }
            else
            {
                _planeToFind = trackableId;
                _gameState = GameState.SearchingForSharedPlane;
            }
        }

        private void OnObjectSpawned(Pose objectPose)
        {
            // even if plane has not synced yet, anchor still exists
            SpawnObject(objectPose);
        }
    }
}
