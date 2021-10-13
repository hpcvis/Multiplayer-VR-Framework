using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;

/**
 * DESCRIPTION:
 * A very important script, it disables other player's scripts and behaviors that would otherwise interfere with gameplay
 * Photon works by creating another player when someone else joins that room, and Unity by default will override your current player with that new player
 * However it will still maintain some dependencies to both your current player and your old player, since it believes that you own both
 * Therefore you need to disable every script that isn't your own, this includes other camera's, controller movement scripts, etc.
 * Otherwise, you will get interesting scenarios of swapping positions, controlling other player's scripts (like movement which is NOT recommended), and you owning local instances of both your own and their hands
 * 
 * INSTRUCTIONS:
 * To properly disable something: 
 * 0. Put the disable script onto the player 
 * 1. Go to the disable script in the editor
 * 2. Go to the section of elements that are from the other player
 * 3. Adjust the size of the according array to match the amount of elements that you will disable
 * 4. Drag and drop the element that you want to disable into the script
 * 5. Repeat until all elements of the other player are in the array
 * 
 * INSTRUCTIONS FOR FALLBACK:
 * 0. Add the rollback player prefab to the parent class of the player and disable it in the editor
 * 1. Add EnableFallback.cs to both an object that the VR player depends on (in this case, it is put on the [SteamVR] component of the player)
 *    and also to the rollback player
 * 2. Check Is Fall Back on the rollback player script, and leave if off for the VR player
 */

public class Disable : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public Photon.Pun.PhotonView m_PhotonView;

    public GameObject fallbackObjects;
    public GameObject steamVRObjects;
    /*The behaviors to disable, values must be entered into here from the editor*/
    public List<Behaviour> disable;
    /*The colliders to disable, values must be entered into here from the editor*/
    public List<MeshRenderer> disableLocalRenderComponents;



    public bool usingFallback = false;


    // Start is called before the first frame update
    public void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            this.enabled = false;
        }

        if (!OpenVR.IsHmdPresent())
            useAmRpc();
        else
            if (!usingFallback)
            useNotRpc();

        SteamVR_Behaviour_Pose[] getHandPose;
        Hand[] getHands;
        HandAnimation[] getHandAnimation;
        try
        {
            //These scripts are necessary to get at runtime, otherwise it will break every build
            getHandPose = GetComponentsInChildren<SteamVR_Behaviour_Pose>();
            getHands = GetComponentsInChildren<Hand>();
            getHandAnimation = GetComponentsInChildren<HandAnimation>();
            disable.Add(getHandPose[0]);
            disable.Add(getHands[0]);
            disable.Add(getHandAnimation[0]);
            disable.Add(getHandPose[1]);
            disable.Add(getHands[1]);
            disable.Add(getHandAnimation[1]);
        }
        catch
        {
            Debug.LogError("Error: Attempted to access player hands at Disable.cs and failed, maybe a controller was turned off?");
        }
        disable.Add(GetComponentInChildren<Camera>());
        disable.Add(GetComponentInChildren<AudioListener>());
        if (!m_PhotonView.IsMine)
        {
            for (int i = 0; i < disable.Count; i++)
            {
                disable[i].enabled = false;
            }
        }
        foreach(MeshRenderer findPointer in GetComponentsInChildren<MeshRenderer>())
        {
            if (findPointer.gameObject.transform.parent.CompareTag("EventCamera"))
                disableLocalRenderComponents.Add(findPointer);
        }
        if (!m_PhotonView.IsMine)
        {
            for (int i = 0; i < disableLocalRenderComponents.Count; i++)
            {
                disableLocalRenderComponents[i].enabled = false;
            }
        }
    }


    private void Update()
    {
        if (this.photonView.Owner == null)
        {
            disableThisPlayer();
        }

        if (!m_PhotonView.IsMine && usingFallback && !fallbackObjects.activeSelf)
        {
            fallbackObjects.SetActive(true);
            steamVRObjects.SetActive(false);
        }
        else if (!m_PhotonView.IsMine && !usingFallback && !steamVRObjects.activeSelf)
        {
            fallbackObjects.SetActive(false);
            steamVRObjects.SetActive(true);
        }
    }


    public void useAmRpc()
    {
        usingFallback = true;
        this.photonView.RPC("amUsingFallBack", RpcTarget.OthersBuffered);
    }
    public void useNotRpc()
    {
        usingFallback = false;
        this.photonView.RPC("amNotUsingFallBack", RpcTarget.OthersBuffered);
    }


    [PunRPC]
    public void amUsingFallBack()
    {
        if (!m_PhotonView.IsMine)
        {
            fallbackObjects.SetActive(true);
            usingFallback = true;
            steamVRObjects.SetActive(false);
        }
    }

    [PunRPC]
    public void amNotUsingFallBack()
    {
        if (!m_PhotonView.IsMine)
        {
            fallbackObjects.SetActive(false);
            steamVRObjects.SetActive(true);
            usingFallback = false;
        }
    }
 
    
    public void disableThisPlayer()
    {
        gameObject.SetActive(false);
    }


}