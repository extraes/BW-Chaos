using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Data;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BW_Chaos_Effects
{
    public interface IChaosEffect
    {
        int Duration { get; }
        string Name { get; set; }

        void EffectStarts();

        void EffectEnds();
    }
    #region Effect definitions
    public class Template : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Template effect";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            MelonLogger.Error("The template effect should not be added to the list!");
        }
        public void EffectEnds()
        {
            MelonLogger.Error("Template effect \"ended\"");
        }
    }

    public class ZeroGravity : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Zero gravity";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Physics.gravity = new Vector3(0, 0.01f, 0f);
        }

        public void EffectEnds()
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);

        }
    }

    public class Fling : IChaosEffect
    {
        public int Duration = 1;
        public string Name = "Fling everything";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            var rand = new System.Random();
            int[] arr = new int[] { -1, 1 };
            Physics.gravity = new Vector3(9.8f * 5 * arr[rand.Next(0, 2)], 9.8f * 10, 9.8f * 5 * arr[rand.Next(0, 2)]);
        }

        public void EffectEnds()
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class Butterfingers : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Butterfingers";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            while (!EffectIsEnded)
            {
                var rand = new System.Random();

                Interactable HandInteractable = Player.leftHand.attachedInteractable;


                if (rand.Next(0, 2) == 1)
                {
                    HandInteractable = Player.rightHand.attachedInteractable;
                }
                if (HandInteractable != null)
                {
                    InteractableHost CompParent = HandInteractable.GetComponentInParent<InteractableHost>();
                    if (CompParent != null) CompParent.Drop();
                }

                await Task.Delay(rand.Next(4, 10) * 500);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class FuckYourMagazine : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Fuck your magazine";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            while (!EffectIsEnded)
            {
                var rand = new System.Random();

                Hand ChosenHand;
                if (rand.Next(0, 2) == 1) ChosenHand = Player.rightHand;
                else ChosenHand = Player.leftHand;


                Interactable interactable = ChosenHand.attachedInteractable;
                if (interactable == null) { MelonLogger.Msg("There was nothing in the selected hand"); return; }

                Transform parent = interactable.transform.parent;
                if (parent == null) { MelonLogger.Msg("The held item had no parent"); return; }

                MagazineSocket magsocket = parent.GetComponentInChildren<MagazineSocket>();
                if (magsocket == null) { MelonLogger.Msg("The parent of the held item didn't have a magazine socket"); return; }

                magsocket.MagazineRelease();

                await Task.Delay(rand.Next(8, 24) * 500);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class Lag : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Lag";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public bool EffectIsEnded = false;
        public async void EffectStarts()
        {

            var rand = new System.Random();
            while (!EffectIsEnded)
            {
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                }
                else Time.timeScale = 0;
                await Task.Delay(rand.Next(1, 4) * 250);
            }
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            Time.timeScale = 1;
        }
    }

    public class FlingPlayer : IChaosEffect
    {
        public int Duration = 2;
        public string Name = "Fling player";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            var rand = new System.Random();
            try
            {
                PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();
                if (PlayerPhysBody != null)
                {
                    int[] arr = new int[] { -1, 1 };
                    PlayerPhysBody.AddVelocityChange(new Vector3(9.8f * 3 * arr[rand.Next(0, 2)], 9.8f * 6, 9.8f * 3 * arr[rand.Next(0, 2)]));
                }
                else MelonLogger.Msg("Failed to fling player - PhysBody was null");
            }
            catch (Exception err)
            {
                MelonLogger.Error("Error flinging player");
                MelonLogger.Error(err);
            }
        }
        public void EffectEnds()
        {
        }
    }

    public class CreateDogAd : IChaosEffect
    {
        public int Duration = 15;
        public string Name = "Create 2 dog ads";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            ModThatIsNotMod.RandomShit.AdManager.CreateDogAd(); // This can crash MTINM because ???????????????????????? but it usually doesnt happen
        }
        public void EffectEnds()
        {
            ModThatIsNotMod.RandomShit.AdManager.CreateDogAd();
        }
    }

    public class InvertGravity : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Invert gravity";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Physics.gravity = new Vector3(0, 9.8f, 0);
        }
        public void EffectEnds()
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class BootlegGravityCube : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Bootleg gravity cube";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        GameObject GObj;
        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            if (CustomItems.customItemsExist)
            {
                Vector3 spawnposition = Player.controllersExist ? Player.rightController.transform.position + Player.rightController.transform.forward : new Vector3(0, 5, 0); // P.cE ? ...  : ... is for testing

                #region Spawn item
                SpawnableObject spawnable = null;
                while (spawnable == null || spawnable.title.Contains(".bcm") || spawnable.title.Contains("grenade") || spawnable.prefab.GetComponent<Magazine>() != null)
                    spawnable = CustomItems.GetRandomCustomSpawnable();
                MelonLogger.Msg($"Attempting to spawn {spawnable.title} at ({spawnposition.x}, {spawnposition.y}, {spawnposition.z}) with a random rotation for bootleg gravity cube");
                GObj = GameObject.Instantiate(spawnable.prefab, spawnposition, UnityEngine.Random.rotation);
                #endregion

                while (!EffectIsEnded)
                {
                    if (GObj != null && GObj.transform != null)
                    {
                        MelonLogger.Msg($"{spawnable.title}.up = {GObj.transform.up.x} {GObj.transform.up.y} {GObj.transform.up.z}");
                        Physics.gravity = -GObj.transform.up * 12f;
                    }
                    else
                    {
                        MelonLogger.Warning("The spawned game object, or its transform, was null");
                        EffectIsEnded = false; // Stop here to avoid console spam
                    }
                    await Task.Delay(250);
                }
            }
            else MelonLogger.Warning("No custom items - can't spawn bootleg gravity cube");
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            UnityEngine.Object.Destroy(GObj);
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class PointToGo : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Point to go";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            while (!EffectIsEnded)
            {
                Physics.gravity = Player.rightHand.transform.forward * 12f;
                await Task.Delay(100);
            }
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class PlayerPointToGo : IChaosEffect
    {
        public int Duration = 15;
        public string Name = "Point to go (Player)";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();

            if (PlayerPhysBody != null)
            {
                if (Player.handsExist)
                {
                    while (!EffectIsEnded)
                    {
                        PlayerPhysBody._forceToAdd = Player.rightHand.transform.forward.normalized * 1000f;
                        await Task.Delay(100);
                    }
                }
            }
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class Centrifuge : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Centrifuge";

        int IChaosEffect.Duration { get => 30; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            // Basically: Spin the player in a circle.
            while (!EffectIsEnded)
            {
                float r = 1;
                float theta = Time.realtimeSinceStartup % 4 * 360;
                float x = (float)(r * Math.Cos(theta * Math.PI / 180));
                float y = (float)(r * Math.Sin(theta * Math.PI / 180));
                Physics.gravity = new Vector3(x * 10, -4.9f, y * 10);
                await Task.Delay(100);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class California : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "California";

        int IChaosEffect.Duration { get => 30; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            // Basically: Spin gravity in a circle and rock the world up and down
            while (!EffectIsEnded)
            {
                float theta = Time.realtimeSinceStartup % 3 * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));
                float updown = (float)(Math.Sin(theta * 3 * Math.PI / 180) - 0.1f);
                Physics.gravity = new Vector3(x * 10, updown * 15, y * 10);
                await Task.Delay(100);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class PlayerCentrifuge : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Player Centrifuge";

        int IChaosEffect.Duration { get => 30; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private int lastFrame = 0;
        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {

            PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();

            if (PlayerPhysBody != null)
            {
                // Basically: Spin the player in a circle.
                while (!EffectIsEnded)
                {
                    double r = 1;
                    double theta = Time.realtimeSinceStartup % 5 * 360;
                    double x = r * Math.Cos(theta * Math.PI / 180);
                    double y = r * Math.Sin(theta * Math.PI / 180);
                    PlayerPhysBody.AddVelocityChange(new Vector3((float)x / 2, 0, (float)y / 2));
                    if (Time.frameCount == lastFrame) await Task.Delay(7);
                    lastFrame = Time.frameCount;
                }
            }
            else MelonLogger.Msg("Failed to centrifuge player - PhysBody was null");
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class SlowShooting : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "SUPER SHOT";

        int IChaosEffect.Duration { get => 30; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public void EffectStarts()
        {
            EffectIsEnded = false;
            Hooking.OnPostFireGun += BringBackTime;
        }
        public void EffectEnds()
        {
            Hooking.OnPostFireGun -= BringBackTime;
            EffectIsEnded = true;
            Time.timeScale = 1;
        }

        private async void BringBackTime(Gun gun)
        {
            Time.timeScale = 0.05f;
            while (Time.timeScale < 1f && !EffectIsEnded)
            {
                Time.timeScale += 0.05f;
                await Task.Delay(10);
            }
            Time.timeScale = 1;
        }
    }

    public class JetpackJoyride : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Barry Steakfries";

        int IChaosEffect.Duration { get => 30; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnPreFireGun += JetpackGun;
            Hooking.OnPostFireGun += JetpackGunPost;

        }
        public void EffectEnds()
        {
            Hooking.OnPreFireGun -= JetpackGun;
            Hooking.OnPostFireGun -= JetpackGunPost;
        }


        private void JetpackGun(Gun gun)
        {
            gun.kickForce = gun.kickForce * 100;
        }
        private void JetpackGunPost(Gun gun)
        {
            gun.kickForce = gun.kickForce / 100;
        }
    }

    public class Parkinsons : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Parkinsons";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            var rand = new System.Random();
            EffectIsEnded = false;
            try
            {
                PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();
                if (PlayerPhysBody != null)
                {
                    while (!EffectIsEnded)
                    {
                        if (rand.Next() % 2 == 0) Player.rightHand.rb.AddRelativeTorque(10 * new Vector3((float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f));
                        else Player.leftHand.rb.AddRelativeTorque(10 * new Vector3((float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f));
                        await Task.Delay(25);
                    }
                }
                else MelonLogger.Msg("Failed to parkinsons player - PhysBody was null");
            }
            catch (Exception err)
            {
                MelonLogger.Error("Error parkinsonsing player.");
                MelonLogger.Error(err);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class Paralyze : IChaosEffect
    {
        public int Duration = 15;
        public string Name = "Paralyze";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private Vector3 playerpos;
        private Quaternion playerrot;
        private bool EffectIsEnded = false;
        float accel;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            try
            {
                // Change acceleration to epsilon because last time i changed something to 0, it did not end well
                RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
                accel = PlayerRig.ControllerRig.maxAcceleration;
                PlayerRig.ControllerRig.maxAcceleration = float.Epsilon;


                PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();
                if (PlayerPhysBody != null)
                {
                    playerpos = PlayerPhysBody.gameObject.transform.position;
                    playerrot = PlayerPhysBody.gameObject.transform.rotation;
                    while (!EffectIsEnded)
                    {
                        PlayerPhysBody.gameObject.transform.SetPositionAndRotation(playerpos, playerrot);
                        await Task.Delay(250);
                    }
                }
            }
            catch (Exception err)
            {
                MelonLogger.Error("Error paralyzing player");
                MelonLogger.Error(err);
            }
        }
        public void EffectEnds()
        {
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            EffectIsEnded = true;
            PlayerRig.ControllerRig.maxAcceleration = accel;
        }
    }

    // VV NOT DONE VV
    public class VibeCheck : IChaosEffect
    {
        public int Duration = 15;
        public string Name = "Vibe check";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private Vector3 playerpos;
        private Quaternion playerrot;
        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            try
            {
                //var ande = new WNP78.Grenades.Grenade().explosion.Explode();
                //Poolee[] pooled = PoolManager.GetPool("Grenade")._pooledObjects.ToArray();
                PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();
                if (PlayerPhysBody != null)
                {
                    playerpos = PlayerPhysBody.gameObject.transform.position;
                    playerrot = PlayerPhysBody.gameObject.transform.rotation;
                    while (!EffectIsEnded)
                    {
                        PlayerPhysBody.gameObject.transform.SetPositionAndRotation(playerpos, playerrot);
                        await Task.Delay(250);
                    }
                }
            }
            catch (Exception err)
            {
                MelonLogger.Error("Error paralyzing player");
                MelonLogger.Error(err);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class NoRegen : IChaosEffect
    {
        public int Duration = 300;
        public string Name = "No regen";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Player_Health PlayerHealth = GameObject.FindObjectOfType<Player_Health>();
            PlayerHealth.wait_Regen_t = 420;
        }
        public void EffectEnds()
        {
            Player_Health PlayerHealth = GameObject.FindObjectOfType<Player_Health>();
            PlayerHealth.wait_Regen_t = 3;
        }
    }

    public class FuckYourItem : IChaosEffect
    {
        public int Duration = 1;
        public string Name = "Delete held item";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            var rand = new System.Random();
            Interactable HandInteractable = Player.leftHand.attachedInteractable;


            if (rand.Next(0, 2) == 1)
            {
                HandInteractable = Player.rightHand.attachedInteractable;
            }
            if (HandInteractable != null)
            {
                InteractableHost CompParent = HandInteractable.GetComponentInParent<InteractableHost>();
                if (CompParent != null)
                {
                    CompParent.Drop();
                    if (CompParent.gameObject != null) CompParent.gameObject.SetActive(false);
                    // Yes, this will almost certainly break doors and other related items. Cry about it.
                }
            }

        }
        public void EffectEnds()
        {
            if (Duration == 2) MelonLogger.Msg("Deez.");
        }
    }

    public class CrabletRain : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Crablet rain";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<GameObject> Crablets = new List<GameObject> { };
        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            var rand = new System.Random();
            EffectIsEnded = false;
            while (!EffectIsEnded)
            {
                var pos = Player.GetPlayerHead().transform.position + new Vector3(((float)rand.NextDouble() - 0.5f) * 5, 10, ((float)rand.NextDouble() - 0.5f) * 5);

                Crablets.Add(GameObject.Instantiate(PoolManager.GetPool("Crablet").Prefab, pos, Quaternion.identity));
                await Task.Delay(1000);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            MelonCoroutines.Start(RemoveCrablets());
        }

        private IEnumerator RemoveCrablets()
        {
            yield return new WaitForSeconds(1);
            if (Crablets.Count != 0)
            {
                GameObject.Destroy(Crablets[0]);
                Crablets.RemoveAt(0);
                MelonCoroutines.Start(RemoveCrablets());
            }
        }
    }

    public class Accelerate : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Instant acceleration";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            PlayerRig.ControllerRig.maxAcceleration = PlayerRig.ControllerRig.maxAcceleration * 25;
        }
        public void EffectEnds()
        {
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            PlayerRig.ControllerRig.maxAcceleration = PlayerRig.ControllerRig.maxAcceleration / 25;
        }
    }

    public class RandomRigShit : IChaosEffect
    {
        public int Duration = 3;
        public string Name = "Messing with random shit in the rig";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public async void EffectStarts()
        {
            var rand = new System.Random();
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            PlayerRig.ControllerRig.maxAcceleration = PlayerRig.ControllerRig.maxAcceleration * 25;
            PlayerRig.ControllerRig.slowMoEnabled = false;
            PlayerRig.ControllerRig.snapDegreesPerFrame = 83; // odd prime because fuck you
            PlayerRig.ControllerRig.Teleport(new Vector3((float)rand.NextDouble() * 2, (float)rand.NextDouble() * 2, (float)rand.NextDouble() * 2));
            await Task.Delay(1000);
            PlayerRig.ControllerRig.Jump();
        }
        public void EffectEnds()
        {
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            PlayerRig.ControllerRig.maxAcceleration = PlayerRig.ControllerRig.maxAcceleration / 25;

        }
    }

    public class SpeedUpTime : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "4x time (overrides)";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            PlayerRig.ControllerRig.slowMoEnabled = false;
            Time.timeScale = 4;

        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            Time.timeScale = 1;
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            PlayerRig.ControllerRig.slowMoEnabled = true;
        }
    }

    public class Immortality : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Immortality";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }


        public void EffectStarts()
        {
            GameObject.FindObjectOfType<Player_Health>().healthMode = Player_Health.HealthMode.Invincible;
        }
        public void EffectEnds()
        {
            GameObject.FindObjectOfType<Player_Health>().healthMode = Player_Health.HealthMode.Mortal;
        }
    }

    public class JumpThePlayer : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Jump the player";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<GameObject> Nulls = new List<GameObject> { };
        public async void EffectStarts()
        {
            var playerPos = GameObject.FindObjectOfType<PhysBody>().feet.transform.position;
            int i = 0;
            while (i < 7)
            {
                float theta = Time.realtimeSinceStartup % 1 * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                var pos = playerPos + new Vector3(x, 0.1f, y);
                Nulls.Add(GameObject.Instantiate(PoolManager.GetPool("Nullbody").Prefab, pos, Quaternion.identity));

                await Task.Delay(1000 / 7);
                i++;
            }
        }
        public void EffectEnds()
        {
            MelonCoroutines.Start(RemoveNulls());
        }

        private IEnumerator RemoveNulls()
        {
            yield return new WaitForSeconds(5);
            if (Nulls.Count != 0)
            {
                GameObject.Destroy(Nulls[0]);
                Nulls.RemoveAt(0);
                MelonCoroutines.Start(RemoveNulls());
            }
        }
    }

    public class PlayerGravity : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Magnetic player";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<Rigidbody> rigidbodies = new List<Rigidbody> { };
        private bool EffectIsEnded = false;
        private bool modifyingRbs = true;
        public async void EffectStarts()
        {
            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;
            EffectIsEnded = false;
            modifyingRbs = true;
            // Probably best to do this in an ienumerator
            // Get list of active gameobjects with rigidbodies and filter out the rigidbodies that are too far away
            MelonCoroutines.Start(ApplyForce());
            while (!EffectIsEnded)
            {
                modifyingRbs = true;
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject go in allObjects)
                {
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        if (Vector3.SqrMagnitude(go.transform.position - playerPos) < 15 * 15) 
                            if (go.GetComponentInParent<PhysicsRig>() != null)
                                rigidbodies.Add(rb);
                        
                    }
                }
                Array.Clear(allObjects, 0, allObjects.Length); // This is what the kids call "memory management" right?
                modifyingRbs = false;

                await Task.Delay(1000); // Update the list every second
            }

            // Start the routine
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }

        private int continueFrom = 0;
        private IEnumerator ApplyForce()
        {
            yield return new WaitForFixedUpdate();
            if (EffectIsEnded) yield break;

            if (rigidbodies.Count != 0)
            {
                Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;

                if (!modifyingRbs)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        // Make sure we don't get an outofrange exception
                        if (i == rigidbodies.Count)
                        {
                            continueFrom = 0;
                            break;
                        }

                        var rb = rigidbodies[i + continueFrom];
                        rb.AddForce((playerPos - rb.position) / 5, ForceMode.VelocityChange);
                        continueFrom += 15;
                    }
                }
                MelonCoroutines.Start(ApplyForce());
            }
            else yield break;
        }
    }

    public class PlayerInverseGravity : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Repulsive player";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<Rigidbody> rigidbodies = new List<Rigidbody> { };
        private bool EffectIsEnded = false;
        private bool modifyingRbs = true;
        public async void EffectStarts()
        {
            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;
            EffectIsEnded = false;
            modifyingRbs = true;
            // Probably best to do this in an ienumerator
            // Get list of active gameobjects with rigidbodies and filter out the rigidbodies that are too far away
            MelonCoroutines.Start(ApplyForce());
            while (!EffectIsEnded)
            {
                modifyingRbs = true;
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject go in allObjects)
                {
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        if (Vector3.SqrMagnitude(go.transform.position - playerPos) < 15 * 15)
                            if (go.GetComponentInParent<PhysicsRig>() != null)
                                rigidbodies.Add(rb);

                    }
                }
                Array.Clear(allObjects, 0, allObjects.Length); // This is what the kids call "memory management" right?
                modifyingRbs = false;

                await Task.Delay(1000); // Update the list every second
            }

            // Start the routine
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }

        private int continueFrom = 0;
        private IEnumerator ApplyForce()
        {
            yield return new WaitForFixedUpdate();
            if (EffectIsEnded) yield break;

            if (rigidbodies.Count != 0)
            {
                Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;

                if (!modifyingRbs)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        // Make sure we don't get an outofrange exception
                        if (i == rigidbodies.Count)
                        {
                            continueFrom = 0;
                            break;
                        }

                        var rb = rigidbodies[i + continueFrom];
                        rb.AddForce(-(playerPos - rb.position) / 5, ForceMode.VelocityChange);
                        continueFrom += 15;
                    }
                }
                MelonCoroutines.Start(ApplyForce());
            }
            else yield break;
        }
    }

    public class WhenNoVTEC : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Low RPM gun";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
            // If the player has a gun in their hand when the effect activates
            OnGrab(null, Player.rightHand);
            OnGrab(null, Player.leftHand);
        }
        public void EffectEnds()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
        }

        Gun rightGun;
        Gun leftGun;
        private void OnGrab(Grip grip, Hand hand)
        {
            if (hand.attachedInteractable.GetComponentInParent<MagazineSocket>() != null)
            {
                Gun gun;
                if (hand == Player.rightHand)
                {
                    gun = Player.GetGunInHand(hand);
                    rightGun = gun;
                }
                else
                {
                    gun = Player.GetGunInHand(hand);
                    leftGun = gun;
                }
                gun.SetRpm(gun.roundsPerMinute / 10);
            }
        }
        private void OnDrop(Grip grip, Hand hand)
        {
            if (hand == Player.rightHand)
            {
                if (rightGun != null)
                {
                    rightGun.SetRpm(rightGun.roundsPerMinute * 10);
                    rightGun = null;
                }
            }
            else
            {
                if (leftGun != null)
                {
                    leftGun.SetRpm(leftGun.roundsPerMinute * 10);
                    leftGun = null;
                }
            }
        }
    }

    public class WhenVTEC : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "High RPM gun";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
            // If the player has a gun in their hand when the effect activates
            OnGrab(null, Player.rightHand);
            OnGrab(null, Player.leftHand);
        }
        public void EffectEnds()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
        }

        Gun rightGun;
        Gun leftGun;
        private void OnGrab(Grip grip, Hand hand)
        {
            if (hand.attachedInteractable.GetComponentInParent<MagazineSocket>() != null)
            {
                Gun gun;
                if (hand == Player.rightHand)
                {
                    gun = Player.GetGunInHand(hand);
                    rightGun = gun;
                }
                else
                {
                    gun = Player.GetGunInHand(hand);
                    leftGun = gun;
                }
                gun.SetRpm(gun.roundsPerMinute * 10);
            }
        }
        private void OnDrop(Grip grip, Hand hand)
        {
            if (hand == Player.rightHand)
            {
                if (rightGun != null)
                {
                    rightGun.SetRpm(rightGun.roundsPerMinute / 10);
                    rightGun = null;
                }
            }
            else
            {
                if (leftGun != null)
                {
                    leftGun.SetRpm(leftGun.roundsPerMinute / 10);
                    leftGun = null;
                }
            }
        }
    }

    public class WhenVTEC : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "High RPM gun";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
            // If the player has a gun in their hand when the effect activates
            OnGrab(null, Player.rightHand);
            OnGrab(null, Player.leftHand);
        }
        public void EffectEnds()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
        }

        Gun rightGun;
        Gun leftGun;
        private void OnGrab(Grip grip, Hand hand)
        {
            if (hand.attachedInteractable.GetComponentInParent<MagazineSocket>() != null)
            {
                Gun gun;
                if (hand == Player.rightHand)
                {
                    gun = Player.GetGunInHand(hand);
                    rightGun = gun;
                }
                else
                {
                    gun = Player.GetGunInHand(hand);
                    leftGun = gun;
                }
                gun.SetRpm(gun.roundsPerMinute * 10);
            }
        }
        private void OnDrop(Grip grip, Hand hand)
        {
            if (hand == Player.rightHand)
            {
                if (rightGun != null)
                {
                    rightGun.SetRpm(rightGun.roundsPerMinute / 10);
                    rightGun = null;
                }
            }
            else
            {
                if (leftGun != null)
                {
                    leftGun.SetRpm(leftGun.roundsPerMinute / 10);
                    leftGun = null;
                }
            }
        }
    }

    #endregion
}

/* LIST OF EFFECTS
 * Pause time at random intervals or when gun is shot
 * Change gravity when gun is shot (throw everything into the sky)
 * Spawn the demon cube for 5s/longer with random rotation at level origin (-this.arrow.up * 12f)
 * Yeet the player
 * Jump the player (spawn 5+ enemies around player)
 * Chance to eject mag on gunshot
 * Cha-Cha slide: BoneworksModdingToolkit.Player.FindPlayer().transform.eulerAngles
 * Create dog ad: ModThatIsNotMod.RandomShit.AdManager.CreateDogAd
 * Super slow guns: ModThatIsNotMod.Extensions.SetRpm
 * Make gravity switch to wall/ceiling at intervals: Physics.gravity = new Vector3(0, 0.5f, 0);
 * An effect that, if it gets picked 5 times exits the game (or crashes pc based on melonprefs entry) <----- FUCK no!
 * Increases recoil or at least when guns are pointed downwards when fired, you get jetpack joyrided
 * Crablet/npc rain
 * Get every NPC loaded and make it invincible/target player
 */
/* KNOWN ISSUES
 * Effects that modify the same values will fuck each other up upon ending (assuming they're longer than 30 seconds and overlap)
 * 
 */ 