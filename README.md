# Rock Paper Scissor Game based on the FinalBiome

Here is a game that runs on the [FinalBiome Network](https://github.com/finalbiome/finalbiome-node) with the Unity game engine, uses the Unity Package [finalbiome-unity](https://github.com/finalbiome/finalbiome-unity) to interact with the network.

![Splash Screen](./docs/showcase.gif)

## Usage
It requires a locally running FinalBiome test node with preset game for it to work.

To make a game configuration in the FinalBiome network, you can use the [finalbiome-impex](https://github.com/finalbiome/finalbiome-impex) utility to quickly create the required assets.

```
finalbiome-impex import --game-spec ./game_spec.json -s //Bob -m //Charlie
```
