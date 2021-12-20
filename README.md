# BONEWORKS Chaos [![forthebadge](https://forthebadge.com/images/badges/contains-tasty-spaghetti-code.svg)](https://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/works-on-my-machine.svg)](https://forthebadge.com)
#### ChaosModV, but for a VR Physics game!
##### And a great example of what happens when you give a shitposting teen an IDE (My VS2022 background is Ralsei smoking a blunt as I type this)

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

  - The mod will automatically determine if this is a Twitch or Discord channel and will start the proper bot accordingly. It will fail if your Twitch name is a bunch of numbers though. Sucks for you I guess.

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

- showWristUI: Determines whether or not the list of previous effects and voting timer should appear on your wrist.

- showCandidatesOnScreen: Shows the candidate effects on screen, with the numbers that need to be voted for each one.

- sendCandidatesInChannel: Only available to Discord remote voting. It will send a list of the effects that users can vote for into the specified channel.

- forceEnabledEffects: A list of effects that will be in the effect pool, regardless of whether or not they adhere to the other preferences.

- forceDisabledEffects: A list of effects that will *never* be in the effect pool, regardless of whether or not they adhere to the other preferences.

## Questions

The mod is open source, so if you see an effect and want to know what it does, just go to the [GitHub repository](https://github.com/extraes/BW-Chaos/tree/rewrite/BW%20Chaos/Effects).

Odds are it's in either the Misc or Player folders, and the file may have a different name than ingame.

If an effect is laggy and not marked as such, tell me and I'll try to optimize it or I'll mark it as laggy. Tell me your specs too, please.

## Crashes/errors

If there's a serious, game-breaking issue, please, TELL ME! Ping me in the BONEWORKS fan server! Preferably in the #mod-general channel, my username and tag are extraes#2048!

If you somehow find a way to break the Twitch bot or Discord bot, or hack them in some way, please tell me so I can ~~troll people before I~~ fix it.

### Known issues/FAQ
#### I don't know if these questions will be frequently asked, but I've got them here just in case

- Errors appearing in the console on a scene transition (changing level)

  - This is harmless. It just means that an effect failed to *"properly"* stop. The error actually appearing means that the effect was forcefully stopped by MelonLoader before anything bad could happen. Thanks MelonLoader! (I'd die on the spot if I got a nullref in unmanaged code)

  - This shows up as something along the lines of "Exception in coroutine of type BWChaos.EffectHandler+<Timer>" and usually contains the term "System.NullReferenceException"

  - Even though I know *why* this happens, it is very tedious to hunt down every instance by myself, so if you told me which effect caused it, I would be grateful.

    - You can find this information by either giving me your **log file** or telling me the first few lines of the error, where it says "[ERROR]" and the first few "at BWChaos.Effects..."

    - If you give me a screenshot of your log file, I will place a pipe bomb in your mailbox, or a comedically timed lit stick of dynamite.

- BWChaos does not work for some people on certain configurations of the Steam version, specifically the "ChaosModStartupException" error

  - What is the square root of a fish? Now I'm sad.

- The timer is too jerky!

  - Yeah maybe I'll make it smoother, make it interpolate or something. It's not really a priority though.

- 'Garble some textures' and 'Swap textures at random' lag the game when they start!

 - Well the game should've loaded on startup, but I guess the textures got nulled, so that didn't pan out. It's unfortunate how the world works sometimes, isn't it? Blame IL2.

- Garble some textures has a chance to make the scene dark/a random color!

 - Yeah that's cause it randomizes the color of 20% of the materials in the game, and since it's 2021, it doesn't discriminate, so something it changes corresponds to the lighting of the scene. Are you telling me to discriminate? That's messed up, man. Shame on you.

- 'Video Textures' lags my game when it's active!

 - I didn't have that issue but someone with a 1060 6G did, so it's 100% a ~~skill~~ GPU bound issue. It's a nice effect that I think is funny as fuck, but it does tax the GPU more than I'd like, so I had to mark it as LAGGY, no matter how much I would've loved to see it get used more. I guess get a new GPU or force disable it.

 - Also you should probably not spam it if you have less than 6G of VRAM, to be honest.

- The texture effects keep ballooning my VRAM usage! (Hi WNP!)

 - That's cause they set the texture of each mesh renderer they encounter, meaning they create a new Material for each object they hit. This is probably a memory leak, but it's a memory leak that's fixable by reloading the scene.

 - Originally these effects loaded their textures and set the textures of each material they encounter, but that made them persist between scenes, and one of my goals with this mod is to make no changes that are not reversible via a scene change, so they create new materials which are deleted on scene change/reload.

- Vine boom sound effects keeps going after it ends! And it's inconsistent in what it *does* do!

 - It was a bitch to even do what it currently does, so count your blessings.

 - No, but for real, it doesn't look for inactive gameobjects, which I may change in the future.

- Does Get Fucking Doxxed show my real IP?

 - No, run it again and find out. It generates a random, yet valid, IP every time it's ran, with 4 segments each containing a number between 0 and 255. And then it does spinnies cause I thought it was funny. And then Vine boom sound effect, also, because I thought it was funny.

- The music effects are too loud/overwhelming!

 - They obey your music mixer volume setting, so decrease that and the music will go adios.

- An effect doesn't work on the Oculus version of the game!

 - Please tell me what effect it is! Give me a log, or send me the first few lines of the error, where it says "[ERROR]" and the first few "at BWChaos.Effects..."

 - I try to make the mod compatible between the Steam and Oculus versions, but I'm not Rich Uncle Pennybags, so I can't buy the game multiple times for the niche of people that use an Oculus copy, but if you help me test, then we can all sing Kumbaya.

- The Entanglement module doesn't work!

 - It only syncs the running of effects, not what those effects do. Yes, It's basic, but it's also, to my knowledge, the first mod to have its own entanglement module, so you can't expect it to be magic on the first go.

 - That said, there is "proper" syncing planned, with effects able to send and recieve data over the wire.

   - As of writing, I'm working on per-effect syncing. If you think an effect could be synced but isn't, or should be synced differently, please tell me your ideas! (SpawnDogAds cannot be synced, it gets dog pics from a website, so I can't do much about that)

- The remote voting process doesn't work!

 - Yeah. Enable it in preferences.

- What do "Meta" effects do?

 - Meta does not refer to the company. Fuck Facebook. Meta refers to effects that act upon the mod itself or its effects, like Faster Effect Timer speeds up the timer, and Triple Threat runs 3 other effects.

### Credits

- Code modders: occasional help and ideas (shoutouts to trev for rewriting my original code, creating a better foundation to base Chaos off of)

- Sychke: Testing the Twitch bot and providing much needed testing and feedback.

- Alfie: Making the mod's icon, over 4 months go, in July. Sorry it took me this long!

- ZCubed & Lakatrazz: Making Entanglement so I don't have to make my own way of sending data over the wire.

- GS: IDK if he wants to be credited but he helped me test earlier versions of the mod, on both codebases Legacy and Rewrite, as well as helping me test Entanglement syncing.

- ~~Chap: Making ModThatIsNotMod~~ actually nah screw that guy for making his namespace so damn long.

- MelonLoader Team: Making a great framework for mod development, including persistent data/preferences, coroutine running, IL2CPP type injection, automatic harmony instance creation and patching, and of course, being the first universal Unity modloader that could mods in IL2CPP games. Oh and, including a zip file library that I could ~~yoink~~ use.

- Samboy: I haven't directly interacted with them but Tomlet is great and I'm using it for a personal project and I don't think I'll return to JSON, and CPP2IL, while not perfect, is magic for what it's already capable of.

### Contributing

If you want to add another effect, by all means feel free to do so. Make a Pull Request on the GitHub repo and add a new effect.

If you want to change the UI, you can make a PR for that too, since the Unity project I use for building the UI is in the git repo too.

If you want to add a new image, instruction, or video, do that through the Unity project too. (Or just send it in the BW server's meme channel and ping me to add it, and I may do it)

I don't have a code of coduct or anything, so feel free to evade taxes and stuff after making a PR.