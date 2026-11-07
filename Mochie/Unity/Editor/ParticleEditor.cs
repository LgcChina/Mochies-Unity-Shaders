using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Mochie;

namespace Mochie {

    public class ParticleEditor : ShaderGUI {

        GUIContent texLabel = new GUIContent("基础颜色");
        GUIContent tex2Label = new GUIContent("次级颜色");
        GUIContent normalLabel = new GUIContent("法线贴图");
        GUIContent applyStreamsText = new GUIContent("修复顶点流", "将顶点流布局应用到使用此材质的所有粒子系统");

        static Dictionary<Material, Toggles> foldouts = new Dictionary<Material, Toggles>();
        Toggles toggles = new Toggles(new string[] {
                "基础",
                "光照",
                "滤镜",
                "扭曲",
                "脉冲",
                "衰减",
                "Audio Link",
                "滤镜 ",
                "扭曲 ",
                "溶解",
                "不透明度",
                "镂空",
                "随机色调",
                "描边",
                "边缘光",
                "光照体积",
                "渲染设置"
        }, 0);

        string versionLabel = "v3.3.2 | LGC 中文版 1.78.2";

        // Render Settings
        MaterialProperty _BlendMode = null;
        MaterialProperty _AlphaSource = null;
        MaterialProperty _DstBlend = null;
        MaterialProperty _Culling = null;
        MaterialProperty _ZTest = null;
        MaterialProperty _Falloff = null;
        MaterialProperty _IsCutout = null;
        MaterialProperty _Cutoff = null;
        MaterialProperty _FlipbookBlending = null;
        MaterialProperty _LightingToggle = null;

        // Color
        MaterialProperty _MainTex = null;
        MaterialProperty _MainTexUVMode = null;
        MaterialProperty _MainTexSpeed = null;
        MaterialProperty _MainTexPolarRotation = null;
        MaterialProperty _MainTexPolarSpeed = null;
        MaterialProperty _MainTexPolarRadius = null;
        MaterialProperty _AlphaMask = null;
        MaterialProperty _AlphaMaskUVMode = null;
        MaterialProperty _AlphaMaskSpeed = null;
        MaterialProperty _AlphaMaskPolarRotation = null;
        MaterialProperty _AlphaMaskPolarSpeed = null;
        MaterialProperty _AlphaMaskPolarRadius = null;
        MaterialProperty _AlphaMaskChannel = null;
        MaterialProperty _SecondTex = null;
        MaterialProperty _SecondTexUVMode = null;
        MaterialProperty _SecondTexSpeed = null;
        MaterialProperty _SecondTexPolarRotation = null;
        MaterialProperty _SecondTexPolarSpeed = null;
        MaterialProperty _SecondTexPolarRadius = null;
        MaterialProperty _TexBlendMode = null;
        MaterialProperty _Color = null;
        MaterialProperty _SecondColor = null;
        MaterialProperty _Softening = null;
        MaterialProperty _SoftenStr = null;
        MaterialProperty _Opacity = null;
        MaterialProperty _CutoutRim = null;
        MaterialProperty _CutoutRimWidth = null;
        MaterialProperty _CutoutRimColor = null;
        MaterialProperty _CutoutRimBlend = null;

        // Lighting
        MaterialProperty _Workflow = null;
        MaterialProperty _NormalMapLighting = null;
        MaterialProperty _NormalMapLightingUVMode = null;
        MaterialProperty _NormalMapLightingSpeed = null;
        MaterialProperty _NormalMapLightingPolarRotation = null;
        MaterialProperty _NormalMapLightingPolarSpeed = null;
        MaterialProperty _NormalMapLightingPolarRadius = null;
        MaterialProperty _NormalMapLightingScale = null;
        MaterialProperty _Metallic = null;
        MaterialProperty _Roughness = null;
        MaterialProperty _Occlusion = null;
        MaterialProperty _MetallicMap = null;
        MaterialProperty _MetallicMapUVMode = null;
        MaterialProperty _MetallicMapSpeed = null;
        MaterialProperty _MetallicMapPolarRotation = null;
        MaterialProperty _MetallicMapPolarSpeed = null;
        MaterialProperty _MetallicMapPolarRadius = null;
        MaterialProperty _RoughnessMap = null;
        MaterialProperty _RoughnessMapUVMode = null;
        MaterialProperty _RoughnessMapSpeed = null;
        MaterialProperty _RoughnessMapPolarRotation = null;
        MaterialProperty _RoughnessMapPolarSpeed = null;
        MaterialProperty _RoughnessMapPolarRadius = null;
        MaterialProperty _OcclusionMap = null;
        MaterialProperty _OcclusionMapUVMode = null;
        MaterialProperty _OcclusionMapSpeed = null;
        MaterialProperty _OcclusionMapPolarRotation = null;
        MaterialProperty _OcclusionMapPolarSpeed = null;
        MaterialProperty _OcclusionMapPolarRadius = null;
        MaterialProperty _PackedMap = null;
        MaterialProperty _PackedMapUVMode = null;
        MaterialProperty _PackedMapSpeed = null;
        MaterialProperty _PackedMapPolarRotation = null;
        MaterialProperty _PackedMapPolarSpeed = null;
        MaterialProperty _PackedMapPolarRadius = null;
        MaterialProperty _MetallicChannel = null;
        MaterialProperty _RoughnessChannel = null;
        MaterialProperty _OcclusionChannel = null;
        MaterialProperty _PackedMetallicStrength = null;
        MaterialProperty _PackedRoughnessStrength = null;
        MaterialProperty _PackedOcclusionStrength = null;
        MaterialProperty _ReflectionsToggle = null;
        MaterialProperty _SpecularHighlightsToggle = null;
        MaterialProperty _ReflectionStrength = null;
        MaterialProperty _SpecularHighlightStrength = null;
        MaterialProperty _SharpHighlights = null;
        MaterialProperty _SharpHighlightSteps = null;
        MaterialProperty _SphericalHarmonics = null;
        MaterialProperty _Emission = null;
        MaterialProperty _EmissionColor = null;
        MaterialProperty _EmissionMap = null;
        MaterialProperty _EmissionMapUVMode = null;
        MaterialProperty _EmissionMapSpeed = null;
        MaterialProperty _EmissionMapPolarRotation = null;
        MaterialProperty _EmissionMapPolarSpeed = null;
        MaterialProperty _EmissionMapPolarRadius = null;
        MaterialProperty _EmissionLightReactivity = null;
        MaterialProperty _EmissionLightReactivityMin = null;
        MaterialProperty _EmissionLightReactivityMax = null;

        // Filtering
        MaterialProperty _Filtering = null;
        MaterialProperty _AutoShift = null;
        MaterialProperty _AutoShiftSpeed = null;
        MaterialProperty _Hue = null;
        MaterialProperty _Saturation = null;
        MaterialProperty _Contrast = null;
        MaterialProperty _HDR = null;
        MaterialProperty _Brightness = null;
        MaterialProperty _HueMode = null;
        MaterialProperty _MonoTint = null;

        // Distortion
        MaterialProperty _Distortion = null;
        MaterialProperty _NormalMap = null;
        MaterialProperty _NormalMapUVMode = null;
        MaterialProperty _NormalMapSpeed = null;
        MaterialProperty _NormalMapPolarRotation = null;
        MaterialProperty _NormalMapPolarSpeed = null;
        MaterialProperty _NormalMapPolarRadius = null;
        MaterialProperty _DistortionStr = null;
        MaterialProperty _DistortionBlend = null;
        MaterialProperty _DistortMainTex = null;
        MaterialProperty _MeshRefraction = null;
        MaterialProperty _RefractionIOR = null;

        // Rim Light
        MaterialProperty _RimLight = null;
        MaterialProperty _RimTex = null;
        MaterialProperty _RimTexUVMode = null;
        MaterialProperty _RimTexSpeed = null;
        MaterialProperty _RimTexPolarRotation = null;
        MaterialProperty _RimTexPolarSpeed = null;
        MaterialProperty _RimTexPolarRadius = null;
        MaterialProperty _RimColor = null;
        MaterialProperty _RimBlending = null;
        MaterialProperty _RimStrength = null;
        MaterialProperty _RimWidth = null;
        MaterialProperty _RimEdge = null;
        MaterialProperty _RimLifetime = null;
        MaterialProperty _RimLifetimeMax = null;
        MaterialProperty _RimLifetimeMin = null;
        MaterialProperty _RimStartWidth = null;
        MaterialProperty _RimEndWidth = null;

        // Pulse
        MaterialProperty _Pulse = null;
        MaterialProperty _Waveform = null;
        MaterialProperty _PulseStr = null;
        MaterialProperty _PulseSpeed = null;

        // Falloff
        MaterialProperty _MinRange = null;
        MaterialProperty _MaxRange = null;
        MaterialProperty _NearMinRange = null;
        MaterialProperty _NearMaxRange = null;

        //Dissolve
        MaterialProperty _Dissolve = null;
        MaterialProperty _DissolveMode = null;
        MaterialProperty _DissolveNoise = null;
        MaterialProperty _DissolveNoiseUVMode = null;
        MaterialProperty _DissolveNoiseSpeed = null;
        MaterialProperty _DissolveNoisePolarRotation = null;
        MaterialProperty _DissolveNoisePolarSpeed = null;
        MaterialProperty _DissolveNoisePolarRadius = null;
        MaterialProperty _DissolveAgeThresholdMin = null;
        MaterialProperty _DissolveAgeThresholdMax = null;
        MaterialProperty _DissolveAmount = null;
        MaterialProperty _DissolveRandomOffset = null;
        MaterialProperty _DissolveRimColor = null;
        MaterialProperty _DissolveRimWidth = null;
        MaterialProperty _DissolveRimBlend = null;

        // Random Hue
        MaterialProperty _RandomHue = null;
        MaterialProperty _RandomHueMode = null;
        MaterialProperty _RandomHueMonoTint = null;
        MaterialProperty _RandomHueMax = null;
        MaterialProperty _RandomHueMin = null;
        MaterialProperty _RandomSatMax = null;
        MaterialProperty _RandomSatMin = null;

        // Outlines
        MaterialProperty _Outlines = null;
        MaterialProperty _OutlineThickness = null;
        MaterialProperty _OutlineColor = null;
        MaterialProperty _OutlineStencilPass = null;
        MaterialProperty _OutlineStencilCompare = null;
        MaterialProperty _OutlineStencilToggle = null;

        // Light Volumes
        MaterialProperty _LightVolumesToggle = null;
        MaterialProperty _LightVolumeStrength = null;
        MaterialProperty _LightVolumeSpecularity = null;
        MaterialProperty _LightVolumeSpecularityStrength = null;
        MaterialProperty _LightVolumeBias = null;

        // Audio Link
        MaterialProperty _AudioLink = null;
        MaterialProperty _AudioLinkStrength = null;
        MaterialProperty _AudioLinkRemapMin = null;
        MaterialProperty _AudioLinkRemapMax = null;

        MaterialProperty _AudioLinkFilterBand = null;
        MaterialProperty _AudioLinkFilterStrength = null;
        MaterialProperty _AudioLinkRemapFilterMin = null;
        MaterialProperty _AudioLinkRemapFilterMax = null;

        MaterialProperty _AudioLinkDistortionBand = null;
        MaterialProperty _AudioLinkDistortionStrength = null;
        MaterialProperty _AudioLinkRemapDistortionMin = null;
        MaterialProperty _AudioLinkRemapDistortionMax = null;

        MaterialProperty _AudioLinkOpacityBand = null;
        MaterialProperty _AudioLinkOpacityStrength = null;
        MaterialProperty _AudioLinkRemapOpacityMin = null;
        MaterialProperty _AudioLinkRemapOpacityMax = null;

        MaterialProperty _AudioLinkCutoutBand = null;
        MaterialProperty _AudioLinkCutoutStrength = null;
        MaterialProperty _AudioLinkRemapCutoutMin = null;
        MaterialProperty _AudioLinkRemapCutoutMax = null;

        MaterialProperty _AudioLinkEmissionBand = null;
        MaterialProperty _AudioLinkEmissionStrength = null;
        MaterialProperty _AudioLinkRemapEmissionMin = null;
        MaterialProperty _AudioLinkRemapEmissionMax = null;

        MaterialProperty _AudioLinkOutlineBand = null;
        MaterialProperty _AudioLinkOutlineStrength = null;
        MaterialProperty _AudioLinkRemapOutlineMin = null;
        MaterialProperty _AudioLinkRemapOutlineMax = null;

        // Stencil
        MaterialProperty _StencilRef = null;
        MaterialProperty _StencilPass = null;
        MaterialProperty _StencilFail = null;
        MaterialProperty _StencilZFail = null;
        MaterialProperty _StencilCompare = null;

        // Render Settings
        MaterialProperty _QueueOffset = null;


        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        List<ParticleSystemRenderer> m_RenderersUsingThisMaterial = new List<ParticleSystemRenderer>();
        bool m_FirstTimeApply = true;

        public override void OnGUI(MaterialEditor me, MaterialProperty[] props) {
            if (!me.isVisible)
                return;

            foreach (var property in GetType().GetFields(bindingFlags)){
                if (property.FieldType == typeof(MaterialProperty))
                    property.SetValue(this, FindProperty(property.Name, props));
            }
            if (_DstBlend.floatValue == 0) _DstBlend.floatValue = 10;
            Material mat = (Material)me.target;
            if (m_FirstTimeApply){
                CacheRenderersUsingThisMaterial(mat);
                m_FirstTimeApply = false;
            }

            if (mat.GetInt("_MaterialResetCheck") == 0){
                mat.SetInt("_MaterialResetCheck", 1);
                ApplyMaterialSettings(mat);
            }

            bool isXVersion = MGUI.IsXVersion(mat);

            List<ParticleSystemVertexStream> streams = new List<ParticleSystemVertexStream>();
            streams.Add(ParticleSystemVertexStream.Position);
            streams.Add(ParticleSystemVertexStream.Normal);
            streams.Add(ParticleSystemVertexStream.Tangent);
            streams.Add(ParticleSystemVertexStream.Color);
            streams.Add(ParticleSystemVertexStream.UV);
            streams.Add(ParticleSystemVertexStream.UV2);
            streams.Add(ParticleSystemVertexStream.AnimBlend);
            streams.Add(ParticleSystemVertexStream.AnimFrame);
            streams.Add(ParticleSystemVertexStream.Center);
            streams.Add(ParticleSystemVertexStream.AgePercent);
            streams.Add(ParticleSystemVertexStream.StableRandomXYZW);

            string warnings = "";
            List<ParticleSystemVertexStream> rendererStreams = new List<ParticleSystemVertexStream>();
            foreach (ParticleSystemRenderer renderer in m_RenderersUsingThisMaterial){
                if (renderer != null){
                    renderer.GetActiveVertexStreams(rendererStreams);
                    bool streamsValid = rendererStreams.SequenceEqual(streams);
                    if (!streamsValid) warnings += "  " + renderer.name + "\n";
                }
            }

            if (isXVersion)
                MGUI.DoHeader("粒子 X");
            else
                MGUI.DoHeader("粒子");

            EditorGUI.BeginChangeCheck(); {

                foreach (var obj in _BlendMode.targets)
                    ApplyMaterialSettings((Material)obj);

                if (!foldouts.ContainsKey(mat))
                    foldouts.Add(mat, toggles);

                bool notOpaque = false;
                if (_BlendMode.floatValue != 6)
                    notOpaque = true;

                // Vertex Stream Handler
                if (warnings != ""){
                    EditorGUILayout.HelpBox("检测到不正确或丢失的顶点流：\n" + warnings, MessageType.Warning, true);
                    if (GUILayout.Button(applyStreamsText, EditorStyles.miniButton)){
                        foreach (ParticleSystemRenderer renderer in m_RenderersUsingThisMaterial){
                            if (renderer != null){
                                if (renderer != null)
                                    renderer.SetActiveVertexStreams(streams);
                            }
                        }
                    }
                    MGUI.Space4();
                }

                // Base
                Foldouts.DoFoldout(foldouts, mat, "基础", 1, Foldouts.Style.StandardButton);
                if (Foldouts.DoFoldoutButton(MGUI.collapseLabel, 11)) Toggles.CollapseFoldouts(mat, foldouts, 1);
                if (Foldouts.BeginFoldout(foldouts, mat, "基础")) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(() =>{
                            me.ShaderProperty(_BlendMode, "混合模式");
                            if (notOpaque || _IsCutout.floatValue == 1){
                                me.ShaderProperty(_AlphaSource, "Alpha 来源");
                            }
                            if (notOpaque)
                                me.ShaderProperty(_Opacity, "不透明度");
                            MGUI.ToggleSlider(me, "镂空", _IsCutout, _Cutoff);
                            if (notOpaque)
                                MGUI.ToggleSlider(me, Tips.softening, _Softening, _SoftenStr);
                            me.ShaderProperty(_FlipbookBlending, Tips.flipbookBlending);
                            if (_IsCutout.floatValue == 1 && isXVersion)
                                me.ShaderProperty(_CutoutRim, "镂空边缘光");
                        });
                        if (_IsCutout.floatValue == 1 && _CutoutRim.floatValue == 1 && isXVersion){
                            MGUI.PropertyGroup(() =>{
                                me.ShaderProperty(_CutoutRimWidth, "边缘光宽度");
                                me.ShaderProperty(_CutoutRimColor, "边缘光颜色");
                                me.ShaderProperty(_CutoutRimBlend, "边缘光混合");
                            });
                        }
                        MGUI.PropertyGroup( () => {
                            me.TexturePropertySingleLine(texLabel, _MainTex, _Color);
                            DrawUVBlock(mat, me, _MainTex, _MainTexUVMode, _MainTexSpeed, _MainTexPolarRadius, _MainTexPolarRotation, _MainTexPolarSpeed);
                            me.TexturePropertySingleLine(tex2Label, _SecondTex, _SecondColor, _TexBlendMode);
                            MGUI.TexPropLabel("混合", 113, true);
                            DrawUVBlock(mat, me, _SecondTex, _SecondTexUVMode, _SecondTexSpeed, _SecondTexPolarRadius, _SecondTexPolarRotation, _SecondTexPolarSpeed);
                            if (_AlphaSource.floatValue == 1 && (notOpaque || _IsCutout.floatValue == 1)){
                                me.TexturePropertySingleLine(Tips.alphaMaskText, _AlphaMask, _AlphaMaskChannel);
                                DrawUVBlock(mat, me, _AlphaMask, _AlphaMaskUVMode, _AlphaMaskSpeed, _AlphaMaskPolarRadius, _AlphaMaskPolarRotation, _AlphaMaskPolarSpeed);
                            }
                        });
                    });
                }
                Foldouts.EndFoldout();

                // Lighting
                if (Foldouts.DoFoldout(foldouts, mat, me, _LightingToggle, "光照", Foldouts.Style.StandardToggle)){
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_LightingToggle.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Workflow, Tips.workflowText);
                            me.TexturePropertySingleLine(Tips.normalMapText, _NormalMapLighting, _NormalMapLighting.textureValue ? _NormalMapLightingScale : null);
                            DrawUVBlock(mat, me, _NormalMapLighting, _NormalMapLightingUVMode, _NormalMapLightingSpeed, _NormalMapLightingPolarRadius, _NormalMapLightingPolarRotation, _NormalMapLightingPolarSpeed);
                            if (_Workflow.floatValue == 0){
                                me.TexturePropertySingleLine(Tips.metallicText, _MetallicMap, _Metallic);
                                DrawUVBlock(mat, me, _MetallicMap, _MetallicMapUVMode, _MetallicMapSpeed, _MetallicMapPolarRadius, _MetallicMapPolarRotation, _MetallicMapPolarSpeed);
                                me.TexturePropertySingleLine(Tips.roughnessText, _RoughnessMap, _Roughness);
                                DrawUVBlock(mat, me, _RoughnessMap, _RoughnessMapUVMode, _RoughnessMapSpeed, _RoughnessMapPolarRadius, _RoughnessMapPolarRotation, _RoughnessMapPolarSpeed);
                                me.TexturePropertySingleLine(Tips.occlusionText, _OcclusionMap, _OcclusionMap.textureValue ? _Occlusion : null);
                                DrawUVBlock(mat, me, _OcclusionMap, _OcclusionMapUVMode, _OcclusionMapSpeed, _OcclusionMapPolarRadius, _OcclusionMapPolarRotation, _OcclusionMapPolarSpeed);
                                bool hasAnyTexture = _OcclusionMap.textureValue != null || _RoughnessMap.textureValue != null || _MetallicMap.textureValue != null;
                                if (hasAnyTexture && MGUI.PropertyButton("打包贴图")){
                                    Undo.RecordObjects(me.targets, "打包贴图");
                                    bool allPacked = true;
                                    foreach (Material target in me.targets)
                                        allPacked &= PackPBRTextures(target);
                                    // Only switch workflows once every selected material has its packed map
                                    if (allPacked){
                                        _Workflow.floatValue = 1f;
                                        _OcclusionChannel.floatValue = 0f;
                                        _RoughnessChannel.floatValue = 1f;
                                        _MetallicChannel.floatValue = 2f;
                                        _PackedMetallicStrength.floatValue = 1f;
                                        _PackedRoughnessStrength.floatValue = 1f;
                                        _PackedOcclusionStrength.floatValue = 1f;
                                        foreach (Material target in me.targets)
                                            ApplyMaterialSettings(target);
                                    }
                                }
                            }
                            else {
                                me.TexturePropertySingleLine(Tips.packedMapText, _PackedMap);
                                DrawUVBlock(mat, me, _PackedMap, _PackedMapUVMode, _PackedMapSpeed, _PackedMapPolarRadius, _PackedMapPolarRotation, _PackedMapPolarSpeed);
                                me.ShaderProperty(_MetallicChannel, "金属度通道");
                                me.ShaderProperty(_RoughnessChannel, "粗糙度通道");
                                me.ShaderProperty(_OcclusionChannel, "环境光遮蔽通道");
                                me.ShaderProperty(_PackedMetallicStrength, Tips.metallicPackedText);
                                me.ShaderProperty(_PackedRoughnessStrength, Tips.roughnessPackedText);
                                me.ShaderProperty(_PackedOcclusionStrength, Tips.occlusionPackedText);
                            }
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Emission, "自发光");
                            MGUI.ToggleGroup(_Emission.floatValue == 0);
                            if (isXVersion){
                                me.ShaderProperty(_EmissionLightReactivity, "光照响应");
                                if (_EmissionLightReactivity.floatValue == 1)
                                    MGUI.SliderMinMax01(_EmissionLightReactivityMin, _EmissionLightReactivityMax, "光照阈值", 0);
                            }
                            me.TexturePropertySingleLine(Tips.emissionText, _EmissionMap, _EmissionColor);
                            DrawUVBlock(mat, me, _EmissionMap, _EmissionMapUVMode, _EmissionMapSpeed, _EmissionMapPolarRadius, _EmissionMapPolarRotation, _EmissionMapPolarSpeed);
                            MGUI.ToggleGroupEnd();
                        });
                        MGUI.PropertyGroup(()=>{
                            MGUI.ToggleFloat(me, Tips.cubemapReflectionsText, _ReflectionsToggle, _ReflectionStrength);
                            MGUI.ToggleFloat(me, Tips.specularHighlightsText, _SpecularHighlightsToggle, _SpecularHighlightStrength);
                            if (_SpecularHighlightsToggle.floatValue == 1){
                                MGUI.ToggleIntSlider(me, "锐利高光", _SharpHighlights, _SharpHighlightSteps);
                            }
                            me.ShaderProperty(_SphericalHarmonics, "球谐光照");
                        });
                        MGUI.ToggleGroupEnd();
                        MGUI.DisplayInfo("请注意，要获得完整的照明支持，您需要启用粒子系统“渲染器”选项卡底部的各种照明选项。");
                        MGUI.SpaceN1();
                    });
                }
                Foldouts.EndFoldout();

                // Filtering
                if (Foldouts.DoFoldout(foldouts, mat, me, _Filtering, "滤镜", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_Filtering.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_HueMode, Tips.hueModeText);
                            me.ShaderProperty(_AutoShift, Tips.autoShift);
                            me.ShaderProperty(_MonoTint, Tips.monoTintText);
                            if (_AutoShift.floatValue == 1)
                                me.ShaderProperty(_AutoShiftSpeed, "速度");
                            else
                                me.ShaderProperty(_Hue, "色调");
                            me.ShaderProperty(_Saturation, "饱和度");
                            me.ShaderProperty(_Brightness, "亮度");
                            me.ShaderProperty(_Contrast, "对比度");
                            me.ShaderProperty(_HDR, "HDR");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Distortion
                if (Foldouts.DoFoldout(foldouts, mat, me, _Distortion, "扭曲", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_Distortion.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(normalLabel, _NormalMap, _DistortMainTex);
                            MGUI.TexPropLabel("扭曲 UV", 124, false);
                            DrawUVBlock(mat, me, _NormalMap, _NormalMapUVMode, _NormalMapSpeed, _NormalMapPolarRadius, _NormalMapPolarRotation, _NormalMapPolarSpeed);
                            me.ShaderProperty(_DistortionStr, "强度");
                            MGUI.ToggleFloat(me, Tips.meshRefractionText, _MeshRefraction, _RefractionIOR);
                            if (_BlendMode.floatValue != 6)
                                me.ShaderProperty(_DistortionBlend, "混合");
                        });
                        if (_BlendMode.floatValue == 6)
                            MGUI.DisplayInfo("请注意，当材质不透明时，只有 UV 失真起作用。");
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Special Effects
                if (isXVersion){

                    // Dissolve
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Dissolve, "溶解", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Dissolve.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_DissolveMode, "模式");
                                MGUI.Space2();
                                me.TexturePropertySingleLine(new GUIContent("噪波"), _DissolveNoise, _DissolveRandomOffset);
                                MGUI.TexPropLabel(Tips.randomOffsetText, 100, false);
                                DrawUVBlock(mat, me, _DissolveNoise, _DissolveNoiseUVMode, _DissolveNoiseSpeed, _DissolveNoisePolarRadius, _DissolveNoisePolarRotation, _DissolveNoisePolarSpeed);
                                if (_DissolveMode.floatValue == 1)
                                    me.ShaderProperty(_DissolveAmount, "溶解量");
                                else
                                    MGUI.SliderMinMax01(_DissolveAgeThresholdMin, _DissolveAgeThresholdMax, "年龄阈值", 0); // me.ShaderProperty(_DissolveAgeThreshold, "年龄阈值");
                            });
                            MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_DissolveRimBlend, "边缘光混合");
                            me.ShaderProperty(_DissolveRimColor, "边缘光颜色");
                            me.ShaderProperty(_DissolveRimWidth, "边缘光宽度");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Randomn Hue
                    if (Foldouts.DoFoldout(foldouts, mat, me, _RandomHue, "随机色调", Foldouts.Style.StandardToggle)){
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_RandomHue.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_RandomHueMode, "模式");
                                me.ShaderProperty(_RandomHueMonoTint, Tips.monoTintText);
                                MGUI.SliderMinMax01(_RandomHueMin, _RandomHueMax, "色调范围", 0);
                                MGUI.SliderMinMax01(_RandomSatMin, _RandomSatMax, "饱和度范围", 0);
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Rim Light
                    if (Foldouts.DoFoldout(foldouts, mat, me, _RimLight, "边缘光", Foldouts.Style.StandardToggle)){
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_RimLight.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.TexturePropertySingleLine(Tips.rimCol, _RimTex, _RimColor, _RimBlending);
                                MGUI.TexPropLabel("混合", 105, true);
                                DrawUVBlock(mat, me, _RimTex, _RimTexUVMode, _RimTexSpeed, _RimTexPolarRadius, _RimTexPolarRotation, _RimTexPolarSpeed);
                                me.ShaderProperty(_RimStrength, Tips.rimStr);
                                me.ShaderProperty(_RimEdge, Tips.rimEdge);
                                if (_RimLifetime.floatValue == 0){
                                    me.ShaderProperty(_RimWidth, Tips.rimWidth);
                                }
                                else {
                                    me.ShaderProperty(_RimStartWidth, "起始宽度");
                                    me.ShaderProperty(_RimEndWidth, "结束宽度");
                                    MGUI.SliderMinMax01(_RimLifetimeMin, _RimLifetimeMax, "年龄阈值", 0);
                                }
                                me.ShaderProperty(_RimLifetime, "按生命周期缩放");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Outlines
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Outlines, "描边", Foldouts.Style.StandardToggle)){
                        if (_BlendMode.floatValue != 6){
                            MGUI.Space6();
                            MGUI.DisplayError("描边仅适用于不透明材质。");
                            MGUI.SpaceN6();
                        }
                        MGUI.ToggleGroup(_BlendMode.floatValue != 6);
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Outlines.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_OutlineColor, "颜色");
                                me.ShaderProperty(_OutlineThickness, "厚度");
                                me.ShaderProperty(_OutlineStencilToggle, Tips.stencilMode);
                                if (EditorGUI.EndChangeCheck()){
                                    if (_OutlineStencilToggle.floatValue == 0)
                                        DoStencilReset();
                                    else
                                        ApplyOutlineStencilConfig(mat);
                                }
                            });
                            MGUI.ToggleGroupEnd();
                        });
                        MGUI.ToggleGroupEnd();
                    }
                    Foldouts.EndFoldout();
                }

                // Pulse
                if (Foldouts.DoFoldout(foldouts, mat, me, _Pulse, "脉冲", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_Pulse.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Waveform, "波形");
                            me.ShaderProperty(_PulseStr, "强度");
                            me.ShaderProperty(_PulseSpeed, "速度");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Falloff
                if (notOpaque){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Falloff, "衰减", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Falloff.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_MinRange, "远最小范围");
                                me.ShaderProperty(_MaxRange, "远最大范围");
                                MGUI.Space4();
                                me.ShaderProperty(_NearMinRange, "近最小范围");
                                me.ShaderProperty(_NearMaxRange, "近最大范围");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();
                }

                // Audio Link
                if (Foldouts.DoFoldout(foldouts, mat, me, _AudioLink, "Audio Link", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_AudioLink.floatValue == 0);
                        me.ShaderProperty(_AudioLinkStrength, "全局强度");
                        MGUI.SliderMinMax01(_AudioLinkRemapMin, _AudioLinkRemapMax, "全局重映射", 1);
                        MGUI.Space4();
                        MGUI.BoldLabel("滤镜");
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_AudioLinkFilterBand, "色带");
                            me.ShaderProperty(_AudioLinkFilterStrength, "强度");
                            MGUI.SliderMinMax01(_AudioLinkRemapFilterMin, _AudioLinkRemapFilterMax, "重映射", 1);
                        });
                        MGUI.BoldLabel("扭曲");
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_AudioLinkDistortionBand, "色带");
                            me.ShaderProperty(_AudioLinkDistortionStrength, "强度");
                            MGUI.SliderMinMax01(_AudioLinkRemapDistortionMin, _AudioLinkRemapDistortionMax, "重映射", 1);
                        });
                        if (notOpaque){
                            MGUI.BoldLabel("不透明度");
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_AudioLinkOpacityBand, "色带");
                                me.ShaderProperty(_AudioLinkOpacityStrength, "强度");
                                MGUI.SliderMinMax01(_AudioLinkRemapOpacityMin, _AudioLinkRemapOpacityMax, "重映射", 1);
                            });
                        }
                        MGUI.BoldLabel("镂空");
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_AudioLinkCutoutBand, "色带");
                            me.ShaderProperty(_AudioLinkCutoutStrength, "强度");
                            MGUI.SliderMinMax01(_AudioLinkRemapCutoutMin, _AudioLinkRemapCutoutMax, "重映射", 1);
                        });
                        if (isXVersion && !notOpaque){
                            MGUI.BoldLabel("描边");
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_AudioLinkOutlineBand, "色带");
                                me.ShaderProperty(_AudioLinkOutlineStrength, "强度");
                                MGUI.SliderMinMax01(_AudioLinkRemapOutlineMin, _AudioLinkRemapOutlineMax, "重映射", 1);
                            });
                        }
                        MGUI.BoldLabel("自发光");
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_AudioLinkEmissionBand, "色带");
                            me.ShaderProperty(_AudioLinkEmissionStrength, "强度");
                            MGUI.SliderMinMax01(_AudioLinkRemapEmissionMin, _AudioLinkRemapEmissionMax, "重映射", 1);
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Light Volumes
                if (_LightingToggle.floatValue == 1){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _LightVolumesToggle, "光照体积", Foldouts.Style.StandardToggle)) {
                        MGUI.ToggleGroup(_LightVolumesToggle.floatValue == 0);
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_LightVolumeStrength, "强度");
                                MGUI.ToggleFloat(me, Tips.lightVolumeSpecText, _LightVolumeSpecularity, _LightVolumeSpecularityStrength);
                                me.ShaderProperty(_LightVolumeBias, Tips.lightVolumeBiasText);
                            });
                        });
                        MGUI.ToggleGroupEnd();
                    }
                    Foldouts.EndFoldout();
                }

                // Rendering
                if (Foldouts.DoFoldout(foldouts, mat, "渲染设置", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            _QueueOffset.floatValue = (int)_QueueOffset.floatValue;
                            me.ShaderProperty(_QueueOffset, Tips.queueOffset);
                            MGUI.SpaceN1();
                            MGUI.DummyProperty("渲染队列：", mat.renderQueue.ToString());
                            me.ShaderProperty(_Culling, "剔除");
                            me.ShaderProperty(_ZTest, "Z 测试");
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_StencilRef, "参考值");
                            me.ShaderProperty(_StencilPass, "Pass 操作");
                            me.ShaderProperty(_StencilFail, "失败操作");
                            me.ShaderProperty(_StencilZFail, "Z 失败操作");
                            me.ShaderProperty(_StencilCompare, "比较函数");
                        });
                    });
                }
                Foldouts.EndFoldout();
            }

            MGUI.DoFooter(versionLabel, "Particle-Shader");
        }

        // Set blending mode
        public static void SetBlendMode(Material mat) {
            switch (mat.GetInt("_BlendMode")) {
                case 0:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_ALPHA_ADD_ON");
                    mat.DisableKeyword("_ALPHA_ADD_SOFT_ON");
                    mat.DisableKeyword("_ALPHA_MUL_ON");
                    mat.DisableKeyword("_ALPHA_MULX2_ON");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    break;
                case 1:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_ALPHA_ADD_ON");
                    mat.DisableKeyword("_ALPHA_ADD_SOFT_ON");
                    mat.DisableKeyword("_ALPHA_MUL_ON");
                    mat.DisableKeyword("_ALPHA_MULX2_ON");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    break;
                case 2:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.EnableKeyword("_ALPHA_ADD_ON");
                    mat.DisableKeyword("_ALPHA_ADD_SOFT_ON");
                    mat.DisableKeyword("_ALPHA_MUL_ON");
                    mat.DisableKeyword("_ALPHA_MULX2_ON");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    break;
                case 3:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_ALPHA_ADD_ON");
                    mat.EnableKeyword("_ALPHA_ADD_SOFT_ON");
                    mat.DisableKeyword("_ALPHA_MUL_ON");
                    mat.DisableKeyword("_ALPHA_MULX2_ON");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    break;
                case 4:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_ALPHA_ADD_ON");
                    mat.DisableKeyword("_ALPHA_ADD_SOFT_ON");
                    mat.EnableKeyword("_ALPHA_MUL_ON");
                    mat.DisableKeyword("_ALPHA_MULX2_ON");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcColor);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    break;
                case 5:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_ALPHA_ADD_ON");
                    mat.DisableKeyword("_ALPHA_ADD_SOFT_ON");
                    mat.DisableKeyword("_ALPHA_MUL_ON");
                    mat.EnableKeyword("_ALPHA_MULX2_ON");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcColor);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    break;
                case 6:
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.DisableKeyword("_ALPHA_ADD_ON");
                    mat.DisableKeyword("_ALPHA_ADD_SOFT_ON");
                    mat.DisableKeyword("_ALPHA_MUL_ON");
                    mat.DisableKeyword("_ALPHA_MULX2_ON");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    if (mat.GetInt("_IsCutout") == 1 || mat.GetInt("_Dissolve") == 1){
                        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest+mat.GetInt("_QueueOffset");
                        mat.SetOverrideTag("RenderType", "TransparentCutout");
                    }
                    else {
                        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry+mat.GetInt("_QueueOffset");
                        mat.SetOverrideTag("RenderType", "Opaque");
                    }
                    break;
            }
        }

        void ApplyMaterialSettings(Material mat){
            int blendMode = mat.GetInt("_BlendMode");
            int workflow = mat.GetInt("_Workflow");
            bool softening = mat.GetInt("_Softening") == 1 && blendMode != 6;
            bool distortion = mat.GetInt("_Distortion") == 1;
            bool distortUV = mat.GetInt("_DistortMainTex") == 1 && distortion;
            bool layering = mat.GetTexture("_SecondTex");
            bool pulse = mat.GetInt("_Pulse") == 1;
            bool falloff = mat.GetInt("_Falloff") == 1 && blendMode != 6;
            bool flipbook = mat.GetInt("_FlipbookBlending") == 1;
            bool cutout = mat.GetInt("_IsCutout") == 1;
            bool alphaMask = mat.GetTexture("_AlphaMask") && (blendMode != 6 || cutout) && mat.GetInt("_AlphaSource") == 1;
            bool filtering = mat.GetInt("_Filtering") == 1;
            bool audiolink = mat.GetInt("_AudioLink") == 1;
            bool lighting = mat.GetInt("_LightingToggle") == 1;
            bool normalMap = mat.GetTexture("_NormalMapLighting") && lighting;
            bool reflections = mat.GetInt("_ReflectionsToggle") == 1 && lighting;
            bool specHighlight = mat.GetInt("_SpecularHighlightsToggle") == 1 && lighting;
            bool metallicMap = mat.GetTexture("_MetallicMap") && lighting && workflow == 0;
            bool roughnessMap = mat.GetTexture("_RoughnessMap") && lighting && workflow == 0;
            bool occlusionMap = mat.GetTexture("_OcclusionMap") && lighting && workflow == 0;
            bool packedMode = lighting && workflow == 1;
            bool dissolve = mat.GetInt("_Dissolve") == 1;
            bool randomHue = mat.GetInt("_RandomHue") == 1;
            bool outlines = mat.GetInt("_Outlines") == 1 && blendMode == 6;
            bool outlineStencil = mat.GetInt("_OutlineStencilToggle") == 1;
            bool emission = mat.GetInt("_Emission") == 1 && lighting;
            bool rimLight = mat.GetInt("_RimLight") == 1;

            MGUI.SetKeyword(mat, "_ALPHATEST_ON", cutout);
            MGUI.SetKeyword(mat, "_FADING_ON", softening);
            MGUI.SetKeyword(mat, "_DISTORTION_ON", distortion);
            MGUI.SetKeyword(mat, "_DISTORTION_UV_ON", distortUV);
            MGUI.SetKeyword(mat, "_LAYERED_TEX_ON", layering);
            MGUI.SetKeyword(mat, "_PULSE_ON", pulse);
            MGUI.SetKeyword(mat, "_FALLOFF_ON", falloff);
            MGUI.SetKeyword(mat, "_FLIPBOOK_BLENDING", flipbook);
            MGUI.SetKeyword(mat, "_FILTERING_ON", filtering);
            MGUI.SetKeyword(mat, "_AUDIOLINK_ON", audiolink);
            MGUI.SetKeyword(mat, "_LIGHTING_ON", lighting);
            MGUI.SetKeyword(mat, "_NORMALMAP_ON", normalMap);
            MGUI.SetKeyword(mat, "_REFLECTIONS_ON", reflections);
            MGUI.SetKeyword(mat, "_SPECULAR_HIGHLIGHTS_ON", specHighlight);
            MGUI.SetKeyword(mat, "_METALLIC_MAP_ON", metallicMap);
            MGUI.SetKeyword(mat, "_ROUGHNESS_MAP_ON", roughnessMap);
            MGUI.SetKeyword(mat, "_OCCLUSION_MAP_ON", occlusionMap);
            MGUI.SetKeyword(mat, "_WORKFLOW_PACKED_ON", packedMode);
            MGUI.SetKeyword(mat, "_DISSOLVE_ON", dissolve);
            MGUI.SetKeyword(mat, "_ALPHA_MASK_ON", alphaMask);
            MGUI.SetKeyword(mat, "_RANDOM_HUE_ON", randomHue);
            MGUI.SetKeyword(mat, "_EMISSION_ON", emission);
            MGUI.SetKeyword(mat, "_RIMLIGHT_ON", rimLight);

            mat.SetShaderPassEnabled("Always", outlines);
            mat.SetShaderPassEnabled("GrabPass", distortion && blendMode != 6);

            if (outlineStencil)
                mat.SetInt("_OutlineCulling", 0);
            else
                mat.SetInt("_OutlineCulling", 1);

            if (!outlineStencil){
                mat.SetFloat("_OutlineStencilPass", mat.GetFloat("_StencilPass"));
                mat.SetFloat("_OutlineStencilCompare", mat.GetFloat("_StencilCompare"));
            }

            SetBlendMode(mat);
        }

        void ApplyOutlineStencilConfig(Material mat){
            mat.SetFloat("_StencilPass", (float)UnityEngine.Rendering.StencilOp.Replace);
            mat.SetFloat("_StencilFail", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_StencilZFail", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_StencilCompare", (float)UnityEngine.Rendering.CompareFunction.Always);

            mat.SetFloat("_OutlineStencilPass", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_OutlineStencilCompare", (float)UnityEngine.Rendering.CompareFunction.NotEqual);
        }

        void DoStencilReset(){
            _StencilRef.floatValue = 1f;
            _StencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
            _StencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
            _StencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
            _StencilCompare.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
            _OutlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
            _OutlineStencilCompare.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
        }

        public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader) {
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
            MGUI.ClearKeywords(mat);
            ApplyMaterialSettings(mat);
        }

        // UV mode, tiling, scrolling and polar settings of one texture
        struct UVSettings {
            public int mode;
            public Vector4 scaleOffset;
            public Vector4 speed;
            public float polarRadius;
            public float polarRotation;
            public float polarSpeed;

            public static UVSettings Get(Material mat, string texName){
                return new UVSettings {
                    // Only the X version has UV modes, the regular version always scales and scrolls
                    mode = MGUI.IsXVersion(mat) ? mat.GetInt(texName + "UVMode") : 0,
                    scaleOffset = TexturePacker.GetScaleOffset(mat, texName),
                    speed = mat.GetVector(texName + "Speed"),
                    polarRadius = mat.GetFloat(texName + "PolarRadius"),
                    polarRotation = mat.GetFloat(texName + "PolarRotation"),
                    polarSpeed = mat.GetFloat(texName + "PolarSpeed")
                };
            }

            public void Apply(Material mat, string texName){
                mat.SetInt(texName + "UVMode", mode);
                mat.SetTextureScale(texName, new Vector2(scaleOffset.x, scaleOffset.y));
                mat.SetTextureOffset(texName, new Vector2(scaleOffset.z, scaleOffset.w));
                mat.SetVector(texName + "Speed", speed);
                mat.SetFloat(texName + "PolarRadius", polarRadius);
                mat.SetFloat(texName + "PolarRotation", polarRotation);
                mat.SetFloat(texName + "PolarSpeed", polarSpeed);
            }

            public bool IsStatic => mode == 0 && speed.x == 0f && speed.y == 0f;

            // Only compares the settings the mode actually uses
            public bool Matches(UVSettings other){
                if (mode != other.mode)
                    return false;
                if (mode == 1)
                    return polarRadius == other.polarRadius && polarRotation == other.polarRotation && polarSpeed == other.polarSpeed;
                return scaleOffset == other.scaleOffset && speed == other.speed;
            }
        }

        // Separate maps are sampled from their green channel, and the strengths are baked in because the editor
        // resets the packed strengths to 1 afterwards. Maps sharing one UV setup pass it on to the packed map;
        // otherwise only static tiling can be baked in, so scrolling and polar UVs would be lost.
        static bool PackPBRTextures(Material mat){
            string[] maps = { "_OcclusionMap", "_RoughnessMap", "_MetallicMap" };
            UVSettings[] uvs = new UVSettings[maps.Length];
            UVSettings? sharedUV = null;
            bool allMatch = true;
            bool allStatic = true;
            for (int i = 0; i < maps.Length; i++){
                uvs[i] = UVSettings.Get(mat, maps[i]);
                if (!mat.GetTexture(maps[i]))
                    continue;
                if (sharedUV == null)
                    sharedUV = uvs[i];
                else if (!uvs[i].Matches(sharedUV.Value))
                    allMatch = false;
                allStatic &= uvs[i].IsStatic;
            }

            if (!allMatch && !allStatic){
                string message = $"{mat.name} 上的贴图使用了不同的滚动或极坐标 UV 设置，无法合并为一张打包贴图。 "
                    + "打包会将贴图平铺设置烘焙进去，并丢弃 UV 动画。";
                if (!EditorUtility.DisplayDialog("打包贴图", message, "仍然打包", "取消"))
                    return false;
            }

            float metallic = mat.GetFloat("_Metallic");
            float roughness = mat.GetFloat("_Roughness");
            TexturePacker.Channel[] channels = {
                new TexturePacker.Channel(mat.GetTexture(maps[0]), 1, mat.GetFloat("_Occlusion"), 1f, true),
                new TexturePacker.Channel(mat.GetTexture(maps[1]), 1, roughness, roughness),
                new TexturePacker.Channel(mat.GetTexture(maps[2]), 1, metallic, metallic)
            };
            if (!allMatch){
                for (int i = 0; i < channels.Length; i++)
                    channels[i].scaleOffset = uvs[i].scaleOffset;
            }

            if (TexturePacker.PackTextures(mat, "_PackedMap", channels[0], channels[1], channels[2]) == null)
                return false;

            if (allMatch){
                sharedUV.Value.Apply(mat, "_PackedMap");
            }
            else {
                // The packed map already has identity tiling from the packer
                mat.SetInt("_PackedMapUVMode", 0);
                mat.SetVector("_PackedMapSpeed", Vector4.zero);
            }
            return true;
        }

        void DrawUVBlock(
            Material mat,
            MaterialEditor me,
            MaterialProperty texture,
            MaterialProperty uvMode,
            MaterialProperty speed,
            MaterialProperty polarRadius,
            MaterialProperty polarRotation,
            MaterialProperty polarSpeed
        ){
            if (texture.textureValue){
                if (MGUI.IsXVersion(mat)){
                    me.ShaderProperty(uvMode, "UV 模式");
                    if (uvMode.floatValue != 1){
                        MGUI.TextureSOScroll(me, texture, speed);
                    }
                    else {
                        me.ShaderProperty(polarRadius, "半径");
                        me.ShaderProperty(polarRotation, "旋转");
                        me.ShaderProperty(polarSpeed, "旋转速度");
                    }
                }
                else {
                    MGUI.TextureSOScroll(me, texture, speed);
                }
                MGUI.Space6();
            }
        }

        void CacheRenderersUsingThisMaterial(Material mat){
            m_RenderersUsingThisMaterial.Clear();

            ParticleSystemRenderer[] renderers = UnityEngine.Object.FindObjectsOfType(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
            foreach (ParticleSystemRenderer renderer in renderers)
            {
                if (renderer.sharedMaterial == mat)
                    m_RenderersUsingThisMaterial.Add(renderer);
            }
        }
    }
}
