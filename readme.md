# BONEWORKS Chaos [![forthebadge](https://forthebadge.com/images/badges/contains-tasty-spaghetti-code.svg)](https://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/works-on-my-machine.svg)](https://forthebadge.com)
### A BW equivalent of ChaosModV, but way less stable!

## Installation

You can drag the two folders that are in the 'BONEWORKS' folder here into your Boneworks install directory and everything'll get merged fine by default, but if you update MelonLoader, you'll need to reinstall WatsonWebsocket.

For manual installation, place BW Chaos.dll into your MODS folder and put WatsonWebsocket.dll into your MELONLOADER\MANAGED folder. (Not ILRepacked because the mod fails to work properly when ILrepacked)

This mod also requires **MelonLoader** 0.4.0, any version earlier **will not work**.

If you do not know how to get to these folders that are located in your **BW install folder**, tough shit, hold this L and put your quest back on to play some more Physics Playground.

If you do not know how to get a BOT token or the ID of a channel, once more, tough shit, cope seethe and dilate, go see if your computer's wifi connection has improved to the point that airlink will work

## Crash/bug reporting

If you report a crash to me and you send a screencap instead of the LOG FILE (or if you don't send the log file at all), I will perform a **Harmful Honda(tm) Random Act of Trolling.**

To help me out, you can (not required tho) send me the log file located at `%localappdata%low\Stress Level Zero\BONEWORKS\output_log.txt`

## Modprefs

If you want to play solo, make the token and channel random things and enable randomEffectOnNoVotes.

If you're prone to motion sickness, I'd suggest not playing this mod.

If you want to play the campaign, set useGravityEffects to false.

If you want to not get 30fps in a VR game, set useLaggyEffects to false.

### Misc info/postamble

If you ignore these instructions and ask me why you're getting a "missing dependency" error or something similar, I will ghost ping you up to FIVE TIMES in different channels and tell you to fuck off for not paying attention.

If you encounter any other error, please ping me and tell me WHAT YOU WERE DOING and include a LOG FILE. If you do not include a log file, I will ghost ping you up to FIVE TIMES in different channels and tell you to fuck off for not paying attention.

In addition, if you have a suggestion for an effect, PLEASE TELL ME! Leave your suggestion in #mod-general because I do not have my DM's open. If you know HOW to implement it, please PING ME telling me how to do so in C#. If you do not know, do not ping me because I'm an ass about pings. Also, please don't expect it to be implemented because I'm new to C# and game modding in general. Hell, this is the first major project I've made.

These "rules" are here because I just want to make a mod, I don't want to be a slave to #mod-help, repeating the same thing day in day out. Also obligatorily because if you break them, I get to do a little trolling.

# ATTRIBUTIONS 

 * Hawaii - Being GMT-11, it's still technically friday as of the time of upload, so that's why this is the Hawaii build.
 * GS - Testing the mod with me over several weeks. The tens of crashes you endured weren't in vain (I hope)
 * GS - Suggesting new QOL modpref settings (Changing nodejs path)
 * Cyanide - Testing during the final ~2 weeks of development
 * The homies in chill (Mr. Gaming, D4-LT, KooloEdits, Maranara, Riggle, Parzival, TabloidA, TheDarkElk, L4rs, TrevTV, WNP78) - Helping test the mod by voting on effects
 * TrevTV - General C# help
 * TrevTV - MelonPreferences implementation
 * TrevTV - Packaging nodejs and file into an executable
 * TrevTV - Embedding executable into dll and read it (I didnt end up needing to embed an executable, but I embedded a zip file)
 * TrevTV - Pointing me towards the Unity Scripting Reference and websockets and WatsonWebsocket
 * TrevTV - Rewriting the discord bot in C# (testing builds still use cbot.zip though lol) <- and the first release, apparently
 * TrevTV - Giving me code to hook punching (used in SUPER PUNCH, an effect that didn't make primetime due to instability)
 * TrevTV the fucking GOAT - telling me how to attach a monobehaviour to the player and make code run when the player's gameobject is destroyed
 * Lars - & tear, if you get me
 * WNP - Told me to use sqrMagnitude instead of calculating the distance becaust sqrt is slower
 * * Iterating through a long ass list of gameobjects needs to be as fast as possible, so this was important to PlayerGravity and the other effects that use "local gravity"
 * WNP - Telling me about GlobalPool.Spawn
 * WNP & Lars - Telling me that Poolee.Despawn() is broken and to just set a gameobject as inactive
 * WNP & Lars - Telling me about Poolee.Pool.Despawnall() (it may come in handy later lol)
 * YOWChap, Lars, & WNP - Helping me with stupid problems I should have noticed
 * YOWChap - BMTK/MTINM
 * Adamdev - Testing the initial proof of concept
 * Adamdev - Telling me about UnityExplorer (and sending me the DLL)
 * Adamdev - Helping me with FuckYourMagazine and Butterfingers
 * Lakatrazz - Telling me about PhysBody
 * TheShadowNinja - Telling me about AddComponent (add rb to gravity cube)
 * Elarelda - Elareldeffect idea & details
 * Microsoft - C# documentation (duh)
 * FatWrinkleZ - SusArrow, I dnspy'd it to get the code for bootleg gravity cube & pointtogo.
 * C# - If else statements 🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏
 * Stackoverflow - Generate list of random numbers without repeats (https://stackoverflow.com/questions/30014901/generating-random-numbers-without-repeating-c)
 * Stackoverflow - Probably a lot of other things but I don't remember
 * Forums - Change gravity direction, get list of all gameobjects
 * ChaosModV - "Inspiration" for some effects

Hypothetically, I could make this mod grab your token and use that but I'm not trying to break TOS and get people banned, so deal with the 7 extra clicks.
also you can fuck with the js all you want, its no skin off my back, but dont redistribute any part of this mod without prior permission from me, extraes.
(the mod's "version checking" only checks the name so if you change the actual contents of the file, those changes will remain until you update to a newer version)