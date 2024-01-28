using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rect worldBounds;
    Vector2 cameraSize;
    Transform follow;

    // Update is called once per frame
    void Update()
    {
        if (!follow)
        {
            SearchForplayer();
        }

        cameraSize = new Vector2(Camera.main.aspect, 1) * Camera.main.orthographicSize;
    }

    private void LateUpdate()
    {
        if (follow)
        {
            transform.position = new Vector3(
                Mathf.Clamp(follow.position.x, worldBounds.x + cameraSize.x, worldBounds.x + worldBounds.width - cameraSize.x),
                Mathf.Clamp(follow.position.y, worldBounds.y - worldBounds.height + cameraSize.y, worldBounds.y - cameraSize.y),
                -10);

        }
    }

    private void SearchForplayer()
    {
        foreach (PlayerInput player in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                follow = player.gameObject.transform;
            }
        }
    }
}
