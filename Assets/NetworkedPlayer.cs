using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that implements an instance of a SteamVR player over the network.
/// NetworkedPlayer represents a SteamVR player, as well as components of the player that should be seen over the network,
/// such as the player's head and hands, and synchronizes the positions and andimations of these components.
/// </summary>

public class NetworkedPlayer : Valve.VR.InteractionSystem.Player
{
    public GameObject remotePlayerHeadPrefab;
    public GameObject remotePlayerLeftHandPrefab;
    public GameObject remotePlayerRightHandPrefab;
    public Transform cameraTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    // public for debug purposes
    public GameObject networkedPlayerHead;
    public GameObject networkedPlayerLeftHand;
    public GameObject networkedPlayerRightHand;

    /// <summary>
    /// Instantates network representations of the player (head, hands)
    /// Instantiation done here since Awake() and Start() are private members of Valve.VR.InteractionSystem.Player 
    /// </summary>
    private void OnEnable()
    {
        networkedPlayerHead = PhotonNetwork.Instantiate(
            remotePlayerHeadPrefab.name,
            cameraTransform.position,
            cameraTransform.rotation);
        networkedPlayerLeftHand = PhotonNetwork.Instantiate(
            remotePlayerLeftHandPrefab.name,
            leftHandTransform.position,
            leftHandTransform.rotation);
        networkedPlayerRightHand = PhotonNetwork.Instantiate(
            remotePlayerRightHandPrefab.name,
            rightHandTransform.position,
            rightHandTransform.rotation);
    }

    /// <summary>
    /// Calls SteamVR player Update() function and synchronizes the positions of the network object representations.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (networkedPlayerHead)
        {
            SyncNetworkComponent(networkedPlayerHead, cameraTransform);
        }
        if (networkedPlayerLeftHand)
        {
            SyncNetworkComponent(networkedPlayerLeftHand, leftHandTransform);
        }
        if (networkedPlayerRightHand)
        {
            SyncNetworkComponent(networkedPlayerRightHand, rightHandTransform);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Copies transforms of the local player to the network representations.
    /// </summary>
    /// <param name="networkRepresentation"></param>
    /// <param name="sourceTransform"></param>
    private void SyncNetworkComponent(GameObject networkRepresentation, Transform sourceTransform)
    {
        networkRepresentation.transform.position = sourceTransform.position;
        networkRepresentation.transform.rotation = sourceTransform.rotation;
    }

    /// <summary>
    /// Destroys the networked representations of each object.
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("NetworkedPlayer::OnDestroy()");
        PhotonNetwork.Destroy(networkedPlayerHead);
        PhotonNetwork.Destroy(networkedPlayerLeftHand);
        PhotonNetwork.Destroy(networkedPlayerRightHand);
    }

    /// <summary>
    /// Destroys the networked representations of each object in the case that the application quits.
    /// Needs to be done, since OnDestroy is called after OnApplicationQuit, and Photon is disconnected in OnApplicationQuit
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("NetworkedPlayer::OnApplicationQuit()");
        PhotonNetwork.Destroy(networkedPlayerHead);
        PhotonNetwork.Destroy(networkedPlayerLeftHand);
        PhotonNetwork.Destroy(networkedPlayerRightHand);
    }
}
