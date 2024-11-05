# **Jachkar Museum Project**


## Setup
<details>
<summary>Click to expand</summary>
This project is made for Unity **2022.3.31f1**. Compability with other Unity versions is not guaranteed.

1) [Download Unity hub](https://unity.com/es/download) 

2) Donwload the version **2022.3.31f1**

3) Add WebGL module (required if you want to build for web browser)
</details>

## Toggle between singleplayer and multiplayer versions
<details>
<summary>Click to expand</summary>

The project has two versions: one that supports multiplayer but cannot be deployed in a web browser due to network limitations, and another that supports single-player mode and can be deployed in a web browser.

By default, the project uses the Singleplayer version. However, you can switch to the Multiplayer version by enabling the `USE_MULTIPLAYER` macro. To do this, follow these steps:

1) Open the Unity Editor.
2) Go to Edit > Project Settings > Player.
3) Go to WebGL Settings and scroll down to **Scripting Define Symbols**
4) Press `+`, write `USE_MULTIPLAYER` and **PRESS ENTER**.
5) Apply

</details>
