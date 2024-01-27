using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkOptions : Singleton<NetworkOptions>
{
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

    public void Ping()
    {

    }
}
