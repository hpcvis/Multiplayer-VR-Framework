###Made by:
Daniel Enriquez and Christopher Lewis

###Description:
This package is a template for VR multiplayer with voice, interactables, and more
Multiplayer works by using Photon Unity Networking, where players, interactables, and anything you want to see update across the network have Photon Views attached.
The scene comes with a premade options menu to enable/disable features developed inside this package. 

###Interactables:
Interactables work by transfering ownership between players. If you want to make a competitive game, make the ownership transfer to the server after a user is done with it.
To add interactable objects add a prefab to the Resources directory in the assets folder. Next, give it in the interactable tag. In the instantiation.cs script add the prefab name for each interactable prefab you have
An important note is that you should disable the rigidbody and sphere collider in case the photon scene object gets insantiated before it updates, otherwise slight movement is caused. It is also recommended to remove Photonviews, as it will make the server keep track of less things.
There is a default SteamVR Interactables script. We had to edit this code to allow transfer ownership to work. There is a CustomInteractable.cs script in the root asset folder. This is a copy of the Interactable.cs script with our changes.
Adding CustomInteractable.cs to things does not work due to the Hand.cs script (valve made) needing the interactable script to work. We would need to create a customhand.cs script too and a whole bunch of other changes, this is easier.
As indicated by pins, multiple objects can be updated across the network in a GameObject container. 

###Photon Server Settings:
To edit the PhotonServerSettings and add in your own IP find the pun wizard and have it navigate you to the PhotonServerSettings. Our current server requires you uncheck "Use Name Server", have port 5055, and it's IP was 172.20.199.152.

###Options Menu:
Teleport teleports the player to the position (0, ~, 0), Spawn Interactables spawns in a new set of interactable objects,
Scene Transition transitions a player into a new scene (same server room), Speech-to-text toggles enables and disables Google Cloud Streaming, Toggle Voice disables and enables Network Voice Manager, and Toggle Pointer toggles the pointer aka line renderer coming from the left hand.

###Player:
The player works by being a game object that contains both the VR player (aka [CameraRig]) component as well as a Fallback Player component. It also has Debug UI to switch between the two inside the editor. It also allows for Voice, as it is a common medium for both Fallback and VR players
The player itself has "Player.cs" from SteamVR, Photon Views/Photon Transform Views, "Disable.cs", "PlayerManager.cs", and "STTData"
Player Manager instantiates the local player instance and STTData streams speech to text data over the network (Final transcriptions)
The CameraRig class was imported from SteamVR. The components "EnableFallback.cs", Photon Views/Photon Transform Views, and the Player Pointer Game Object were added to the VR Player.

###Disable.cs
Disable.cs was originally written by Lucas Calabrese. We have since modified it to provide more flexibility with Fallback players, hand models, and audio.
The purpose of this script is to disable player components that belong to other players. 
It disables objects at run time locally so for example you can't move your hands and have the other person's hands move instead or in addition to your own. 
It also gets rid of the multiple listener error that Unity would throw.
Refer to code comments to see what is automatically disabled and what needs to be accessed and then disabled via the prefab editor
Certain things need to be found at runtime, not during the editor such as the player hands, while other components such as fallback should be added via the editor.

###Google Cloud API Speech-To-Text
To disable Google Cloud API Speech-To-Text:
Delete the Google Cloud Streaming Object within the hierarchy. Next, ensure that the Google Cloud Streaming Speech-To-Text package is deleted

To enable Google Cloud API Speech-To-Text:
Follow the instructions listed here: https://github.com/oshoham/UnityGoogleStreamingSpeechToText
Note: All the library dependencies mentioned are required for installation. An API key is also needed, you will need to get your own by registering your account at cloud.google.com (There is a 90 day trial with $300 of credits to spend, your account will not be charged if you exceed either limit)