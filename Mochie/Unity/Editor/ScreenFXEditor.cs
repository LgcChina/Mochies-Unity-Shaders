using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using Mochie;

namespace Mochie {

    public class ScreenFXEditor : ShaderGUI {

        public enum BlendingModes {Opaque, Alpha, Premultiplied, Additive, Soft_Additive, Multiply, Multiply_2x}

        GUIContent screenTexLabel = new GUIContent("贴图");
        GUIContent shakeNoiseTexLabel = new GUIContent("噪波贴图");
        GUIContent normalMapLabel = new GUIContent("法线贴图");
        GUIContent tpTexLabel = new GUIContent("贴图");
        GUIContent tpNoiseTexLabel = new GUIContent("噪波贴图");

        public static Dictionary<Material, Toggles> foldouts = new Dictionary<Material, Toggles>();

        Toggles toggles = new Toggles(new string[] {
                "常规",
                "滤镜",
                "滤镜 ",
                "震动",
                "震动 ",
                "扭曲",
                "扭曲 ",
                "模糊",
                "模糊 ",
                "缩放",
                "缩放 ",
                "图像叠加",
                "雾",
                "雾 ",
                "三平面",
                "三平面 ",
                "描边",
                "描边 ",
                "杂项",
                "杂项 ",
                "黑边",
                "高对比失真",
                "脉冲",
                "UV 操作",
                "取整",
                "法线贴图滤镜",
                "深度缓冲",
                "安全区",
                "噪波",
                "噪波 ",
                "Audio Link",
                "图像叠加 ",
                "Sobel 滤镜",
        }, 0);

        string versionLabel = "v1.22 | LGC 中文版 1.78.1";

        // Commonly used strings
        string strengthLabel = "强度";
        string fadeLabel = "渐隐";
        string radiusLabel = "半径";
        string minLabel = "最小范围";
        string maxLabel = "最大范围";
        string p2oLabel = "玩家到对象";
        string ugfLabel = "使用全局衰减";
        string colorLabel = "颜色";
        string speedXLabel = "速度 X";
        string speedYLabel = "速度 Y";
        string speedLabel = "速度";

        // General
        // MaterialProperty _MirrorRenderMode = null;
        MaterialProperty _CameraRenderMode = null;
        MaterialProperty _BlendMode = null;
        MaterialProperty _FalloffMode = null;
        MaterialProperty _MinRange = null;
        MaterialProperty _MaxRange = null;
        MaterialProperty _BoxSize = null;
        MaterialProperty _BoxOffset = null;
        MaterialProperty _BlendDistance = null;
        MaterialProperty _Opacity = null;

        // Color Filtering
        MaterialProperty _FilterModel = null;
        MaterialProperty _FilterStrength = null;
        MaterialProperty _ColorUseGlobal = null;
        MaterialProperty _ColorMinRange = null;
        MaterialProperty _ColorMaxRange = null;
        MaterialProperty _ColorFalloffMode = null;
        MaterialProperty _ColorBoxSize = null;
        MaterialProperty _ColorBoxOffset = null;
        MaterialProperty _ColorBlendDistance = null;
        MaterialProperty _Color = null;
        MaterialProperty _RGB = null;
        MaterialProperty _Contrast = null;
        MaterialProperty _HDR = null;
        MaterialProperty _Invert = null;
        MaterialProperty _InvertR = null;
        MaterialProperty _InvertG = null;
        MaterialProperty _InvertB = null;
        MaterialProperty _AutoShift = null;
        MaterialProperty _AutoShiftSpeed = null;
        MaterialProperty _Hue = null;
        MaterialProperty _Saturation = null;
        MaterialProperty _Brightness = null;
        MaterialProperty _SaturationR = null;
        MaterialProperty _SaturationB = null;
        MaterialProperty _SaturationG = null;
        MaterialProperty _ClampToggle = null;
        MaterialProperty _ClampMax = null;
        MaterialProperty _HueMode = null;
        MaterialProperty _MonoTint = null;

        // Shake
        MaterialProperty _ShakeModel = null;
        MaterialProperty _ShakeUseGlobal = null;
        MaterialProperty _ShakeMinRange = null;
        MaterialProperty _ShakeMaxRange = null;
        MaterialProperty _ShakeFalloffMode = null;
        MaterialProperty _ShakeBoxSize = null;
        MaterialProperty _ShakeBoxOffset = null;
        MaterialProperty _ShakeBlendDistance = null;
        MaterialProperty _ShakeNoiseTex = null;
        MaterialProperty _Amplitude = null;
        MaterialProperty _ShakeSpeedX = null;
        MaterialProperty _ShakeSpeedY = null;
        MaterialProperty _ShakeSpeedXY = null;

        // Distortion
        MaterialProperty _DistortionModel = null;
        MaterialProperty _DistortionUseGlobal = null;
        MaterialProperty _DistortionMinRange = null;
        MaterialProperty _DistortionMaxRange = null;
        MaterialProperty _DistortionFalloffMode = null;
        MaterialProperty _DistortionBoxSize = null;
        MaterialProperty _DistortionBoxOffset = null;
        MaterialProperty _DistortionBlendDistance = null;
        MaterialProperty _NormalMap = null;
        MaterialProperty _DistortionStr = null;
        MaterialProperty _DistortionSpeed = null;
        MaterialProperty _DistortionRadius = null;
        MaterialProperty _DistortionFade = null;
        MaterialProperty _DistortionP2O = null;

        // Blur
        MaterialProperty _BlurModel = null;
        MaterialProperty _BlurUseGlobal = null;
        MaterialProperty _BlurMinRange = null;
        MaterialProperty _BlurMaxRange = null;
        MaterialProperty _BlurFalloffMode = null;
        MaterialProperty _BlurBoxSize = null;
        MaterialProperty _BlurBoxOffset = null;
        MaterialProperty _BlurBlendDistance = null;
        MaterialProperty _RGBSplit = null;
        MaterialProperty _DoF = null;
        MaterialProperty _BlurOpacity = null;
        MaterialProperty _BlurStr = null;
        MaterialProperty _DoFP2O = null;
        MaterialProperty _DoFFade = null;
        MaterialProperty _DoFRadius = null;
        MaterialProperty _PixelationStr = null;
        MaterialProperty _RippleGridStr = null;
        MaterialProperty _BlurRadius = null;
        MaterialProperty _BlurY = null;
    // 	MaterialProperty _BloomThreshold = null;
        MaterialProperty _BlurSamples = null;
        MaterialProperty _PixelBlurSamples = null;
        MaterialProperty _CrushBlur = null;

        // Noise
        MaterialProperty _ScanLine = null;
        MaterialProperty _ScanLineThick = null;
        MaterialProperty _ScanLineSpeed = null;
        MaterialProperty _NoiseMode = null;
        MaterialProperty _Noise = null;
        MaterialProperty _NoiseRGB = null;
        MaterialProperty _NoiseStrength = null;

        // Fog
        MaterialProperty _Fog = null;
        MaterialProperty _FogUseGlobal = null;
        MaterialProperty _FogMinRange = null;
        MaterialProperty _FogMaxRange = null;
        MaterialProperty _FogFalloffMode = null;
        MaterialProperty _FogBoxSize = null;
        MaterialProperty _FogBoxOffset = null;
        MaterialProperty _FogBlendDistance = null;
        MaterialProperty _FogColor = null;
        MaterialProperty _FogRadius = null;
        MaterialProperty _FogSafeZone = null;
        MaterialProperty _FogSafeRadius = null;
        MaterialProperty _FogSafeMaxRange = null;
        MaterialProperty _FogP2O = null;
        MaterialProperty _FogFade = null;
        MaterialProperty _FogSafeOpacity = null;
        // MaterialProperty _FogHeightMin = null;
        // MaterialProperty _FogHeightMax = null;
        // MaterialProperty _HeightFalloff = null;

        // Zoom
        MaterialProperty _Zoom = null;
        MaterialProperty _ZoomUseGlobal = null;
        MaterialProperty _ZoomMinRange = null;
        MaterialProperty _ZoomMaxRange = null;
        MaterialProperty _ZoomFalloffMode = null;
        MaterialProperty _ZoomBoxSize = null;
        MaterialProperty _ZoomBoxOffset = null;
        MaterialProperty _ZoomBlendDistance = null;
        MaterialProperty _ZoomStr = null;
        MaterialProperty _ZoomStrR = null;
        MaterialProperty _ZoomStrG = null;
        MaterialProperty _ZoomStrB = null;

        // SST
        MaterialProperty _SST = null;
        MaterialProperty _SSTUseGlobal = null;
        MaterialProperty _SSTMinRange = null;
        MaterialProperty _SSTMaxRange = null;
        MaterialProperty _SSTFalloffMode = null;
        MaterialProperty _SSTBoxSize = null;
        MaterialProperty _SSTBoxOffset = null;
        MaterialProperty _SSTBlendDistance = null;
        MaterialProperty _SSTBlend = null;
        MaterialProperty _ScreenTex = null;
        MaterialProperty _SSTColor = null;
        MaterialProperty _SSTWidth = null;
        MaterialProperty _SSTHeight = null;
        MaterialProperty _SSTScale = null;
        MaterialProperty _SSTLR = null;
        MaterialProperty _SSTUD = null;
        MaterialProperty _SSTColumnsX = null;
        MaterialProperty _SSTRowsY = null;
        MaterialProperty _SSTAnimationSpeed = null;
        MaterialProperty _SSTAnimatedDist = null;
        MaterialProperty _ScrubPos = null;
        MaterialProperty _ManualScrub = null;

        // Triplanar
        MaterialProperty _Triplanar = null;
        MaterialProperty _TPUseGlobal = null;
        MaterialProperty _TPMinRange = null;
        MaterialProperty _TPMaxRange = null;
        MaterialProperty _TPFalloffMode = null;
        MaterialProperty _TPBoxSize = null;
        MaterialProperty _TPBoxOffset = null;
        MaterialProperty _TPBlendDistance = null;
        MaterialProperty _TPRadius = null;
        MaterialProperty _TPFade = null;
        MaterialProperty _TPP2O = null;
        MaterialProperty _TPColor = null;
        MaterialProperty _TPTexture = null;
        MaterialProperty _TPScroll = null;
        MaterialProperty _TPNoiseTex = null;
        MaterialProperty _TPNoiseScroll = null;
        MaterialProperty _TPThickness = null;
        MaterialProperty _TPNoise = null;
        MaterialProperty _TPScanFade = null;

        // Audio Link
        MaterialProperty _AudioLinkToggle = null;
        MaterialProperty _AudioLinkStrength = null;
        MaterialProperty _AudioLinkMin = null;
        MaterialProperty _AudioLinkMax = null;
        MaterialProperty _AudioLinkFilteringStrength = null;
        MaterialProperty _AudioLinkFilteringBand = null;
        MaterialProperty _AudioLinkFilteringMin = null;
        MaterialProperty _AudioLinkFilteringMax = null;
        MaterialProperty _AudioLinkShakeStrength = null;
        MaterialProperty _AudioLinkShakeBand = null;
        MaterialProperty _AudioLinkShakeMin = null;
        MaterialProperty _AudioLinkShakeMax = null;
        MaterialProperty _AudioLinkDistortionStrength = null;
        MaterialProperty _AudioLinkDistortionBand = null;
        MaterialProperty _AudioLinkDistortionMin = null;
        MaterialProperty _AudioLinkDistortionMax = null;
        MaterialProperty _AudioLinkBlurStrength = null;
        MaterialProperty _AudioLinkBlurBand = null;
        MaterialProperty _AudioLinkBlurMin = null;
        MaterialProperty _AudioLinkBlurMax = null;
        MaterialProperty _AudioLinkNoiseStrength = null;
        MaterialProperty _AudioLinkNoiseBand = null;
        MaterialProperty _AudioLinkNoiseMin = null;
        MaterialProperty _AudioLinkNoiseMax = null;
        MaterialProperty _AudioLinkZoomStrength = null;
        MaterialProperty _AudioLinkZoomBand = null;
        MaterialProperty _AudioLinkZoomMin = null;
        MaterialProperty _AudioLinkZoomMax = null;
        MaterialProperty _AudioLinkSSTStrength = null;
        MaterialProperty _AudioLinkSSTBand = null;
        MaterialProperty _AudioLinkSSTMin = null;
        MaterialProperty _AudioLinkSSTMax = null;
        MaterialProperty _AudioLinkFogOpacity = null;
        MaterialProperty _AudioLinkFogRadius = null;
        MaterialProperty _AudioLinkFogBand = null;
        MaterialProperty _AudioLinkFogMin = null;
        MaterialProperty _AudioLinkFogMax = null;
        MaterialProperty _AudioLinkTriplanarOpacity = null;
        MaterialProperty _AudioLinkTriplanarBand = null;
        MaterialProperty _AudioLinkTriplanarMin = null;
        MaterialProperty _AudioLinkTriplanarMax = null;
        MaterialProperty _AudioLinkTriplanarRadius = null;
        MaterialProperty _AudioLinkOutlineStrength = null;
        MaterialProperty _AudioLinkOutlineBand = null;
        MaterialProperty _AudioLinkOutlineMin = null;
        MaterialProperty _AudioLinkOutlineMax = null;
        MaterialProperty _AudioLinkOutlineColBand = null;
        MaterialProperty _AudioLinkOutlineColStrength = null;
        MaterialProperty _AudioLinkOutlineColMin = null;
        MaterialProperty _AudioLinkOutlineColMax = null;
        MaterialProperty _AudioLinkOutlineMixBand = null;
        MaterialProperty _AudioLinkOutlineMixStrength = null;
        MaterialProperty _AudioLinkOutlineMixMin = null;
        MaterialProperty _AudioLinkOutlineMixMax = null;
        MaterialProperty _AudioLinkMiscStrength = null;
        MaterialProperty _AudioLinkMiscBand = null;
        MaterialProperty _AudioLinkMiscMin = null;
        MaterialProperty _AudioLinkMiscMax = null;

        // Extras
        MaterialProperty _Pulse = null;
        MaterialProperty _PulseColor = null;
        MaterialProperty _PulseSpeed = null;
        MaterialProperty _WaveForm = null;
        MaterialProperty _Letterbox = null;
        MaterialProperty _LetterboxStr = null;
        MaterialProperty _UseZoomFalloff = null;
        MaterialProperty _Shift = null;
        MaterialProperty _InvertX = null;
        MaterialProperty _InvertY = null;
        MaterialProperty _ShiftX = null;
        MaterialProperty _ShiftY = null;
        MaterialProperty _OutlineCol = null;
        MaterialProperty _OutlineTex = null;
        MaterialProperty _OutlineTexAlt = null;
        MaterialProperty _OutlineTexCoord = null;
        MaterialProperty _OutlineTexCoordAlt = null;
        MaterialProperty _OutlineThresh = null;
        MaterialProperty _OutlineAltEnable = null;
        MaterialProperty _OutlineThreshAlt = null;
        MaterialProperty _OutlineColAlt = null;
        MaterialProperty _BackgroundCol = null;
        MaterialProperty _OutlineType = null;
        MaterialProperty _OutlineThiccS = null;
        MaterialProperty _OLUseGlobal = null;
        MaterialProperty _OLMinRange = null;
        MaterialProperty _OLMaxRange = null;
        MaterialProperty _OLFalloffMode = null;
        MaterialProperty _OLBoxSize = null;
        MaterialProperty _OLBoxOffset = null;
        MaterialProperty _OLBlendDistance = null;
        MaterialProperty _OutlineCube = null;
        MaterialProperty _OutlineCubeAlt = null;
        MaterialProperty _OutlineBackgroundCube = null;
        MaterialProperty _OutlineCubeToggle = null;
        MaterialProperty _OutlineHueShift = null;
        MaterialProperty _OutlineHueShiftAuto = null;
        MaterialProperty _OutlineHueShiftSpeed = null;
        MaterialProperty _CubeRotate = null;
        MaterialProperty _CubeRotateAlt = null;
        MaterialProperty _CubeRotateBG = null;
        MaterialProperty _AutoRotate = null;
        MaterialProperty _AutoRotateAlt = null;
        MaterialProperty _AutoRotateBG = null;
        MaterialProperty _SobelClearInner = null;
        MaterialProperty _RoundingToggle = null;
        MaterialProperty _Rounding = null;
        MaterialProperty _RoundingOpacity = null;
        MaterialProperty _NormalMapFilter = null;
        MaterialProperty _NMFOpacity = null;
        MaterialProperty _NMFToggle = null;
        MaterialProperty _DepthBufferToggle = null;
        MaterialProperty _DBOpacity = null;
        MaterialProperty _DBColor = null;
        MaterialProperty _AuraFade = null;
        MaterialProperty _AuraSampleCount = null;
        MaterialProperty _AuraStr = null;
        MaterialProperty _NoiseUseGlobal = null;
        MaterialProperty _NoiseMinRange = null;
        MaterialProperty _NoiseMaxRange = null;
        MaterialProperty _NoiseFalloffMode = null;
        MaterialProperty _NoiseBoxSize = null;
        MaterialProperty _NoiseBoxOffset = null;
        MaterialProperty _NoiseBlendDistance = null;
        MaterialProperty _DisplayGlobalGizmo = null;
        MaterialProperty _SobelFilterToggle = null;
        MaterialProperty _SobelFilterColor = null;
        MaterialProperty _SobelFilterBackgroundColor = null;
        MaterialProperty _SobelFilterOpacity = null;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        static void DrawFeatureGizmo(MeshRenderer meshRenderer, Material material, string modeProp, string minProp, string maxProp, string boxSizeProp, string boxOffsetProp, string blendDistProp, Color innerCol, Color outerCol){
            Vector3 position = meshRenderer.transform.position;
            if (material.GetInt(modeProp) == 1){
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Transform t = meshRenderer.transform;
                Vector3 boxOffset = material.GetVector(boxOffsetProp);
                Vector3 boxSize = material.GetVector(boxSizeProp);
                float blendDist = material.GetFloat(blendDistProp);
                Vector3 center = position + t.rotation * boxOffset;
                Gizmos.matrix = Matrix4x4.TRS(center, t.rotation, Vector3.one);

                Gizmos.color = outerCol;
                Gizmos.DrawWireCube(Vector3.zero, boxSize);

                Vector3 innerBoxSize = Vector3.Max(Vector3.zero, boxSize - Vector3.one * (blendDist * 2f));
                Gizmos.color = innerCol;
                Gizmos.DrawWireCube(Vector3.zero, innerBoxSize);

                Gizmos.matrix = oldMatrix;
            }
            else {
                Gizmos.color = innerCol;
                Gizmos.DrawWireSphere(position, material.GetFloat(minProp));
                Gizmos.color = outerCol;
                Gizmos.DrawWireSphere(position, material.GetFloat(maxProp));
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmo(MeshRenderer meshRenderer, GizmoType gizmoType){
            if (meshRenderer.sharedMaterial != null){
                Material material = meshRenderer.sharedMaterial;
                if (!material.shader.name.Contains("Mochie/Screen FX")) return;
                if (!foldouts.ContainsKey(material)) return;
                if (material.GetFloat("_DisplayGlobalGizmo") == 0) return;
                Vector3 position = meshRenderer.transform.position;
                Toggles toggles = foldouts[material];
                bool drewGlobal = false;

                if (toggles.GetState("常规")){
                    DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                    drewGlobal = true;
                }
                if (toggles.GetState("滤镜")){
                    if (material.GetFloat("_ColorUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_ColorFalloffMode", "_ColorMinRange", "_ColorMaxRange", "_ColorBoxSize", "_ColorBoxOffset", "_ColorBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("震动")){
                    if (material.GetFloat("_ShakeUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_ShakeFalloffMode", "_ShakeMinRange", "_ShakeMaxRange", "_ShakeBoxSize", "_ShakeBoxOffset", "_ShakeBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("扭曲")){
                    if (material.GetFloat("_DistortionUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_DistortionFalloffMode", "_DistortionMinRange", "_DistortionMaxRange", "_DistortionBoxSize", "_DistortionBoxOffset", "_DistortionBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("模糊")){
                    if (material.GetFloat("_BlurUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_BlurFalloffMode", "_BlurMinRange", "_BlurMaxRange", "_BlurBoxSize", "_BlurBoxOffset", "_BlurBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("噪波")){
                    if (material.GetFloat("_NoiseUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_NoiseFalloffMode", "_NoiseMinRange", "_NoiseMaxRange", "_NoiseBoxSize", "_NoiseBoxOffset", "_NoiseBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("缩放")){
                    if (material.GetFloat("_ZoomUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_ZoomFalloffMode", "_ZoomMinRange", "_ZoomMaxRange", "_ZoomBoxSize", "_ZoomBoxOffset", "_ZoomBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("图像叠加")){
                    if (material.GetFloat("_SSTUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_SSTFalloffMode", "_SSTMinRange", "_SSTMaxRange", "_SSTBoxSize", "_SSTBoxOffset", "_SSTBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("雾")){
                    if (material.GetFloat("_FogUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_FogFalloffMode", "_FogMinRange", "_FogMaxRange", "_FogBoxSize", "_FogBoxOffset", "_FogBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("三平面")){
                    if (material.GetFloat("_TPUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_TPFalloffMode", "_TPMinRange", "_TPMaxRange", "_TPBoxSize", "_TPBoxOffset", "_TPBlendDistance", Color.white, new Color(0.5f, 0.5f, 0.5f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
                if (toggles.GetState("描边")){
                    if (material.GetFloat("_OLUseGlobal") == 0)
                        DrawFeatureGizmo(meshRenderer, material, "_OLFalloffMode", "_OLMinRange", "_OLMaxRange", "_OLBoxSize", "_OLBoxOffset", "_OLBlendDistance", new Color(1f, 0.647f, 0f, 1f), new Color(1f, 0.847f, 0.2f, 1f));
                    else if (!drewGlobal){
                        DrawFeatureGizmo(meshRenderer, material, "_FalloffMode", "_MinRange", "_MaxRange", "_BoxSize", "_BoxOffset", "_BlendDistance", Color.yellow, new Color(0.9f, 0.9f, 0.3f, 1f));
                        drewGlobal = true;
                    }
                }
            }
        }

        void DoFalloffSettings(MaterialEditor me, MaterialProperty useGlobal, MaterialProperty falloffMode, MaterialProperty minRange, MaterialProperty maxRange, MaterialProperty boxSize, MaterialProperty boxOffset, MaterialProperty blendDistance){
            me.ShaderProperty(useGlobal, ugfLabel);
            if (useGlobal.floatValue == 0){
                MGUI.PropertyGroup(()=>{
                    me.ShaderProperty(falloffMode, "形状");
                    if (falloffMode.floatValue == 0){
                        me.ShaderProperty(minRange, minLabel);
                        me.ShaderProperty(maxRange, maxLabel);
                    }
                    else {
                        MGUI.Vector3Field(boxSize, "框尺寸", false);
                        MGUI.Vector3Field(boxOffset, "框偏移", false);
                        me.ShaderProperty(blendDistance, "混合距离");
                    }
                });
            }
            else MGUI.Space6();
        }

        public override void OnGUI(MaterialEditor me, MaterialProperty[] props) {
            Material mat = (Material)me.target;

            if (!me.isVisible)
                return;
            foreach (var property in GetType().GetFields(bindingFlags)){
                if (property.FieldType == typeof(MaterialProperty))
                    property.SetValue(this, FindProperty(property.Name, props));
            }

            bool isSFXX = MGUI.IsXVersion(mat);

            // Add mat to foldout dictionary if it isn't in there yet
            if (!foldouts.ContainsKey(mat))
                foldouts.Add(mat, toggles);

            ApplyMaterialSettings(mat);
            SetBlendMode(mat);

            string headerText = "屏幕特效";
            if (isSFXX) headerText += " X";
            MGUI.DoHeader(headerText);

            EditorGUI.BeginChangeCheck(); {

                // Global
                Foldouts.DoFoldout(foldouts, mat, "常规", 1, Foldouts.Style.StandardButton);
                if (Foldouts.DoFoldoutButton(MGUI.collapseLabel, 11)) Toggles.CollapseFoldouts(mat, foldouts, 1);
                if (Foldouts.BeginFoldout(foldouts, mat, "常规")) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                        me.ShaderProperty(_DisplayGlobalGizmo, "显示范围辅助线");
                        me.RenderQueueField();
                        // me.ShaderProperty(_MirrorRenderMode, "VRC Mirror");
                        me.ShaderProperty(_CameraRenderMode, "VRChat 手持相机");
                        EditorGUI.BeginChangeCheck();
                        me.ShaderProperty(_BlendMode, "混合模式");
                        MGUI.Space4();
                        if (EditorGUI.EndChangeCheck())
                            SetBlendMode(mat);
                        if (_BlendMode.floatValue > 0)
                            me.ShaderProperty(_Opacity, "不透明度");
                        if (_BlendMode.floatValue > 1)
                            MGUI.DisplayInfo("除不透明和 Alpha 之外的混合模式使用全局衰减来淡出整个材质。这是因为这些混合模式默认会更改屏幕值，并且需要根据某些内容进行衰减，即使没有启用任何效果也是如此。");
                        GUILayout.Label("全局衰减", EditorStyles.boldLabel);
                        MGUI.SpaceN2();
                        me.ShaderProperty(_FalloffMode, "形状");
                        if (_FalloffMode.floatValue == 0){
                            me.ShaderProperty(_MinRange, minLabel);
                            me.ShaderProperty(_MaxRange, maxLabel);
                        }
                        else {
                            MGUI.Vector3Field(_BoxSize, "框尺寸", false);
                            MGUI.Vector3Field(_BoxOffset, "框偏移", false);
                            me.ShaderProperty(_BlendDistance, "混合距离");
                        }
                        });
                    });
                }
                Foldouts.EndFoldout();

                // Filtering
                if (Foldouts.DoFoldout(foldouts, mat, me, _FilterModel, "滤镜", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_FilterModel.floatValue == 0);
                        me.ShaderProperty(_FilterStrength, "不透明度");
                        DoFalloffSettings(me, _ColorUseGlobal, _ColorFalloffMode, _ColorMinRange, _ColorMaxRange, _ColorBoxSize, _ColorBoxOffset, _ColorBlendDistance);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Color, "色调");
                            if (_AutoShift.floatValue == 0)
                                me.ShaderProperty(_Hue, "色调");
                            else
                                me.ShaderProperty(_AutoShiftSpeed, "偏移速度");
                            me.ShaderProperty(_HueMode, Tips.hueModeText);
                            me.ShaderProperty(_MonoTint, Tips.monoTintText);
                            me.ShaderProperty(_AutoShift, "自动色调偏移");
                        });
                        MGUI.PropertyGroup(()=>{
                            MGUI.Vector3FieldRGB(_RGB, "RGB 倍率");
                            me.ShaderProperty(_Brightness, "亮度");
                            me.ShaderProperty(_Contrast, "对比度");
                            me.ShaderProperty(_HDR, "HDR");
                            MGUI.ToggleFloat(me, "图像限制", _ClampToggle, _ClampMax);
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Saturation, "饱和度");
                            me.ShaderProperty(_SaturationR, "红色饱和度");
                            me.ShaderProperty(_SaturationG, "绿色饱和度");
                            me.ShaderProperty(_SaturationB, "蓝色饱和度");
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Invert, "反转");
                            me.ShaderProperty(_InvertR, "红色反转");
                            me.ShaderProperty(_InvertG, "绿色反转");
                            me.ShaderProperty(_InvertB, "蓝色反转");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Shake
                if (Foldouts.DoFoldout(foldouts, mat, me, _ShakeModel, "震动", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_ShakeModel.floatValue == 0);
                        DoFalloffSettings(me, _ShakeUseGlobal, _ShakeFalloffMode, _ShakeMinRange, _ShakeMaxRange, _ShakeBoxSize, _ShakeBoxOffset, _ShakeBlendDistance);
                        MGUI.PropertyGroup(()=>{
                            if (_ShakeModel.floatValue < 3){
                                me.ShaderProperty(_Amplitude, "振幅");
                                me.ShaderProperty(_ShakeSpeedX, speedXLabel);
                                me.ShaderProperty(_ShakeSpeedY, speedYLabel);
                            }
                            if (_ShakeModel.floatValue == 3){
                                me.TexturePropertySingleLine(shakeNoiseTexLabel, _ShakeNoiseTex);
                                me.ShaderProperty(_Amplitude, "振幅");
                                me.ShaderProperty(_ShakeSpeedXY, speedLabel);
                            }
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Distortion
                if (Foldouts.DoFoldout(foldouts, mat, me, _DistortionModel, "扭曲", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_DistortionModel.floatValue == 0);
                        DoFalloffSettings(me, _DistortionUseGlobal, _DistortionFalloffMode, _DistortionMinRange, _DistortionMaxRange, _DistortionBoxSize, _DistortionBoxOffset, _DistortionBlendDistance);
                        MGUI.PropertyGroup(()=>{
                            me.TexturePropertySingleLine(normalMapLabel, _NormalMap);
                            me.TextureScaleOffsetProperty(_NormalMap);
                            me.ShaderProperty(_DistortionStr, strengthLabel);
                            me.ShaderProperty(_DistortionSpeed, speedLabel);
                        });
                        if (_DistortionModel.floatValue == 2){
                            MGUI.PropertyGroup(()=>{
                                MGUI.DisplayInfo("此功能需要在以下位置找到“Depth Light”预制件：Assets/Mochie/Unity/Prefabs");
                                me.ShaderProperty(_DistortionRadius, radiusLabel);
                                me.ShaderProperty(_DistortionFade, fadeLabel);
                                me.ShaderProperty(_DistortionP2O, p2oLabel);
                            });
                        }
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Blur
                if (Foldouts.DoFoldout(foldouts, mat, me, _BlurModel, "模糊", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_BlurModel.floatValue == 0);
                        if (_BlurModel.floatValue == 1){
                            me.ShaderProperty(_PixelBlurSamples, "采样数量");
                            if (_PixelBlurSamples.floatValue > 43){
                                MGUI.Space6();
                                MGUI.DisplayWarning("高样本数可能会非常滞后！如果您的强度值较低，请考虑保持在 43 个样本或以下。");
                            }
                        }
                        DoFalloffSettings(me, _BlurUseGlobal, _BlurFalloffMode, _BlurMinRange, _BlurMaxRange, _BlurBoxSize, _BlurBoxOffset, _BlurBlendDistance);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_BlurOpacity, "不透明度");
                            me.ShaderProperty(_BlurStr, strengthLabel);
                            if (_BlurModel.floatValue == 3){
                                me.ShaderProperty(_BlurSamples, "采样数量");
                                me.ShaderProperty(_BlurRadius, radiusLabel);
                            }
                            me.ShaderProperty(_PixelationStr, "像素化");
                            me.ShaderProperty(_RippleGridStr, "涟漪网格");
                            if (_BlurModel.floatValue == 3){
                                me.ShaderProperty(_DoF, "景深");
                                if (_DoF.floatValue == 1){
                                    MGUI.PropertyGroup(()=>{
                                        MGUI.DisplayInfo("此功能需要在以下位置找到“Depth Light”预制件：Assets/Mochie/Unity/Prefabs");
                                        me.ShaderProperty(_DoFRadius, radiusLabel);
                                        me.ShaderProperty(_DoFFade, fadeLabel);
                                        me.ShaderProperty(_DoFP2O, p2oLabel);
                                    });
                                }
                            }
                        });
                        MGUI.PropertyGroup(_BlurModel.floatValue != 3, ()=>{
                            if (_BlurModel.floatValue != 3){
                                me.ShaderProperty(_DoF, "景深");
                                if (_DoF.floatValue == 1){
                                    MGUI.PropertyGroup(()=>{
                                        MGUI.DisplayInfo("此功能需要在以下位置找到“Depth Light”预制件：Assets/Mochie/Unity/Prefabs");
                                        me.ShaderProperty(_DoFRadius, radiusLabel);
                                        me.ShaderProperty(_DoFFade, fadeLabel);
                                        me.ShaderProperty(_DoFP2O, p2oLabel);
                                    });
                                }
                                me.ShaderProperty(_BlurY, "仅 Y 轴");
                                me.ShaderProperty(_RGBSplit, "色差");
                            }
                            if (_BlurModel.floatValue == 1)
                                me.ShaderProperty(_CrushBlur, "压缩");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Noise
                if (Foldouts.DoFoldout(foldouts, mat, me, _NoiseMode, "噪波", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_NoiseMode.floatValue == 0);
                        me.ShaderProperty(_NoiseStrength, "不透明度");
                        MGUI.Space6();
                        DoFalloffSettings(me, _NoiseUseGlobal, _NoiseFalloffMode, _NoiseMinRange, _NoiseMaxRange, _NoiseBoxSize, _NoiseBoxOffset, _NoiseBlendDistance);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Noise, "噪波（灰度）");
                            MGUI.Vector3FieldRGB(_NoiseRGB, "噪波（RGB）");
                            me.ShaderProperty(_ScanLine, "扫描线");
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_ScanLineThick, "条形厚度");
                                me.ShaderProperty(_ScanLineSpeed, "滚动速度");
                            });
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                if (isSFXX){
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Zoom, "缩放", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Zoom.floatValue == 0);
                            DoFalloffSettings(me, _ZoomUseGlobal, _ZoomFalloffMode, _ZoomMinRange, _ZoomMaxRange, _ZoomBoxSize, _ZoomBoxOffset, _ZoomBlendDistance);
                            MGUI.PropertyGroup(()=>{
                                if (_Zoom.floatValue == 2){
                                    me.ShaderProperty(_ZoomStrR, "红");
                                    me.ShaderProperty(_ZoomStrG, "绿");
                                    me.ShaderProperty(_ZoomStrB, "蓝");
                                }
                                else me.ShaderProperty(_ZoomStr, strengthLabel);
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Screenspace Texture Overlay
                    if (Foldouts.DoFoldout(foldouts, mat, me, _SST, "图像叠加", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_SST.floatValue == 0);
                            DoFalloffSettings(me, _SSTUseGlobal, _SSTFalloffMode, _SSTMinRange, _SSTMaxRange, _SSTBoxSize, _SSTBoxOffset, _SSTBlendDistance);
                            MGUI.PropertyGroup(()=>{
                                if (_SST.floatValue != 3){
                                    me.TexturePropertySingleLine(screenTexLabel, _ScreenTex, _SSTColor, _SSTBlend);
                                }
                                else {
                                    me.TexturePropertySingleLine(normalMapLabel, _ScreenTex);
                                    me.ShaderProperty(_SSTAnimatedDist, "强度");
                                }
                                if (_SST.floatValue > 1){
                                    me.ShaderProperty(_SSTColumnsX, "列（X）");
                                    me.ShaderProperty(_SSTRowsY, "行（Y）");
                                    MGUI.CustomToggleSlider("帧", _ManualScrub, _ScrubPos, 0f, (_SSTColumnsX.floatValue*_SSTRowsY.floatValue)-1);
                                    MGUI.ToggleGroup(_ManualScrub.floatValue == 1);
                                    me.ShaderProperty(_SSTAnimationSpeed, "FPS");
                                    MGUI.ToggleGroupEnd();
                                }
                            });
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_SSTScale, "缩放");
                                me.ShaderProperty(_SSTWidth, "宽度");
                                me.ShaderProperty(_SSTHeight, "高度");
                                me.ShaderProperty(_SSTLR, "左 / 右");
                                me.ShaderProperty(_SSTUD, "下 / 上");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Triplanar Mapping
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Triplanar, "三平面", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Triplanar.floatValue == 0);
                            DoFalloffSettings(me, _TPUseGlobal, _TPFalloffMode, _TPMinRange, _TPMaxRange, _TPBoxSize, _TPBoxOffset, _TPBlendDistance);
                            MGUI.PropertyGroup(()=>{
                                MGUI.DisplayInfo("此功能需要在以下位置找到“Depth Light”预制件：Assets/Mochie/Unity/Prefabs");
                                me.TexturePropertySingleLine(tpTexLabel, _TPTexture, _TPColor);
                                if (_TPTexture.textureValue){
                                    MGUI.TextureSO(me, _TPTexture);
                                    MGUI.SpaceN2();
                                    MGUI.Vector3Field(_TPScroll, "滚动速度", false);
                                    MGUI.Space4();
                                }
                                me.TexturePropertySingleLine(tpNoiseTexLabel, _TPNoiseTex);
                                if (_TPNoiseTex.textureValue){
                                    MGUI.TextureSO(me, _TPNoiseTex);
                                    MGUI.SpaceN2();
                                    MGUI.Vector3Field(_TPNoiseScroll, "滚动速度", false);
                                }
                            });
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_TPRadius, radiusLabel);
                                if (_Triplanar.floatValue == 2){
                                    me.ShaderProperty(_TPScanFade, fadeLabel);
                                    me.ShaderProperty(_TPThickness, "厚度");
                                    me.ShaderProperty(_TPNoise, "噪波");
                                }
                                else me.ShaderProperty(_TPFade, fadeLabel);
                                me.ShaderProperty(_TPP2O, p2oLabel);
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Outline
                    if (Foldouts.DoFoldout(foldouts, mat, me, _OutlineType, "描边", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_OutlineType.floatValue == 0);
                                me.ShaderProperty(_OutlineCubeToggle, "颜色来源");
                                DoFalloffSettings(me, _OLUseGlobal, _OLFalloffMode, _OLMinRange, _OLMaxRange, _OLBoxSize, _OLBoxOffset, _OLBlendDistance);
                                MGUI.DisplayInfo("此功能需要场景中存在“Depth Light”预制件。\n（发现于：Assets/Mochie/Unity/Prefabs）");
                                if (_OutlineType.floatValue == 1){
                                    MGUI.PropertyGroup(()=>{
                                        me.ShaderProperty(_OutlineAltEnable, "启用混合色调");
                                        me.ShaderProperty(_SobelClearInner, "移除内部着色");
                                    });
                                }
                                MGUI.PropertyGroup(()=>{
                                if (_OutlineType.floatValue == 2)
                                    me.ShaderProperty(_AuraSampleCount, "采样数量");
                                me.ShaderProperty(_OutlineHueShiftAuto, "自动色调偏移");
                                if (_OutlineHueShiftAuto.floatValue == 1){
                                    me.ShaderProperty(_OutlineHueShiftSpeed, "偏移速度");
                                }
                                else {
                                    me.ShaderProperty(_OutlineHueShift, "色调");
                                }
                                if (_OutlineCubeToggle.floatValue == 0){
                                    me.TexturePropertySingleLine(new GUIContent("基线色调"), _OutlineTex, _OutlineCol);
                                    if (_OutlineTex.textureValue)
                                        MGUI.Vector2Field(_OutlineTexCoord, "坐标");
                                }
                                else {
                                    me.TexturePropertySingleLine(new GUIContent("基础立方体贴图"), _OutlineCube, _OutlineCol);
                                    if (_OutlineCube.textureValue){
                                        MGUI.PropertyGroup(()=>{
                                            MGUI.SpaceN2();
                                            MGUI.Vector3Field(_CubeRotate, _AutoRotate.floatValue == 1 ? "速度" : "旋转", false);
                                            me.ShaderProperty(_AutoRotate, "自动旋转");
                                            MGUI.SpaceN2();
                                        });
                                        MGUI.Space6();
                                    }
                                }
                                if (_OutlineType.floatValue == 1 && _OutlineAltEnable.floatValue == 1){
                                    if (_OutlineCubeToggle.floatValue == 0){
                                        me.TexturePropertySingleLine(new GUIContent("混合线色调"), _OutlineTexAlt, _OutlineColAlt);
                                        if (_OutlineTexAlt.textureValue)
                                            MGUI.Vector2Field(_OutlineTexCoordAlt, "坐标");
                                    }
                                    else {
                                        me.TexturePropertySingleLine(new GUIContent("混合立方体贴图"), _OutlineCubeAlt, _OutlineColAlt);
                                        if (_OutlineCubeAlt.textureValue){
                                            MGUI.PropertyGroup(()=>{
                                                MGUI.SpaceN2();
                                                MGUI.Vector3Field(_CubeRotateAlt, _AutoRotateAlt.floatValue == 1 ? "速度" : "旋转", false);
                                                me.ShaderProperty(_AutoRotateAlt, "自动旋转");
                                                MGUI.SpaceN2();
                                            });
                                            MGUI.Space6();
                                        }
                                    }
                                }
                                if (_OutlineCubeToggle.floatValue == 0){
                                    me.ShaderProperty(_BackgroundCol, "背景色调");
                                }
                                else {
                                    me.TexturePropertySingleLine(new GUIContent("背景立方体贴图"), _OutlineBackgroundCube, _BackgroundCol);
                                    if (_OutlineBackgroundCube.textureValue){
                                        MGUI.PropertyGroup(()=>{
                                            MGUI.SpaceN2();
                                            MGUI.Vector3Field(_CubeRotateBG, _AutoRotateBG.floatValue == 1 ? "速度" : "旋转", false);
                                            me.ShaderProperty(_AutoRotateBG, "自动旋转");
                                            MGUI.SpaceN2();
                                        });
                                        MGUI.Space6();
                                    }
                                }
                                if (_OutlineType.floatValue == 1){
                                    me.ShaderProperty(_OutlineThiccS, "厚度");
                                    me.ShaderProperty(_OutlineThresh, "基础强度");
                                    if (_OutlineAltEnable.floatValue == 1){
                                        me.ShaderProperty(_OutlineThreshAlt, "混合强度");
                                    }
                                }
                                else if (_OutlineType.floatValue == 2){
                                    me.ShaderProperty(_AuraStr, "厚度");
                                    me.ShaderProperty(_AuraFade, "渐隐");
                                }
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Fog
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Fog, "雾", Foldouts.Style.StandardToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Fog.floatValue == 0);
                            DoFalloffSettings(me, _FogUseGlobal, _FogFalloffMode, _FogMinRange, _FogMaxRange, _FogBoxSize, _FogBoxOffset, _FogBlendDistance);
                            MGUI.PropertyGroup(()=>{
                                MGUI.DisplayInfo("此功能需要在以下位置找到“Depth Light”预制件：Assets/Mochie/Unity/Prefabs");
                                me.ShaderProperty(_FogColor, colorLabel);
                                me.ShaderProperty(_FogRadius, radiusLabel);
                                me.ShaderProperty(_FogFade, fadeLabel);
                                me.ShaderProperty(_FogP2O, p2oLabel);
                                me.ShaderProperty(_FogSafeZone, "安全区");
                                if (_FogSafeZone.floatValue == 1){
                                    MGUI.PropertyGroup(()=>{
                                        me.ShaderProperty(_FogSafeRadius, "视野半径");
                                        me.ShaderProperty(_FogSafeMaxRange, "外边界");
                                        me.ShaderProperty(_FogSafeOpacity, "不透明度");
                                    });
                                }
                                // me.ShaderProperty(_HeightFalloff, "Height Falloff");
                                // if (_HeightFalloff.floatValue == 1){
                                // 	MGUI.PropertyGroup(()=>{
                                // 		me.ShaderProperty(_FogHeightMin, "Min Height");
                                // 		me.ShaderProperty(_FogHeightMax, "Max Height");
                                // 	});
                                // }
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Misc
                    if (Foldouts.DoFoldout(foldouts, mat, "杂项", Foldouts.Style.Standard)) {
                        MGUI.Space6();
                        if (Foldouts.DoFoldout(foldouts, mat, me, _Letterbox, "黑边", Foldouts.Style.ThinToggle)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_Letterbox.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_UseZoomFalloff, "使用缩放衰减");
                                    me.ShaderProperty(_LetterboxStr, "条形宽度");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, me, _Pulse, "脉冲", Foldouts.Style.ThinToggle)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_Pulse.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_WaveForm, "波形");
                                    MGUI.ToggleGroup(_WaveForm.floatValue == 0);
                                    me.ShaderProperty(_PulseColor, "包含滤镜");
                                    me.ShaderProperty(_PulseSpeed, speedLabel);
                                    MGUI.ToggleGroupEnd();
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, me, _Shift, "UV 操作", Foldouts.Style.ThinToggle)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_Shift.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_InvertX, "反转 X");
                                    me.ShaderProperty(_InvertY, "反转 Y");
                                    me.ShaderProperty(_ShiftX, "偏移 X");
                                    me.ShaderProperty(_ShiftY, "偏移 Y");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, me, _RoundingToggle, "取整", Foldouts.Style.ThinToggle)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_RoundingToggle.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_RoundingOpacity, "不透明度");
                                    me.ShaderProperty(_Rounding, "精度");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, me, _NMFToggle, "法线贴图滤镜", Foldouts.Style.ThinToggle)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_NMFToggle.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_NMFOpacity, "不透明度");
                                    me.ShaderProperty(_NormalMapFilter, "强度");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, me, _DepthBufferToggle, "深度缓冲", Foldouts.Style.ThinToggle)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_DepthBufferToggle.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_DBOpacity, "不透明度");
                                    me.ShaderProperty(_DBColor, "色调");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                            MGUI.SpaceN4();
                            MGUI.DisplayInfo("此功能需要在以下位置找到“Depth Light”预制件：Assets/Mochie/Unity/Prefabs");
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, me, _SobelFilterToggle, "Sobel 滤镜", Foldouts.Style.ThinToggle)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.ToggleGroup(_SobelFilterToggle.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_SobelFilterColor, "线条颜色");
                                    me.ShaderProperty(_SobelFilterBackgroundColor, "背景颜色");
                                    me.ShaderProperty(_SobelFilterOpacity, "不透明度");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();
                        MGUI.Space6();
                    }
                    Foldouts.EndFoldout();
                }

                // Audio Link
                if (Foldouts.DoFoldout(foldouts, mat, me, _AudioLinkToggle, "Audio Link", Foldouts.Style.StandardToggle)) {
                    MGUI.ToggleGroup(_AudioLinkToggle.floatValue == 0);
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_AudioLinkStrength, "强度");
                            MGUI.SliderMinMax(_AudioLinkMin, _AudioLinkMax, 0f, 2f, "重映射", 1);
                        });
                    });

                    if (Foldouts.DoFoldout(foldouts, mat, "滤镜 ", Foldouts.Style.Thin)){
                        MGUI.PropertyGroupParent(()=>{
                            me.ShaderProperty(_AudioLinkFilteringBand, "色带");
                            me.ShaderProperty(_AudioLinkFilteringStrength, "强度");
                            MGUI.SliderMinMax(_AudioLinkFilteringMin, _AudioLinkFilteringMax, 0f, 2f, "重映射", 1);
                        });
                    }
                    Foldouts.EndFoldout();

                    if (Foldouts.DoFoldout(foldouts, mat, "震动 ", Foldouts.Style.Thin)){
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_AudioLinkShakeBand, "色带");
                                me.ShaderProperty(_AudioLinkShakeStrength, "强度");
                                MGUI.SliderMinMax(_AudioLinkShakeMin, _AudioLinkShakeMax, 0f, 2f, "重映射", 1);
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    if (Foldouts.DoFoldout(foldouts, mat, "扭曲 ", Foldouts.Style.Thin)){
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_AudioLinkDistortionBand, "色带");
                                me.ShaderProperty(_AudioLinkDistortionStrength, "强度");
                                MGUI.SliderMinMax(_AudioLinkDistortionMin, _AudioLinkDistortionMax, 0f, 2f, "重映射", 1);
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    if (Foldouts.DoFoldout(foldouts, mat, "模糊 ", Foldouts.Style.Thin)){
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_AudioLinkBlurBand, "色带");
                                me.ShaderProperty(_AudioLinkBlurStrength, "强度");
                                MGUI.SliderMinMax(_AudioLinkBlurMin, _AudioLinkBlurMax, 0f, 2f, "重映射", 1);
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    if (Foldouts.DoFoldout(foldouts, mat, "噪波 ", Foldouts.Style.Thin)){
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_AudioLinkNoiseBand, "色带");
                                me.ShaderProperty(_AudioLinkNoiseStrength, "强度");
                                MGUI.SliderMinMax(_AudioLinkNoiseMin, _AudioLinkNoiseMax, 0f, 2f, "重映射", 1);
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    if (isSFXX){
                        if (Foldouts.DoFoldout(foldouts, mat, "缩放 ", Foldouts.Style.Thin)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkZoomBand, "色带");
                                    me.ShaderProperty(_AudioLinkZoomStrength, "强度");
                                    MGUI.SliderMinMax(_AudioLinkZoomMin, _AudioLinkZoomMax, 0f, 2f, "重映射", 1);
                                });
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, "图像叠加 ", Foldouts.Style.Thin)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkSSTBand, "色带");
                                    me.ShaderProperty(_AudioLinkSSTStrength, "强度");
                                    MGUI.SliderMinMax(_AudioLinkSSTMin, _AudioLinkSSTMax, 0f, 2f, "重映射", 1);
                                });
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, "雾 ", Foldouts.Style.Thin)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkFogBand, "色带");
                                    me.ShaderProperty(_AudioLinkFogOpacity, "不透明度");
                                    me.ShaderProperty(_AudioLinkFogRadius, "半径");
                                    MGUI.SliderMinMax(_AudioLinkFogMin, _AudioLinkFogMax, 0f, 2f, "重映射", 1);
                                });
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, "三平面 ", Foldouts.Style.Thin)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkTriplanarBand, "色带");
                                    me.ShaderProperty(_AudioLinkTriplanarOpacity, "不透明度");
                                    me.ShaderProperty(_AudioLinkTriplanarRadius, "半径");
                                    MGUI.SliderMinMax(_AudioLinkTriplanarMin, _AudioLinkTriplanarMax, 0f, 2f, "重映射", 1);
                                });
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, "描边 ", Foldouts.Style.Thin)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.BoldLabel("厚度");
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkOutlineBand, "色带");
                                    me.ShaderProperty(_AudioLinkOutlineStrength, "强度");
                                    MGUI.SliderMinMax(_AudioLinkOutlineMin, _AudioLinkOutlineMax, 0f, 2f, "重映射", 1);
                                });
                                MGUI.BoldLabel("基线不透明度");
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkOutlineColBand, "色带");
                                    me.ShaderProperty(_AudioLinkOutlineColStrength, "强度");
                                    MGUI.SliderMinMax(_AudioLinkOutlineColMin, _AudioLinkOutlineColMax, 0f, 2f, "重映射", 1);
                                });
                                MGUI.BoldLabel("混合线不透明度");
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkOutlineMixBand, "色带");
                                    me.ShaderProperty(_AudioLinkOutlineMixStrength, "强度");
                                    MGUI.SliderMinMax(_AudioLinkOutlineMixMin, _AudioLinkOutlineMixMax, 0f, 2f, "重映射", 1);
                                });
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, "杂项 ", Foldouts.Style.Thin)){
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_AudioLinkMiscBand, "色带");
                                    me.ShaderProperty(_AudioLinkMiscStrength, "强度");
                                    MGUI.SliderMinMax(_AudioLinkMiscMin, _AudioLinkMiscMax, 0f, 2f, "重映射", 1);
                                });
                            });
                        }
                        Foldouts.EndFoldout();
                    }
                    MGUI.ToggleGroupEnd();
                }
                Foldouts.EndFoldout();
            }
            MGUI.DoFooter(versionLabel, "ScreenFX-Shader");
        }

        void ApplyMaterialSettings(Material mat){
            int zoomMode = mat.GetInt("_Zoom");
            int sstMode = mat.GetInt("_SST");
            int letterboxMode = mat.GetInt("_Letterbox");

            bool isXVersion = MGUI.IsXVersion(mat);
            bool zoomEnabled = zoomMode == 1 && isXVersion;
            bool zoomRGBEnabled = zoomMode == 2 && isXVersion;
            bool sstEnabled = sstMode < 3 && sstMode > 0 && isXVersion;
            bool sstDistEnabled = sstMode == 3 && isXVersion;
            bool letterboxEnabled = letterboxMode > 0 && isXVersion;

            MGUI.SetKeyword(mat, "_COLOR_ON", mat.GetInt("_FilterModel") > 0);
            MGUI.SetKeyword(mat, "_SHAKE_ON", mat.GetInt("_ShakeModel") > 0);
            MGUI.SetKeyword(mat, "_DISTORTION_ON", mat.GetInt("_DistortionModel") == 1);
            MGUI.SetKeyword(mat, "_DISTORTION_WORLD_ON", mat.GetInt("_DistortionModel") == 2);
            MGUI.SetKeyword(mat, "_BLUR_PIXEL_ON", mat.GetInt("_BlurModel") == 1);
            MGUI.SetKeyword(mat, "_BLUR_DITHER_ON", mat.GetInt("_BlurModel") == 2);
            MGUI.SetKeyword(mat, "_BLUR_RADIAL_ON", mat.GetInt("_BlurModel") == 3);
            MGUI.SetKeyword(mat, "_BLUR_Y_ON", mat.GetInt("_BlurY") == 1);
            MGUI.SetKeyword(mat, "_CHROMATIC_ABBERATION_ON", mat.GetInt("_RGBSplit") == 1);
            MGUI.SetKeyword(mat, "_DOF_ON", mat.GetInt("_DoF") == 1);
            MGUI.SetKeyword(mat, "_ZOOM_ON", zoomMode == 1 && isXVersion);
            MGUI.SetKeyword(mat, "_ZOOM_RGB_ON", zoomMode == 2 && isXVersion);
            MGUI.SetKeyword(mat, "_IMAGE_OVERLAY_ON", sstMode < 3 && sstMode > 0 && isXVersion);
            MGUI.SetKeyword(mat, "_IMAGE_OVERLAY_DISTORTION_ON", sstMode == 3 && isXVersion);
            MGUI.SetKeyword(mat, "_FOG_ON", mat.GetInt("_Fog") == 1 && isXVersion);
            MGUI.SetKeyword(mat, "_TRIPLANAR_ON", mat.GetInt("_Triplanar") > 0 && isXVersion);
            MGUI.SetKeyword(mat, "_OUTLINE_ON", mat.GetInt("_OutlineType") > 0 && isXVersion);
            MGUI.SetKeyword(mat, "_NOISE_ON", mat.GetInt("_NoiseMode") == 1);
            MGUI.SetKeyword(mat, "_AUDIOLINK_ON", mat.GetInt("_AudioLinkToggle") == 1);
            MGUI.SetKeyword(mat, "_SOBEL_FILTER_ON", mat.GetInt("_SobelFilterToggle") == 1);

            mat.SetShaderPassEnabled("Always", zoomEnabled || zoomRGBEnabled || sstEnabled || sstDistEnabled ||  letterboxEnabled);
        }

        public static void SetBlendMode(Material mat) {
            int mode = mat.GetInt("_BlendMode");
            switch (mode) {
                case 0: // Opaque:
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case 1: // Alpha:
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
                case 2: // Premultiplied:
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
                case 3: // Additive:
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case 4: // Soft_Additive:
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case 5: // Multiply:
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case 6: // Multiply_2x:
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcColor);
                    break;
            }
        }
    }
}