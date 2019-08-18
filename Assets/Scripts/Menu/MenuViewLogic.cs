using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuViewLogic : MonoBehaviour
{
    [SerializeField, Tooltip("Connection button")]
    private Button _connectionButton;

    [SerializeField, Tooltip("Connection button text")]
    private Text _connectionButtonText;

    [SerializeField, Tooltip("Connection status textfield")]
    private Text _connectionStatusText;

    public Action ConnectionButtonPressed;

    public Action ChangeColorButtonPressed;

    public Action SendWorldMapButtonPressed;

    private void Awake()
    {
        _connectionButton.gameObject.SetActive(false);
    }

    public void SetConnectionName(string connectionName)
    {
        _connectionButton.gameObject.SetActive(true);
        _connectionButtonText.text = "press to connect to: " + connectionName;
        _connectionStatusText.text = "Found user...";
    }

    public void OnConnectionButtonPressed()
    {
        _connectionStatusText.text = "Connecting";
        ConnectionButtonPressed?.Invoke();
    }

    public void SetStateConnectionEstablished()
    {
        _connectionStatusText.text = "Connected";
    }

    public void OnColorChangeButtonPressed()
    {
        ChangeColorButtonPressed?.Invoke();
    }

    public void OnSendWorldMapButtonPressed()
    {
#if UNITY_IOS
        SendWorldMapButtonPressed?.Invoke();
#else
        Debug.Log("World map not supported on this platform");
#endif
    }
}
