//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName : CustomRenderPassFeature.cs   Time : 2022-05-01 23:50:14
//
//======================================================================

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Light2DInfo.Scripts{
    public class Light2DInfoRenderPassFeature : ScriptableRendererFeature{
        public bool disableLightDataUpdate;

        private Light2DInfoRenderPass _scriptablePass;

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        private ScriptableRenderer _scriptableRenderer;

        public void Reset(){ Create(); }

        /// <inheritdoc />
        public override void Create(){
            var light2DHandler = FindObjectOfType<Light2DHandler>();
            if (!light2DHandler){
                light2DHandler = new GameObject().AddComponent<Light2DHandler>();
                light2DHandler.name = "Light2DHandler";
            }
            light2DHandler.Init();
            _scriptablePass = new Light2DInfoRenderPass(light2DHandler, this);
            // Configures where the render pass should be injected.
            _scriptablePass.renderPassEvent = RenderPassEvent.BeforeRendering;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData){ renderer.EnqueuePass(_scriptablePass); }

        private class Light2DInfoRenderPass : ScriptableRenderPass{
            private readonly Light2DHandler _light2DHandler;
            private readonly Light2DInfoRenderPassFeature _light2DInfoRenderPassFeature;

            public Light2DInfoRenderPass(Light2DHandler light2DHandler, Light2DInfoRenderPassFeature light2DInfoRenderPassFeature){
                _light2DHandler = light2DHandler;
                _light2DInfoRenderPassFeature = light2DInfoRenderPassFeature;
            }

            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in a performant manner.
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData){ }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            // TODO There should be a cache and it should be reactive。 by roc 2022.5.2
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData){
                if (_light2DInfoRenderPassFeature.disableLightDataUpdate) return;

                if (_light2DHandler.TryGetRender(out var commandBuffer)){
                    context.ExecuteCommandBuffer(commandBuffer);
                    CommandBufferPool.Release(commandBuffer);
                }
            }

            // Cleanup any allocated resources that were created during the execution of this render pass.
            public override void OnCameraCleanup(CommandBuffer cmd){ }
        }
    }


    [CustomEditor(typeof(Light2DInfoRenderPassFeature))]
    public class Light2DInfoRenderPassFeatureEditor : Editor{
        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            if (GUILayout.Button("Refresh Light Info")){
                var light2DInfoRenderPassFeature = (Light2DInfoRenderPassFeature) target;
                light2DInfoRenderPassFeature.Reset();
            }
        }
    }
}