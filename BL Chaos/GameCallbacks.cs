using Il2CppSLZ.Interaction;
using Il2CppSLZ.Marrow.PuppetMasta;
using Jevil.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLChaos;

internal sealed class GameCallbacks : IPatcher
{
    private GameCallbacks() { }
    public static void Patch()
    {
        Chaos.Log("Patching AudioSource.Play");
        Redirect.FromMethod(typeof(AudioSource).GetMethod(nameof(AudioSource.Play), Array.Empty<Type>())!, AudioSourcePlay, false);
        Chaos.Log("Patching RemapRig.Jump");
        Hook.OntoMethod(typeof(RemapRig), nameof(RemapRig.Jump), RemapRigJump);
        Chaos.Log("Patching Projectile.OnEnable");
        Hook.OntoMethod(typeof(Projectile), nameof(Projectile.OnEnable), ProjectileOnEnable);
        Chaos.Log("Patching Projectile.Awake");
        Hook.OntoMethod(typeof(Projectile), nameof(Projectile.Awake), ProjectileAwake);
        Chaos.Log("Patching ButtonToggle.Awake");
        Hook.OntoMethod(typeof(ButtonToggle), nameof(ButtonToggle.Awake), ButtonToggleAwake);
        Chaos.Log("Patching HandSFX.PunchAttack");
        Hook.OntoMethod(typeof(HandSFX), nameof(HandSFX.PunchAttack), HandSFXPunchAttack);
        Chaos.Log("Patching InventoryAmmoReceiver.OnHandGrab");
        Hook.OntoMethod(typeof(InventoryAmmoReceiver), nameof(InventoryAmmoReceiver.OnHandGrab), InventoryAmmoReceiverOnHandGrab);

        PuppetMaster.add_OnDeathStatsEvent(new Action<PuppetMaster>(DeathStats));
        OnProjectileAwake += (Projectile proj) => proj.onCollision.AddListener(new Action<Collider, Vector3, Vector3>(ProjectileOnCollision));
    }

    public static event Action<AudioSource>? OnPreAudioSourcePlay;
    static void AudioSourcePlay(AudioSource src) => OnPreAudioSourcePlay?.InvokeSafeSync(src);
    
    public static event Action? OnJump;
    static void RemapRigJump()
    {
        if (!GlobalVariables.Player_PhysRig == null && GlobalVariables.Player_PhysRig.physG.isGrounded)
            OnJump?.InvokeSafeSync();
    }

    public static event Action<Projectile>? OnProjectileEnable;
    static void ProjectileOnEnable(Projectile proj) => OnProjectileEnable?.InvokeSafeSync(proj);

    public static event Action<Projectile>? OnProjectileAwake;
    static void ProjectileAwake(Projectile proj) => OnProjectileAwake?.InvokeSafeSync(proj);


    public static event Action? OnButtonPress;
    public static void ButtonToggleAwake(ButtonToggle __instance)
    {
        if (OnButtonPress is not null)
            __instance.onPress.AddListener(OnButtonPress);
    }

    public static event Action? OnPunch;
    static void HandSFXPunchAttack() => OnPunch?.InvokeSafeSync();

    public static event Action<Hand>? OnMagGrabbed;
    static void InventoryAmmoReceiverOnHandGrab(Hand hand) => OnMagGrabbed?.InvokeSafeSync(hand);

    public static event Action<PuppetMaster>? OnPuppetMasterDeath;
    static void DeathStats(PuppetMaster pm) => OnPuppetMasterDeath?.InvokeSafeSync(pm);

    /// <summary>
    /// col, pos, normal
    /// </summary>
    public static event Action<Collider, Vector3, Vector3>? OnBulletHit; 
    static void ProjectileOnCollision(Collider col, Vector3 pos, Vector3 normal) => OnBulletHit?.InvokeSafeSync(col, pos, normal);
}
