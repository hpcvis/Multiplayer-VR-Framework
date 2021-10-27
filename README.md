# Multiplayer VR Framework

##Made by:
Daniel Enriquez, Christopher Lewis, and Erik Marsh

##Description:
This package is a template for VR multiplayer with voice, interactables, and more
Multiplayer works by using Photon Unity Networking, where players, interactables, and anything you want to see update across the network have Photon Views attached.
The scene comes with a premade options menu to enable/disable features developed inside this package. 

##Interactables:
Interactables work by transfering ownership between players. If you want to make a competitive game, make the ownership transfer to the server after a user is done with it.
To add interactable objects add a prefab to the Resources directory in the assets folder. Next, give it in the interactable tag. In the instantiation.cs script add the prefab name for each interactable prefab you have
An important note is that you should disable the rigidbody and sphere collider in case the photon scene object gets insantiated before it updates, otherwise slight movement is caused. It is also recommended to remove Photonviews, as it will make the server keep track of less things.
There is a default SteamVR Interactables script. We had to edit this code to allow transfer ownership to work. There is a CustomInteractable.cs script in the root asset folder. This is a copy of the Interactable.cs script with our changes.
Adding CustomInteractable.cs to things does not work due to the Hand.cs script (valve made) needing the interactable script to work. We would need to create a customhand.cs script too and a whole bunch of other changes, this is easier.
As indicated by pins, multiple objects can be updated across the network in a GameObject container. 

##Photon Server Settings:
To edit the PhotonServerSettings and add in your own IP find the pun wizard and have it navigate you to the PhotonServerSettings. Our current server requires you uncheck "Use Name Server", have port 5055, and it's IP was 172.20.199.152.

##Options Menu:
Teleport teleports the player to the position (-5, 1.1, 0), Spawn Interactables spawns in a new set of interactable objects,
Scene Transition transitions a player into a new scene (same server room), Speech-to-text toggles enables and disables Google Cloud Streaming, Toggle Voice disables and enables Network Voice Manager, and Toggle Pointer toggles the pointer aka line renderer coming from the left hand.

##Player:
<!-- The player works by being a game object that contains both the VR player (aka [CameraRig]) component as well as a Fallback Player component. It also has Debug UI to switch between the two inside the editor. It also allows for Voice, as it is a common medium for both Fallback and VR players
The player itself has "Player.cs" from SteamVR, Photon Views/Photon Transform Views, "Disable.cs", "PlayerManager.cs", and "STTData"
Player Manager instantiates the local player instance and STTData streams speech to text data over the network (Final transcriptions)
The CameraRig class was imported from SteamVR. The components "EnableFallback.cs", Photon Views/Photon Transform Views, and the Player Pointer Game Object were added to the VR Player. -->

The player is a modified version of the SteamVR player prefab. It mainly uses the NetworkedPlayer.cs script, which inherits from the SteamVR Player.cs script.
The singleton nature of the SteamVR player object caused issues with input and camera control over the network,
so we instead create dummy objects that represent the player's head and hands.
These dummy objects are invisible to the local player, but are visible to other players.
The NetworkedPlayer.cs script copies the head transform, hand transform, and hand animation state to these dummy objects every frame.
The dummy objects have Photon Views, Photon Transform Views, and Photon Animator views to sync dummy state over the network.
This ensures that the dummies serve as an accurate visual representation of a player in the world.

##Google Cloud API Speech-To-Text
To disable Google Cloud API Speech-To-Text:
Delete the Google Cloud Streaming Object within the hierarchy. Next, ensure that the Google Cloud Streaming Speech-To-Text package is deleted

To enable Google Cloud API Speech-To-Text:
Follow the instructions listed here: https://github.com/oshoham/UnityGoogleStreamingSpeechToText
Note: All the library dependencies mentioned are required for installation. An API key is also needed, you will need to get your own by registering your account at cloud.google.com (There is a 90 day trial with $300 of credits to spend, your account will not be charged if you exceed either limit)