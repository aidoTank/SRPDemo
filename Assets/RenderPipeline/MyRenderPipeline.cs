using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

using Conditional = System.Diagnostics.ConditionalAttribute;


public class MyRenderPipeline : RenderPipeline
{
    CullResults cull;
    CommandBuffer cameraBuffer = new CommandBuffer
    {
        name = "Render Camera"
    };
    Material errorMaterial;
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        ScriptableCullingParameters cullingParameters;
        if (!CullResults.GetCullingParameters(camera, out cullingParameters))
        {
            return;
        }

#if UNITY_EDITOR
        if (camera.cameraType == CameraType.SceneView)
        { 
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
#endif
        
        // CullResults cull = CullResults.Cull(ref cullingParameters, context);
        CullResults.Cull(ref cullingParameters, context, ref cull);

        context.SetupCameraProperties(camera);
        
        CameraClearFlags clearFlags = camera.clearFlags;
        
        cameraBuffer.ClearRenderTarget((clearFlags & CameraClearFlags.Depth) != 0,
            (clearFlags & CameraClearFlags.Color) != 0,
            camera.backgroundColor);
        cameraBuffer.BeginSample("Render Camera");
        // cameraBuffer.EndSample("Render Camera");
        context.ExecuteCommandBuffer(cameraBuffer);
        cameraBuffer.Clear();
        
        //draw
        DrawRendererSettings drawSettings = new DrawRendererSettings(camera, 
            new ShaderPassName("SRPDefaultUnlit"));
        var filterSettings = new FilterRenderersSettings(true)
        {
            renderQueueRange = RenderQueueRange.opaque
        };
        context.DrawRenderers(cull.visibleRenderers,  ref drawSettings, filterSettings);
        
        context.DrawSkybox(camera);
        
        filterSettings.renderQueueRange = RenderQueueRange.transparent;
        
        context.DrawRenderers(cull.visibleRenderers,  ref drawSettings, filterSettings);

        DrawDefaultPipeline(context, camera);
        //完了再清除
        cameraBuffer.EndSample("Render Camera"); 
        context.ExecuteCommandBuffer(cameraBuffer);
        cameraBuffer.Clear();
                
        context.Submit();
    }

    [Conditional("UNITY_EDITOR")]
    void DrawDefaultPipeline(ScriptableRenderContext context, Camera camera)
    {
        if (errorMaterial == null) {
            Shader errorShader = Shader.Find("Hidden/InternalErrorShader");
            errorMaterial = new Material(errorShader) {
                hideFlags = HideFlags.HideAndDontSave
            };
        }
        var drawSettings = new DrawRendererSettings(
            camera, new ShaderPassName("ForwardBase")
        );
        
        drawSettings.SetShaderPassName(1, new ShaderPassName("PrepassBase"));
        drawSettings.SetShaderPassName(2, new ShaderPassName("Always"));
        drawSettings.SetShaderPassName(3, new ShaderPassName("Vertex"));
        drawSettings.SetShaderPassName(4, new ShaderPassName("VertexLMRGBM"));
        drawSettings.SetShaderPassName(5, new ShaderPassName("VertexLM"));
        drawSettings.SetOverrideMaterial(errorMaterial,0);
		
        var filterSettings = new FilterRenderersSettings(true);
		
        context.DrawRenderers(
            cull.visibleRenderers, ref drawSettings, filterSettings
        );
    }

    public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        base.Render(renderContext, cameras);

        foreach (var camera in cameras)
        {
            Render(renderContext, camera);
            
        }
    }
}
