using UnityEngine;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu(menuName = "Rendering/MyRenderPipeline")]
public class MyRenderPipelineAsset : RenderPipelineAsset
{

    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new MyRenderPipeline();
    }
}