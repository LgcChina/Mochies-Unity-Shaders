using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Mochie;

namespace Mochie {

    public class WaterEditor : ShaderGUI {

        GUIContent texLabel = new GUIContent("基础颜色");
        GUIContent normalLabel = new GUIContent("法线贴图");
        GUIContent flowLabel = new GUIContent("流动贴图");
        GUIContent noiseLabel = new GUIContent("噪波贴图");
        GUIContent foamLabel = new GUIContent("泡沫贴图");
        GUIContent cubeLabel = new GUIContent("立方体贴图");
        GUIContent emissLabel = new GUIContent("自发光贴图");

        static Dictionary<Material, Toggles> foldouts = new Dictionary<Material, Toggles>();
        Toggles toggles = new Toggles(new string[] {
                "基础",
                "法线贴图",
                "高光",
                "流动映射",
                "顶点偏移",
                "全局",
                "波浪 1",
                "波浪 2",
                "波浪 3",
                "波浪 4",
                "次表面散射",
                "焦散",
                "深度雾",
                "泡沫",
                "边缘渐隐",
                "雨",
                "曲面细分",
                "自发光",
                "LTCGI",
                "AreaLit",
                "光照体积",
                "光照贴图设置",
                "渲染设置"
        }, 0);

        string versionLabel = "v1.30 | LGC 中文版 1.78.1";

        MaterialProperty _Color = null;
        MaterialProperty _NonGrabColor = null;
        MaterialProperty _AngleTint = null;
        MaterialProperty _OpaqueAngleTint = null;
        MaterialProperty _BackfaceTint = null;
        MaterialProperty _MainTex = null;
        MaterialProperty _MainTexScroll = null;
        MaterialProperty _DistortionStrength = null;
        MaterialProperty _Roughness = null;
        MaterialProperty _Metallic = null;
        MaterialProperty _Opacity = null;
        MaterialProperty _Reflections = null;
        MaterialProperty _ReflStrength = null;
        MaterialProperty _Specular = null;
        MaterialProperty _SpecStrength = null;
        MaterialProperty _CullMode = null;
        MaterialProperty _ZWrite = null;
        MaterialProperty _BaseColorDistortionStrength = null;
        MaterialProperty _NormalMap0 = null;
        MaterialProperty _NormalStr0 = null;
        MaterialProperty _NormalMapScale0 = null;
        MaterialProperty _Rotation0 = null;
        MaterialProperty _NormalMapScroll0 = null;
        MaterialProperty _Normal1Toggle = null;
        MaterialProperty _NormalMap1 = null;
        MaterialProperty _NormalStr1 = null;
        MaterialProperty _NormalMapScale1 = null;
        MaterialProperty _Rotation1 = null;
        MaterialProperty _NormalMapScroll1 = null;
        MaterialProperty _FlowToggle = null;
        MaterialProperty _FlowMap = null;
        MaterialProperty _FlowSpeed = null;
        MaterialProperty _FlowStrength = null;
        MaterialProperty _FlowMapScale = null;
        MaterialProperty _NoiseTex = null;
        MaterialProperty _NoiseTexScale = null;
        MaterialProperty _NoiseTexScroll = null;
        MaterialProperty _NoiseTexBlur = null;
        MaterialProperty _WaveHeight = null;
        MaterialProperty _Offset = null;
        MaterialProperty _VertOffsetMode = null;
        MaterialProperty _WaveSpeedGlobal = null;
        MaterialProperty _WaveStrengthGlobal = null;
        MaterialProperty _WaveScaleGlobal = null;
        MaterialProperty _WaveSpeed0 = null;
        MaterialProperty _WaveSpeed1 = null;
        MaterialProperty _WaveSpeed2 = null;
        MaterialProperty _WaveSpeed3 = null;
        MaterialProperty _WaveScale0 = null;
        MaterialProperty _WaveScale1 = null;
        MaterialProperty _WaveScale2 = null;
        MaterialProperty _WaveScale3 = null;
        MaterialProperty _WaveStrength0 = null;
        MaterialProperty _WaveStrength1 = null;
        MaterialProperty _WaveStrength2 = null;
        MaterialProperty _WaveStrength3 = null;
        MaterialProperty _WaveDirection0 = null;
        MaterialProperty _WaveDirection1 = null;
        MaterialProperty _WaveDirection2 = null;
        MaterialProperty _WaveDirection3 = null;
        MaterialProperty _WaveToggle0 = null;
        MaterialProperty _WaveToggle1 = null;
        MaterialProperty _WaveToggle2 = null;
        MaterialProperty _WaveToggle3 = null;
        MaterialProperty _WaveWarpStrength = null;
        MaterialProperty _WaveWarpScale = null;
        MaterialProperty _WaveWarpSpeed = null;
        // MaterialProperty _Turbulence = null;
        // MaterialProperty _TurbulenceSpeed = null;
        // MaterialProperty _TurbulenceScale = null;
        MaterialProperty _VoronoiScale = null;
        MaterialProperty _VoronoiScroll = null;
        MaterialProperty _VoronoiWaveHeight = null;
        MaterialProperty _VoronoiOffset = null;
        MaterialProperty _VoronoiSpeed = null;
        MaterialProperty _CausticsTex = null;
        MaterialProperty _CausticsToggle = null;
        MaterialProperty _CausticsOpacity = null;
        MaterialProperty _CausticsScale = null;
        MaterialProperty _CausticsSpeed = null;
        MaterialProperty _CausticsFade = null;
        MaterialProperty _CausticsDisp = null;
        MaterialProperty _CausticsShadow = null;
        MaterialProperty _CausticsShadowStrength = null;
        MaterialProperty _CausticsDistortion = null;
        MaterialProperty _CausticsDistortionTex = null;
        MaterialProperty _CausticsDistortionScale = null;
        MaterialProperty _CausticsDistortionSpeed = null;
        MaterialProperty _CausticsRotateWithLight = null;
        MaterialProperty _CausticsRotation = null;
        MaterialProperty _CausticsColor = null;
        MaterialProperty _CausticsPower = null;
        MaterialProperty _FogToggle = null;
        MaterialProperty _FogTint = null;
        MaterialProperty _FogPower = null;
        MaterialProperty _FoamToggle = null;
        MaterialProperty _FoamTex = null;
        MaterialProperty _FoamNoiseTex = null;
        MaterialProperty _FoamTexScale = null;
        MaterialProperty _FoamRoughness = null;
        MaterialProperty _FoamColor = null;
        MaterialProperty _FoamPower = null;
        MaterialProperty _FoamEdgeStrength = null;
        MaterialProperty _FoamCrestStrength = null;
        MaterialProperty _FoamCrestThreshold = null;
        MaterialProperty _FoamNoiseTexScroll = null;
        MaterialProperty _FoamNoiseTexCrestStrength = null;
        MaterialProperty _FoamNoiseTexStrength = null;
        MaterialProperty _FoamNoiseTexScale = null;
        MaterialProperty _EdgeFadeToggle = null;
        MaterialProperty _EdgeFadePower = null;
        MaterialProperty _EdgeFadeOffset = null;
        MaterialProperty _SSRStrength = null;
        MaterialProperty _SSR = null;
        MaterialProperty _EdgeFadeSSR = null;
        MaterialProperty _Normal0StochasticToggle = null;
        MaterialProperty _Normal1StochasticToggle = null;
        MaterialProperty _FoamStochasticToggle = null;
        MaterialProperty _FoamTexScroll = null;
        MaterialProperty _BaseColorStochasticToggle = null;
        // MaterialProperty _NormalMapOffset1 = null;
        // MaterialProperty _FoamOffset = null;
        // MaterialProperty _NormalMapOffset0 = null;
        // MaterialProperty _BaseColorOffset = null;
        MaterialProperty _FoamDistortionStrength = null;
        MaterialProperty _VertRemapMin = null;
        MaterialProperty _VertRemapMax = null;
        MaterialProperty _LightDir = null;
        MaterialProperty _SpecTint = null;
        MaterialProperty _ReflCube = null;
        MaterialProperty _ReflTint = null;
        MaterialProperty _ReflCubeRotation = null;
        MaterialProperty _RainToggle = null;
        MaterialProperty _RippleScale = null;
        MaterialProperty _RippleSpeed = null;
        MaterialProperty _RippleStr = null;
        MaterialProperty _FoamNormalToggle = null;
        MaterialProperty _FoamNormalStrength = null;
        MaterialProperty _DepthEffects = null;
        MaterialProperty _TessMin = null;
        MaterialProperty _TessMax = null;
        MaterialProperty _TessDistMin = null;
        MaterialProperty _TessDistMax = null;
        MaterialProperty _TessellationOffsetMask = null;
        MaterialProperty _BlendNoise = null;
        MaterialProperty _BlendNoiseScale = null;
        MaterialProperty _BlendNoiseSource = null;
        MaterialProperty _FlowMapUV = null;
        MaterialProperty _BackfaceReflections = null;
        MaterialProperty _RoughnessMap = null;
        MaterialProperty _MetallicMap = null;
        MaterialProperty _EmissionMap = null;
        MaterialProperty _EmissionMapStochasticToggle = null;
        MaterialProperty _EmissionColor = null;
        MaterialProperty _EmissionMapScroll = null;
        MaterialProperty _DetailBaseColor = null;
        MaterialProperty _DetailBaseColorTint = null;
        MaterialProperty _AreaLitToggle = null;
        MaterialProperty _AreaLitMask = null;
        MaterialProperty _AreaLitStrength = null;
        MaterialProperty _AreaLitRoughnessMult = null;
        MaterialProperty _LightMesh = null;
        MaterialProperty _LightTex0 = null;
        MaterialProperty _LightTex1 = null;
        MaterialProperty _LightTex2 = null;
        MaterialProperty _LightTex3 = null;
        MaterialProperty _OpaqueLights = null;
        MaterialProperty _TransparencyMode = null;
        MaterialProperty _DetailNormal = null;
        MaterialProperty _DetailNormalStrength = null;
        MaterialProperty _DetailTextureMode = null;
        MaterialProperty _DetailScroll = null;
        MaterialProperty _StencilRef = null;
        MaterialProperty _RecalculateNormals = null;
        MaterialProperty _ShadowStrength = null;
        MaterialProperty _SSRHeight = null;
        MaterialProperty _CausticsTexArray = null;
        MaterialProperty _NormalMapFlipbook = null;
        MaterialProperty _NormalMapFlipbookSpeed = null;
        MaterialProperty _NormalMapFlipbookStrength = null;
        MaterialProperty _NormalMapMode = null;
        MaterialProperty _VertOffsetFlipbook = null;
        MaterialProperty _VertOffsetFlipbookStrength = null;
        MaterialProperty _VertOffsetFlipbookSpeed = null;
        MaterialProperty _NormalMapFlipbookScale = null;
        MaterialProperty _CausticsFlipbookSpeed = null;
        MaterialProperty _RippleSize = null;
        MaterialProperty _RippleDensity = null;
        MaterialProperty _OpacityMask = null;
        MaterialProperty _OpacityMaskScroll = null;
        MaterialProperty _NonGrabBackfaceTint = null;
        MaterialProperty _EmissionDistortionStrength = null;
        MaterialProperty _FogBrightness = null;
        MaterialProperty _VertexOffsetMask = null;
        MaterialProperty _VertexOffsetMaskStrength = null;
        MaterialProperty _VertexOffsetMaskChannel = null;
        MaterialProperty _SubsurfaceTint = null;
        MaterialProperty _SubsurfaceThreshold = null;
        MaterialProperty _SubsurfaceBrightness = null;
        MaterialProperty _SubsurfaceStrength = null;
        MaterialProperty _SubsurfaceToggle = null;
        MaterialProperty _TexCoordSpace = null;
        MaterialProperty _TexCoordSpaceSwizzle = null;
        MaterialProperty _GlobalTexCoordScaleUV = null;
        MaterialProperty _GlobalTexCoordScaleWorld = null;
        MaterialProperty _MirrorNormalOffsetSwizzle = null;
        MaterialProperty _InvertNormals = null;
        MaterialProperty _VisualizeFlowmap = null;
        MaterialProperty _AudioLink = null;
        MaterialProperty _AudioLinkStrength = null;
        MaterialProperty _AudioLinkBand = null;
        MaterialProperty _CausticsFlipbookDisp = null;
        MaterialProperty _RippleMask = null;
        MaterialProperty _QueueOffset = null;
        MaterialProperty _HorizonAdjustment = null;
        MaterialProperty _HorizonAdjustmentDistance = null;
        MaterialProperty _EmissionFlowToggle = null;
        MaterialProperty _FogContribution = null;
        MaterialProperty _LightmapDistortion = null;
        MaterialProperty _IndirectStrength = null;
        MaterialProperty _IndirectSaturation = null;
        MaterialProperty _VRSSR = null;
        MaterialProperty _CausticsFlipbookBlend = null;
        MaterialProperty _HorizonTint = null;
        MaterialProperty _OpaqueHorizonTint = null;
        MaterialProperty _HorizonTintDistance = null;
        MaterialProperty _HorizonTintStrength = null;
        MaterialProperty _FoamMap = null;
        MaterialProperty _FoamMode = null;
        MaterialProperty _FoamMapUVSet = null;
        MaterialProperty _FoamVertexColorChannel = null;
        MaterialProperty _FoamStrength = null;
        // MaterialProperty _BackfaceFadeColor = null;
        // MaterialProperty _BackfaceFadeStrength = null;
        // MaterialProperty _WireframeVisualization = null;
        // MaterialProperty _WireframeColor = null;

        // Light Volumes
        MaterialProperty _LightVolumesToggle = null;
        MaterialProperty _LightVolumeStrength = null;
        MaterialProperty _AdditiveLightVolumesToggle = null;
        MaterialProperty _AdditiveLightVolumeStrength = null;
        MaterialProperty _LightVolumeSpecularity = null;
        MaterialProperty _LightVolumeSpecularityStrength = null;
        MaterialProperty _LightVolumeBias = null;

        // Lightmapping Settings
        MaterialProperty _BAKERY_LMSPEC = null;
        MaterialProperty _BakeryLMSpecStrength = null;
       //  MaterialProperty _BAKERY_SHNONLINEAR = null;
        MaterialProperty _BakeryMode = null;
        MaterialProperty _BicubicSampling = null;
        MaterialProperty _IgnoreRealtimeGI = null;

        // LTCGI
        MaterialProperty _LTCGI = null;
        MaterialProperty _LTCGIStrength = null;
        MaterialProperty _LTCGIRoughness = null;
        MaterialProperty _LTCGI_SpecularColor = null;

        // MaterialProperty _FoamCrestPower = null;
        // MaterialProperty _FogTint2 = null;
        // MaterialProperty _FogPower2 = null;
        // MaterialProperty _FogBrightness2 = null;

        // MaterialProperty _NormalFlipbookStochasticToggle = null;

        // MaterialProperty _Test1 = null;
        // MaterialProperty _Test2 = null;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        MaterialEditor me;

        public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] props) {

            me = matEditor;

            if (!me.isVisible)
                return;

            foreach (var property in GetType().GetFields(bindingFlags)){
                if (property.FieldType == typeof(MaterialProperty))
                    property.SetValue(this, FindProperty(property.Name, props));
            }
            Material mat = (Material)me.target;

            if (!foldouts.ContainsKey(mat))
                foldouts.Add(mat, toggles);

            // Shader .meta defaults only reach new materials, so existing ones need the wave warp noise assigned here
            foreach (Material m in me.targets){
                if (m.HasProperty("_TANoiseTex") && m.GetTexture("_TANoiseTex") == null){
                    var tanoisePath = AssetDatabase.GUIDToAssetPath("c29b465ac9f3944508dfdf1555925ed5");
                    m.SetTexture("_TANoiseTex", AssetDatabase.LoadAssetAtPath<Texture2D>(tanoisePath));
                }
            }

            if (mat.GetInt("_MaterialResetCheck") == 0){
                mat.SetInt("_MaterialResetCheck", 1);
                SetKeywords(mat);
                SetBlendMode(mat);
            }

            bool isTessellated = MGUI.IsTessellated(mat);

            MGUI.DoHeader(isTessellated ? "细分水体" : "水体");

            EditorGUI.BeginChangeCheck(); {

                int transMode = mat.GetInt("_TransparencyMode");

                // Base
                Foldouts.DoFoldout(foldouts, mat, "基础", 1, Foldouts.Style.StandardButton);
                if (Foldouts.DoFoldoutButton(MGUI.collapseLabel, 11)) Toggles.CollapseFoldouts(mat, foldouts, 1);
                if (Foldouts.BeginFoldout(foldouts, mat, "基础")) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(texLabel, _MainTex, _BaseColorStochasticToggle);
                            MGUI.TexPropLabel(Tips.stochasticLabel, 117, false);
                            if (_MainTex.textureValue){
                                MGUI.TextureSOScroll(me, _MainTex, _MainTexScroll);
                                // me.ShaderProperty(_BaseColorOffset, Tips.parallaxOffsetLabel);
                                me.ShaderProperty(_BaseColorDistortionStrength, "扭曲强度");
                                MGUI.Space4();
                            }
                            if (transMode > 0){
                                me.TexturePropertySingleLine(new GUIContent("不透明度"), _OpacityMask, _Opacity);
                                MGUI.TextureSOScroll(me, _OpacityMask, _OpacityMaskScroll, _OpacityMask.textureValue);
                            }
                            else {
                                me.ShaderProperty(_ShadowStrength, "阴影强度");
                            }
                        });
                        MGUI.PropertyGroup(()=>{
                            bool hasDetailBC = _DetailBaseColor.textureValue;
                            bool hasDetailN = _DetailNormal.textureValue;
                            me.TexturePropertySingleLine(new GUIContent("贴花基础颜色"), _DetailBaseColor, hasDetailBC ? _DetailBaseColorTint : null);
                            me.TexturePropertySingleLine(new GUIContent("贴花法线贴图"), _DetailNormal, hasDetailN ? _DetailNormalStrength : null);
                            MGUI.TextureSOScroll(me, _DetailBaseColor, _DetailScroll, hasDetailBC || hasDetailN);
                        });
                        MGUI.PropertyGroup(() => {
                            if (transMode == 2)
                                me.ShaderProperty(_Color, "表面色调");
                            else
                                me.ShaderProperty(_NonGrabColor, "表面色调");
                            if (transMode == 0)
                                me.ShaderProperty(_OpaqueAngleTint, "掠射色调");
                            else
                                me.ShaderProperty(_AngleTint, "掠射色调");
                            if (transMode == 2)
                                me.ShaderProperty(_BackfaceTint, "背面色调");
                            else
                                me.ShaderProperty(_NonGrabBackfaceTint, "背面色调");
                        });
                        MGUI.PropertyGroup(() => {
                            if (transMode == 0)
                                me.ShaderProperty(_OpaqueHorizonTint, "地平线色调");
                            else
                                me.ShaderProperty(_HorizonTint, "地平线色调");
                            me.ShaderProperty(_HorizonTintDistance, "地平线色调距离");
                            me.ShaderProperty(_HorizonTintStrength, "地平线色调强度");
                        });
                        // MGUI.PropertyGroup(() => {
                        //     me.ShaderProperty(_BackfaceFadeColor, "Backface Fade Color");
                        //     me.ShaderProperty(_BackfaceFadeStrength, "Backface Fade Strength");
                        // });
                    });
                }
                Foldouts.EndFoldout();

                // Normal Maps
                if (Foldouts.DoFoldout(foldouts, mat, me, _NormalMapMode, "法线贴图", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        me.ShaderProperty(_DistortionStrength, "折射强度");
                        me.ShaderProperty(_HorizonAdjustment, Tips.horizonAdjustmentText);
                        me.ShaderProperty(_HorizonAdjustmentDistance, "地平线调整距离");
                        me.ShaderProperty(_InvertNormals, "反转");
                        MGUI.Space4();
                        if (_NormalMapMode.floatValue == 0){
                            MGUI.BoldLabel("主");
                            MGUI.PropertyGroup(() => {
                                me.TexturePropertySingleLine(Tips.normalMapText, _NormalMap0, _Normal0StochasticToggle);
                                MGUI.TexPropLabel(Tips.stochasticLabel, 117, false);
                                me.ShaderProperty(_NormalStr0, "强度");
                                MGUI.Vector2Field(_NormalMapScale0, "缩放");
                                MGUI.Vector2Field(_NormalMapScroll0, "滚动");
                                me.ShaderProperty(_Rotation0, "旋转");
                            });
                            MGUI.BoldLabel("次级");
                            MGUI.SpaceN18();
                            me.ShaderProperty(_Normal1Toggle, " ");
                            MGUI.PropertyGroup(() => {
                                MGUI.ToggleGroup(_Normal1Toggle.floatValue == 0);
                                me.TexturePropertySingleLine(Tips.normalMapText, _NormalMap1, _Normal1StochasticToggle);
                                MGUI.TexPropLabel(Tips.stochasticLabel, 117, false);
                                me.ShaderProperty(_NormalStr1, "强度");
                                MGUI.Vector2Field(_NormalMapScale1, "缩放");
                                MGUI.Vector2Field(_NormalMapScroll1, "滚动");
                                me.ShaderProperty(_Rotation1, "旋转");
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        else {
                            MGUI.PropertyGroup(()=>{
                                me.TexturePropertySingleLine(new GUIContent("序列帧"), _NormalMapFlipbook);
                                MGUI.Vector2Field(_NormalMapFlipbookScale, "缩放");
                                me.ShaderProperty(_NormalMapFlipbookStrength, "强度");
                                me.ShaderProperty(_NormalMapFlipbookSpeed, "速度");
                            });
                        }
                    });
                }
                Foldouts.EndFoldout();

                // Specularity
                if (Foldouts.DoFoldout(foldouts, mat, "高光", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        me.TexturePropertySingleLine(Tips.roughnessText, _RoughnessMap, _Roughness);
                        MGUI.TextureSO(me, _RoughnessMap, _RoughnessMap.textureValue && _DetailTextureMode.floatValue != 1);
                        me.TexturePropertySingleLine(Tips.metallicText, _MetallicMap, _Metallic);
                        MGUI.TextureSO(me, _MetallicMap, _MetallicMap.textureValue && _DetailTextureMode.floatValue != 1);
                        me.ShaderProperty(_DetailTextureMode, Tips.detailMode);
                        MGUI.Space8();
                        me.ShaderProperty(_Reflections, "反射");
                        MGUI.PropertyGroup(()=>{
                            MGUI.ToggleGroup(_Reflections.floatValue == 0);
                            if (_Reflections.floatValue == 2){
                                me.TexturePropertySingleLine(cubeLabel, _ReflCube, _ReflTint);
                                MGUI.Vector3Field(_ReflCubeRotation, "旋转", false);
                            }
                            else {
                                if (_Reflections.floatValue == 3)
                                    me.ShaderProperty(_MirrorNormalOffsetSwizzle, Tips.mirrorNormalSwizzleText);
                                me.ShaderProperty(_ReflTint, "色调");
                            }
                            me.ShaderProperty(_ReflStrength, "强度");
                            if (_DepthEffects.floatValue == 1 && _Reflections.floatValue != 3){
                                MGUI.ToggleFloat(me, "屏幕空间反射", _SSR, _SSRStrength);
                                if (_SSR.floatValue > 0){
                                    me.ShaderProperty(_EdgeFadeSSR, Tips.ssrEdgeFadeText);
                                    me.ShaderProperty(_SSRHeight, Tips.ssrDepthText);
                                    me.ShaderProperty(_VRSSR, "在 VR 中启用 SSR");
                                }
                            }
                            me.ShaderProperty(_BackfaceReflections, "应用于背面");
                            MGUI.ToggleGroupEnd();
                        });
                        if (_Reflections.floatValue == 3){
                            MGUI.DisplayWarning("镜像模式需要 VRChat 镜像组件，并在自定义着色器字段中选择此着色器。网格还必须使其前方向着局部 Z 轴（可参考 Unity 默认四边形）。此外，其性能开销与其他 VRChat 镜像相同，请谨慎使用。");
                        }
                        MGUI.Space8();
                        me.ShaderProperty(_Specular, "高光");
                        MGUI.PropertyGroup( ()=>{
                            MGUI.ToggleGroup(_Specular.floatValue == 0);
                            me.ShaderProperty(_SpecTint, "色调");
                            me.ShaderProperty(_SpecStrength, "强度");
                            if (_Specular.floatValue == 2){
                                MGUI.Vector3Field(_LightDir, "光照方向", false);
                            }
                            MGUI.ToggleGroupEnd();
                        });
                    });
                }
                Foldouts.EndFoldout();

                // Emission
                if (Foldouts.DoFoldout(foldouts, mat, "自发光", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(!me.EmissionEnabledProperty());
                        me.LightmapEmissionFlagsProperty(0, true);
                        MGUI.Space4();
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(emissLabel, _EmissionMap, _EmissionMapStochasticToggle);
                            MGUI.TexPropLabel(Tips.stochasticLabel, 117, false);
                            me.ShaderProperty(_EmissionColor, "色调");
                            MGUI.TextureSOScroll(me, _EmissionMap, _EmissionMapScroll);
                            me.ShaderProperty(_EmissionDistortionStrength, "扭曲强度");
                            me.ShaderProperty(_EmissionFlowToggle, "应用流动映射");
                            me.ShaderProperty(_AudioLink, "Audio Link");
                            if (_AudioLink.floatValue == 1){
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkBand, "色带");
                                    me.ShaderProperty(_AudioLinkStrength, "强度");
                                });
                            }
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Flow Mapping
                if (Foldouts.DoFoldout(foldouts, mat, me, _FlowToggle, "流动映射", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            MGUI.ToggleGroup(_FlowToggle.floatValue == 0);
                            me.TexturePropertySingleLine(flowLabel, _FlowMap, _FlowMapUV);
                            MGUI.sRGBWarning(_FlowMap);
                            if (_BlendNoiseSource.floatValue == 1){
                                me.TexturePropertySingleLine(Tips.blendNoise, _BlendNoise);
                                MGUI.sRGBWarning(_BlendNoise);
                            }
                            MGUI.Vector2Field(_FlowMapScale, "流动贴图缩放");
                            if (_BlendNoiseSource.floatValue == 1)
                                MGUI.Vector2Field(_BlendNoiseScale, "混合噪波缩放");
                            me.ShaderProperty(_FlowSpeed, "速度");
                            me.ShaderProperty(_FlowStrength, "强度");
                            me.ShaderProperty(_BlendNoiseSource, "混合噪波源");
                            me.ShaderProperty(_VisualizeFlowmap, "可视化");
                            MGUI.ToggleGroupEnd();
                        });
                    });
                }
                Foldouts.EndFoldout();

                // Vertex Offset
                if (Foldouts.DoFoldout(foldouts, mat, me, _VertOffsetMode, "顶点偏移", Foldouts.Style.StandardToggle)) {
                    MGUI.Space6();
                    MGUI.ToggleGroup(_VertOffsetMode.floatValue == 0);
                    if (_VertOffsetMode.floatValue == 2){

                        // Global
                        if (Foldouts.DoFoldout(foldouts, mat, "全局", Foldouts.Style.Thin)) {
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.PropertyGroup(() => {
                                    me.ShaderProperty(_WaveStrengthGlobal, "强度");
                                    me.ShaderProperty(_WaveScaleGlobal, "缩放");
                                    me.ShaderProperty(_WaveSpeedGlobal, "速度");
                                    me.ShaderProperty(_RecalculateNormals, "重新计算法线");
                                });
                                MGUI.PropertyGroup(() => {
                                    me.ShaderProperty(_WaveWarpStrength, Tips.waveWarpStrength);
                                    me.ShaderProperty(_WaveWarpScale, "扭曲缩放");
                                    me.ShaderProperty(_WaveWarpSpeed, "扭曲速度");
                                });
                                MGUI.PropertyGroup(()=>{
                                    me.TexturePropertySingleLine(Tips.maskText, _VertexOffsetMask, _VertexOffsetMaskStrength, _VertexOffsetMaskChannel);
                                    MGUI.TextureSO(me, _VertexOffsetMask);
                                });
                            });
                        }
                        Foldouts.EndFoldout();

                        // Waves
                        DoWaveFoldout(mat, "波浪 1", _WaveToggle0, _WaveStrength0, _WaveScale0, _WaveSpeed0, _WaveDirection0);
                        DoWaveFoldout(mat, "波浪 2", _WaveToggle1, _WaveStrength1, _WaveScale1, _WaveSpeed1, _WaveDirection1);
                        DoWaveFoldout(mat, "波浪 3", _WaveToggle2, _WaveStrength2, _WaveScale2, _WaveSpeed2, _WaveDirection2);
                        DoWaveFoldout(mat, "波浪 4", _WaveToggle3, _WaveStrength3, _WaveScale3, _WaveSpeed3, _WaveDirection3);

                        // Turbulence
                        // MGUI.BoldLabel("Turbulence");
                        // MGUI.PropertyGroup(() => {
                        // 	me.ShaderProperty(_Turbulence, Tips.turbulence);
                        // 	me.ShaderProperty(_TurbulenceSpeed, "Speed");
                        // 	me.ShaderProperty(_TurbulenceScale, "Scale");
                        // });
                    }
                    else {
                        MGUI.PropertyGroupParent(()=>{
                            if (_VertOffsetMode.floatValue == 1 || _VertOffsetMode.floatValue == 0){
                                MGUI.PropertyGroup(()=>{
                                    me.TexturePropertySingleLine(noiseLabel, _NoiseTex);
                                    me.ShaderProperty(_NoiseTexBlur, "精度");
                                    MGUI.Vector2Field(_NoiseTexScale, "缩放");
                                    MGUI.Vector2Field(_NoiseTexScroll, "滚动");
                                });
                                MGUI.PropertyGroup(()=>{
                                    MGUI.Vector3Field(_Offset, "强度", false);
                                    me.ShaderProperty(_WaveHeight, "强度倍率");
                                    MGUI.SliderMinMax(_VertRemapMin, _VertRemapMax, -1f, 1f, "重映射", 1);
                                });
                            }
                            else if (_VertOffsetMode.floatValue == 3){
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_VoronoiSpeed, "速度");
                                    MGUI.Vector2Field(_VoronoiScale, "缩放");
                                    MGUI.Vector2Field(_VoronoiScroll, "滚动");
                                    MGUI.Vector3Field(_VoronoiOffset, "强度", false);
                                    me.ShaderProperty(_VoronoiWaveHeight, "强度倍率");
                                });
                            }
                            else if (_VertOffsetMode.floatValue == 4){
                                MGUI.PropertyGroup(()=>{
                                    me.TexturePropertySingleLine(new GUIContent("顶点偏移序列帧"), _VertOffsetFlipbook);
                                    MGUI.Vector2Field(_NormalMapFlipbookScale, "缩放");
                                    me.ShaderProperty(_VertOffsetFlipbookStrength, "强度");
                                    me.ShaderProperty(_VertOffsetFlipbookSpeed, "速度");
                                });
                            }
                            if (_VertOffsetMode.floatValue > 0){
                                MGUI.PropertyGroup(()=>{
                                    me.TexturePropertySingleLine(Tips.maskText, _VertexOffsetMask, _VertexOffsetMaskStrength, _VertexOffsetMaskChannel);
                                    MGUI.TextureSO(me, _VertexOffsetMask);
                                });
                            }
                        });
                    }

                    // Subsurface Scattering
                    if (_VertOffsetMode.floatValue > 0){
                        if (Foldouts.DoFoldout(foldouts, mat, me, _SubsurfaceToggle, "次表面散射", Foldouts.Style.ThinToggle)) {
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_SubsurfaceToggle.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_SubsurfaceTint, "色调");
                                    me.ShaderProperty(_SubsurfaceStrength, "强度");
                                    me.ShaderProperty(_SubsurfaceBrightness, "亮度");
                                    me.ShaderProperty(_SubsurfaceThreshold, "阈值");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();
                    }
                    MGUI.ToggleGroupEnd();
                    MGUI.Space6();
                }
                Foldouts.EndFoldout();

                // Caustics
                if (_DepthEffects.floatValue == 1 && transMode == 2){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _CausticsToggle, "焦散", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_CausticsToggle.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                if (_CausticsToggle.floatValue == 2){
                                    me.TexturePropertySingleLine(new GUIContent("焦散贴图"), _CausticsTex);
                                    me.TexturePropertySingleLine(new GUIContent("阴影贴图"), _CausticsShadow, _CausticsShadowStrength);
                                }
                                else if (_CausticsToggle.floatValue == 3){
                                    me.TexturePropertySingleLine(new GUIContent("焦散序列帧"), _CausticsTexArray);
                                    me.ShaderProperty(_CausticsFlipbookBlend, "混合");
                                }
                                me.ShaderProperty(_CausticsColor, "颜色");
                                me.ShaderProperty(_CausticsOpacity, "强度");

                                if (_CausticsToggle.floatValue == 1){
                                    me.ShaderProperty(_CausticsPower, "强度");
                                    me.ShaderProperty(_CausticsDisp, "色散");
                                }
                                if (_CausticsToggle.floatValue != 3)
                                    me.ShaderProperty(_CausticsSpeed, "速度");
                                else {
                                    me.ShaderProperty(_CausticsFlipbookSpeed, "速度");
                                    me.ShaderProperty(_CausticsFlipbookDisp, "色散");
                                }
                                me.ShaderProperty(_CausticsScale, "缩放");
                                me.ShaderProperty(_CausticsFade, Tips.causticsFade);
                                // me.ShaderProperty(_CausticsSurfaceFade, Tips.causticsSurfaceFade);
                                MGUI.ToggleGroup(_CausticsRotateWithLight.floatValue == 1);
                                MGUI.Vector3Field(_CausticsRotation, Tips.causticsRotation, false);
                                MGUI.ToggleGroupEnd();
                                me.ShaderProperty(_CausticsRotateWithLight, Tips.causticsRotateWithLight);
                            });
                            if (_CausticsToggle.floatValue != 3){
                                MGUI.PropertyGroup( ()=>{
                                    me.TexturePropertySingleLine(new GUIContent("扭曲贴图"), _CausticsDistortionTex);
                                    me.ShaderProperty(_CausticsDistortion, "扭曲强度");
                                    me.ShaderProperty(_CausticsDistortionScale, "扭曲缩放");
                                    MGUI.Vector2Field(_CausticsDistortionSpeed, "扭曲速度");
                                });
                            }
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();
                }

                // Foam
                if (Foldouts.DoFoldout(foldouts, mat, me, _FoamToggle, "泡沫", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_FoamToggle.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_FoamMode, "模式");
                            if (_FoamMode.floatValue == 1){
                                me.TexturePropertySingleLine(new GUIContent("泡沫贴图"), _FoamMap);
                                MGUI.TextureSO(me, _FoamMap);
                                me.ShaderProperty(_FoamMapUVSet, "UV 集");
                            }
                            else if (_FoamMode.floatValue == 2){
                                me.ShaderProperty(_FoamVertexColorChannel, "通道");
                            }
                            me.ShaderProperty(_FoamRoughness, Tips.foamRoughness);
                            if (_FoamMode.floatValue == 0){
                                me.ShaderProperty(_FoamPower, Tips.foamPower);
                                me.ShaderProperty(_FoamEdgeStrength, Tips.foamEdgeStrength);
                            }
                            else {
                                me.ShaderProperty(_FoamStrength, "强度");
                            }
                            me.ShaderProperty(_FoamCrestStrength, Tips.foamCrestStrength);
                            // me.ShaderProperty(_FoamCrestPower, Tips.foamCrestPower);
                            me.ShaderProperty(_FoamCrestThreshold, Tips.foamCrestThreshold);
                        });
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(foamLabel, _FoamTex, _FoamColor, _FoamStochasticToggle);
                            MGUI.TexPropLabel(Tips.stochasticLabel, 117, true);
                            MGUI.Space2();
                            MGUI.Vector2Field(_FoamTexScale, "缩放");
                            MGUI.Vector2Field(_FoamTexScroll, "滚动");
                            // me.ShaderProperty(_FoamOffset, Tips.parallaxOffsetLabel);
                            me.ShaderProperty(_FoamDistortionStrength, "扭曲强度");
                            MGUI.ToggleFloat(me, Tips.foamNormal, _FoamNormalToggle, _FoamNormalStrength);
                        });
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(noiseLabel, _FoamNoiseTex);
                            MGUI.Vector2Field(_FoamNoiseTexScale, "缩放");
                            MGUI.Vector2Field(_FoamNoiseTexScroll, "滚动");
                            me.ShaderProperty(_FoamNoiseTexStrength, Tips.foamNoiseTexStrength);
                            me.ShaderProperty(_FoamNoiseTexCrestStrength, Tips.foamNoiseTexCrestStrength);
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Depth Fog
                if (_DepthEffects.floatValue == 1 && transMode == 2){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _FogToggle, "深度雾", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_FogToggle.floatValue == 0);
                            // MGUI.BoldLabel("Layer 1");
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_FogTint, "颜色");
                                me.ShaderProperty(_FogBrightness, "亮度");
                                me.ShaderProperty(_FogPower, "强度");
                            });
                            // MGUI.BoldLabel("Layer 2");
                            // MGUI.PropertyGroup(()=>{
                            // 	me.ShaderProperty(_FogTint2, "Color");
                            // 	me.ShaderProperty(_FogBrightness2, "Brightness");
                            // 	me.ShaderProperty(_FogPower2, "Power");
                            // });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();
                }

                // Edge Fade
                if (_DepthEffects.floatValue == 1 && transMode == 2){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _EdgeFadeToggle, "边缘渐隐", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.PropertyGroup(()=>{
                                MGUI.ToggleGroup(_EdgeFadeToggle.floatValue == 0);
                                me.ShaderProperty(_EdgeFadePower, "强度");
                                me.ShaderProperty(_EdgeFadeOffset, "偏移");
                                MGUI.ToggleGroupEnd();
                            });
                        });
                    }
                    Foldouts.EndFoldout();
                }

                // Rain
                if (Foldouts.DoFoldout(foldouts, mat, me, _RainToggle, "雨", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_RainToggle.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(Tips.maskText, _RippleMask);
                            MGUI.TextureSO(me, _RippleMask, _RippleMask.textureValue);
                            me.ShaderProperty(_RippleStr, "强度");
                            me.ShaderProperty(_RippleSpeed, "速度");
                            me.ShaderProperty(_RippleScale, "缩放");
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_RippleDensity, "涟漪密度");
                            me.ShaderProperty(_RippleSize, "涟漪尺寸");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // LTCGI
                if (Shader.Find("LTCGI/Blur Prefilter") != null){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _LTCGI, "LTCGI", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_LTCGI.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_LTCGIStrength, "强度");
                                me.ShaderProperty(_LTCGIRoughness, "粗糙度倍率");
                                me.ShaderProperty(_LTCGI_SpecularColor, "色调");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();
                }
                else {
                    _LTCGI.floatValue = 0;
                    mat.SetInt("_LTCGI", 0);
                    mat.DisableKeyword("LTCGI");
                }

                // AreaLit
                if (Shader.Find("AreaLit/Standard") != null){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _AreaLitToggle, "AreaLit", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            bool reflDisabled = _AreaLitToggle.floatValue == 1 && _Reflections.floatValue == 0;
                            bool cantInteract = _AreaLitToggle.floatValue == 0 || reflDisabled;
                            if (reflDisabled){
                                MGUI.DisplayError("反射已禁用，请启用它们以使用 AreaLit。");
                            }
                            MGUI.ToggleGroup(cantInteract);
                            MGUI.PropertyGroup(()=>{
                                me.TexturePropertySingleLine(Tips.maskText, _AreaLitMask);
                                MGUI.TextureSO(me, _AreaLitMask, _AreaLitMask.textureValue);
                                me.ShaderProperty(_AreaLitStrength, "强度");
                                me.ShaderProperty(_AreaLitRoughnessMult, "粗糙度倍率");
                                me.ShaderProperty(_OpaqueLights, Tips.opaqueLightsText);
                            });
                            MGUI.PropertyGroup(()=>{
                                var lightMeshText = !_LightMesh.textureValue ? Tips.lightMeshText : new GUIContent(
                                    Tips.lightMeshText.text + $" (max: {_LightMesh.textureValue.height})", Tips.lightMeshText.tooltip
                                );
                                me.TexturePropertySingleLine(lightMeshText, _LightMesh);
                                me.TexturePropertySingleLine(Tips.lightTex0Text, _LightTex0);
                                MGUI.CheckTrilinear(_LightTex0.textureValue, me);
                                me.TexturePropertySingleLine(Tips.lightTex1Text, _LightTex1);
                                MGUI.CheckTrilinear(_LightTex1.textureValue, me);
                                me.TexturePropertySingleLine(Tips.lightTex2Text, _LightTex2);
                                MGUI.CheckTrilinear(_LightTex2.textureValue, me);
                                me.TexturePropertySingleLine(Tips.lightTex3Text, _LightTex3);
                                MGUI.CheckTrilinear(_LightTex3.textureValue, me);
                            });
                            MGUI.ToggleGroupEnd();
                            MGUI.DisplayInfo("AreaLit 包文件必须位于 Assets 文件夹下直接创建的、名为 AreaLit 的文件夹中（区分大小写，路径为 Assets/AreaLit）。");
                            MGUI.Space2();
                        });
                    }
                    Foldouts.EndFoldout();
                }
                else {
                    _AreaLitToggle.floatValue = 0f;
                    mat.SetInt("_AreaLitToggle", 0);
                    mat.DisableKeyword("_AREALIT_ON");
                }

                // Tessellation
                if (isTessellated){
                    if (Foldouts.DoFoldout(foldouts, mat, "曲面细分", Foldouts.Style.Standard)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_TessellationOffsetMask, "顶点偏移遮罩");
                                // me.ShaderProperty(_WireframeVisualization, "Debug Wireframe");
                                // if (_WireframeVisualization.floatValue == 1)
                                //     me.ShaderProperty(_WireframeColor, "Wireframe Color");
                                me.ShaderProperty(_TessMin, "最小系数");
                                me.ShaderProperty(_TessMax, "最大系数");
                                me.ShaderProperty(_TessDistMin, "最小距离");
                                me.ShaderProperty(_TessDistMax, "最大距离");
                            });
                            MGUI.DisplayWarning("警告：曲面细分在部分硬件上可能导致问题，设置过高时性能开销会显著增加。建议实际测试后再确定参数，因为相关设置需要根据网格的基础三角形数量调整。");
                            MGUI.DisplayInfo("使用“线框”场景视图可以轻松实现可视化。");
                        });
                    }
                    Foldouts.EndFoldout();
                }

                // Light Volumes
                if (Foldouts.DoFoldout(foldouts, mat, me, _LightVolumesToggle, "光照体积", Foldouts.Style.StandardToggle)) {
                    MGUI.ToggleGroup(_LightVolumesToggle.floatValue == 0);
                    MGUI.PropertyGroupParent(() => {
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_LightVolumeStrength, "强度");
                            MGUI.ToggleFloat(me, Tips.additiveLightVolumeText, _AdditiveLightVolumesToggle, _AdditiveLightVolumeStrength);
                            MGUI.ToggleFloat(me, Tips.lightVolumeSpecText, _LightVolumeSpecularity, _LightVolumeSpecularityStrength);
                            me.ShaderProperty(_LightVolumeBias, Tips.lightVolumeBiasText);
                        });
                    });
                    MGUI.ToggleGroupEnd();
                }
                Foldouts.EndFoldout();

                // Lightmap Settings
                if (Foldouts.DoFoldout(foldouts, mat, "光照贴图设置", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_BakeryMode, Tips.bakeryMode);
                            MGUI.ToggleFloat(me, "Bakery 高光", _BAKERY_LMSPEC, _BakeryLMSpecStrength);
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_LightmapDistortion, "光照贴图 UV 扭曲");
                            me.ShaderProperty(_IndirectStrength, "光照贴图强度");
                            me.ShaderProperty(_IndirectSaturation, "光照贴图饱和度");
                            me.ShaderProperty(_FogContribution, "深度雾贡献");
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_BicubicSampling, Tips.bicubicLightmap);
                            me.ShaderProperty(_IgnoreRealtimeGI, Tips.ignoreRealtimeGIText);
                        });
                    });
                }
                Foldouts.EndFoldout();

                // Render Settings
                if (Foldouts.DoFoldout(foldouts, mat, "渲染设置", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            _QueueOffset.floatValue = (int)_QueueOffset.floatValue;
                            EditorGUI.BeginChangeCheck();
                            me.ShaderProperty(_QueueOffset, Tips.queueOffset);
                            if (EditorGUI.EndChangeCheck()){
                                SetBlendMode(mat);
                            }
                            MGUI.SpaceN1();
                            MGUI.DummyProperty("渲染队列：", mat.renderQueue.ToString());
                            me.ShaderProperty(_StencilRef, "模板参考值");
                        });
                        MGUI.PropertyGroup(()=>{
                            EditorGUI.BeginChangeCheck();
                            me.ShaderProperty(_TransparencyMode, "透明模式");
                            if (EditorGUI.EndChangeCheck()){
                                SetBlendMode(mat);
                            }
                            me.ShaderProperty(_CullMode, "剔除模式");
                            me.ShaderProperty(_ZWrite, "Z 写入");
                            if (transMode == 2){
                                me.ShaderProperty(_DepthEffects, "深度效果");
                            }
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_TexCoordSpace, Tips.waterTexCoordSpaceText);
                            if (_TexCoordSpace.floatValue == 1f){
                                me.ShaderProperty(_TexCoordSpaceSwizzle, Tips.waterSwizzleText);
                                me.ShaderProperty(_GlobalTexCoordScaleWorld, "贴图坐标缩放");
                            }
                            else {
                                me.ShaderProperty(_GlobalTexCoordScaleUV, "贴图坐标缩放");
                            }
                        });
                        if (_DepthEffects.floatValue == 1 && transMode == 2){
                            MGUI.DisplayInfo("深度效果需要场景中存在“Depth Light”预制件。\n（发现于：Assets/Mochie/Unity/Prefabs）");
                            MGUI.Space2();
                        }
                    });
                }
                Foldouts.EndFoldout();

            }
            if (EditorGUI.EndChangeCheck()){
                SetKeywords(mat);
            }

            MGUI.DoFooter(versionLabel, "Water-Shader");
        }

        void DoWaveFoldout(Material mat, string header, MaterialProperty toggle, MaterialProperty strength, MaterialProperty scale, MaterialProperty speed, MaterialProperty direction){
            if (Foldouts.DoFoldout(foldouts, mat, me, toggle, header, Foldouts.Style.ThinToggle)) {
                MGUI.PropertyGroupParent(()=>{
                    MGUI.ToggleGroup(toggle.floatValue == 0);
                    MGUI.PropertyGroup(() => {
                        me.ShaderProperty(strength, "强度");
                        me.ShaderProperty(scale, "缩放");
                        me.ShaderProperty(speed, "速度");
                        me.ShaderProperty(direction, "方向");
                    });
                    MGUI.ToggleGroupEnd();
                });
            }
            Foldouts.EndFoldout();
        }

        public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader) {
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
            MGUI.ClearKeywords(mat);
            SetKeywords(mat);
            SetBlendMode(mat);
        }

        void SetBlendMode(Material mat){
            int transMode = mat.GetInt("_TransparencyMode");
            // The shadow caster can't do transparency and would cast a solid shadow, so only opaque water casts shadows. The built-in
            // renderer casts them even with the pass disabled, so the shader collapses the shadow caster in the other modes (WaterVert.cginc)
            mat.SetShaderPassEnabled("ShadowCaster", transMode == 0);
            switch (transMode){

                // Opaque
                case 0:
                    mat.SetOverrideTag("RenderType", "Opaque");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.SetShaderPassEnabled("GrabPass", false);
                    mat.EnableKeyword("_OPAQUE_MODE_ON");
                    mat.DisableKeyword("_PREMUL_MODE_ON");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry+mat.GetInt("_QueueOffset");
                break;

                // Premultiplied
                case 1:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.SetShaderPassEnabled("GrabPass", false);
                    mat.DisableKeyword("_OPAQUE_MODE_ON");
                    mat.EnableKeyword("_PREMUL_MODE_ON");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                break;

                // Grabpass
                case 2:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 0);
                    mat.SetShaderPassEnabled("GrabPass", true);
                    mat.DisableKeyword("_OPAQUE_MODE_ON");
                    mat.DisableKeyword("_PREMUL_MODE_ON");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                break;

                default: break;
            }
        }

        void SetKeywords(Material mat){
            MaterialEditor.FixupEmissiveFlag(mat);
            bool isEmissive = (mat.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
            int transMode = mat.GetInt("_TransparencyMode");
            int depthFXToggle = mat.GetInt("_DepthEffects");
            int vertMode = mat.GetInt("_VertOffsetMode");
            int reflMode = mat.GetInt("_Reflections");
            int specMode = mat.GetInt("_Specular");
            int causticsMode = mat.GetInt("_CausticsToggle");
            int normalMapMode = mat.GetInt("_NormalMapMode");
            int bicubic = mat.GetInt("_BicubicSampling");
            int ssrToggle = mat.GetInt("_SSR");
            bool ssrEnabled = ssrToggle == 1 && depthFXToggle == 1 && transMode == 2 && (reflMode == 1 || reflMode == 2);

            MGUI.SetKeyword(mat, "_EMISSION_ON", isEmissive);
            MGUI.SetKeyword(mat, "_REFLECTIONS_ON", reflMode > 0);
            MGUI.SetKeyword(mat, "_REFLECTIONS_MANUAL_ON", reflMode == 2);
            MGUI.SetKeyword(mat, "_REFLECTIONS_MIRROR_ON", reflMode == 3);
            MGUI.SetKeyword(mat, "_SPECULAR_ON", specMode > 0);
            MGUI.SetKeyword(mat, "_NOISE_TEXTURE_ON", vertMode == 1);
            MGUI.SetKeyword(mat, "_GERSTNER_WAVES_ON", vertMode == 2);
            MGUI.SetKeyword(mat, "_VORONOI_ON", vertMode == 3);
            MGUI.SetKeyword(mat, "_VERT_FLIPBOOK_ON", vertMode == 4);
            MGUI.SetKeyword(mat, "_DEPTH_EFFECTS_ON", depthFXToggle == 1 && transMode == 2);
            MGUI.SetKeyword(mat, "_DETAIL_BASECOLOR_ON", mat.GetTexture("_DetailBaseColor"));
            MGUI.SetKeyword(mat, "_DETAIL_NORMAL_ON", mat.GetTexture("_DetailNormal"));
            MGUI.SetKeyword(mat, "_CAUSTICS_VORONOI_ON", causticsMode == 1 && depthFXToggle == 1 && transMode != 0);
            MGUI.SetKeyword(mat, "_CAUSTICS_TEXTURE_ON", causticsMode == 2 && depthFXToggle == 1 && transMode != 0);
            MGUI.SetKeyword(mat, "_CAUSTICS_FLIPBOOK_ON", causticsMode == 3 && depthFXToggle == 1 && transMode != 0);
            MGUI.SetKeyword(mat, "_NORMALMAP_FLIPBOOK_ON", normalMapMode == 1);
            MGUI.SetKeyword(mat, "_SCREENSPACE_REFLECTIONS_ON", ssrEnabled);
            MGUI.SetKeyword(mat, "_BICUBIC_LIGHTMAPPING_ON", bicubic == 1);
            MGUI.SetKeyword(mat, "_FOAM_NORMALS_ON", mat.GetInt("_FoamNormalToggle") == 1);
            MGUI.SetKeyword(mat, "BAKERY_SH", mat.GetInt("_BakeryMode") == 1);
            MGUI.SetKeyword(mat, "BAKERY_RNM", mat.GetInt("_BakeryMode") == 2);
            MGUI.SetKeyword(mat, "BAKERY_MONOSH", mat.GetInt("_BakeryMode") == 3);
            MGUI.SetKeyword(mat, "BAKERY_LMSPEC", mat.GetInt("_BAKERY_LMSPEC") == 1);
            MGUI.SetKeyword(mat, "_EMISSION_FLOW_ON", mat.GetInt("_EmissionFlowToggle") == 1);
            MGUI.SetKeyword(mat, "LTCGI", mat.GetInt("_LTCGI") == 1);
        }
    }
}
