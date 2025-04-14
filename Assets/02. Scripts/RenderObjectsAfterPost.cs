using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderObjectsAfterPost : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public LayerMask renderLayer;
        public Material overrideMaterial;
        public string passTag = "RenderObjectsAfterPost";
    }

    public Settings settings = new Settings();

    class CustomRenderPass : ScriptableRenderPass
    {
        private string profilerTag;
        private Material overrideMaterial;
        private LayerMask layerMask;
        private FilteringSettings filteringSettings;
        private ShaderTagId shaderTagId = new ShaderTagId("UniversalForward");

        public CustomRenderPass(string profilerTag, LayerMask layerMask, Material overrideMat)
        {
            this.profilerTag = profilerTag;
            this.layerMask = layerMask;
            this.overrideMaterial = overrideMat;

            filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawingSettings =
                CreateDrawingSettings(shaderTagId, ref renderingData, SortingCriteria.CommonTransparent);
            if (overrideMaterial != null)
                drawingSettings.overrideMaterial = overrideMaterial;

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }
    }

    CustomRenderPass customPass;

    public override void Create()
    {
        customPass = new CustomRenderPass(settings.passTag, settings.renderLayer, settings.overrideMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(customPass);
    }
}