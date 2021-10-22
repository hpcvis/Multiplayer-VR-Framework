﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity;
using System;
using Valve.VR;

//Used to dont destroy on load for the player. Attached to the player
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager inst;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene. Do not set manually")]
    public GameObject LocalPlayerInstance;

    void Awake()
    {
        if (!inst)
        {
            inst = this;
        }
        //if (this.GetComponent<PhotonView>().IsMine)
        //{
        //    PlayerManager.LocalPlayerInstance = this.gameObject;
        //}
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Sets the local player instance. For use in any instantiation scripts only.
    /// </summary>
    /// <param name="localPlayer">Instance of a Networked player that is the local player</param>
    public void SetLocalPlayerInstance(GameObject localPlayer)
    {
        LocalPlayerInstance = localPlayer;
    }

    /// <summary>
    /// Destroy the local player (and its networked representation) when the application suddenly quits (e.g. ALT-F4)
    /// </summary>
    //private void OnApplicationQuit()
    //{
    //    if (LocalPlayerInstance)
    //        Destroy(LocalPlayerInstance);
    //}


    // weird garbage collecter idea:
    //public struct PlayerNetworkObjects
    //{
    //    public bool playerIsConnected;
    //    public GameObject[] networkedRepresentations;
    //}

    //public PlayerNetworkObjects[] networkObjects;

    //// perhaps only run this every few seconds
    //private void Update()
    //{
    //    foreach (PlayerNetworkObjects obj in networkObjects)
    //    {
    //        if (!obj.playerIsConnected)
    //        {
    //            for (int i = 0; i < obj.networkedRepresentations.Length; i++)
    //            {
    //                PhotonNetwork.Destroy(obj.networkedRepresentations[i]);
    //            }
    //        }
    //    }
    //}
}
