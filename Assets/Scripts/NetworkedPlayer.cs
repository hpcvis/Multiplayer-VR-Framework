using Photon.Pun;
using Photon.Pun.Simple;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that implements an instance of a SteamVR player over the network.
/// NetworkedPlayer represents a SteamVR player, as well as components of the player that should be seen over the network,
/// such as the player's head and hands, and synchronizes the positions and andimations of these components.
/// </summary>

public class NetworkedPlayer : Valve.VR.InteractionSystem.Player, IOnPreQuit
{
    public GameObject remotePlayerHeadPrefab;
    public GameObject remotePlayerHandPrefab;
    public Transform cameraTransform;

    public Transform[] handTransforms;
    public Animator[] handAnimators;

    // public for debug purposes
    public GameObject networkedPlayerHead;
    public GameObject[] networkedHands;
    public Animator[] networkedHandAnimators;

    /// <summary>
    /// Instantates network representations of the player (head, hands)
    /// Instantiation done here since Awake() and Start() are private members of Valve.VR.InteractionSystem.Player 
    /// </summary>
    private void OnEnable()
    {
        CreateNetworkedRepresentation();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        NetMasterCallbacks.onPreQuits.Add(this);
    }

    /// <summary>
    /// Calls SteamVR player Update() function and synchronizes the positions of the network object representations.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (networkedPlayerHead)
        {
            SyncNetworkTransform(networkedPlayerHead, cameraTransform);
        }
        for (int i = 0; i < networkedHands.Length; i++)
        {
            if (networkedHands[i])
            {
                SyncNetworkTransform(networkedHands[i], handTransforms[i]);
                SyncNetworkHandAnimations(networkedHandAnimators[i], handAnimators[i]);
            }
        }

        // debug
        if (Input.GetKeyDown(KeyCode.K))
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Destroys the networked representations of each object.
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("NetworkedPlayer::OnDestroy()");
        DestroyNetworkedRepresentation();
    }

    /// <summary>
    /// Destroys the networked representations of each object in the case that the application quits.
    /// Needs to be done, since OnDestroy is called after OnApplicationQuit, and Photon is disconnected in OnApplicationQuit
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("NetworkedPlayer::OnApplicationQuit()");
        DestroyNetworkedRepresentation();
    }

    /// <summary>
    /// Initializes the networked representation of the plate on scene load.
    /// Necessary, since the player object is a DontDestroyOnLoad object.
    /// Note: This is not called on game initialization, presumably because the delegates have not been assigned yet.
    /// </summary>
    /// <param name="scene">Loaded scene</param>
    /// <param name="mode">Loaded scene mode</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("NetworkedPlayer::OnSceneLoaded()");
        CreateNetworkedRepresentation();
    }

    /// <summary>
    /// Destroys the networked representation of the player object on scene unload.
    /// Necessary, since the player object is a DontDestroyOnLoad object.
    /// Note: This does not seem to be called when the game is force quit,
    ///       meaning we still have to deal with zombie network objects
    /// </summary>
    /// <param name="current">Current scene name</param>
    void OnSceneUnloaded(Scene current)
    {
        Debug.Log("NetworkedPlayer::OnSceneUnloaded()");
        DestroyNetworkedRepresentation();
    }

    public void OnPreQuit()
    {
        Debug.Log("NetworkedPlayer::OnPreQuit()");
        DestroyNetworkedRepresentation();
    }

    /// <summary>
    /// Initializes the networked representations of each object.
    /// </summary>
    public void CreateNetworkedRepresentation()
    {
        networkedPlayerHead = PhotonNetwork.Instantiate(
            remotePlayerHeadPrefab.name,
            cameraTransform.position,
            cameraTransform.rotation);

        // disable local rendering of the player head to avoid visual issues with shadows
        if (networkedPlayerHead.GetComponent<PhotonView>().IsMine)
        {
            networkedPlayerHead.GetComponent<MeshRenderer>().enabled = false;
        }

        // 0 => left hand
        // 1 => right hand
        networkedHands = new GameObject[2];
        networkedHandAnimators = new Animator[2];
        for (int i = 0; i < networkedHands.Length; i++)
        {
            networkedHands[i] = PhotonNetwork.Instantiate(
                remotePlayerHandPrefab.name,
                handTransforms[i].position,
                handTransforms[i].rotation);
            networkedHandAnimators[i] = networkedHands[i].GetComponentInChildren<Animator>();

            // disable the mesh of the networked player instance locally, since there are SteamVR hands to render
            if (networkedHands[i].GetComponent<PhotonView>().IsMine)
            {
                networkedHands[i].GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
        }

        // flip the model of the right hand so it looks like a right hand over the network
        Vector3 rightHandScale = networkedHands[1].transform.localScale;
        rightHandScale.x *= -1.0f;
        networkedHands[1].transform.localScale = rightHandScale;
    }

    /// <summary>
    /// Destroys the networked representations of each object.
    /// </summary>
    public void DestroyNetworkedRepresentation()
    {
        PhotonNetwork.Destroy(networkedPlayerHead);
        for (int i = 0; i < networkedHands.Length; i++)
        {
            PhotonNetwork.Destroy(networkedHands[i]);
        }
    }

    /// <summary>
    /// Copies transforms of the local player to the network representations.
    /// </summary>
    /// <param name="networkRepresentation"></param>
    /// <param name="sourceTransform"></param>
    private void SyncNetworkTransform(GameObject networkRepresentation, Transform sourceTransform)
    {
        networkRepresentation.transform.position = sourceTransform.position;
        networkRepresentation.transform.rotation = sourceTransform.rotation;
    }

    /// <summary>
    /// Copies animation state of the local player's hands to their network representation.
    /// </summary>
    /// <param name="networkedHand"></param>
    /// <param name="sourceHand"></param>
    private void SyncNetworkHandAnimations(Animator networkedHand, Animator sourceHand)
    {
        networkedHand.SetBool("IsGrabbing", sourceHand.GetBool("IsGrabbing"));
    }
}
