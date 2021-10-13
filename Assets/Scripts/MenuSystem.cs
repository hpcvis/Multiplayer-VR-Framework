﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

//This is the options system for the menu integrated in both the template and example scene.
//This script is meant to be put onto the canvas, put it can work and can be put onto anything
//However since this interacts with the given menu specifically, it is better to put it there
//
//How this works:
//The function toggle is called inside a button, where the button has an onclick event that will call toggle
//At that point, it will pass in a string of the function that it wants to call (aka the name in string)
//And then given this code, it implements that feature.
//
//Note how the text on the button isn't the string that is being sent
//Note how spawn balls is not here (but is on the options menu in the "Template" scene), this was done to show that it is possible to call things outside of this menu (for reference it is in Instantiation.cs)
//However, it is not in an incredibly obvious spot, so despite both being adequate uses of the menu system, this usage is easier to follow

public class MenuSystem : MonoBehaviour
{
    public GameObject VoiceToggle;
    public GameObject STTToggle;
    private LineRenderer PointerSystem;
    private MeshRenderer PointerBall;

    public void Toggle(string name)
    {
        switch (name){
            case "Voice":
                if (VoiceToggle.activeSelf)
                {
                    VoiceToggle.SetActive(false);
                }
                else
                {
                    VoiceToggle.SetActive(true);
                }
                break;

            case "STT": //STT stands for Speech-To-Text
                if (STTToggle.activeSelf)
                {
                    STTToggle.SetActive(false);
                }
                else
                {
                    STTToggle.SetActive(true);
                }
                break;

            case "Pointer":
                try
                {
                    PointerSystem = GameObject.FindGameObjectWithTag("EventCamera").GetComponent<LineRenderer>();
                    PointerBall = PointerSystem.GetComponentInChildren<MeshRenderer>();
                    if (PointerSystem.enabled)
                    {
                        PointerSystem.enabled = false;
                    }
                    else
                    {
                        PointerSystem.enabled = true;
                    }
                    if (PointerBall.enabled)
                    {
                        PointerBall.enabled = false;
                    }
                    else
                    {
                        PointerBall.enabled = true;
                    }
                }
                catch (Exception)
                {
                    if (GameObject.FindGameObjectWithTag("FallbackPlayer").activeSelf)
                        Debug.Log("Pointer can not be disabled on Fallback Player since a Pointer does not exist on the Fallback Player");
                    else
                        Debug.LogError("Error: Line renderer for VR Player not found, not able to toggle Pointer");
                }

                break;

            case "Teleport":
                if (PlayerManager.inst.LocalPlayerInstance.GetPhotonView().IsMine) //Teleport the player that selects the button only
                {
                    PlayerManager.inst.LocalPlayerInstance.transform.position = new Vector3(-5f, 1.1f, 0f);
                }
                else
                {
                    Debug.Log("Error: " + PlayerManager.inst.LocalPlayerInstance.GetPhotonView().IsMine + " ");
                }
                break;

            case "Scene Transition":
                LoadNewScene("Example Scene");
                break;

            case "Return":
                LoadNewScene("Template");
                break;

            default:
                Debug.LogError("Menu item \"toggle\" not named correctly in button menu");
                break;
        }
    }
  

    //This is to tell the photon network to load a new scene in
    //The master client needs to be the one to load in the new level, non-master clients can not instantiate and automatically synchronize new levels
    //
    //How this works:
    //It takes in a scene name and it looks to see if it is valid, if it is valid then it checks to see if you are the master client
    //If you are not the master client, it will not work, if you are, then a new scene will be loaded in.
    //If the scene name doesn't exist, then you passed in a wrong scene name.

    public void LoadNewScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Example Scene":
                if (!PhotonNetwork.IsMasterClient)
                { 
                    Debug.LogError("Can not load new scene if not the master client"); 
                }
                else if (PlayerManager.inst.LocalPlayerInstance.GetPhotonView().IsMine)
                {
                    PhotonNetwork.LoadLevel(sceneName);
                }
                break;

            case "Template":
                if (!PhotonNetwork.IsMasterClient)
                {
                    Debug.LogError("Can not load new scene if not the master client");
                }
                else if (PlayerManager.inst.LocalPlayerInstance.GetPhotonView().IsMine)
                {
                    PhotonNetwork.LoadLevel(sceneName);
                }
                break;

            default:
                Debug.LogError("Menu item \"load scene\" not named correctly in button menu");
                break;
        }
    }
}
