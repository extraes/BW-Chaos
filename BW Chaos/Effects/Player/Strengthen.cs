using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using StressLevelZero.VRMK;

namespace BWChaos.Effects
{
    internal class Strengthen : EffectBase
    {
        public Strengthen() : base("Live Simulation of ME (Chad)", 60) { }

        public override void OnEffectStart()
        {
            MultiplyForces(Player.leftHand.physHand);
            MultiplyForces(Player.rightHand.physHand);
        }

        public override void OnEffectEnd()
        {
            MultiplyForces(Player.leftHand.physHand, (1/200f), (1/10f));
            MultiplyForces(Player.rightHand.physHand, (1/200f), (1/10f));
        }

        /*
         * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
         * I GANKED THE FUCK OUT OF THIS CODE FROM LAKATRAZZ'S SPIDERMAN MOD
         *      IT'S NOT OPEN SOURCE SO I HOPE I DON'T GET SUED, BUT IF HE WANTS THIS GONE, FINE BY ME, ILL REMOVE IT
         *      I DNSPY'D THE SPIDERMAN MOD AND COPY PASTED THE SPIDERMAN.MODOPTIONS.MULTIPLYFORCES METHOD
         *      I THINK ITS FINE THO CAUSE I ASKED HIM AND HE SAID "just take code from any of my mods tbh i dont care"
         *      (https://discord.com/channels/563139253542846474/724595991675797554/913613134885974036)
         * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
         */
        static void MultiplyForces(PhysHand hand, float mult = 200f, float torque = 10f)
        {
            hand.xPosForce = 90f * mult;
            hand.yPosForce = 90f * mult;
            hand.zPosForce = 340f * mult;
            hand.xNegForce = 90f * mult;
            hand.yNegForce = 200f * mult;
            hand.zNegForce = 360f * mult;
            hand.newtonDamp = 80f * mult;
            hand.angDampening = torque;
            hand.dampening = 0.2f * mult;
            hand.maxTorque = 30f * torque;
        }
        /*
         * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
         * I GANKED THE FUCK OUT OF THIS CODE FROM LAKATRAZZ'S SPIDERMAN MOD
         *      IT'S NOT OPEN SOURCE SO I HOPE I DON'T GET SUED, BUT IF HE WANTS THIS GONE, FINE BY ME, ILL REMOVE IT
         *      I DNSPY'D THE SPIDERMAN MOD AND COPY PASTED THE SPIDERMAN.MODOPTIONS.MULTIPLYFORCES METHOD
         *      I THINK ITS FINE THO CAUSE I ASKED HIM AND HE SAID "just take code from any of my mods tbh i dont care"
         *      (https://discord.com/channels/563139253542846474/724595991675797554/913613134885974036)
         * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
         */
    }
}
