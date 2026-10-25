using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class RenderObjectsAfterPost : ScriptableRendererFeature
{
    public Settings settings = new();

    private CustomRenderPass customPass;

    public override void Create()
    {
        customPass = new CustomRenderPass(settings.passTag, settings.renderLayer, settings.overrideMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(customPass);
    }

    [Serializable]
    public class Settings
    {
        public LayerMask renderLayer;
        public Material overrideMaterial;
        public string passTag = "RenderObjectsAfterPost";
    }

    private class CustomRenderPass : ScriptableRenderPass
    {
        private FilteringSettings filteringSettings;
        private LayerMask layerMask;
        private readonly Material overrideMaterial;
        private string profilerTag;
        private readonly ShaderTagId shaderTagId = new("UniversalForward");

        public CustomRenderPass(string profilerTag, LayerMask layerMask, Material overrideMat)
        {
            this.profilerTag = profilerTag;
            this.layerMask = layerMask;
            overrideMaterial = overrideMat;

            filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        private class PassData
        {
            public RendererListHandle rendererList;
        }

        // Unity 6.3 (URP 17.3): Compatibility Mode 제거로 RenderGraph 경로가 필수
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var renderingData = frameData.Get<UniversalRenderingData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var lightData = frameData.Get<UniversalLightData>();

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(profilerTag, out var passData))
            {
                var drawingSettings = CreateDrawingSettings(shaderTagId, renderingData, cameraData, lightData,
                    SortingCriteria.CommonTransparent);
                if (overrideMaterial != null)
                    drawingSettings.overrideMaterial = overrideMaterial;

                var listParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                passData.rendererList = renderGraph.CreateRendererList(listParams);
                builder.UseRendererList(passData.rendererList);

                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                if (resourceData.activeDepthTexture.IsValid())
                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    context.cmd.DrawRendererList(data.rendererList));
            }
        }

        [Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawingSettings =
                CreateDrawingSettings(shaderTagId, ref renderingData, SortingCriteria.CommonTransparent);
            if (overrideMaterial != null)
                drawingSettings.overrideMaterial = overrideMaterial;

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }
    }
}