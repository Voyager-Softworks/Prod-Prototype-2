void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#if defined(SHADERGRAPH_PREVIEW)
   Direction = float3(0.5, 0.5, 0);
   Color = float3(1.0, 1.0, 1.0);
   DistanceAtten = 1;
   ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
   float4 clipPos = TransformWorldToHClip(WorldPos);
   float4 shadowCoord = ComputeScreenPos(clipPos);
#else
   float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
   Light mainLight = GetMainLight(shadowCoord);
   Direction = mainLight.direction;
   Color = mainLight.color;
   DistanceAtten = mainLight.distanceAttenuation;
   ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void AdditionalLights_float(float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, out float3 Diffuse, out float3 Specular)
{
   float3 diffuseColor = 0;
   float3 specularColor = 0;

#if !defined(SHADERGRAPH_PREVIEW)
   WorldNormal = normalize(WorldNormal);
   WorldView = SafeNormalize(WorldView);
   int pixelLightCount = GetAdditionalLightsCount();
   for (int i = 0; i < pixelLightCount; ++i)
   {
       Light light = GetAdditionalLight(i, WorldPosition);
       diffuseColor += saturate(dot(light.direction, WorldNormal)) * light.color * (light.distanceAttenuation * light.shadowAttenuation);
       specularColor += pow((saturate(dot(normalize(light.direction + WorldView), WorldNormal)) * step(0.0, dot(light.direction, WorldNormal))), Smoothness) * light.color * (light.distanceAttenuation * light.shadowAttenuation);
   }
#endif

   Diffuse = diffuseColor;
   Specular = specularColor;
}
