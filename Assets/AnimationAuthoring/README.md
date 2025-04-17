# Animation Authoring
Welcome to the **Animation Authoring** project, where our goal is to provide a user-friendly Unity application for creating and executing animations within a Mixed Reality Environment on the Hololens 2.

For an Overview over the final Animation Prefabs look at the last chapter of this Documentation!

## Animation Prefab
As part of this project, we've developed prefabs that serve two purposes: simplifying the creation of future prefabs and demonstrating various functionalities.

If you're interested in crafting your own animations or expanding existing ones, follow these steps: right-click on a prefab within the Unity editor and navigate to Create > Prefab Variant. All the prefabs within the project are essentially variants of the "Base" prefab.

### Base Prefab Structure
For this system to function, a prefab requires a minimum of three components. The "base_script" component must be attached to the root of the prefab, along with two child game objects assigned to the start and end state fields of the base_script. Additionally, the end_state game object should have the "animation_step" script attached to it. This skeleton structure is already in place within the "Base" prefab.

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
Currently, two interpolation options are available: linear and interpolation. Interpolation facilitates smooth easing in and out of the animation.
The top animation showcases the interpolated animation option, while the bottom one features linear animation.

![](./Images/Int_Both.webm)

#### Animation Duration
This property provides a slider to adjust the overall duration of an animation step in seconds.

#### Do Animate
You can deactivate an animation step by toggling the "Do Animate" property. This allows you to experiment with different animation variations within a single prefab.

### Ancor
Each Object contained within an animation step has an ancor property. All of these ancors can bee seen in the inspector view of an animation step, by expanding the ancor section. All animation steps have a near interaction grabbable attached to them for manual repositioning of any objects that are ancored to the world with the "World" ancor. There are currently four choices for ancors, Left Hand, Right Hand, World and Left Side Body. When the left or right hand anchor is chosen, an additional Vector3 can be given to offset the tracking point. The origin of an prefab is tracked to either the left or right hand palm. Grouping animation objects in empty gameobjects allows for tracking of specifically arranged object groups to a tracked ancor as can be seen in the "Crashing" Prefab.

### Adding Additional Animation Objects

You can incorporate more objects into a animation step. Just ensure that the order of children in the inspector remains consistent across all steps, as not the name or the type of the object is used for identification, only the order in the inspector defines which object is animated in what way. The initial child in the start state regardless of its name or type will always animate to the first child (keyframe) in following animation steps.

This animation illustrates the simultaneous animation of multiple objects.

![](./Images/Deck.webm)

### Adding Additional Animation Steps

This system supports additional animation steps. To achieve this, append an empty game object to the root of the prefab and attach an "Animation_Step" script to it. This new step functions like the end state. The sequence of operations always follows: Start State -> Step 1 -> ... -> Step n -> End State. Remember that the order of steps, akin to children, relies on their positioning within the inspector. Unity prevents the placement of children between the start and end states of the Base prefab.

A complex animation could be constructed as follows:

![](./Images/ManySteps.png)

Keep in mind that the End State will be called last!

The base_script provides an overview of all parameters across all animation steps.
### Children of Children

This system additionally accommodates nested children, allowing nested objects to undergo animation akin to direct children of an animation step. This can be used to more easily animate groups of objects, by attaching them to a empty gameobject within the scene.

## Networking

This project now uses the Photon PUN 2 free Networking solution. To use the already built in PUN 2 Networking you require an free account on the Photon homepage. Once there create an application in your dashboard and copy the app-id. Navigate to Window->Photon Unity Networking -> Setup Project and paste your app-id. When using one of the premade scenes networking should already function! When creating a new scene ensure that two empty gameobject are present within the scene, one holding Launcher script (Assets -> Scripts -> Networking -> Launcher) and the other holding the GameManager Script (Assets -> Scripts -> Networking -> Launcher). The base prefab is already equipped with the necessary Photon scripts so any Prefab-Variants that are created from the base prefab should work with photon. The current networking only synchronizes the position of the prefabs and the sequential animation of the prefabs. 

When opening the application in a scene where networking is set up, the client tries to find a room and join that room, if no room is available under your app-id or all available rooms are full a new room is created. The client to open the room is the master client and the designated actor. Only this client can control the scene and the animation. All clients that join later only observe the animation but cannot affect it. Currently the room size is set to 2, allowing one actor and one observer, if more observers are required navigate to your scene to the Launcher script and change the Max Players Per Room variable to the desired number. 

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