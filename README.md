# BONEWORKS Chaos [![forthebadge](https://forthebadge.com/images/badges/contains-tasty-spaghetti-code.svg)](https://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/works-on-my-machine.svg)](https://forthebadge.com)
#### ChaosModV, but for a VR Physics game!

## Installation

Drag BWChaos.dll into your mods folder, that's basically it.

To configure BW Chaos, use BoneMenu or **MelonPreferences.cfg**, but note that BoneMenu is missing some options that are present in MelonPreferences

## Audience interaction

This mod supports two methods of audience interaction, via Discord and Twitch.

To enable this, set **enableRemoteVoting** to **true** in MelonPreferences

### Twitch

- Generate a Twitch OAuth2 token by going to this [Twitch Chat OAuth Generator](https://twitchapps.com/tmi/)

- Paste it into the **token** field in MelonPreferences

- Put your channel name in the **channel** field in MelonPreferences

- It's recommended that you set **showCandidatesOnScreen** to **true** so that your viewer can see what they're voting for

### Discord

#### This requires that you have the Manage Server permission in the server you wish to use this in, because you need to add a bot that listens for numbers.

- Go to the [Discord developer portal](https://discord.com/developers/applications)

- Create a new application, give it whatever name you want, it do not matter.

- Open the application, go into the Bot tab, and create new bot user

- Copy its token and paste it into the **token** field in MelonPreferences

- Go back into the General Information tab and copy the Application ID

- Paste it into this "template", replacing `<APPLICATION_ID>` with your application ID: `https://discord.com/oauth2/authorize?client_id=<APPLICATION_ID>&scope=bot&permissions=1024`

- Open that link and select the server you want to add the bot to, and add it

- The hard part's over. Now open your Discord app

- Go into the settings page

- Enter the Advanced tab, it's the last one in the App Settings category, 6th from the bottom.

- Enable developer mode

- Go to the channel you want people to vote in, make a new one if you want, because that channel will likely be flooded.

- Right click the channel in the channel list, and click "Copy ID"

- Paste the channel ID into the MelonPreferences field **channel**

## MelonPreferences

This version, unlike the Legacy version, has sensible defaults, meaning the game's main campaign is playable from first install.

There are "crazier" effects, like the legendary 'Change Steam name for 2m', but that is behind a MelonPreferences entry

Here is a brief overview of the configurable values in MelonPreferences:

- token: The authentication code for the bot

- channel: The name of the Twitch channel or Discord channel for the bot to listen to, assuming enableRemoteVoting is true

  - The mod will automatically determine if this is a Twitch or Discord channel and will start the proper bot accordingly. It will fail if your Twitch name is a bunch of numbers though.

- randomEffectOnNoVotes: Determines if the mod will run effects when there were no votes.

- useGravityEffects: Enables or disables effects that change the game's gravity value. Effects that change gravity are likely to crash the game on campaign levels.

- useLaggyEffects: Enables or disabled the more processing intensive effects, intended for people that can stomach low framerates, because there wasn't a year spent on optimizing it.

- useSteamProfileEffects: Determines if effects changing your Steam profile will run, like "Change Steam name for 5m"

- syncEffectsViaEntanglement: Determines whether the Entanglement module is enabled or disabled. 

- - When enabled, if you're the host, any effects that you run will attempt to be ran on the other clients.

  - If you're a client, then any effect the host runs will attempt to be ran. If you have that effect disabled, it will inform you what was going to run.

- ignoreRepeatVotesFromSameUser: Determines whether spamming is an effective method of bumping up vote counts.

- proportionalVoting: Determines if proportional voting is enabled or not. If yes, an effect with 60% of votes has a 60% chance of winning, otherwise the top voted effect wins.

- enableRemoteVoting: Enables taking votes from Discord/Twitch.

- showCandidatesOnScreen: Shows the candidate effects on screen, with the numbers that need to be voted for each one.

- sendCandidatesInChannel: Only available to Discord remote voting. It will send a list of the effects that users can vote for into the specified channel.

- forceEnabledEffects: A list of effects that will be in the effect pool, regardless of whether or not they adhere to the other preferences.

- forceDisabledEffects: A list of effects that will *never* be in the effect pool, regardless of whether or not they adhere to the other preferences.

## Questions

The mod is open source, so if you see an effect and want to know what it does, just go to the [GitHub repository](https://github.com/extraes/BW-Chaos/tree/rewrite/BW%20Chaos/Effects).

Odds are it's in either the Misc or Player folders.

## Crashes/errors

If there's a serious, game-breaking issue, please, TELL ME! Ping me in the BONEWORKS fan server! Preferably in the #mod-general channel, my username and tag are extraes#2048!

### Known issues

- Errors appearing in the console on a scene transition (changing level)

  - This is harmless. It just means that an effect failed to properly stop. The effect should automatically stop.

  - This shows up as something along the lines of "Exception in coroutine of type BWChaos.EffectHandler+<Timer>" and usually contains the term "System.NullReferenceException"

  - Even though I know *why* this happens, it is very tedious to hunt down every instance by myself, so if you told me which effect caused it, I would be grateful.

    - You can find this information by either giving me your **log file** or telling me the first few lines of the error (e.g. where it says "[ERROR]" and the first few "at BWChaos.Effects...")

- BWChaos does not work for some people on certain configurations of the Steam version, specifically the "ChaosModStartupException" error

  - What is the square root of a fish? Now I'm sad.

- The timer is too jerky!

  - Yeah maybe I'll make it smoother, make it interpolate or something. It's not really a priority though.

### Contributing

If you want to add another effect, by all means feel free to do so. Make a Pull Request on the GitHub repo and add a new effect.

If you want to change the UI, you can make a PR for that too, since the Unity project I use for building the UI is in the git repo too.

I don't have a code of coduct or anything, so feel free to evade taxes and stuff after making a PR.