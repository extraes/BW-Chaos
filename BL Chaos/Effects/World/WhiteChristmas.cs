using BoneLib;
using SLZ.Interaction;
using UnityEngine;

namespace BLChaos.Effects;

internal class WhiteChristmas : EffectBase
{
    public WhiteChristmas() : base("White Christmas", 30, EffectTypes.DONT_SYNC) { }
    static readonly Shader shader = Shader.Find(Const.URP_LIT_NAME);
    static readonly Material mat;

    // static initializer cause otherwise i cant set hideflags
    static WhiteChristmas()
    {
        mat = new Material(shader);
        mat.hideFlags = HideFlags.DontUnloadUnusedAsset;
        mat.color = Color.white;
    }

    public override void OnEffectUpdate()
    {
        Hand hand = Time.frameCount % 2 == 0 ? Player.leftHand : Player.rightHand;
        Vector3 dir = hand.transform.forward;

        if (Physics.Raycast(hand.transform.position + dir / 20, dir, out RaycastHit hitInfo, 50f))
        {
            Collider col = hitInfo.collider;
            MeshRenderer rend = col.GetComponentInParent<MeshRenderer>();
            if (rend != null)
            {
                rend.material = mat;
            }
        }
    }
}
