using UnityEngine;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu(menuName = "Rendering/MyRenderPipeline")]
public class MyRenderPipelineAsset : RenderPipelineAsset
{

    [SerializeField] private bool dynamicBatching;

    [SerializeField] private bool instancing;
    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new MyRenderPipeline(dynamicBatching, instancing);
    }
}