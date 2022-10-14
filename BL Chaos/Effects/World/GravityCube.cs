using ModThatIsNotMod;
using System.Collections;
using UnityEngine;

namespace BLChaos.Effects;

internal class GravityCube : EffectBase
{
    public GravityCube() : base("Gravity Cube", 90, EffectTypes.AFFECT_GRAVITY) { }

    private Transform gravObject;
    private Vector3 previousGrav;

    public override void OnEffectStart()
    {
        previousGrav = Physics.gravity;

        Vector3 spawnPosition = Player.rightController.transform.position + Player.rightController.transform.forward;
        gravObject = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        gravObject.position = spawnPosition;
        gravObject.rotation = Random.rotation;
        Rigidbody rb = gravObject.gameObject.AddComponent<Rigidbody>();
        rb.angularDrag = 0.05f;
        rb.drag = 0.05f;
    }

    public override void OnEffectUpdate()
    {
        Physics.gravity = -gravObject.up * 12f;
        if (Time.frameCount % 4 == 0) SendNetworkData(gravObject.transform.SerializePosRot());
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        #region Null check and debug log

        if (gravObject == null)
        {
#if DEBUG
            Chaos.Warn(nameof(gravObject) + " is null, but it's trying to be moved!");
#endif
            return;
        }

        #endregion

        gravObject.DeserializePosRot(data);
    }

    public override void OnEffectEnd()
    {
        Physics.gravity = previousGrav;
        GameObject.Destroy(gravObject);
    }

    // make sure its not fucking BORING!!!!
    [AutoCoroutine]
    public IEnumerator KeepItSpicy()
    {
        yield return null;
        Vector3 oldGrav = Physics.gravity;
        while (Active)
        {
            yield return new WaitForSecondsRealtime(5);
            if (Physics.gravity == oldGrav) gravObject.rotation = Random.rotation;
            oldGrav = Physics.gravity;
        }
    }
}
