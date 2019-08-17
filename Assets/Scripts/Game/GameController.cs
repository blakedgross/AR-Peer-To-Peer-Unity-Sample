using ARPeerToPeerSample.Network;
using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARPeerToPeerSample.Game
{
    public class GameController : MonoBehaviour
    {
        private NetworkManagerBase _networkManager;

        [SerializeField, Tooltip("Wifi object for Android")]
        private GameObject _androidWifiObject;

        [SerializeField, Tooltip("Menu view logic object")]
        private MenuViewLogic _menuViewLogic;

        [SerializeField, Tooltip("Cube object")]
        private GameObject _cube;

        [SerializeField, Tooltip("Controller which detects hits")]
        private ARHitController _arHitController;

        [SerializeField, Tooltip("Anchor prefab which is the tracked objects")]
        private GameObject _anchorPrefab;

        private GameObject _anchor;

        private void Awake()
        {
#if UNITY_ANDROID
            GameObject androidNetworkGO = Instantiate(_androidWifiObject);
            _networkManager = new NetworkManagerAndroid(androidNetworkGO.GetComponent<WifiDirectImpl>());
#endif
            _networkManager.ServiceFound += OnServiceFound;
            _networkManager.ConnectionEstablished += OnConnectionEstablished;
            _networkManager.MessageReceived += OnMessageReceived;
            _networkManager.AnchorPostComplete += OnAnchorPostComplete;
            _networkManager.Start();

            _menuViewLogic.ConnectionButtonPressed += OnConnectionButtonPressed;
            _menuViewLogic.ChangeColorButtonPressed += OnChangeColorAndSendMessage;

            _anchor = Instantiate(_anchorPrefab);
            _anchor.SetActive(false);
        }

        private void Update()
        {
            ARRaycastHit hitInfo; ARPlane arPlane;

            if (_arHitController.CheckHitOnPlane(out hitInfo, out arPlane))
            {
                _anchor.transform.position = hitInfo.pose.position;
                _anchor.transform.rotation = hitInfo.pose.rotation;
                _anchor.SetActive(true);
                _menuViewLogic.SetAnchorState("Local anchor created");

                _networkManager.CreateAnchor(arPlane.nativePtr);
            }
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

        private void OnMessageReceived(string message)
        {
            print("received color: " + message);
            SetColor(_cube.GetComponent<Renderer>(), StringToColor(message));
        }

        private void OnAnchorPostComplete(string anchorStatus)
        {
            print("anchor posted: " + anchorStatus);
            _menuViewLogic.SetAnchorState(anchorStatus);
            // todo: send anchor id peer
        }

        private void OnChangeColorAndSendMessage()
        {
            string colorToSend = string.Empty;
            int colorToSendNum = UnityEngine.Random.Range(0, 3);
            if (colorToSendNum == 0)
            {
                colorToSend = "red";
            }
            else if (colorToSendNum == 1)
            {
                colorToSend = "blue";
            }
            else
            {
                colorToSend = "green";
            }

            SetColor(_cube.GetComponent<Renderer>(), StringToColor(colorToSend));
            _networkManager.SendMessage(colorToSend);
        }

        private Color StringToColor(string color)
        {
            switch (color)
            {
                case "red":
                    return Color.red;
                case "blue":
                    return Color.blue;
                case "green":
                    return Color.green;
                default:
                    return Color.magenta;
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
    }
}