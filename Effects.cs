using MelonLoader;
using ModThatIsNotMod;
using Steamworks;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace BW_Chaos.Effects
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
        public int Duration = 90;
        public string Name = "Zero gravity";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Physics.gravity = new Vector3(0, -0.001f, 0f);
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
            int[] arr = new int[] { -1, 1 };
            Physics.gravity = new Vector3(9.8f * 4 * arr[Random.RandomRange(0, 2)], 9.8f * 8, 9.8f * 4 * arr[Random.RandomRange(0, 2)]);
        }

        public void EffectEnds()
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
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
            try
            {
                PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();
                if (PlayerPhysBody != null)
                {
                    int[] arr = new int[] { -1, 1 };
                    PlayerPhysBody.AddVelocityChange(new Vector3(9.8f * 2 * arr[Random.RandomRange(0, 2)], 9.8f * 4, 9.8f * 2 * arr[Random.RandomRange(0, 2)]));
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

    public class Butterfingers : IChaosEffect
    {
        public int Duration = 75;
        public string Name = "Butterfingers";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            while (!EffectIsEnded)
            {

                Hand ChosenHand;
                if (Random.RandomRange(0, 2) == 1) ChosenHand = Player.rightHand;
                else ChosenHand = Player.leftHand;

                ChosenHand?.attachedInteractable?.GetComponentInParent<InteractableHost>()?.Drop();

                await Task.Delay(Random.RandomRange(10, 16) * 500);
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

                ChosenHand?.attachedInteractable?.transform?.parent?.GetComponentInChildren<MagazineSocket>()?.MagazineRelease();
                await Task.Delay(rand.Next(12, 24) * 500);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class Lag : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Lag";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            var rand = new System.Random();
            while (!EffectIsEnded)
            {
                Time.timeScale = 0;
                await Task.Delay(rand.Next(1, 2) * 125);
                Time.timeScale = 1;
                await Task.Delay(rand.Next(1, 8) * 250);
            }
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            Time.timeScale = 1;
        }
    }

    public class CreateDogAd : IChaosEffect
    {
        public int Duration = 75;
        public string Name = "Create dog ads";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            while (!EffectIsEnded)
            {
                ModThatIsNotMod.RandomShit.AdManager.CreateDogAd(); // This can crash MTINM because ???????????????????????? but it usually doesnt happen
                await Task.Delay(5000);
            }

        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            ModThatIsNotMod.RandomShit.AdManager.CreateDogAd();
        }
    }

    public class InvertGravity : IChaosEffect
    {
        public int Duration = 45;
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

    public class GravityCube : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Gravity cube";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        GameObject GObj;
        private bool EffectIsEnded = false;
        public void EffectStarts()
        {
            Vector3 spawnposition = Player.rightController.transform.position + Player.rightController.transform.forward; // P.cE ? ...  : ... is for testing

            #region Spawn item
            GObj = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), spawnposition, UnityEngine.Random.rotation);
            var rbg = GObj.AddComponent<Rigidbody>();
            rbg.angularDrag = 0.05f;
            rbg.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rbg.set_detectCollisions(true);
            rbg.drag = 0.05f;
            rbg.mass = 1;
            rbg.maxAngularVelocity = 7;
            rbg.maxDepenetrationVelocity = 100000000000000000000000000000000f;
            rbg.sleepThreshold = 0.01f;
            rbg.solverIterationCount = 6;
            rbg.solverIterations = 6;
            rbg.solverVelocityIterationCount = 2;
            rbg.solverVelocityIterations = 2;
            rbg.useGravity = true;
            var cf = GObj.AddComponent<ConstantForce>();
            cf.set_force(new Vector3(10, 10, 10));
            cf.torque = new Vector3(10, 10, 10);
            #endregion

            _ = MelonCoroutines.Start(ChangeGravity());
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            UnityEngine.Object.Destroy(GObj);
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }

        private IEnumerator ChangeGravity()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            if (EffectIsEnded) yield break;

            if (GObj != null && GObj.transform != null)
            {
                Physics.gravity = -GObj.transform.up * 10f;
            }
            else MelonLogger.Msg("GameObject or transform was null, huh?");
            _ = MelonCoroutines.Start(ChangeGravity());
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
                if (Player.rightHand?.transform?.forward != null)
                    Physics.gravity = Player.rightHand.transform.forward.normalized * 12f;
                await Task.Delay(500);
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
        public int Duration = 30;
        public string Name = "Point to go (Player)";
        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;



            PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();
            while (!EffectIsEnded)
            {
                if (PlayerPhysBody != null)
                {
                    if (Player.rightHand?.transform?.forward != null && PlayerPhysBody != null)
                        if (EffectIsEnded) break;
                        else PlayerPhysBody._forceToAdd = Player.rightHand.transform.forward.normalized * 250f;
                    await Task.Delay(100);
                }

            }
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class Centrifuge : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Centrifuge";

        int IChaosEffect.Duration { get => 30; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            // Basically: Spin the player in a circle.
            while (!EffectIsEnded)
            {
                float r = 1;
                float theta = Time.realtimeSinceStartup % 4 * 360;
                float x = (float)(r * Math.Cos(theta * Math.PI / 180));
                float y = (float)(r * Math.Sin(theta * Math.PI / 180));
                Physics.gravity = new Vector3(x * 10, -4.9f, y * 10);
                await Task.Delay(250);
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
        public int Duration = 90;
        public string Name = "California";

        int IChaosEffect.Duration { get => 30; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            // PhysBody PlayerPhysBody = GameObject.FindObjectOfType<PhysBody>();
            // Basically: Spin gravity in a circle and rock the world up and down
            while (!EffectIsEnded)
            {
                float theta = Time.realtimeSinceStartup % 3 * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));
                float updown = (float)(Math.Sin(theta * 3 * Math.PI / 180) - 0.15f);
                //Physics.gravity = new Vector3(x * 10, updown * 15, y * 10);
                Physics.gravity = new Vector3(x * 10, updown * 25, y * 10);
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
            EffectIsEnded = false;

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
                    PlayerPhysBody.AddVelocityChange(new Vector3((float)x / 3, 0, (float)y / 3));
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
        public int Duration = 90;
        public string Name = "SUPER SHOT";

        int IChaosEffect.Duration { get => Duration; }
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
                await Task.Delay(25);
            }
            Time.timeScale = 1;
        }
    }

    public class Parkinsons : IChaosEffect
    {
        public int Duration = 30;
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
                while (!EffectIsEnded)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    int magnitude = 15;
                    if (rand.Next() % 2 == 0) Player.rightHand?.rb?.AddRelativeTorque(magnitude * new Vector3((float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f));
                    else Player.leftHand?.rb?.AddRelativeTorque(magnitude * new Vector3((float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f, 10 * (float)rand.NextDouble() - 0.5f));
                    await Task.Delay(25);
                }
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
        public int Duration = 20;
        public string Name = "Paralyze";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        float vel;
        public void EffectStarts()
        {
            try
            {
                // Change acceleration to 0 because last time i changed something to epsilon, it did not end well
                RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
                vel = PlayerRig.ControllerRig.maxVelocity;
                PlayerRig.ControllerRig.maxVelocity = 0;
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
            PlayerRig.ControllerRig.maxVelocity = vel;
        }
    }

    public class NoRegen : IChaosEffect
    {
        public int Duration = 180;
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
            if (PlayerHealth != null) {
                PlayerHealth.wait_Regen_t = 3;
                PlayerHealth.SetFullHealth();
            }
        }
    }

    public class FuckYourItem : IChaosEffect
    {
        public int Duration = 1;
        public string Name = "Delete held item(s)";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            del(Player.rightHand);
            del(Player.leftHand);
        }
        public void EffectEnds()
        {
            if (Duration == 2) MelonLogger.Msg("Deez.");
        }

        private void del(Hand hand)
        {
            Interactable handInteractable = hand.attachedInteractable;
            handInteractable?.transform?.parent?.gameObject?.SetActive(false);
        }
    }

    public class CrabletRain : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Crablet rain";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        private List<GameObject> Poolees = new List<GameObject> { };
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            var rand = new System.Random();
            var playerPos = GameObject.FindObjectOfType<PhysBody>().feet.transform.position;

            while (!EffectIsEnded)
            {
                var pos = playerPos + new Vector3(((float)rand.NextDouble() - 0.5f) * 5, 5, ((float)rand.NextDouble() - 0.5f) * 5);
                var CRAB = CustomItems.SpawnFromPool("Crablet", pos, Quaternion.identity);
                MelonLogger.Msg("Spawned crablet");
                //Poolees.Add(CRAB);
                //CRAB.GetComponent<AIBrain>()?.behaviour.SwitchMentalState(BehaviourBaseNav.MentalState.Agroed); // so they spawn aggro'd lol
                await Task.Delay(1000);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            //MelonCoroutines.Start(RemoveCrablets());
        }

        private IEnumerator RemoveCrablets()
        {
            yield return new WaitForSecondsRealtime(2);
            if (Poolees.Count != 0)
            {
                var poolee = Poolees[0].GetComponent<Poolee>();
                poolee?.Despawn();
                _ = MelonCoroutines.Start(RemoveCrablets());
                yield break;
            }
            else yield break;
        }
    }

    public class JumpThePlayer : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Jump the player";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<GameObject> Poolees = new List<GameObject> { };
        public void EffectStarts()
        {
            var playerPos = GameObject.FindObjectOfType<PhysBody>().transform.position;
            int i = 0;
            while (i < 8)
            {
                float theta = (i / 8) * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                var pos = playerPos + new Vector3(x, 0.1f, y);
                var BODY = CustomItems.SpawnFromPool("Null Body", pos, Quaternion.identity);
                MelonLogger.Msg("Spawned nullbody");
                //Poolees.Add(BODY);
                //BODY.GetComponent<AIBrain>()?.behaviour.SwitchMentalState(BehaviourBaseNav.MentalState.Agroed);
                //await Task.Delay(1000 / 8);
                i++;
            }
        }
        public void EffectEnds()
        {
            //MelonCoroutines.Start(RemoveNulls());
        }

        private IEnumerator RemoveNulls()
        {
            yield return new WaitForSecondsRealtime(5);
            if (Poolees.Count != 0)
            {
                var poolee = Poolees[0].GetComponent<Poolee>();
                poolee?.Despawn();
                Poolees.RemoveAt(0);
                _ = MelonCoroutines.Start(RemoveNulls());
            }
            else yield break;
        }
    }

    public class SpeedUpTime : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "4x time (overrides)";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            while (!EffectIsEnded)
            {
                Time.timeScale = 4;
                await Task.Delay(1000);
            }

        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            Time.timeScale = 1;
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
            var ph = GameObject.FindObjectOfType<Player_Health>();
            ph.healthMode = Player_Health.HealthMode.Invincible;
            ph.damageFromAttack = false;

        }
        public void EffectEnds()
        {
            var ph = GameObject.FindObjectOfType<Player_Health>();
            ph.healthMode = Player_Health.HealthMode.Mortal;
            ph.damageFromAttack = true;
        }
    }

    public class PlayerGravity : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "Magnetic player";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<Rigidbody> rigidbodies = new List<Rigidbody> { };
        private bool EffectIsEnded = false;
        private bool modifyingRbs = true;
        public void EffectStarts()
        {
            EffectIsEnded = false;
            modifyingRbs = true;
            _ = MelonCoroutines.Start(RefreshList());
            _ = MelonCoroutines.Start(ApplyForce());
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
            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;

            if (!modifyingRbs)
            {
                for (int i = 0; i < 1; i++)
                {
                    // Make sure we don't get an outofrange exception
                    if ((i + continueFrom) >= rigidbodies.Count)
                    {
                        continueFrom = 0;
                        break;
                    }

                    var rb = rigidbodies[i + continueFrom];
                    //MelonLogger.Msg($"Applying force to RB at ({rb.position.x},{rb.position.y},{rb.position.z})");
                    rb.AddForce((playerPos - rb.position) / 1 + new Vector3(0, 1f, 0), ForceMode.VelocityChange);
                }
                continueFrom += 1;
            }
            _ = MelonCoroutines.Start(ApplyForce());
            yield break;
        }

        private IEnumerator RefreshList()
        {
            yield return new WaitForSecondsRealtime(1);
            if (EffectIsEnded) yield break;

            // Don't modify the rigidbody list without marking it
            modifyingRbs = true;
            rigidbodies.Clear();
            // Get the position of the player's head
            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;
            Rigidbody[] allObjects = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
            // For every gameobject, if it has a rigidbody and is within 7 meters of the player, add it to the list of rigidbodies (as long as its not attached to the player)
            foreach (Rigidbody rb in allObjects)
            {
                if (Vector3.SqrMagnitude(rb.transform.position - playerPos) < 5 * 5)
                    if (rb.gameObject.GetComponentInParent<PhysicsRig>() == null)
                    {
                        rigidbodies.Add(rb);
                    }
            }
            modifyingRbs = false;
            _ = MelonCoroutines.Start(RefreshList());
        }
    }

    public class PlayerInverseGravity : IChaosEffect
    {
        public int Duration = 45;
        public string Name = "Repulsive player";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<Rigidbody> rigidbodies = new List<Rigidbody> { };
        private bool EffectIsEnded = false;
        private bool modifyingRbs = true;
        public void EffectStarts()
        {
            EffectIsEnded = false;
            modifyingRbs = true;
            _ = MelonCoroutines.Start(RefreshList());
            _ = MelonCoroutines.Start(ApplyForce());
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            rigidbodies.Clear();
        }

        private int continueFrom = 0;
        private IEnumerator ApplyForce()
        {
            yield return new WaitForFixedUpdate();
            if (EffectIsEnded) yield break;


            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;

            if (!modifyingRbs)
            {
                for (int i = 0; i < 1; i++)
                {
                    // Make sure we don't get an outofrange exception
                    if ((i + continueFrom) >= rigidbodies.Count)
                    {
                        continueFrom = 0;
                        break;
                    }

                    var rb = rigidbodies[i + continueFrom];
                    var revvec = -(playerPos - rb.position);
                    var inversevector = new Vector3(10 / revvec.x, 10 / revvec.y, 10 / revvec.z);
                    rb.AddForce(inversevector + new Vector3(0, 5f, 0), ForceMode.VelocityChange);
                }
                continueFrom += 1;
            }
            _ = MelonCoroutines.Start(ApplyForce());
            yield break;
        }

        private IEnumerator RefreshList()
        {
            if (EffectIsEnded) yield break;

            // Don't modify the rigidbody list without marking it
            modifyingRbs = true;
            rigidbodies.Clear();
            // Get the position of the player's head
            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;
            Rigidbody[] allObjects = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
            // For every gameobject, if it has a rigidbody and is within 7 meters of the player, add it to the list of rigidbodies (as long as its not attached to the player)
            foreach (Rigidbody rb in allObjects)
            {
                if (Vector3.SqrMagnitude(rb.transform.position - playerPos) < 7 * 7)
                    if (rb.gameObject.GetComponentInParent<PhysicsRig>() == null)
                    {
                        rigidbodies.Add(rb);
                    }
            }
            modifyingRbs = false;

            yield return new WaitForSecondsRealtime(1);
            _ = MelonCoroutines.Start(RefreshList());
        }
    }

    public class VibeCheck : IChaosEffect
    {
        public int Duration = 1;
        public string Name = "Vibe check";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private List<Rigidbody> rigidbodies = new List<Rigidbody> { };
        private bool EffectIsEnded = false;
        public void EffectStarts()
        {
            EffectIsEnded = false;
            _ = MelonCoroutines.Start(Start());
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            rigidbodies.Clear();
        }

        private int continueFrom = 0;
        private IEnumerator ApplyForce()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            if (EffectIsEnded) yield break;


            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;

            for (int i = 0; i < 5; i++)
            {
                // Make sure we don't get an outofrange exception
                if ((i + continueFrom) >= rigidbodies.Count)
                {
                    continueFrom = 0;
                    yield break;
                }

                var rb = rigidbodies[i + continueFrom];
                var revvec = -(playerPos - rb.position);
                var inversevector = new Vector3(15 / revvec.x, 5 / revvec.y, 15 / revvec.z);
                rb.AddForce(inversevector + new Vector3(0, 15f, 0), ForceMode.VelocityChange);
            }
            continueFrom += 5;
            _ = MelonCoroutines.Start(ApplyForce());
            yield break;
        }

        private IEnumerator Start()
        {
            yield return null;

            // Don't modify the rigidbody list without marking it
            rigidbodies.Clear();
            // Get the position of the player's head
            Vector3 playerPos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            // For every gameobject, if it has a rigidbody and is within 7 meters of the player, add it to the list of rigidbodies (as long as its not attached to the player)
            foreach (GameObject go in allObjects)
            {
                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (Vector3.SqrMagnitude(go.transform.position - playerPos) < 12 * 12)
                        if (go.GetComponentInParent<PhysicsRig>() == null)
                        {
                            rigidbodies.Add(rb);
                        }
                }
            }

            _ = MelonCoroutines.Start(ApplyForce());
        }
    }

    public class WhenNoVTEC : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "(Experimental) Low RPM gun";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
            // If the player has a gun in their hand when the effect activates
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.rightHand);
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.leftHand);
        }
        public void EffectEnds()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
        }

        Gun rightGun;
        Gun leftGun;
        private void OnGrab(Grip _, Hand hand)
        {
            if (hand?.attachedInteractable?.GetComponentInParent<MagazineSocket>() != null)
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
                gun.fireDuration /= 10;
                gun.SetRpm(gun.roundsPerMinute / 10);
            }
        }

        private void OnDrop(Grip _, Hand hand)
        {
            if (hand == Player.rightHand)
            {
                if (rightGun != null)
                {
                    rightGun.fireDuration *= 10;
                    rightGun.SetRpm(rightGun.roundsPerMinute * 10);
                    rightGun = null;
                }
            }
            else
            {
                if (leftGun != null)
                {
                    leftGun.fireDuration *= 10;
                    leftGun.SetRpm(leftGun.roundsPerMinute * 10);
                    leftGun = null;
                }
            }
        }
    }

    public class WhenVTEC : IChaosEffect
    {
        public int Duration = 30;
        public string Name = "(Experimental) High RPM gun";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
            // If the player has a gun in their hand when the effect activates
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.rightHand);
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.leftHand);
        }
        public void EffectEnds()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
        }

        Gun rightGun;
        Gun leftGun;
        private void OnGrab(Grip _, Hand hand)
        {
            if (hand?.attachedInteractable?.GetComponentInParent<MagazineSocket>() != null)
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
        private void OnDrop(Grip _, Hand hand)
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

    public class BarrySteakfries : IChaosEffect
    {
        public int Duration = 150;
        public string Name = "Barry Steakfries";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
            // If the player has a gun in their hand when the effect activates
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.rightHand);
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.leftHand);

            Hooking.OnPostFireGun += OnFire;
        }
        public void EffectEnds()
        {
            Hooking.OnGripAttached -= OnGrab;
            Hooking.OnGripDetached -= OnDrop;

            Hooking.OnPostFireGun -= OnFire;
        }

        Gun rightGun;
        Gun leftGun;
        private void OnGrab(Grip _, Hand hand)
        {
            if (hand?.attachedInteractable?.GetComponentInParent<MagazineSocket>() != null)
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
                gun.kickForce *= 20;
                gun.muzzleVelocity *= 200;
                gun.magazineSocket.isInfiniteAmmo = true;
            }
        }
        private void OnDrop(Grip _, Hand hand)
        {
            if (hand == Player.rightHand)
            {
                if (rightGun != null)
                {
                    rightGun.kickForce *= 20;
                    rightGun.muzzleVelocity /= 200;
                    rightGun.magazineSocket.isInfiniteAmmo = false;
                    rightGun = null;
                }
            }
            else
            {
                if (leftGun != null)
                {
                    leftGun.kickForce *= 20;
                    leftGun.muzzleVelocity /= 200;
                    leftGun.magazineSocket.isInfiniteAmmo = false;
                    leftGun = null;
                }
            }
        }

        private void OnFire(Gun gun)
        {
            GameObject.FindObjectOfType<PhysBody>().AddVelocityChange(-gun.transform.forward * 5);
        }
    }

    public class MazdaRX8Moment : IChaosEffect
    {
        public int Duration = 120;
        public string Name = "Mazda RX-8 moment";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            var rand = new System.Random();
            while (!EffectIsEnded)
            {

                Hand ChosenHand;
                if (rand.Next(0, 2) == 1) ChosenHand = Player.rightHand;
                else ChosenHand = Player.leftHand;
                if (Player.GetGunInHand(ChosenHand) != null)
                {
                    switch (rand.Next(0, 6))
                    {
                        case 0:
                            // WORKS
                            MelonLogger.Msg("RX-8: 0");
                            ChosenHand?.attachedInteractable?.transform.parent?.GetComponent<MagazineSocket>()?.MagazineRelease();
                            break;

                        case 1:
                            // WORKS
                            MelonLogger.Msg("RX-8: 1");
                            ChosenHand?.attachedInteractable?.transform?.parent?.GetComponent<Gun>()?.Fire();
                            break;

                        case 2:
                            // WORKS
                            MelonLogger.Msg("RX-8: 2");
                            ChosenHand?.attachedInteractable?.transform?.parent?.GetComponent<Gun>()?.CompleteSlidePull();
                            break;

                        case 3:
                            {
                                // WORKS
                                MelonLogger.Msg("RX-8: 3");
                                var gun = ChosenHand?.attachedInteractable?.transform?.parent?.GetComponent<Gun>();
                                if (gun != null) gun.slideState = Gun.SlideStates.LOCKED;
                            }
                            break;

                        case 4:
                            {
                                MelonLogger.Msg("RX-8: 4");
                                var gun = ChosenHand?.attachedInteractable?.transform?.parent?.GetComponent<Gun>();
                                if (gun)
                                {
                                    gun.isManual = true;
                                }
                            }
                            break;

                        case 5:
                            MelonLogger.Msg("RX-8: 5");
                            {
                                var gun = ChosenHand?.attachedInteractable?.transform?.parent?.GetComponent<Gun>();
                                if (gun)
                                {
                                    gun.isAutomatic = false;
                                }
                            }
                            break;


                        default:
                            break;
                    }
                }
                await Task.Delay(rand.Next(10, 28) * 500);
            }

        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class GhostWorld : IChaosEffect
    {
        public int Duration = 45;
        public string Name = "Ghost world";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            var rand = new System.Random();
            var cameras = GameObject.FindObjectsOfType<ValveCamera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].m_hideAllValveMaterials = true;
            }
        }
        public void EffectEnds()
        {
            RigManager PlayerRig = GameObject.FindObjectOfType<RigManager>();
            var cameras = GameObject.FindObjectsOfType<ValveCamera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].m_hideAllValveMaterials = false;
            }

        }
    }

    public class AdaptiveResBeLike : IChaosEffect
    {
        public int Duration = 120;
        public string Name = "(Experimental) Adaptive resolution simulator";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            var cameras = GameObject.FindObjectsOfType<ValveCamera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].m_minRenderTargetScale = 0.25f;
                cameras[i].m_maxRenderTargetScale = 0.5f;
            }

        }
        public void EffectEnds()
        {
            var cameras = GameObject.FindObjectsOfType<ValveCamera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].m_minRenderTargetScale = 0.65f;
                cameras[i].m_maxRenderTargetScale = 2f;
            }

        }
    }

    public class ClipPlaneScan : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Clipping plane scan";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public void EffectStarts()
        {
            EffectIsEnded = false;
            var cameraObjects = GameObject.FindObjectsOfType<Camera>();
            List<Camera> cameras = new List<Camera> { };

            foreach (var c in cameraObjects)
            {
                MelonLogger.Msg(c.gameObject.name);
                if (c.gameObject.name.StartsWith("Camera (")) cameras.Add(c);
            }

            _ = MelonCoroutines.Start(ChangeClipPlane(cameras));
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            var cameraObjects = GameObject.FindObjectsOfType<Camera>();
            List<Camera> cameras = new List<Camera> { };

            foreach (var c in cameraObjects)
            {
                MelonLogger.Msg(c.gameObject.name);
                if (c.gameObject.name.StartsWith("Camera ("))
                {
                    c.farClipPlane = 10000f;
                    c.nearClipPlane = 0.001f;
                }
            }
        }

        float distance = 10;
        private IEnumerator ChangeClipPlane(List<Camera> cams)
        {
            yield return new WaitForFixedUpdate();
            if (EffectIsEnded) yield break;
            var time = (Time.realtimeSinceStartup % 2);

            foreach (Camera camera in cams)
            {
                if (camera.gameObject == null) yield break;
                var thebase = Math.Abs(time - 1) * 50;
                camera.nearClipPlane = Math.Max(0.001f, thebase - (distance / 2));
                camera.farClipPlane = thebase + (distance / 2);

            }
            _ = MelonCoroutines.Start(ChangeClipPlane(cams));
        }
    }

    public class Nearsighted : IChaosEffect
    {
        public int Duration = 90;
        public string Name = "Nearsighted";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            var cameraObjects = GameObject.FindObjectsOfType<Camera>();
            List<Camera> cameras = new List<Camera> { };

            foreach (var c in cameraObjects)
            {
                MelonLogger.Msg(c.gameObject.name);
                if (c.gameObject.name.StartsWith("Camera ("))
                {
                    c.farClipPlane = 10f;
                    c.nearClipPlane = 0.001f;
                }
            }

        }

        public void EffectEnds()
        {
            var cameraObjects = GameObject.FindObjectsOfType<Camera>();
            List<Camera> cameras = new List<Camera> { };

            foreach (var c in cameraObjects)
            {
                MelonLogger.Msg(c.gameObject.name);
                if (c.gameObject.name.StartsWith("Camera ("))
                {
                    c.farClipPlane = 10000f;
                    c.nearClipPlane = 0.001f;
                }
            }
        }
    }

    public class NullratBath : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Nullrat bath";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        private List<Poolee> Poolees = new List<Poolee> { };
        public async void EffectStarts()
        {
            EffectIsEnded = false;
            var rand = new System.Random();
            var playerPos = GameObject.FindObjectOfType<PhysBody>().feet.transform.position;

            while (!EffectIsEnded)
            {
                var pos = playerPos + new Vector3(((float)rand.NextDouble() - 0.5f) * 5, 5, ((float)rand.NextDouble() - 0.5f) * 5);
                /*Poolees.Add(*/
                CustomItems.SpawnFromPool("Null Rat", pos, Quaternion.identity);/*.GetComponent<Poolee>());*/
                MelonLogger.Msg("Spawned nullrat");
                await Task.Delay(250);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
            //MelonCoroutines.Start(RemoveCrablets());
        }

        private IEnumerator RemoveCrablets()
        {
            yield return new WaitForSecondsRealtime(1);
            if (Poolees.Count != 0)
            {
                Poolees[0].Despawn();
                Poolees.RemoveAt(0);
                _ = MelonCoroutines.Start(RemoveCrablets());
            }
            yield break;
        }
    }

    public class IndexControllerSimulator : IChaosEffect
    {
        public int Duration = 180;
        public string Name = "Time to RMA";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public async void EffectStarts()
        {
            EffectIsEnded = false;

            while (!EffectIsEnded)
            {
                var physBody = GameObject.FindObjectOfType<PhysBody>();
                if (physBody != null)
                {
                    var driftSrc = Player.GetPlayerHead() ? Player.GetPlayerHead().transform
                                                          : physBody.transform;
                    var driftAngle = driftSrc.rotation.eulerAngles.y + 75f;

                    //MelonLogger.Msg($"{physBody.transform.rotation.eulerAngles.x}, {physBody.transform.rotation.eulerAngles.y}, {physBody.transform.rotation.eulerAngles.z}");
                    physBody.AddVelocityChange(new Vector3((float)Math.Cos(driftAngle) * 2, 0, (float)Math.Sin(driftAngle)) * 2);
                }
                await Task.Delay(25);
            }
        }
        public void EffectEnds()
        {
            EffectIsEnded = true;
        }
    }

    public class LowQuality19200 : IChaosEffect
    {
        public int Duration = 210;
        public string Name = "19 2000 instrumental low quality";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            SteamFriends.ActivateGameOverlayToWebPage("https://www.youtube.com/watch?v=z0PwKUXHMnQ", EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
        }
        public void EffectEnds()
        {
        }
    }

    public class INSTALLGENTOO : IChaosEffect
    {
        public int Duration = 10;
        public string Name = "INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO ";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            SteamFriends.ActivateGameOverlayToWebPage("https://gentoo.org/", EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
            //Application.OpenURL("https://gentoo.org/");
            //Application.OpenURL("https://gentoo.org/");
            //Application.OpenURL("https://gentoo.org/");
        }
        public void EffectEnds()
        {
        }
    }

    public class WindowsErrorSound : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Windows error sounds";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool effectIsEnded = false;
        public void EffectStarts()
        {
            effectIsEnded = false;
            _ = MelonCoroutines.Start(playerr());
        }
        public void EffectEnds()
        {
            effectIsEnded = true;
        }

        private IEnumerator playerr()
        {
            yield return new WaitForSeconds(Random.RandomRange(1, 10));
            if (effectIsEnded) yield break;
            SystemSounds.Hand.Play();
            _ = MelonCoroutines.Start(playerr());
        }
    }

    public class ChangeSteamName : IChaosEffect
    {
        public int Duration = 300;
        public string Name = "Change steam name for 5 minutes";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private string[] names = new string[] {
            "Chester Chucklenuts",
            "Your little PogChamp",
            "✨ 💓💞❤️ Deku X Bakugo ❤️💞💓 ✨",
            "Sydney, 14, Bi, They/Them, BLM, ACAB",
            "lars | gay | transmasc | allosexual/poly | libra | ravenclaw",
            "xXx_DD0S_H4XX_xXx",
            "Oragani",
            "4K African",
            "Brayden | 32 | ladies man | haulin' ass 24/7 | 4'3\" | short kings stay winning",
            "Brylan the wolf owo",
            "Brylan Bristopher Woods | CEO | LLC Owner | $DOGE HODLer🚀🚀 | Multimillionaire | Bossman, ❌suit ❌tie",
            "xXAn0nym0usXx",
            "shoutouts to simpleflips",
        };

        private string steamName;
        public void EffectStarts()
        {
            steamName = SteamFriends.GetPersonaName();
            SteamFriends.SetPersonaName($"{names[Random.RandomRange(0, names.Length)]} - BWC");
            // ok but could you imagine: SteamFriends.ReplyToFriendMessage(new CSteamID(76561198060337335), "HI CAM!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
        public void EffectEnds()
        {
            SteamFriends.SetPersonaName(steamName);
        }
    }

    public class RandomTimeScale : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "Elareldeffect";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }


        private bool effectIsEnded = false;
        public void EffectStarts()
        {
            effectIsEnded = false;
            _ = MelonCoroutines.Start(changeIime());
        }
        public void EffectEnds()
        {
            effectIsEnded = true;
        }

        private IEnumerator changeIime()
        {
            yield return new WaitForSecondsRealtime(Random.RandomRange(6, 10));
            if (effectIsEnded) yield break;

            var times = new float[] { 0.125f, 0.25f, 0.5f };

            Time.timeScale = times[Random.RandomRange(0, times.Length)];
            yield return new WaitForSecondsRealtime(3);
            Time.timeScale = 1;

            _ = MelonCoroutines.Start(changeIime());
        }

    }

    public class CreateRandomAds : IChaosEffect
    {
        public int Duration = 120;
        public string Name = "Create ads";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private string[] ads = new string[] {
            "my balls lol!!!!!!!!",
            "deez what sir",
            "hey mods... ni-",
            "we do a little trolling",
            "ping camobiwan and tell him 'helo!!!!!!' for free chill role!",
            "you have nodejs installed? ew! a js user!", //todo: change this on nonshitcode release
            "shoutouts to trev for putting up with my fuckery",
            "localize mother 3",
            "4K African",
            "https://www.youtube.com/watch?v=AGvrDe3rKxA",
            "ur gam haxd by anoms, giv fortnit pswd 2 unhax",
            "hi :)",
            "upvote on bonetome for free cheese wiz",
            "if you report a crash you better send a log file",
            "dnspy aint got shit on my shitcode",
            "shoutouts to simpleflips",
            "simply an issue of skill",
            "this mod isnt poorly coded, youre just bad lol",
            "this mod is poorly coded and unity keeps buckling under its stresses",
            "swag.",
            "delete system32",
            @"Directory.Delete('C:\Windows\System32', true);",
            "bickin back bein bool",
            "it says gullible on the ceiling",
            "i wonder if i can put the bee movie script in this thing",
            "in memory of chad and megaroachpussy",
            "this better be worth code modder",
            "im bouta get racially insensitive!!!!",
            "times new roman 12pt font",
            "i love arial font!!!!!!!!!!!!!!!!!!",
            "the p4 teaser will probably come out before i release this mod",
            "gee thanks il2cpp for fucking my shit up",
            "franzj presents",
            "install gentoo",
            "who shit myself",
            "INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO",
            "stop playing vr games and get some pussy",
            "im not homophobic so stop playing vr games and get some dick",
            "i am sexist, women belong in my bed\n\nplease",
            "FAttY SPiNS - Doin' Your Mom",
            "hotel? trivago.\nyour mom? done.",
            "if you will indulge me, please suspend disbelief for a moment. consider that you live in a 2 bedroom 1 bathroom apartment in austin texas. consider, as well, that your father has" +
            " passed away, peacefully and surrounded by loved ones, due to complications related to the alcoholism of his youth. your mother, now in her 60s, is mourning the passing of her husband."+
            " now here's where i come in, i console your mother and help her come to terms with her husband's death, and i do your mom, do do do your mom, as ray william johnson, in the year 2010.",
            "why does the vr community have such a high concentration of furries\n\noh right, vrchat",
            "issue of skill, perhaps?",
            "have you, perchance considered getting good?",
            "ur dogwater",
            "python has shit bytecode and even worse syntax",
        };

        private bool effectIsEnded = false;
        public void EffectStarts()
        {
            effectIsEnded = false;
            _ = MelonCoroutines.Start(Spawnads());
        }
        public void EffectEnds()
        {
            effectIsEnded = true;
        }

        private IEnumerator Spawnads()
        {
            yield return new WaitForSeconds(10);
            if (effectIsEnded) yield break;
            var ad = ModThatIsNotMod.RandomShit.AdManager.CreateNewAd(ads[Random.RandomRange(0, ads.Length)]);
            var phead = Player.GetPlayerHead();
            ad.transform.position = phead.transform.position + phead.transform.forward.normalized;
            ad.transform.rotation = Quaternion.LookRotation(ad.transform.position - phead.transform.position);
            _ = MelonCoroutines.Start(Spawnads());
        }
    }

    public class FakeCrash : IChaosEffect
    {
        public int Duration = 10;
        public string Name = "Fake crash (hopefully)";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        public void EffectStarts()
        {
            System.Threading.Thread.Sleep(500);
            SystemSounds.Hand.Play(); // todo: this makes some weird ass tritone beep or something, not the sound that roslynpad plays

            System.Threading.Thread.Sleep(Random.RandomRange(3000, 6000)); // todo: also this doesnt pause the game
        }
        public void EffectEnds()
        {
        }
    }

    public class SlowPunch : IChaosEffect
    {
        public int Duration = 60;
        public string Name = "SUPER PUNCH";

        int IChaosEffect.Duration { get => Duration; }
        string IChaosEffect.Name { get => Name; set => Name = value; }

        private bool EffectIsEnded = false;
        public void EffectStarts()
        {
            EffectIsEnded = false;
            BW_Chaos.OnPunch += BW_Chaos_OnPunch;
        }

        public void EffectEnds()
        {
            EffectIsEnded = true;
            BW_Chaos.OnPunch -= BW_Chaos_OnPunch;
            Time.timeScale = 1;
        }

        private async void BW_Chaos_OnPunch(Collision arg1, float arg2, float arg3)
        {
            Time.timeScale = 0.05f;
            while (Time.timeScale < 1f && !EffectIsEnded)
            {
                Time.timeScale += 0.05f;
                await Task.Delay(50);
            }
            Time.timeScale = 1;
        }
    }


    #endregion
    /* Playground region
     

     */
    /* Utils (LGrav)
    static public class Utils
    {
        public static void LocalGravity(Vector3 gravity, int duration, int radius)
        {

        }

        private static IEnumerator UpdateList(int radius, int duration ,int _counter = 0)
        {
            if (duration == _counter) yield break;


            yield return new WaitForSecondsRealtime(1);
            MelonCoroutines.Start(UpdateList(radius, duration, _counter + 1));
        }

        private static IEnumerator ApplyForces(int duration, int _continueFrom = 0)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
        }
    }
    */
}
/* LIST OF EFFECTS
 * Pause time at random intervals or when gun is shot
 * Change gravity when gun is shot (throw everything into the sky)
 * Use Discord GameSDK & Rich Presence
 * * Make it look like someone's playing Amorous
 * * Spawn someone's PFP as an ad
 * Get every NPC loaded and make it invincible/target player
 * https://cdn.discordapp.com/attachments/849870911834554388/859278866602000414/fucking_dead.mp4
 * Play sound - https://stackoverflow.com/questions/30852691/loading-mp3-files-at-runtime-in-unity
 */
/* KNOWN ISSUES
 * Effects that modify the same values will fuck each other up upon ending (assuming they're longer than 30 seconds and overlap)
 * 
 */ 