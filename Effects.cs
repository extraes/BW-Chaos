using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Data;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using StressLevelZero.VRMK;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BW_Chaos_Effects
{
    public interface ChaosEffect
    {
        int Duration { get; }
        string Name { get; set; }

        void EffectStarts();

        void EffectEnds();
    }
    #region Effect definitions
    public class Placeholder : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Placeholder effect name";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            MelonLogger.Error("Placeholder effect invoked");
        }
        public void EffectEnds()
        {
            MelonLogger.Error("Placeholder effect ended");
        }
    }

    public class ZeroGravity : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Zero gravity";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Physics.gravity = new Vector3(0, 0.01f, 0f);
        }

        public void EffectEnds()
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);

        }
    }

    public class Fling : ChaosEffect
    {
        public int Duration = 1;
        public string Name = "Fling everything";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class Butterfingers : ChaosEffect
    {
        public int Duration = 60;
        public string Name = "Butterfingers";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class FuckYourMagazine : ChaosEffect
    {
        public int Duration = 90;
        public string Name = "Fuck your magazine";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class Lag : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Lag";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class FlingPlayer : ChaosEffect
    {
        public int Duration = 2;
        public string Name = "Fling player";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class CreateDogAd : ChaosEffect
    {
        public int Duration = 15;
        public string Name = "Create 2 dog ads";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            ModThatIsNotMod.RandomShit.AdManager.CreateDogAd(); // This can crash MTINM because ???????????????????????? but it usually doesnt happen
        }
        public void EffectEnds()
        {
            ModThatIsNotMod.RandomShit.AdManager.CreateDogAd();
        }
    }

    public class InvertGravity : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Invert gravity";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Physics.gravity = new Vector3(0, 9.8f, 0);
        }
        public void EffectEnds()
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }

    public class BootlegGravityCube : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Bootleg gravity cube";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class PointToGo : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Point to go";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class PlayerPointToGo : ChaosEffect
    {
        public int Duration = 15;
        public string Name = "Point to go (Player)";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    /*public class ForMyNextTrick : ChaosEffect
    {
        public int Duration = 2;
        public string Name = "For my next trick, float.MaxValue!";
        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

        private int NumberOfTimesRan = 0;
        public void EffectEnds()
        {
            NumberOfTimesRan++;
            if (NumberOfTimesRan > 5)
            {
                MelonLogger.Error("Here's a premature error. Physics.gravity was set to a vector3 with one float.MaxValue aka " + float.MaxValue);
                Physics.gravity = new Vector3(0, float.MaxValue, 0);
            }
        }

        public void EffectStarts()
        {
            MelonLogger.Msg("... The game didn't crash? Whatever, take your gravity and go.");
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }*/

    public class Centrifuge : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Centrifuge";

        int ChaosEffect.Duration { get => 30; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class California : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "California";

        int ChaosEffect.Duration { get => 30; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class PlayerCentrifuge : ChaosEffect
    {
        public int Duration = 60;
        public string Name = "Player Centrifuge";

        int ChaosEffect.Duration { get => 30; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class SlowShooting : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "SUPER SHOT";

        int ChaosEffect.Duration { get => 30; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class JetpackJoyride : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Barry Steakfries";

        int ChaosEffect.Duration { get => 30; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class Parkinsons : ChaosEffect
    {
        public int Duration = 90;
        public string Name = "Parkinsons";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class Paralyze : ChaosEffect
    {
        public int Duration = 15;
        public string Name = "Paralyze";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

        private Vector3 playerpos;
        private Quaternion playerrot;
        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            try
            {
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

    public class VibeCheck : ChaosEffect
    {
        public int Duration = 15;
        public string Name = "Vibe check";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    public class NoRegen : ChaosEffect
    {
        public int Duration = 300;
        public string Name = "No regen";

        int ChaosEffect.Duration { get => Duration; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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

    /*public class WhenNoVTEC : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "Low RPM gun";

        int ChaosEffect.Duration { get => 30; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
        }
        public void EffectEnds()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
        }


        private void OnGrab(Grip grip, Hand hand)
        {
            hand.attachedInteractable
            gun.SetRpm(gun.roundsPerMinute / 10);
        }
        private void OnDrop(Grip grip, Hand hand)
        {
            gun.SetRpm(gun.roundsPerMinute * 10);
        }
    }

    public class WhenVTEC : ChaosEffect
    {
        public int Duration = 30;
        public string Name = "High RPM gun";

        int ChaosEffect.Duration { get => 30; }
        string ChaosEffect.Name { get => Name; set => Name = value; }

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
            gun.SetRpm(gun.roundsPerMinute * 10);
        }
        private void JetpackGunPost(Gun gun)
        {
            gun.SetRpm(gun.roundsPerMinute / 10);
        }
    }*/
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
 */
/* KNOWN ISSUES
 * Effects that modify the same values will fuck each other up upon ending (assuming they're longer than 30 seconds and overlap)
 * 
 */ 