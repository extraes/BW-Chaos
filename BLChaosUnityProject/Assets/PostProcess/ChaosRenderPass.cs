using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChaosRenderPass : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RenderTargetIdentifier source;
        private Settings settings;
        static private Material _blitMat;
        private RenderTargetHandle tempTexHandle;

        public CustomRenderPass(Settings settings)
        {
            Debug.Log("h");
            this.settings = settings;
            tempTexHandle.Init("_MainTex");

            _EnsureBlitMat();
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            Debug.Log("i");
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Debug.Log("j");
            //Debug.Log(new System.Diagnostics.StackTrace());
            RenderTargetIdentifier cameraSource = renderingData.cameraData.renderer.cameraColorTarget;
            //renderingData.cameraData.camera.GetUniversalAdditionalCameraData
            CommandBuffer cmd = CommandBufferPool.Get("ChaosFX");
            cmd.Clear();

            //Debug.Log("1" + cameraSource);
            // new RenderTargetIdentifier(camTarget, 0, CubemapFace.Unknown, -1)
            cmd.GetTemporaryRT(tempTexHandle.id, renderingData.cameraData.cameraTargetDescriptor);

            cmd.SetGlobalTexture("_DepthTex", renderingData.cameraData.renderer.cameraDepthTarget);
            cmd.SetGlobalTexture("_MainTex", cameraSource);
            cmd.SetRenderTarget(tempTexHandle.Identifier());
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, settings.mat, 0, 0);

            //Blitlike(cmd, cameraSource, tempTexHandle.Identifier());
            Blitlike(cmd, tempTexHandle.Identifier(), cameraSource);

            //cmd.Blit(camTarget, tempTexHandle.Identifier(), settings.mat);
            //cmd.Blit(tempTexHandle.Identifier(), camTarget);
            //Debug.Log("2 " + cameraSource);

            cmd.ReleaseTemporaryRT(tempTexHandle.id);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            //cmd.Release();
            CommandBufferPool.Release(cmd);

            //context.DrawGizmos(renderingData.cameraData.camera, GizmoSubset.PostImageEffects);
        }

        static void Blitlike(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dst)
        {
            _EnsureBlitMat();

            cmd.SetGlobalTexture("_MainTex", src);
            cmd.SetRenderTarget(new RenderTargetIdentifier(dst, 0, CubemapFace.Unknown,  RenderTargetIdentifier.AllDepthSlices));
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _blitMat, 0, 0);
        }

        private static void _EnsureBlitMat()
        {
            if (_blitMat == null) 
                _blitMat = new Material(Shader.Find("extraes/BlitWithoutBlit"));

            _blitMat.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(_blitMat);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            Debug.Log("k");
        }
    }

    [Serializable]
    public class Settings
    {
        public Material mat = null;
    }

    public Settings settings = new Settings();
    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        Debug.Log($"Creating render pass for " + settings.mat);
        foreach (ScriptableRendererFeature srp in GameObject.FindObjectsOfType<ScriptableRendererFeature>())
        {
            Debug.Log($" - {srp}");
        }
        m_ScriptablePass = new CustomRenderPass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //if (settings.mat == null) return;
        Debug.Log($"Enqueued render pass {m_ScriptablePass} on {renderer}");
        m_ScriptablePass.source = renderer.cameraColorTarget;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
