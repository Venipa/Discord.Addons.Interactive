# Interactive
<img align="right" width="128" src="https://github.com/foxbot/Discord.Addons.Interactive/raw/master/marketing/Logo.svg?sanitize=true">

An addon for [Discord.Net](https://github.com/RogueException/Discord.Net) that adds interactivity to your commands.

## Differences from the original repository
- Uses the nightly (pre-release) versions of Discord.Net.Commands and Discord.Net.Websocket
- Uses [PassiveModding's](https://github.com/PassiveModding/Discord.Addons.Interactive) Inline Reaction methods
- The whole base code is slighlty altered to fit with my own coding style

## Supported Features

- Waiting for messages over the gateway
- Paginated messages
- Reaction callbacks
- A powerful criteria system

## Installation

`dotnet add package Discord.Addons.ShukaInteractive --version 1.0.3`

## Usage

To use, add an `InteractiveService` to your service provider. It is also recommended to make your modules inherit from `InteractiveBase`
