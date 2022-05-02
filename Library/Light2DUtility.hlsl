#define MAX_VISIBLE_LIGHTS 256

uint _Light2DCount = 0;
half3 _Light2DColor[MAX_VISIBLE_LIGHTS];
float3 _Light2DPosition[MAX_VISIBLE_LIGHTS];
float _Light2DIntensity[MAX_VISIBLE_LIGHTS];
float _Light2DOuter[MAX_VISIBLE_LIGHTS];
float _Light2DInner[MAX_VISIBLE_LIGHTS];

half3 _MainLight2DColor;
half3 _MainLight2DPosition;
float _MainLight2DIntensity;

struct Light2DInfo
{
    half3 color;
    float3 position;
    float intensity;
};

struct SpotLight2DInfo
{
    half3 color;
    float3 position;
    float intensity;
    float inner;
    float outer;
};

SpotLight2DInfo GetAdditionalLight2D(uint index)
{
    SpotLight2DInfo light_2d_info;
    light_2d_info.color = _Light2DColor[index];
    light_2d_info.position = _Light2DPosition[index];
    light_2d_info.intensity = _Light2DIntensity[index];
    light_2d_info.outer = _Light2DOuter[index];
    light_2d_info.inner = _Light2DInner[index];
    return light_2d_info;
}

Light2DInfo GetMainLight2D()
{
    Light2DInfo light_2d_info;
    light_2d_info.color = _MainLight2DColor;
    light_2d_info.position = _MainLight2DPosition;
    light_2d_info.intensity = _MainLight2DIntensity;
    return light_2d_info;
}

void GetAdditionalLight2D_float(float index, out float3 color, out float3 position, out float intensity)
{
    uint _index = floor(index);
    color = _Light2DColor[_index];
    position = _Light2DPosition[_index];
    intensity = _Light2DIntensity[_index];
}

void GetAdditionalLight2D_half(half index, out half3 color, out half3 position, out half intensity)
{
    uint _index = floor(index);
    color = _Light2DColor[_index];
    position = _Light2DPosition[_index];
    intensity = _Light2DIntensity[_index];
}

void GetMainLight2D_float(out float3 color, out float3 position, out float intensity)
{
    color = _MainLight2DColor;
    position = _MainLight2DPosition;
    intensity = _MainLight2DIntensity;
}

void GetMainLight2D_half(out half3 color, out half3 position, out half intensity)
{
    color = _MainLight2DColor;
    position = _MainLight2DPosition;
    intensity = _MainLight2DIntensity;
}
