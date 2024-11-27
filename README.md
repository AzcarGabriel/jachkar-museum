# **Jachkar Museum Project**


## Setup
<details>
<summary>Click to expand</summary>
This project is made for Unity **2022.3.31f1**. Compability with other Unity versions is not guaranteed.

1) [Download Unity hub](https://unity.com/es/download) 

2) Donwload the version **2022.3.31f1**

3) Add WebGL module (required if you want to build for web browser)
</details>

## Toggle between singleplayer and multiplayer version
<details>
<summary>Click to expand</summary>

The project has two versions: one that supports multiplayer but cannot be deployed in a web browser due to network limitations, and another that supports single-player mode and can be deployed in a web browser.

By default, the project uses the Singleplayer version. However, you can switch to the Multiplayer version by enabling the `USE_MULTIPLAYER` macro. To do this, follow these steps:

1) Open the Unity Editor.
2) Go to Edit > Project Settings > Player.
3) Go to WebGL Settings.

    ![ProjectSettings1](./.repo_assets/imgs/project_webgl.jpg?raw=true "Title")

4) Scroll down to **Scripting Define Symbols** and press `+` to add a new macro.

    ![ProjectSettings2](./.repo_assets/imgs/project_webgl2.jpg?raw=true "Scripting Define Symbols")

5) Write `USE_MULTIPLAYER`, **PRESS ENTER** and click in the Apply button

    ![ProjectSettings3](./.repo_assets/imgs/project_webgl3.jpg?raw=true "USE_MULTIPLAYER")

</details>

## Static values

There is an script where static values are stored. These values are used in different sections of the code. The script is located in: 

`Assets/Scripts/StaticValues.cs`

## Maps scenes

<details> 
<summary>Click to expand</summary>

Currently, the museum has 5 maps, each with a base version used for single-player mode and versions used for the multiplayer mode. In other words, the multiplayer versions of the maps "inherit" from the single-player versions, making scene management more scalable.

The maps are:
* EchmiyadzinAlly
* EchmiyadzinWall
* Noradus
* Noravank
* Plains

The scenes with the base maps is located in `Assets/Scenes/BaseMaps`. All maps has a `SceneController` prefab, which contains a child (Menu/SpawnPoint) used to determine where khachkars will be spawned. The SpawnPoint can be moved and rotated to change the position and rotation of the spawned khachkars.

</details>

## How Khachkars are included in the museum

<details>
<summary>Click to expand</summary>

The khachkars are loaded in AssetBundles, that are separated in three types:

* One asset bundle called `stones_thumbs`: That contains all the thumbnails of the khachkars. Is used to list them and show them in the khachkar selection menu.

* Multiple asset bundles of type `stones_X_Y`: They have the prefabs of the khachkars (their Mesh, Box Collider and Halo) from the ID `X` to the ID `Y`. They are grouped in 5 stones.

* Multiple asset bundles of type `stones_metadata_X_Y`: They have the JSONs with the metadata of the khachkars from the ID `X` to the ID `Y`. They are grouped in 5 stones.

These asset bundles can be loaded from two sources:
* Local: Khachkars that are stored in the repository.
* Remote: Khachkars stored in Saduewa, sourced through the crowdsourcing mechanism.

The method used to decide which source to use is through the variable `StaticValues.OnlineAssetBundles`. The script responsible for managing the AssetBundles and spawning the stones is `Assets/Scripts/StoneService.cs`

</details>

# Please update this README if the code changes, or add new sections to document any previously undocumented features.