namespace BLChaos;

internal static class Const
{
    public const int SizeV3 = sizeof(float) * 3;
    public const float FPI = (float)System.Math.PI;
    public const float DEFAULT_SEC_EACH_EFFECT = 30;

    public const string URP_LIT_NAME = "Universal Render Pipeline/Lit (PBR Workflow)";
    public const string URP_MAINTEX_NAME = "_BaseMap"; // .mainTex doesnt work anymore; go fuck yourself unity
}
