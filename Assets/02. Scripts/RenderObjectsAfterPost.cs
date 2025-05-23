using System;
using UnityEngine;
using UnityEngine.Rendering;
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