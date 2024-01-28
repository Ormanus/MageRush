using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkOptions : Singleton<NetworkOptions>
{
    public UnityTransport transport;

    string _ip;
    int _port;

    public void ChangeIP(string ip)
    {
        _ip = ip;
        transport.ConnectionData.Address = ip;
    }

    public void ChangePort(string port)
    {
        _port = int.Parse(port);
        transport.ConnectionData.Port = (ushort)_port;
    }


    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void Move(Vector2 direction)
    {
        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<CharacterController>().MoveToDirection(direction);
        }
        else
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<CharacterController>();
            player.MoveToDirection(direction);
        }
    }
}
