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
        void Start()
        {
            RenderPipelineManager.endCameraRendering += EndCameraRendering;
        }


        void OnDestroy()
        {
            RenderPipelineManager.endCameraRendering -= EndCameraRendering;
        }

        void EndCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            CommandBuffer cmd = CommandBufferPool.Get("ChaosPostFX");
            cmd.Clear();

            
            //Blitter.BlitTexture(cmd)
        }
    }
}
