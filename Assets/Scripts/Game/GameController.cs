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

        private GameObject _anchor;

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
            _networkManager.Start();

            _menuViewLogic.ConnectionButtonPressed += OnConnectionButtonPressed;
            _menuViewLogic.ChangeColorButtonPressed += OnChangeColorAndSendMessage;
            _menuViewLogic.SendWorldMapButtonPressed += OnSendWorldMap;

            _anchor = Instantiate(_anchorPrefab);
            _anchor.SetActive(false);

            // uncomment to unit test packet serialization
            //print("color serialization result: " + _networkManager.TestColorSerialization() + " network package: "  + _networkManager.TestNetworkPacketSerialization());
        }

        private void Update()
        {
            ARRaycastHit hitInfo; ARPlane trackedPlane;
            if (_arHitController.CheckHitOnPlane(out hitInfo, out trackedPlane))
            {
                print("found hit on plane: " + hitInfo.pose.position);
                _anchor.SetActive(true);
                _anchor.transform.localPosition = new Vector3(0f, 0.25f, 0f);
                _anchor.transform.SetParent(trackedPlane.transform);
                _networkManager.SendAnchor(trackedPlane);
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
    }
}