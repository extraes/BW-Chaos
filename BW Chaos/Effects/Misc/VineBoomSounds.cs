using MelonLoader;
using StressLevelZero.SFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BWChaos.Effects
{
    //todo: this doesn't fully restore sounds to what they previously were - why? how fix?
    // i suppose i could just not even attempt to change it back, but... do i even bother?
    internal class VineBoomSounds : EffectBase
    {
        static AudioClip vineBoomSound;
        Dictionary<AudioSource, AudioClip> sourceDict = new Dictionary<AudioSource, AudioClip>(); // lord forgive me for how awful this is
        Dictionary<ImpactSFX, AudioClip[][]> impactSFXDict = new Dictionary<ImpactSFX, AudioClip[][]>();
        Dictionary<FootstepSFX, AudioClip[][]> footstepSFXDict = new Dictionary<FootstepSFX, AudioClip[][]>();
        Dictionary<WalkSoundFX, AudioClip[][]> walkSFXDict = new Dictionary<WalkSoundFX, AudioClip[][]>();
        Dictionary<HandSFX, AudioClip[][]> handSFXDict = new Dictionary<HandSFX, AudioClip[][]>();
        Dictionary<GunSFX, AudioClip[][]> gunSFXDict = new Dictionary<GunSFX, AudioClip[][]>();
        //Dictionary<Type, PropertyInfo[]> typeToPropInfo = new Dictionary<Type, PropertyInfo[]>();
        public VineBoomSounds() : base("Vine Boom Sound Effects", 30) { Init(); }

        private void Init()
        {
            vineBoomSound = GlobalVariables.EffectResources.LoadAsset<AudioClip>(GlobalVariables.ResourcePaths.FirstOrDefault(p => p.ToLower().Contains("vineboom")));
            vineBoomSound.hideFlags = HideFlags.DontUnloadUnusedAsset;

#if DEBUG
            Chaos.Log("Loaded the moyai sound into VineBoomSounds");
            if (vineBoomSound == null) Chaos.Error("Scratch that, it's null. Blame the IRS. And the CIA, those bioluminescent fucks");
#endif

            //var asmCSharp = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            //foreach (var type in asmCSharp.GetTypes())
            //{
            //    var props = type.GetProperties();
            //    if (props.Length == 0) continue;
            //    foreach (var prop in props)
            //    {

            //    }
            //}
        }

        public override void OnEffectStart()
        {
            #region Initialize
            if (vineBoomSound == null) Init();
            #endregion

            /* Here's what this does, and why it does it
             * BONEWORKS doesn't solely use AudioSource's for its sound effects, it has classes that are specifically _dedicated_ to sound effects, and they share AudioSource's
             * This is, I think, because having fuck tons of AS's is not possible, or otherwise costly, especially considering Unity's virtual audio channels that it automatically manages
             * Having SFX components share AS's avoids the problem of some AS's playing over each other or having 20,000 AS's in a scene for each object that needs to make noise when hit
             * This comes at the cost of sound effects cutting each other out, but it is an ingenious solution, as instead of having 20,000 AS's, there are 20,000 SFX's
             * Now here's where I come in, trying to fuck with everything and make it play the vine boom sound effect.
             * 
             * Frankly, I should've just done a harmony patch on the .Play method, but I decided to do things the hard way
             * The hard way being taking EVERY SINGLE SFX AND PUT IT INTO A FUCKING DICTIONARY WITH ITS SOUNDS
             * Then, do what a normal person would do, replace the sounds
             * And when the effect ends, take the shit from the dictionary, and slap it back into the SFXes
             */

            // Create an array here so I don't have to constantly do new AudioClip[] { };
            var boomArray = new AudioClip[] { vineBoomSound, vineBoomSound, vineBoomSound, vineBoomSound, vineBoomSound, vineBoomSound, };

            

            // SAUCE?
            foreach (var sauce in Utilities.FindAll<AudioSource>())
            {
                sourceDict.Add(sauce, sauce.clip);
                sauce.clip = vineBoomSound;
            }

            foreach (var iSFX in Utilities.FindAll<ImpactSFX>())
            {
                impactSFXDict.Add(iSFX, new AudioClip[][]
                {
                    iSFX.altImpact,
                    iSFX.destruction,
                    iSFX.impactHard,
                    iSFX.impactSoft,
                    iSFX.jointBreak,
                });

                iSFX.altImpact = boomArray;
                iSFX.destruction = boomArray;
                iSFX.impactHard = boomArray;
                iSFX.impactSoft = boomArray;
                iSFX.jointBreak = boomArray;
            }

            foreach (var fSFX in Utilities.FindAll<FootstepSFX>())
            {
                footstepSFXDict.Add(fSFX, new AudioClip[][] {
                    fSFX.crouchConcrete, // this is a fucking empty array why am i doing this
                    fSFX.runConcrete,
                    fSFX.walkConcrete,
                    fSFX.walkMetal,
                });

                fSFX.crouchConcrete = boomArray;
                fSFX.runConcrete = boomArray;
                fSFX.walkConcrete = boomArray;
                fSFX.walkMetal = boomArray;
            }

            foreach (var wSFX in Utilities.FindAll<WalkSoundFX>())
            {
                walkSFXDict.Add(wSFX, new AudioClip[][] {
                wSFX.leftSteps,
                wSFX.rightSteps,
                });

                wSFX.leftSteps = boomArray;
                wSFX.rightSteps = boomArray;
            }

            foreach (var hSFX in Utilities.FindAll<HandSFX>())
            {
                handSFXDict.Add(hSFX, new AudioClip[][] {
                hSFX.backhand,
                hSFX.backhandSlowMo,
                hSFX.bodySlot,
                hSFX.fallbackImpact,
                new AudioClip[] {
                    hSFX.forceGrab,
                    hSFX.forceGrabLong,
                }});

                hSFX.backhand = boomArray;
                hSFX.backhandSlowMo = boomArray;
                hSFX.bodySlot = boomArray;
                hSFX.fallbackImpact = boomArray;
                hSFX.forceGrab = vineBoomSound;
                hSFX.forceGrabLong = vineBoomSound;
            }

            // fucking christ i should just use reflection to do this shit, find all fields of Il2CppReferenceArray<AudioClip> and replace that shit
            foreach (var gSFX in Utilities.FindAll<GunSFX>())
            {
                gunSFXDict.Add(gSFX, new AudioClip[][] {
                gSFX.dryFire,
                gSFX.dryFireSlow,
                gSFX.fire,
                gSFX.fireSlow,
                gSFX.grab,
                gSFX.magazineDrop,
                gSFX.magazineInsert,
                gSFX.secondGrip,
                gSFX.slideLockFire,
                gSFX.slideLockFireSlow,
                gSFX.slidePull,
                gSFX.slideRelease,
                });

                gSFX.dryFire = boomArray;
                gSFX.dryFireSlow = boomArray;
                gSFX.fire = boomArray;
                gSFX.fireSlow = boomArray;
                gSFX.grab = boomArray;
                gSFX.magazineDrop = boomArray;
                gSFX.magazineInsert = boomArray;
                gSFX.secondGrip = boomArray;
                gSFX.slideLockFire = boomArray;
                gSFX.slideLockFireSlow = boomArray;
                gSFX.slidePull = boomArray;
                gSFX.slideRelease = boomArray;
            }
        }

        public override void OnEffectEnd()
        {
            // so the working theory is, if the game is loading or destroying objects, and we get nulls, then TryGetValue should prevent us from getting screwed
            // and if its already loading, then findobjsoftype should just have an array of length 0
            foreach (var sauce in Utilities.FindAll<AudioSource>())
            {
                // if we put nulls into it, we should be fine
                if (sourceDict.TryGetValue(sauce, out AudioClip clip)) sauce.clip = clip;
            }

            foreach (var iSFX in Utilities.FindAll<ImpactSFX>())
            {
                if (impactSFXDict.TryGetValue(iSFX, out AudioClip[][] clipss))
                {
                    iSFX.altImpact = clipss[0];
                    iSFX.destruction = clipss[1];
                    iSFX.impactHard = clipss[2];
                    iSFX.impactSoft = clipss[3];
                    iSFX.jointBreak = clipss[4];
                }
            }

            foreach (var fSFX in Utilities.FindAll<FootstepSFX>())
            {
                if (footstepSFXDict.TryGetValue(fSFX, out AudioClip[][] clipss))
                {
                    fSFX.crouchConcrete = clipss[0];
                    fSFX.runConcrete = clipss[1];
                    fSFX.walkConcrete = clipss[2];
                    fSFX.walkMetal = clipss[3];
                }
            }

            foreach (var wSFX in Utilities.FindAll<WalkSoundFX>())
            {
                if (walkSFXDict.TryGetValue(wSFX, out AudioClip[][] clipss))
                {
                    wSFX.leftSteps = clipss[0];
                    wSFX.rightSteps = clipss[1];
                }
            }

            foreach (var hSFX in Utilities.FindAll<HandSFX>())
            {
                if (handSFXDict.TryGetValue(hSFX, out AudioClip[][] clipss))
                {
                    hSFX.backhand = clipss[0];
                    hSFX.backhandSlowMo = clipss[1];
                    hSFX.bodySlot = clipss[2];
                    hSFX.fallbackImpact = clipss[3];
                    hSFX.forceGrab = clipss[4][0];
                    hSFX.forceGrabLong = clipss[4][1];
                }
            }

            foreach (var gSFX in Utilities.FindAll<GunSFX>())
            {
                if (gunSFXDict.TryGetValue(gSFX, out AudioClip[][] clipss))
                {
                    gSFX.dryFire = clipss[0];
                    gSFX.dryFireSlow = clipss[1];
                    gSFX.fire = clipss[2];
                    gSFX.fireSlow = clipss[3];
                    gSFX.grab = clipss[4];
                    gSFX.magazineDrop = clipss[5];
                    gSFX.magazineInsert = clipss[6];
                    gSFX.secondGrip = clipss[7];
                    gSFX.slideLockFire = clipss[8];
                    gSFX.slideLockFireSlow = clipss[9];
                    gSFX.slidePull = clipss[10];
                    gSFX.slideRelease = clipss[11];
                }
            }
        }
    }
}
