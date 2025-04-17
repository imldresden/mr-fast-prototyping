# Animation Authoring
The goal of the **Animation Authoring** project is to provide a user-friendly Unity application for quick prototyping and executing animations within a Mixed Reality Environment on the Hololens 2.

Such a system could be used to design and showcase interactions without having to develop an appropriate system for the interaction. 

For an Overview over the final Animation Prefabs look at the last chapter of this Documentation!

## Networking

This project uses the Photon PUN 2 free version for networking. This following section will guide you through setting up networking for this project.

First you need to go to the unity asset store and aquire "PUN 2 - FREE". Next go into the project and click window > Package Manager > Packages: My Assests > PUN 2 - FREE > import. This imports Photon into your project and sets up the "PHOTON_UNITY_NETWORKING" Scripting Define Symbol, this tells the script to use the code with the photon logic. You can double check that this is succesful by going into your Player Settings > Other Settings > Scripting Define Symbols, "PHOTON_UNITY_NETWORKING" as well as other variables for Photon should be here. 

NOTE: If you want to remove Photon from your version of the project again make sure to also remove "PHOTON_UNITY_NETWORKING" from the Scripting Define Symbols!

Once PUN is installed you will be prompted to setup your project. For this create an free account on the Photon PUN website. With an account create an application in your dashboard and copy the app-id. Navigate to Window->Photon Unity Networking -> Setup Project and paste your app-id. When using one of the premade scenes networking should already function! When creating a new scene ensure that two empty gameobject are present within the scene, one holding Launcher script (Assets -> Scripts -> Networking -> Launcher) and the other holding the GameManager Script (Assets -> Scripts -> Networking -> Launcher). Alternatively there is a "Networking_base" prefab that holds the required objects. 

Note that many prefabs dont have Photon attached to them by  default. For Photon to function attach a PhotonView and a PhotonTransformView to the object holding the sequence script. 

The current networking only synchronizes the position of the prefabs and the sequential animation of the prefabs. 

When opening the application in a scene where networking is set up, the client tries to find a room and join that room, if no room is available under your app-id or all available rooms are full a new room is created. The client that creates the room is the master client and the designated actor. Only this client can control the scene and the animation. All clients that join later only observe the animation but cannot affect it. Currently the room size is set to 6, allowing one actor and five observers, if more observers are required navigate to your scene to the Launcher script and change the Max Players Per Room variable to the desired number. 

## Scene Structure
For tracking objects to the hands of the users we use an Anchor system that relies on certain gameobjects being present in the scene. A preset for a scene-structure can be found in Prefabs > QRCode or QRCode_Networked. Both prefabs have the QR code structure set up to align a scene with a real world Environment. Both Prefabs contain the anchor objects (LeftHandAnchor, RightHandAnchor, BodyAnchor) which are required for the animation to work correctly. 

Any animation prefab should be attached to the content root for everything to function correctly. 

NOTE: After importing Photon to your project the connection between the PhotonTransformView and the PhotonView will be broken in the LeftHandAnchor, RightHandAnchor and BodyAnchor Gameobject. There is an editor script that should fix this by itself upon start of the scene but you can repair this connection manually by dragging the PhotonTransformView component on the appropriate field in the PhotonView component. 

## Animation Prefab
As part of this project, we've developed prefabs that serve two purposes: simplifying the creation of future prefabs and demonstrating various functionalities.

If you're interested in crafting your own animations or expanding existing ones, follow these steps: right-click on a prefab within the Unity editor and navigate to Create > Prefab Variant and edit this new Prefab. All the prefabs within the project are essentially variants of the "Base" prefab.

### Base Prefab Structure
For this system to function, a prefab requires a minimum of three components. The "sequence"-script component must be attached to the root of the prefab, along with two child game objects assigned to the start and end state fields of the squence-script. Additionally, the end_state game object should have the "animation_step" script attached to it. This skeleton structure is already in place within the "Base" prefab.

![](./Images/baseView.png)

### Creating a Animation

With the skeleton structure in place, you're ready to craft your first animation. Begin by adding an object of the same type to both the Start State and the End State.

![](./Images/SimpleAnimation.png)

In the example above, two quads were included, with one being shifted backward and to the right. 

Let's look at the "Trigger" property of our End State. This property determines the animation's initiation. Currently, the project supports two triggers: "Spacebar" and "None". Selecting "Spacebar" triggers animation upon pressing the spacebar, while "None" starts the animation immediately when the animation step is called. Set the trigger to "Spacebar", launch the scene with the prefab, and hitting the spacebar should set the animation in motion, sliding the object from its initial position to the final destination.

The objects within the start and end states serve as keyframes, with their scale, rotation, and position being animated. At runtime, a new object named "SceneObjects" is created to hold the objects undergoing animation, while the keyframe-objects are deactivated.

![](./Images/SceneObjects.png)


### Animation Modifiers
Now, let's explore the other animation modifiers within an animation step. The "Animation Style" dictates how position, rotation, and scale transformations occur.

#### Animation Style
Currently, four interpolation options are available: linear, linear-to-interpolate, interpolation and interpolation-to-linear. Interpolation facilitates smooth easing in and out of the animation.
The top animation showcases the interpolated animation option, while the bottom one features linear animation.

![](./Images/Int_Both.webm)

#### Animation Duration
This property provides a slider to adjust the overall duration of an animation step in seconds.

#### Do Animate
You can deactivate an animation step by toggling the "Do Animate" property. This allows you to experiment with different animation variations within a single prefab.

### Ancor
Each Object contained within an animation step has an ancor property. All of these ancors can bee seen in the inspector view of an animation step, by expanding the ancor section.  There are currently three choices for ancors, Left Hand, Right Hand and World. The origin of an prefab is tracked to either the left or right hand palm. Grouping animation objects in empty gameobjects allows for tracking of specifically arranged object groups to a tracked ancor as can be seen in the "Crashing" Prefab. 

Generally it is recommended to have a group-object which is just an empty game object to hold all the gameobjects that will be tracked to an anchor and have that group-object switch its anchor and not the child objects. This is because once a object is tracked to an anchor like the left hand it wont perform animation, by appending the group object to the anchor the children can still perform their relative animation.

### Adding Additional Animation Objects

You can incorporate more objects into a animation step. Just ensure that the order of children in the inspector remains consistent across all steps, as not the name or the type of the object is used for identification, only the order in the inspector defines which object is animated in what way. The initial child in the start state regardless of its name or type will always animate to the first child (keyframe) in following animation steps.

This animation illustrates the simultaneous animation of multiple objects.

![](./Images/Deck.webm)

### Adding Additional Animation Steps

This system supports additional animation steps. To achieve this, append an empty game object to the root of the prefab and attach an "Animation_Step" script to it. This new step functions like the end state. The sequence of operations always follows: Start State -> Step 1 -> ... -> Step n -> End State. Remember that the order of steps, akin to children, relies on their positioning within the inspector. Unity prevents the placement of children between the start and end states of the Base prefab.

A complex animation could be constructed as follows:

![](./Images/ManySteps.png)

Keep in mind that the End State will be called last!

The sequence-script provides an overview of all parameters across all animation steps.
### Children of Children

This system additionally accommodates nested children, allowing nested objects to undergo animation akin to direct children of an animation step. This can be used to more easily animate groups of objects, by attaching them to a empty gameobject within the scene.

## Expanding the System

Additional animations can be implemented in the "Animations" script. Additional Triggers and the functions that describe the triggers can be implemented in the "Triggers" script. When implementing new animations or Triggers, make sure to add the option to the respective evaluation functions and Enums!

## Overview over all Prefabs

### Table Prefabs
All of these Prefabs are Prefab  Variants of the "Table_Base" Prefab. 

#### I2-A

![](./Images/I2_A.webm)


#### I2-B

![](./Images/I2_B.webm)


#### I2-C

![](./Images/I2_C.webm)

#### I2-D

![](./Images/I2_D.webm)


### Throw Merge

![](./Images/Throw_Merge.webm)


### Lasso Selection

![](./Images/Lasso.webm)


### Come To Me Prefab

![](./Images/ComeToMe.webm)

### Accordion

![](./Images/Accordion.webm)

### Crash Combination

![](./Images/Crash.webm)

### Grid to Display

![](./Images/GridToDisplay.webm)
