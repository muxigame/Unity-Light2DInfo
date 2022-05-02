//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName : Light2DHandler.cs   Time : 2022-05-02 12:17:00
//
//======================================================================

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Light2DInfo.Scripts{
    [ExecuteAlways]
    public class Light2DHandler : MonoBehaviour{
        [SerializeField] private List<Light2D> additionalLight2D = new List<Light2D>();

        [SerializeField] private Light2D mainLight2D;


        public Queue<CommandBuffer> renderRequest = new Queue<CommandBuffer>();
        public bool autoSetUp = true;

        public float refreshTime = 0.05f;
        private int _Light2DColor;


        private int _Light2DCount;
        private int _Light2DIntensity;
        private int _Light2DPosition;
        private int _MainLight2DColor;
        private int _MainLight2DIntensity;
        private int _MainLight2DPosition;
        private int _Light2DOuter;
        private int _Light2DInner;

        private void Awake(){
            
        }

        private void OnValidate(){
            if (autoSetUp)
                StartCoroutine(nameof(AutoSetUp));
            else
                StopAllCoroutines();
        }

        public IEnumerator AutoSetUp(){
            while (autoSetUp){
                UpdateShaderData();
                yield return new WaitForSeconds(refreshTime);
            }
        }

        public void Init(){
            var light2Ds = FindObjectsOfType<Light2D>();
            mainLight2D = light2Ds.First(x => x.lightType       == Light2D.LightType.Global);
            additionalLight2D = light2Ds.Where(x => x.lightType == Light2D.LightType.Point).ToList();
            _Light2DCount = Shader.PropertyToID("_Light2DCount");
            _Light2DColor = Shader.PropertyToID("_Light2DColor");
            _Light2DPosition = Shader.PropertyToID("_Light2DPosition");
            _Light2DIntensity = Shader.PropertyToID("_Light2DIntensity");
            _MainLight2DColor = Shader.PropertyToID("_MainLight2DColor");
            _MainLight2DPosition = Shader.PropertyToID("_MainLight2DPosition");
            _MainLight2DIntensity = Shader.PropertyToID("_MainLight2DIntensity");
            _Light2DOuter = Shader.PropertyToID("_Light2DOuter");
            _Light2DInner = Shader.PropertyToID("_Light2DInner");
        }

        public void UpdateShaderData(){
            var cmd = CommandBufferPool.Get();
            cmd.SetGlobalInt(_Light2DCount, additionalLight2D.Count);
            cmd.SetGlobalVectorArray(_Light2DColor, additionalLight2D.Select(x => new Vector4(x.color.r, x.color.g, x.color.b, x.color.a)).ToList());
            cmd.SetGlobalVectorArray(_Light2DPosition, additionalLight2D.Select(x => (Vector4) x.transform.position).ToList());
            cmd.SetGlobalFloatArray(_Light2DIntensity, additionalLight2D.Select(x => x.intensity).ToList());
            cmd.SetGlobalFloatArray(_Light2DOuter, additionalLight2D.Select(x => x.pointLightOuterRadius).ToList());
            cmd.SetGlobalFloatArray(_Light2DInner, additionalLight2D.Select(x => x.pointLightInnerRadius).ToList());
            cmd.SetGlobalColor(_MainLight2DColor, mainLight2D.color);
            cmd.SetGlobalVector(_MainLight2DPosition, mainLight2D.transform.position);
            cmd.SetGlobalFloat(_MainLight2DIntensity, mainLight2D.intensity);
            renderRequest.Enqueue(cmd);
        }

        public bool TryGetRender(out CommandBuffer commandBuffer){
            commandBuffer = null;
            if (renderRequest != null && renderRequest.Count > 0){
                var dequeue = renderRequest.Dequeue();
                if (dequeue != null){
                    commandBuffer = dequeue;
                    return true;
                }
            }

            return false;
        }
    }

    [CustomEditor(typeof(Light2DHandler))]
    public class Light2DHandlerEditor : Editor{
        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            if (GUILayout.Button("Refresh Light Info")){
                var light2DInfoRenderPassFeature = (Light2DHandler) target;
                light2DInfoRenderPassFeature.Init();
        
                light2DInfoRenderPassFeature.StopAllCoroutines();
                light2DInfoRenderPassFeature.StartCoroutine(nameof(light2DInfoRenderPassFeature.AutoSetUp));
            }
        }
    }
}