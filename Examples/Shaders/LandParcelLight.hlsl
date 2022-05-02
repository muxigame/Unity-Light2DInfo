#include "../../Library/Light2DUtility.hlsl"
#ifndef  SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif


float rcp(float intensity, float lightDistance)
{
    float a = 0.8f;
    return intensity / (a + a * lightDistance + a * (lightDistance * lightDistance));
}

void WordPositionToScreen_float(float3 worldPosition, out float3 screenPosition)
{
    float4 projectionPos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1));
    float3 noW = projectionPos.xyz / projectionPos.w;
    screenPosition = (noW) / 2 + 0.5;
    screenPosition.z = 0;
    screenPosition.y = 1 - screenPosition.y;
}

void WordPositionToScreen_half(half3 worldPosition, out half3 screenPosition)
{
    half4 projectionPos = mul(UNITY_MATRIX_VP, half4(worldPosition, 1));
    half3 noW = projectionPos.xyz / projectionPos.w;
    screenPosition = (noW) / 2 + 0.5;
    screenPosition.z = 0;
    screenPosition.y = 1 - screenPosition.y;
}

void CalculateLight_float(float3 centerPosition, out float3 color)
{
    Light2DInfo light_2d_info = GetMainLight2D();
    color = light_2d_info.color;
    uint lightCount = _Light2DCount;
    float lightDistance = distance(centerPosition, light_2d_info.position);
    float lightIntensity = rcp(light_2d_info.intensity, lightDistance);
    for (int i = 0; i < lightCount; i += 1)
    {
        SpotLight2DInfo lightInfo = GetAdditionalLight2D(i);
        lightDistance = distance(centerPosition, lightInfo.position);
        float tempColor = lightInfo.color;
        if (lightIntensity < rcp(lightInfo.intensity, lightDistance))
        {
            lightIntensity = max(lightIntensity, rcp(lightInfo.intensity, lightDistance));
            color = tempColor * rcp(lightInfo.intensity, lightDistance);
        }
    }
}


void CalculateLight_half(half3 centerPosition, out half3 color)
{
    Light2DInfo light_2d_info = GetMainLight2D();
    color = light_2d_info.color;
    uint lightCount = _Light2DCount;
    float lightIntensity = light_2d_info.intensity;
    for (int i = 0; i < lightCount; ++i)
    {
        SpotLight2DInfo lightInfo = GetAdditionalLight2D(i);
        float lightDistance = distance(centerPosition, lightInfo.position);
        float tempColor = lightInfo.color / lightDistance * lightDistance;
        half1x3 a = tempColor;
        half3x1 b = half3x1(1, 1, 1);
        if (lightIntensity < float(mul(a, b)))
        {
            color = tempColor;
            lightIntensity = max(lightIntensity, float(mul(a, b)));
        }
    }
}
