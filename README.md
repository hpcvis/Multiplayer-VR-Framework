# Multiplayer VR Framework

## Made by:
Daniel Enriquez, Christopher Lewis, and Erik Marsh

## Description:
This package is a template for VR multiplayer with voice, interactables, and more.
Multiplayer works by using Photon Unity Networking, anything you want to see update across the network (such as interactables and representations of the player) have Photon Views attached.
The scene comes with a premade options menu to enable/disable features developed inside this package. 

## Interactables:
Interactables work by transferring ownership between players.
The last player to interact with an interactable will be its owner.
This is to ensure minimum latency between the player's movements and the interactable's movements.
If you want to make a competitive game, make the ownership transfer to the server after a user is done with it.
To add interactable objects add a prefab to the Resources directory in the assets folder.
Next, give it in the `Interactable` tag.
In the `Instantiation.cs` script, add the prefab name for each interactable prefab you have.
An important note is that you should disable the rigidbody and sphere collider in case the Photon scene object gets instantiated before it updates, otherwise slight movement will occur.
It is also recommended to remove Photon Views, as it lets the server keep track of fewer objects.

There is a default SteamVR `Interactable.cs` script.
We had to edit this code to allow ownership transfer to work.
There is a `CustomInteractable.cs` script in the `Assets/Scripts` folder. This is a copy of the `Interactable.cs` script with our changes.
Adding `CustomInteractable.cs` to things does not work due to the SteamVR `Hand.cs` script needing the interactable script to work.
We would need to create a `CustomHand.cs` script as well (among a whole bunch of other changes; this is easier).
As indicated by pins, multiple objects can be updated across the network in a GameObject container.

## Photon Server Settings:
To edit the PhotonServerSettings and add in your own IP address, open the PUN Wizard (`Window > Photon Unity Networking > PUN Wizard` or `Alt+P`) and navigate to the PhotonServerSettings.
Our current server requires that you uncheck "Use Name Server", use port 5055, and use IP address 172.20.199.152.

## Options Menu:

| Menu Option           | Behavior |
| --------------------- | -------- |
| Teleport              | Teleports the player to the position <-5, 1.1, 0> (arbitrarily chosen) |
| Spawn Interactables   | Spawns in a set of two new InteractableBall objects |
| Scene Transition      | Transitions player into a new scene in the same server room |
| Toggle Speech to Text | Enables/disables Google Cloud Streaming |
| Toggle Voice          | Enables/disables Network Voice Manager |
| Toggle Pointer        | Enables/disables the "pointer" (line renderer) emitted by the left hand |

## Player:

The player is a modified version of the SteamVR player prefab.
It mainly uses the `NetworkedPlayer.cs` script, which inherits from the SteamVR `Player.cs` script.
The singleton nature of the SteamVR player object caused issues with input and camera control over the network,
so we instead create dummy objects that represent the player's head and hands.
These dummy objects are invisible to the local player, but are visible to other players.

The `NetworkedPlayer.cs` script copies the head transform, hand transform, and hand animation state to these dummy objects every frame.
The dummy objects have Photon Views, Photon Transform Views, and Photon Animator views to sync dummy state over the network.
This ensures that the dummies serve as an accurate visual representation of a player in the world.

## Google Cloud API Speech-To-Text
To disable Google Cloud API Speech-To-Text:
* Delete the Google Cloud Streaming Object within the hierarchy
* Next, ensure that the Google Cloud Streaming Speech-To-Text package is deleted.

To enable Google Cloud API Speech-To-Text:
* Follow the instructions listed here: https://github.com/oshoham/UnityGoogleStreamingSpeechToText
  * Note: All the library dependencies mentioned are required for installation.
  * An API key is also needed, you will need to get your own by registering your account at https://cloud.google.com.
   (There is a 90 day trial with $300 of credits to spend, but your account will not be charged if you exceed either limit.)
