using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.PostProcess
{
    public sealed class ChaosPostFX : MonoBehaviour
    {
        public RenderTexture rTex;
        public Material mat;
        public MeshRenderer rend;
        MeshFilter mf;

        void Start()
        {
            RenderPipelineManager.endCameraRendering += EndCameraRendering;
            mf = rend.GetComponent<MeshFilter>();
        }


        void OnDestroy()
        {
            RenderPipelineManager.endCameraRendering -= EndCameraRendering;
        }

        void EndCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            CommandBuffer cmd = CommandBufferPool.Get("ChaosPostTest");
            cmd.Clear();
            //cmd.SetViewMatrix();

            //cmd.SetRenderTarget(cam.activeTexture);
            //cmd.SetRenderTarget(rTex);
            //cmd.ClearRenderTarget(true, true, Color.clear);
            cmd.SetViewMatrix(Camera.main.worldToCameraMatrix);
            cmd.SetProjectionMatrix(Camera.main.projectionMatrix);
            cmd.DrawMesh(mf.sharedMesh, rend.transform.localToWorldMatrix, mat, rend.subMeshStartIndex, 0);
            ctx.ExecuteCommandBuffer(cmd);
            //Debug.Log("Drew!");
            //Blitter.BlitTexture(cmd)
        }
    }
}
