using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Mochie;

namespace Mochie {

    public class GlassEditor : ShaderGUI {

        public static Dictionary<Material, Toggles> foldouts = new Dictionary<Material, Toggles>();
        Toggles toggles = new Toggles(new string[] {
            "表面",
            "高光",
            "自发光",
            "雨",
            "光照贴图设置",
            "光照体积",
            "LTCGI",
            "AreaLit",
            "渲染设置"
        }, 1);

        string versionLabel = "v1.15.2 | LGC 中文版 1.78.1";

        // Surface
        MaterialProperty _Workflow = null;
        MaterialProperty _GrabpassTint = null;
        MaterialProperty _SpecularityTint = null;
        MaterialProperty _BaseColorTint = null;
        MaterialProperty _MainTex = null;
        MaterialProperty _RoughnessMap = null;
        MaterialProperty _MetallicMap = null;
        MaterialProperty _OcclusionMap = null;
        MaterialProperty _NormalMap = null;
        MaterialProperty _Roughness = null;
        MaterialProperty _Metallic = null;
        MaterialProperty _Occlusion = null;
        MaterialProperty _NormalStrength = null;
        MaterialProperty _PackedMap = null;
        MaterialProperty _PackedRoughnessStrength = null;
        MaterialProperty _PackedMetallicStrength = null;
        MaterialProperty _PackedOcclusionStrength = null;
        MaterialProperty _RoughnessChannel = null;
        MaterialProperty _MetallicChannel = null;
        MaterialProperty _OcclusionChannel = null;
        MaterialProperty _Refraction = null;
        MaterialProperty _Blur = null;
        MaterialProperty _BlurQuality = null;
        MaterialProperty _RefractionIOR = null;
        MaterialProperty _RefractVertexNormal = null;

        // Emission
        MaterialProperty _EmissionMap = null;
        MaterialProperty _EmissionColor= null;
        MaterialProperty _EmissionStrength = null;
        MaterialProperty _EmissionParallaxDepth = null;

        // Rain
        // MaterialProperty _RainToggle = null;
        MaterialProperty _Speed = null;
        MaterialProperty _XScale = null;
        MaterialProperty _YScale = null;
        MaterialProperty _Strength = null;
        MaterialProperty _RainMode = null;
        MaterialProperty _RainMask = null;
        MaterialProperty _RainMaskChannel = null;
        MaterialProperty _RippleScale = null;
        MaterialProperty _RippleSpeed = null;
        MaterialProperty _RippleStrength = null;
        MaterialProperty _RippleSize = null;
        MaterialProperty _RippleDensity = null;
        MaterialProperty _DynamicDroplets = null;
        MaterialProperty _RainThreshold = null;
        MaterialProperty _RainThresholdSize = null;

        // Lightmap Settings
        MaterialProperty _LightmappedSpecularity = null;
        MaterialProperty _LightmapDistortion = null;
        MaterialProperty _IndirectStrength = null;
        MaterialProperty _IndirectSaturation = null;
        MaterialProperty _BAKERY_LMSPEC = null;
        MaterialProperty _BakeryLMSpecStrength = null;
        MaterialProperty _BakeryMode = null;
        MaterialProperty _BicubicSampling = null;
        MaterialProperty _IgnoreRealtimeGI = null;

        // Light Volumes
        MaterialProperty _LightVolumesToggle = null;
        MaterialProperty _LightVolumeStrength = null;
        MaterialProperty _AdditiveLightVolumesToggle = null;
        MaterialProperty _AdditiveLightVolumeStrength = null;
        MaterialProperty _LightVolumeSpecularity = null;
        MaterialProperty _LightVolumeSpecularityStrength = null;
        MaterialProperty _LightVolumeBias = null;

        // Area Lit
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

        // LTCGI
        MaterialProperty _LTCGI = null;
        MaterialProperty _LTCGIStrength = null;
        MaterialProperty _LTCGIRoughness = null;
        MaterialProperty _LTCGI_SpecularColor = null;

        // Render Settings
        MaterialProperty _ReflectionsToggle = null;
        MaterialProperty _ReflectionStrength = null;
        MaterialProperty _SpecularToggle = null;
        MaterialProperty _SpecularStrength = null;
        MaterialProperty _SSRToggle = null;
        MaterialProperty _SSRStrength = null;
        MaterialProperty _SSREdgeFade = null;
        MaterialProperty _SSRHeight = null;
        MaterialProperty _GSAAToggle = null;
        MaterialProperty _GSAAStrength = null;
        MaterialProperty _Culling = null;
        MaterialProperty _SamplingMode = null;
        MaterialProperty _BlendMode = null;
        MaterialProperty _LitBaseColor = null;
        MaterialProperty _QueueOffset = null;
        MaterialProperty _TexCoordSpace = null;
        MaterialProperty _TexCoordSpaceSwizzle = null;
        MaterialProperty _GlobalTexCoordScale = null;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public override void OnGUI(MaterialEditor me, MaterialProperty[] props) {
            if (!me.isVisible)
                return;

            foreach (var property in GetType().GetFields(bindingFlags)){
                if (property.FieldType == typeof(MaterialProperty))
                    property.SetValue(this, FindProperty(property.Name, props));
            }
            Material mat = (Material)me.target;
            bool isTwoPass = mat.shader.name.Contains("(Two Pass)");

            if (!foldouts.ContainsKey(mat))
                foldouts.Add(mat, toggles);

            if (mat.GetInt("_MaterialResetCheck") == 0){
                mat.SetInt("_MaterialResetCheck", 1);
                SetKeywords(mat);
                SetBlendMode(mat);
            }

            MGUI.DoHeader(isTwoPass ? "双通道玻璃" : "玻璃");

            EditorGUI.BeginChangeCheck(); {

                // Surface
                Foldouts.DoFoldout(foldouts, mat, "表面", 1, Foldouts.Style.StandardButton);
                if (Foldouts.DoFoldoutButton(MGUI.collapseLabel, 11)) Toggles.CollapseFoldouts(mat, foldouts, 1);
                if (Foldouts.BeginFoldout(foldouts, mat, "表面")) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Workflow, Tips.workflowText);
                            me.ShaderProperty(_BlendMode, "透明度");
                            if (_BlendMode.floatValue == 0)
                                me.ShaderProperty(_GrabpassTint, "GrabPass 色调");
                            me.ShaderProperty(_SpecularityTint, "高光色调");
                            me.ShaderProperty(_BaseColorTint, "基础颜色色调");
                        });
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(Tips.baseColorLabel, _MainTex, _LitBaseColor);
                            MGUI.TexPropLabel(Tips.litBaseColorText, 100, false);
                            MGUI.TextureSO(me, _MainTex, _MainTex.textureValue);
                            me.TexturePropertySingleLine(Tips.normalMapText, _NormalMap, _NormalMap.textureValue ? _NormalStrength : null);
                            MGUI.TextureSO(me, _NormalMap, _NormalMap.textureValue);
                            if (_Workflow.floatValue == 0){
                                me.TexturePropertySingleLine(Tips.metallicText, _MetallicMap, _Metallic);
                                MGUI.sRGBWarning(_MetallicMap);
                                MGUI.TextureSO(me, _MetallicMap, _MetallicMap.textureValue);
                                me.TexturePropertySingleLine(Tips.roughnessTexLabel, _RoughnessMap, _Roughness);
                                MGUI.sRGBWarning(_RoughnessMap);
                                MGUI.TextureSO(me, _RoughnessMap, _RoughnessMap.textureValue);
                                me.TexturePropertySingleLine(Tips.occlusionTexLabel, _OcclusionMap, _OcclusionMap.textureValue ? _Occlusion : null);
                                MGUI.sRGBWarning(_OcclusionMap);
                                MGUI.TextureSO(me, _OcclusionMap, _OcclusionMap.textureValue);
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
                                        foreach (Material target in me.targets){
                                            SetKeywords(target);
                                            SetBlendMode(target);
                                        }
                                    }
                                }
                            }
                            else {
                                me.TexturePropertySingleLine(Tips.packedMapText, _PackedMap);
                                MGUI.sRGBWarning(_PackedMap);
                                MGUI.TextureSO(me, _PackedMap, _PackedMap.textureValue);
                            }
                        });
                        if (_Workflow.floatValue == 1){
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_MetallicChannel, "金属度通道");
                                me.ShaderProperty(_RoughnessChannel, "粗糙度通道");
                                me.ShaderProperty(_OcclusionChannel, "环境光遮蔽通道");
                            });
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_PackedMetallicStrength, Tips.metallicPackedText);
                                me.ShaderProperty(_PackedRoughnessStrength, Tips.roughnessPackedText);
                                me.ShaderProperty(_PackedOcclusionStrength, Tips.occlusionPackedText);
                            });
                        }
                        if (_BlendMode.floatValue == 0){
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_BlurQuality, "模糊质量");
                                me.ShaderProperty(_Blur, "模糊强度");
                                me.ShaderProperty(_Refraction, Tips.normalRefractionText);
                                if (!isTwoPass)
                                    MGUI.ToggleFloat(me, Tips.meshRefractionText, _RefractVertexNormal, _RefractionIOR);
                            });
                        }
                    });
                }
                Foldouts.EndFoldout();

                // Specularity
                if (Foldouts.DoFoldout(foldouts, mat, "高光", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            MGUI.ToggleFloat(me, Tips.cubemapReflectionsText, _ReflectionsToggle, _ReflectionStrength);
                            if (!isTwoPass){
                                MGUI.ToggleFloat(me, Tips.ssrText, _SSRToggle, _SSRStrength);
                                if (_SSRToggle.floatValue == 1){
                                    me.ShaderProperty(_SSREdgeFade, Tips.ssrEdgeFadeText);
                                    me.ShaderProperty(_SSRHeight, Tips.ssrDepthText);
                                }
                            }
                            MGUI.ToggleFloat(me, Tips.specularHighlightsText, _SpecularToggle, _SpecularStrength);
                            MGUI.ToggleFloat(me, Tips.gsaa, _GSAAToggle, _GSAAStrength);
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
                            me.TexturePropertySingleLine(Tips.emissionText, _EmissionMap, _EmissionColor);
                            MGUI.TextureSO(me, _EmissionMap, _EmissionMap.textureValue);
                            me.ShaderProperty(_EmissionStrength, "强度");
                            if (!isTwoPass)
                                me.ShaderProperty(_EmissionParallaxDepth, "视差偏移");
                        });
                        if ((_BlendMode.floatValue == 1 || _BlendMode.floatValue == 2) && _EmissionParallaxDepth.floatValue > 0){
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_BlurQuality, "模糊质量");
                                me.ShaderProperty(_Blur, "模糊强度");
                            });
                        }
                         MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Rain
                if (Foldouts.DoFoldout(foldouts, mat, me, _RainMode, "雨", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        if (_RainMode.floatValue == 0 || _RainMode.floatValue == 1){
                            RainDroplets(me);
                        }
                        else if (_RainMode.floatValue == 2){
                            RainRipples(me);
                        }
                        else if (_RainMode.floatValue == 3){
                            MGUI.BoldLabel("水滴");
                            RainDroplets(me);
                            MGUI.BoldLabel("涟漪");
                            RainRipples(me);
                            MGUI.BoldLabel("两者");
                            RainBoth(me);
                        }
                    });
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
                            me.ShaderProperty(_LightmappedSpecularity, Tips.lightmappedSpecularityText);
                            me.ShaderProperty(_LightmapDistortion, "光照贴图 UV 扭曲");
                            me.ShaderProperty(_IndirectStrength, "光照贴图强度");
                            me.ShaderProperty(_IndirectSaturation, "光照贴图饱和度");
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_BicubicSampling, Tips.bicubicLightmap);
                            me.ShaderProperty(_IgnoreRealtimeGI, Tips.ignoreRealtimeGIText);
                        });
                    });
                }
                Foldouts.EndFoldout();

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
                            MGUI.ToggleGroup(_AreaLitToggle.floatValue == 0);
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
                            MGUI.DisplayInfo("AreaLit 包文件必须位于 Assets 文件夹下直接创建的、名为 AreaLit 的文件夹中（区分大小写，路径为 Assets/AreaLit）。");
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();
                }
                else {
                    _AreaLitToggle.floatValue = 0f;
                    mat.SetInt("_AreaLitToggle", 0);
                    mat.DisableKeyword("_AREALIT_ON");
                }

                // Render Settings
                if (Foldouts.DoFoldout(foldouts, mat, "渲染设置", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_SamplingMode, Tips.samplingModeText);
                            me.ShaderProperty(_TexCoordSpace, Tips.texCoordSpaceText);
                            if (_TexCoordSpace.floatValue == 1f){
                                me.ShaderProperty(_TexCoordSpaceSwizzle, Tips.swizzleText);
                            }
                            me.ShaderProperty(_GlobalTexCoordScale, "贴图坐标缩放");
                        });
                        MGUI.PropertyGroup(()=>{
                            MGUI.SpaceN1();
                            _QueueOffset.floatValue = (int)_QueueOffset.floatValue;
                            MGUI.DummyProperty("渲染队列：", mat.renderQueue.ToString());
                            me.ShaderProperty(_QueueOffset, Tips.queueOffset);
                            if (!isTwoPass)
                                me.ShaderProperty(_Culling, "剔除模式");
                        });
                    });
                }
                Foldouts.EndFoldout();
            }
            if (EditorGUI.EndChangeCheck()){
                SetKeywords(mat);
            }
            SetBlendMode(mat);
            MGUI.Space10();
            MGUI.DoFooter(versionLabel, "Glass-Shader");
        }

        void RainDroplets(MaterialEditor me){
            MGUI.ToggleGroup(_RainMode.floatValue == 0);
            MGUI.PropertyGroup(()=>{
                if (_RainMode.floatValue != 3){
                    me.TexturePropertySingleLine(Tips.maskText, _RainMask, _RainMaskChannel);
                    MGUI.TextureSO(me, _RainMask, _RainMask.textureValue);
                }
                me.ShaderProperty(_Strength, "强度");
                me.ShaderProperty(_Speed, "速度");
                me.ShaderProperty(_XScale, "X 缩放");
                me.ShaderProperty(_YScale, "Y 缩放");
                me.ShaderProperty(_DynamicDroplets, "动态水滴");
            });
            MGUI.ToggleGroupEnd();
        }

        void RainRipples(MaterialEditor me){
            MGUI.PropertyGroup(()=>{
                if (_RainMode.floatValue != 3){
                    me.TexturePropertySingleLine(Tips.maskText, _RainMask, _RainMaskChannel);
                    MGUI.TextureSO(me, _RainMask, _RainMask.textureValue);
                }
                me.ShaderProperty(_RippleStrength, "强度");
                me.ShaderProperty(_RippleSpeed, "速度");
                me.ShaderProperty(_RippleScale, "缩放");
                me.ShaderProperty(_RippleDensity, "密度");
                me.ShaderProperty(_RippleSize, "尺寸");
            });
        }

        void RainBoth(MaterialEditor me){
            MGUI.PropertyGroup(()=>{
                me.TexturePropertySingleLine(Tips.maskText, _RainMask, _RainMaskChannel);
                MGUI.TextureSO(me, _RainMask, _RainMask.textureValue);
                me.ShaderProperty(_RainThreshold, "角度阈值");
                me.ShaderProperty(_RainThresholdSize, "阈值混合");
            });
        }

        // Strengths are baked in because the editor resets the packed strengths to 1 afterwards.
        // Each map has its own tiling: if they all match it moves to the packed map, otherwise it's baked in.
        static bool PackPBRTextures(Material mat){
            float metallic = mat.GetFloat("_Metallic");
            float roughness = mat.GetFloat("_Roughness");
            TexturePacker.Channel occlusionChannel = new TexturePacker.Channel(mat.GetTexture("_OcclusionMap"), 0, mat.GetFloat("_Occlusion"), 1f, true);
            TexturePacker.Channel roughnessChannel = new TexturePacker.Channel(mat.GetTexture("_RoughnessMap"), 0, roughness, roughness);
            TexturePacker.Channel metallicChannel = new TexturePacker.Channel(mat.GetTexture("_MetallicMap"), 0, metallic, metallic);

            bool shared = TexturePacker.TryGetSharedScaleOffset(mat, out Vector4 sharedST, "_OcclusionMap", "_RoughnessMap", "_MetallicMap");
            if (!shared){
                occlusionChannel.scaleOffset = TexturePacker.GetScaleOffset(mat, "_OcclusionMap");
                roughnessChannel.scaleOffset = TexturePacker.GetScaleOffset(mat, "_RoughnessMap");
                metallicChannel.scaleOffset = TexturePacker.GetScaleOffset(mat, "_MetallicMap");
            }

            if (TexturePacker.PackTextures(mat, "_PackedMap", occlusionChannel, roughnessChannel, metallicChannel) == null)
                return false;
            if (shared){
                mat.SetTextureScale("_PackedMap", new Vector2(sharedST.x, sharedST.y));
                mat.SetTextureOffset("_PackedMap", new Vector2(sharedST.z, sharedST.w));
            }
            return true;
        }

        void SetKeywords(Material mat){
            bool isEmissive = (mat.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
            bool isTwoPass = mat.shader.name.Contains("(Two Pass)");
            int rainMode = mat.GetInt("_RainMode");
            int blurMode = mat.GetInt("_BlurQuality");
            MGUI.SetKeyword(mat, "_EMISSION_ON", isEmissive);
            MGUI.SetKeyword(mat, "_WORKFLOW_PACKED_ON", mat.GetInt("_Workflow") == 1);
            MGUI.SetKeyword(mat, "_STOCHASTIC_SAMPLING_ON", mat.GetInt("_SamplingMode") == 1);
            MGUI.SetKeyword(mat, "_NORMALMAP_ON", mat.GetTexture("_NormalMap"));
            MGUI.SetKeyword(mat, "_RAIN_ON", rainMode > 0);
            MGUI.SetKeyword(mat, "_RAINMODE_RIPPLE", rainMode == 2);
            MGUI.SetKeyword(mat, "_RAINMODE_AUTO", rainMode == 3);
            MGUI.SetKeyword(mat, "_BLURQUALITY_LOW", blurMode == 0);
            MGUI.SetKeyword(mat, "_BLURQUALITY_MED", blurMode == 1);
            MGUI.SetKeyword(mat, "_BLURQUALITY_HIGH", blurMode == 2);
            MGUI.SetKeyword(mat, "_BLURQUALITY_ULTRA", blurMode == 3);
            MGUI.SetKeyword(mat, "_AREALIT_ON", mat.GetInt("_AreaLitToggle") == 1);
            MGUI.SetKeyword(mat, "LTCGI", mat.GetInt("_LTCGI") == 1);
            MGUI.SetKeyword(mat, "_REFLECTIONS_ON", mat.GetInt("_ReflectionsToggle") == 1);
            MGUI.SetKeyword(mat, "_SPECULAR_HIGHLIGHTS_ON", mat.GetInt("_SpecularToggle") == 1);
            MGUI.SetKeyword(mat, "_SSR_ON", mat.GetInt("_SSRToggle") == 1 && mat.GetInt("_BlendMode") == 0 && !isTwoPass);
            MGUI.SetKeyword(mat, "BAKERY_SH", mat.GetInt("_BakeryMode") == 1);
            MGUI.SetKeyword(mat, "BAKERY_RNM", mat.GetInt("_BakeryMode") == 2);
            MGUI.SetKeyword(mat, "BAKERY_MONOSH", mat.GetInt("_BakeryMode") == 3);
            MGUI.SetKeyword(mat, "BAKERY_LMSPEC", mat.GetInt("_BAKERY_LMSPEC") == 1);
        }

        void SetBlendMode(Material mat){
            int blendMode = mat.GetInt("_BlendMode");
            // float roughness = mat.GetFloat("_Roughness");
            // bool rainToggle = mat.GetInt("_RainMode") > 0;
            // bool hasNormal = mat.GetTexture("_NormalMap") && mat.GetFloat("_NormalStrength") > 0;
            // bool canGrab = roughness > 0 || rainToggle || hasNormal;
            // if (blendMode == 0) blendMode = canGrab ? blendMode : 1;
            #if UNITY_ANDROID
                if (blendMode == 0){
                    mat.SetInt("_BlendMode", 1);
                    blendMode = 1;
                }
            #endif

            switch (blendMode){
                case 0: // Grabpass
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    mat.SetShaderPassEnabled("GrabPass", true);
                    MGUI.SetKeyword(mat, "_GRABPASS_ON", true);
                    MGUI.SetKeyword(mat, "_PREMULTIPLIED_ON", false);
                    break;
                case 1: // Premultiplied
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+mat.GetInt("_QueueOffset");
                    mat.SetShaderPassEnabled("GrabPass", false);
                    MGUI.SetKeyword(mat, "_GRABPASS_ON", false);
                    MGUI.SetKeyword(mat, "_PREMULTIPLIED_ON", true);
                    break;
                case 2: // Opaque
                    mat.SetOverrideTag("RenderType", "Opaque");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry+mat.GetInt("_QueueOffset");
                    mat.SetShaderPassEnabled("GrabPass", false);
                    MGUI.SetKeyword(mat, "_GRABPASS_ON", false);
                    MGUI.SetKeyword(mat, "_PREMULTIPLIED_ON", false);
                    break;
                default: break;
            }
        }

        public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader) {
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
            MGUI.ClearKeywords(mat);
            SetBlendMode(mat);
            SetKeywords(mat);
        }
    }
}