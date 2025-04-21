using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Blit : ScriptableRendererFeature
{
    public class BlitPass : ScriptableRenderPass
    {
        public enum RenderTarget
        {
            Color,
            RenderTexture,
        }

        public Material blitMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }

        private RTHandle source { get; set; }
        private RTHandle destination { get; set; }

        RTHandle m_TemporaryColorTexture;
        string m_ProfilerTag;
        private bool usesCameraTarget = false;

        public BlitPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag)
        {
            this.renderPassEvent = renderPassEvent;
            this.blitMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
        }

        public void Setup(RTHandle source, RTHandle destination, bool usesCameraTarget)
        {
            this.source = source;
            this.destination = destination;
            this.usesCameraTarget = usesCameraTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            if (usesCameraTarget)
            {
                //RT핸들 없으면 생성
                if (m_TemporaryColorTexture == null)
                {
                    m_TemporaryColorTexture = RTHandles.Alloc(opaqueDesc, name: "_TemporaryColorTexture");
                }

                Blitter.BlitCameraTexture(cmd, source, m_TemporaryColorTexture, blitMaterial, blitShaderPassIndex);
                Blitter.BlitCameraTexture(cmd, m_TemporaryColorTexture, destination);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, source, destination, blitMaterial, blitShaderPassIndex);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // RTHandle은 자동으로 관리되므로 명시적 해제 불필요
        }

        public void Dispose()
        {
            if (m_TemporaryColorTexture != null)
            {
                m_TemporaryColorTexture.Release();
                m_TemporaryColorTexture = null;
            }
        }
    }

    [System.Serializable]
    public class BlitSettings
    {
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

        public Material blitMaterial = null;
        public int blitMaterialPassIndex = 0;
        public Target destination = Target.Color;
        public string textureId = "_BlitPassTexture";
    }

    public enum Target
    {
        Color,
        Texture
    }

    public BlitSettings settings = new BlitSettings();
    RTHandle m_RenderTextureHandle;

    BlitPass blitPass;

    public override void Create()
    {
        var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
        settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
        blitPass = new BlitPass(settings.Event, settings.blitMaterial, settings.blitMaterialPassIndex, name);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.blitMaterial == null)
        {
            Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }

        // 렌더러에서 색상 타겟 가져오기
        var cameraColorTarget = renderer.cameraColorTargetHandle;
        RTHandle dest;
        bool usesCameraTarget = false;

        if (settings.destination == Target.Color)
        {
            dest = cameraColorTarget;
            usesCameraTarget = true;
        }
        else
        {
            //텍스처필요시 생성
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            if (m_RenderTextureHandle == null)
            {
                m_RenderTextureHandle = RTHandles.Alloc(desc, name: settings.textureId);
            }
            dest = m_RenderTextureHandle;
        }

        blitPass.Setup(cameraColorTarget, dest, usesCameraTarget);
        renderer.EnqueuePass(blitPass);
    }

    protected override void Dispose(bool disposing)
    {
        blitPass?.Dispose();

        if (m_RenderTextureHandle != null)
        {
            m_RenderTextureHandle.Release();
            m_RenderTextureHandle = null;
        }
    }
}
