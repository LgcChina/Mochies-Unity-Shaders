using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mochie {

    class GradientObject : ScriptableObject {
        public Gradient gradient = new Gradient();
    }

    internal class UberEditor : ShaderGUI {

        static Dictionary<Material, Toggles> foldouts = new Dictionary<Material, Toggles>();
        Toggles toggles = new Toggles(new string[] {
                "基础",
                "贴图",
                "着色",
                "遮罩",
                "光照",
                "阴影",
                "反射",
                "高光",
                "次表面散射",
                "Matcap",
                "基础边缘光",
                "粗糙度滤镜",
                "法线",
                "自发光",
                "脉冲",
                "光照响应",
                "滤镜",
                "UV 扭曲",
                "描边",
                "特殊功能",
                "距离渐隐",
                "溶解",
                "屏幕空间",
                "克隆",
                "克隆 ",
                "故障效果",
                "破碎剔除",
                "线框",
                "序列帧",
                "环境光遮蔽滤镜",
                "高度滤镜",
                "主层",
                "次级层",
                "环境边缘光",
                "平滑度滤镜",
                "金属度滤镜",
                "渲染设置",
                "曲率滤镜",
                "主贴图",
                "细节贴图",
                "主 Matcap",
                "次级 Matcap",
                "常规",
                "漫反射着色",
                "实时光照",
                "烘焙光照",
                "遮罩 ",
                "折射",
                "顶点操作",
                "折射遮罩",
                "反射遮罩",
                "高光遮罩",
                "高光混合遮罩",
                "环境边缘光遮罩",
                "Matcap 遮罩",
                "Matcap 混合遮罩",
                "各向异性混合遮罩",
                "次表面遮罩",
                "边缘光遮罩",
                "细节遮罩",
                "阴影遮罩",
                "漫反射遮罩",
                "滤镜遮罩",
                "颜色遮罩",
                "自发光遮罩",
                "自发光脉冲遮罩",
                "描边厚度遮罩",
                "基础颜色溶解",
                "Audio Link",
                "自发光 ",
                "边缘光",
                "溶解 ",
                "基础颜色溶解 ",
                "顶点操作 ",
                "三角形偏移",
                "线框 ",
                "虹彩",
                "UV 扭曲 ",
                "描边 ",
                // "Visualizers",
                "示波器",
                "主 Matcap",
                "次级 Matcap",
                "光照体积",
                "调试"
        }, 0);

        private GradientObject gradientObj;
        private SerializedProperty colorGradient;
        private SerializedObject serializedGradient;
        private EditorWindow gradientWindow;
        private Texture2D rampTex;

        private GradientObject iridescenceGradientObj;
        private SerializedProperty iridescenceColorGradient;
        private SerializedObject iridescenceSerializedGradient;
        private EditorWindow iridescenceGradientWindow;
        private Texture2D iridescenceRampTex;

        static readonly int blendingLabelPos = 111;

        static readonly string rampsFolderPath = "Assets/LGC/MochieShaders/Textures/Ramps";
        string versionLabel = "v1.35.2 | LGC 中文版 1.78.2";
        // β

        static void EnsureAssetFolder(string folderPath){
            string[] parts = folderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++){
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        MaterialProperty _RenderMode = null;
        MaterialProperty _CullingMode = null;
        MaterialProperty _BlendMode = null;
        MaterialProperty _Cutoff = null;
        MaterialProperty _DistanceFadeToggle = null;
        MaterialProperty _DistanceFadeMax = null;
        MaterialProperty _DistanceFadeMin = null;
        MaterialProperty _ClipRimColor = null;
        MaterialProperty _ClipRimWidth = null;
        MaterialProperty _ClipRimStr = null;
        MaterialProperty _Color = null;
        MaterialProperty _MainTex = null;
        MaterialProperty _MirrorTex = null;
        MaterialProperty _MainTexScroll = null;
        MaterialProperty _DetailScroll = null;
        MaterialProperty _RimScroll = null;
        MaterialProperty _MirrorBehavior = null;
        MaterialProperty _FilterMask = null;
        MaterialProperty _Saturation = null;
        MaterialProperty _RealtimeSpec = null;
        MaterialProperty _Contrast = null;
        MaterialProperty _RGB = null;
        MaterialProperty _Hue = null;
        MaterialProperty _Filtering= null;
        MaterialProperty _TeamFiltering = null;
        MaterialProperty _AutoShift = null;
        MaterialProperty _AutoShiftSpeed = null;
        MaterialProperty _Brightness = null;
        MaterialProperty _PostFiltering = null;
        MaterialProperty _HDR = null;
        MaterialProperty _Invert = null;
        MaterialProperty _DistortUVMap = null;
        MaterialProperty _DistortUVStr = null;
        MaterialProperty _DistortUVScroll = null;
        MaterialProperty _DistortUVMask = null;
        MaterialProperty _DistortMainUV = null;
        MaterialProperty _DistortEmissUV = null;
        MaterialProperty _DistortDetailUV = null;
        MaterialProperty _DistortRimUV = null;
        MaterialProperty _MainTexCube0 = null;
        MaterialProperty _CubeMode = null;
        MaterialProperty _CubeBlend = null;
        MaterialProperty _CubeRotate0 = null;
        MaterialProperty _AutoRotate0 = null;
        MaterialProperty _CubeColor0 = null;
        MaterialProperty _CubeBlendMode = null;
        MaterialProperty _CubeBlendMask = null;
        MaterialProperty _UnlitMatcap = null;
        MaterialProperty _Spritesheet = null;
        MaterialProperty _SpritesheetPos = null;
        MaterialProperty _SpritesheetScale = null;
        MaterialProperty _SpritesheetRot = null;
        MaterialProperty _RowsColumns = null;
        MaterialProperty _ManualScrub = null;
        MaterialProperty _ScrubPos = null;
        MaterialProperty _FPS = null;
        MaterialProperty _EnableSpritesheet = null;
        MaterialProperty _SpritesheetCol = null;
        MaterialProperty _FrameClipOfs = null;
        MaterialProperty _UnlitSpritesheet = null;
        MaterialProperty _UnlitSpritesheet1 = null;
        MaterialProperty _SpritesheetBlending = null;
        MaterialProperty _Spritesheet1 = null;
        MaterialProperty _SpritesheetPos1 = null;
        MaterialProperty _SpritesheetScale1 = null;
        MaterialProperty _SpritesheetRot1 = null;
        MaterialProperty _RowsColumns1 = null;
        MaterialProperty _ManualScrub1 = null;
        MaterialProperty _IndirectAO = null;
        MaterialProperty _ScrubPos1 = null;
        MaterialProperty _FPS1 = null;
        MaterialProperty _EnableSpritesheet1 = null;
        MaterialProperty _SpritesheetCol1 = null;
        MaterialProperty _FrameClipOfs1 = null;
        MaterialProperty _SpritesheetBlending1 = null;
        MaterialProperty _UseAlphaMask = null;
        MaterialProperty _AlphaMask = null;
        MaterialProperty _AOFiltering = null;
        MaterialProperty _AOLightness = null;
        MaterialProperty _AOIntensity = null;
        MaterialProperty _AOContrast = null;
        MaterialProperty _HeightFiltering = null;
        MaterialProperty _HeightLightness = null;
        MaterialProperty _HeightIntensity = null;
        MaterialProperty _HeightContrast = null;
        MaterialProperty _AOTint = null;
        MaterialProperty _AOTintTex = null;
        MaterialProperty _ShadowMode = null;
        MaterialProperty _ShadowTint = null;
        MaterialProperty _SmoothnessMap = null;
        MaterialProperty _PreviewSmooth = null;
        MaterialProperty _SmoothLightness = null;
        MaterialProperty _SmoothIntensity = null;
        MaterialProperty _SmoothContrast = null;
        MaterialProperty _SmoothnessFiltering = null;
        MaterialProperty _SharpSpecStr = null;
        MaterialProperty _ShadowConditions = null;
        MaterialProperty _DirectAO = null;
        MaterialProperty _DistortionStyle = null;
        MaterialProperty _NoiseScale = null;
        MaterialProperty _NoiseSpeed = null;
        MaterialProperty _NoiseOctaves = null;
        MaterialProperty _PBRWorkflow = null;
        MaterialProperty _StaticLightDirToggle = null;
        MaterialProperty _StaticLightDir = null;
        MaterialProperty _BumpScale = null;
        MaterialProperty _BumpMap = null;
        MaterialProperty _Metallic = null;
        MaterialProperty _MetallicGlossMap = null;
        MaterialProperty _Glossiness = null;
        MaterialProperty _GlossMapScale = null;
        MaterialProperty _SpecCol = null;
        MaterialProperty _SpecGlossMap = null;
        MaterialProperty _OcclusionStrength = null;
        MaterialProperty _OcclusionMap = null;
        MaterialProperty _Parallax = null;
        MaterialProperty _ParallaxMap = null;
        MaterialProperty _ReflCube = null;
        MaterialProperty _DetailMask = null;
        MaterialProperty _DetailAlbedoMap = null;
        MaterialProperty _DetailNormalMapScale = null;
        MaterialProperty _DetailNormalMap = null;
        MaterialProperty _Specular = null;
        MaterialProperty _Reflections = null;
        MaterialProperty _MatcapToggle = null;
        MaterialProperty _Matcap = null;
        MaterialProperty _MatcapStr = null;
        MaterialProperty _MatcapColor = null;
        MaterialProperty _MatcapBlending = null;
        MaterialProperty _MatcapBlending1 = null;
        MaterialProperty _Matcap1 = null;
        MaterialProperty _MatcapColor1 = null;
        MaterialProperty _MatcapStr1 = null;
        MaterialProperty _MatcapBlendMask = null;
        MaterialProperty _DiffuseMask = null;
        MaterialProperty _PackedMap = null;
        MaterialProperty _MetallicChannel = null;
        MaterialProperty _RoughnessChannel = null;
        MaterialProperty _OcclusionChannel = null;
        MaterialProperty _RoughnessFiltering = null;
        MaterialProperty _RoughContrast = null;
        MaterialProperty _RoughLightness = null;
        MaterialProperty _RoughIntensity = null;
        MaterialProperty _MetallicFiltering = null;
        MaterialProperty _MetallicContrast = null;
        MaterialProperty _MetallicLightness = null;
        MaterialProperty _MetallicIntensity = null;
        MaterialProperty _MetallicRemapMin = null;
        MaterialProperty _MetallicRemapMax = null;
        MaterialProperty _PreviewMetallic = null;
        MaterialProperty _VLightCont = null;
        MaterialProperty _SubsurfaceMask = null;
        MaterialProperty _UnlitMatcap1 = null;
        MaterialProperty _Alpha = null;
        MaterialProperty _MaxSteps = null;
        MaterialProperty _Step = null;
        MaterialProperty _LRad = null;
        MaterialProperty _SRad = null;
        MaterialProperty _EdgeFade = null;
        MaterialProperty _SSR = null;
        MaterialProperty _AttenSmoothing = null;
        MaterialProperty _ShadowDithering = null;
        MaterialProperty _MatcapMask = null;
        MaterialProperty _PreviewAO = null;
        MaterialProperty _PreviewRough = null;
        MaterialProperty _PreviewHeight = null;
        MaterialProperty _ZWrite = null;
        MaterialProperty _RippleFrequency = null;
        MaterialProperty _RippleAmplitude = null;
        MaterialProperty _RippleStrength = null;
        MaterialProperty _RippleContinuity = null;
        MaterialProperty _ReflStepping = null;
        MaterialProperty _ReflSteps = null;
        MaterialProperty _PackedMask2 = null;
        MaterialProperty _AnisoStr = null;
        MaterialProperty _MatcapUseRough1 = null;
        MaterialProperty _MatcapRough1 = null;
        MaterialProperty _StencilToggle = null;
        MaterialProperty _StencilRef = null;
        MaterialProperty _StencilPass = null;
        MaterialProperty _StencilFail = null;
        MaterialProperty _StencilZFail = null;
        MaterialProperty _StencilCompare = null;
        MaterialProperty _OutlineStencilPass = null;
        MaterialProperty _OutlineStencilCompare = null;
        MaterialProperty _ZTest = null;
        MaterialProperty _NonlinearSHToggle = null;
        MaterialProperty _SHStr = null;
        MaterialProperty _ColorPreservation = null;
        MaterialProperty _ClearCoat = null;
        MaterialProperty _ReflectionMask = null;
        MaterialProperty _ReflectionStr = null;
        MaterialProperty _DisneyDiffuse = null;
        MaterialProperty _SharpSpecular = null;
        MaterialProperty _SpecularMask = null;
        MaterialProperty _SpecStr = null;
        MaterialProperty _ShadowRamp = null;
        MaterialProperty _RampWidth0 = null;
        MaterialProperty _RampWidth1 = null;
        MaterialProperty _RampWeight = null;
        MaterialProperty _ShadowMask = null;
        MaterialProperty _ShadowStr = null;
        MaterialProperty _DirectCont = null;
        MaterialProperty _IndirectCont = null;
        MaterialProperty _RTDirectCont = null;
        MaterialProperty _RTIndirectCont = null;
        MaterialProperty _AnisoAngleY = null;
        MaterialProperty _AnisoLayerY = null;
        MaterialProperty _AnisoLayerStr = null;
        MaterialProperty _ReflCol = null;
        MaterialProperty _InterpMask = null;
        MaterialProperty _AnisoLerp = null;
        MaterialProperty _MaskingMode = null;
        MaterialProperty _PackedMask0 = null;
        MaterialProperty _PackedMask1 = null;
        MaterialProperty _RTSelfShadow = null;
        MaterialProperty _ClampAdditive = null;
        MaterialProperty _HardenNormals = null;
        MaterialProperty _RampPos = null;
        MaterialProperty _AnisoSteps = null;
        MaterialProperty _DistortMatcap0 = null;
        MaterialProperty _DistortMatcap1 = null;
        MaterialProperty _LightingBasedIOR = null;
        MaterialProperty _OutlineMult = null;
        MaterialProperty _DissolveClones = null;
        MaterialProperty _ShatterClones = null;
        MaterialProperty _DFClones = null;
        MaterialProperty _WFClones = null;
        MaterialProperty _GlitchClones = null;
        MaterialProperty _UseVertexColor = null;
        MaterialProperty _EmissionToggle = null;
        MaterialProperty _EmissionColor = null;
        MaterialProperty _EmissionMap = null;
        MaterialProperty _EmissMask = null;
        MaterialProperty _PulseToggle = null;
        MaterialProperty _PulseWaveform = null;
        MaterialProperty _PulseSpeed = null;
        MaterialProperty _PulseStr = null;
        MaterialProperty _EmissScroll = null;
        MaterialProperty _Crossfade = null;
        MaterialProperty _ReactToggle = null;
        MaterialProperty _CrossMode = null;
        MaterialProperty _ReactThresh = null;
        MaterialProperty _PulseMask = null;
        MaterialProperty _RimLighting = null;
        MaterialProperty _RimBlending = null;
        MaterialProperty _RimMask = null;
        MaterialProperty _RimCol = null;
        MaterialProperty _RimTex = null;
        MaterialProperty _RimStr = null;
        MaterialProperty _RimWidth = null;
        MaterialProperty _RimEdge = null;
        MaterialProperty _UnlitRim = null;
        MaterialProperty _EnvironmentRim = null;
        MaterialProperty _ERimBlending = null;
        MaterialProperty _ERimTint = null;
        MaterialProperty _ERimStr = null;
        MaterialProperty _ERimWidth = null;
        MaterialProperty _ERimEdge = null;
        MaterialProperty _ERimUseRough = null;
        MaterialProperty _ERimRoughness = null;
        MaterialProperty _ApplyOutlineLighting = null;
        MaterialProperty _ApplyOutlineEmiss = null;
        MaterialProperty _ApplyAlbedoTint = null;
        MaterialProperty _OutlineThicc = null;
        MaterialProperty _OutlineCol = null;
        MaterialProperty _OutlineTex = null;
        MaterialProperty _OutlineScroll = null;
        MaterialProperty _OutlineMask = null;
        MaterialProperty _OutlineRange = null;
        MaterialProperty _TeamColorMask = null;
        MaterialProperty _TeamColor0 = null;
        MaterialProperty _TeamColor1 = null;
        MaterialProperty _TeamColor2 = null;
        MaterialProperty _TeamColor3 = null;
        MaterialProperty _GlitchToggle = null;
        MaterialProperty _ShatterToggle = null;
        MaterialProperty _WireframeToggle = null;
        MaterialProperty _Instability = null;
        MaterialProperty _GlitchFrequency = null;
        MaterialProperty _GlitchIntensity = null;
        MaterialProperty _ShatterSpread = null;
        MaterialProperty _ShatterMax = null;
        MaterialProperty _ShatterMin = null;
        MaterialProperty _ShatterCull = null;
        MaterialProperty _WFColor = null;
        MaterialProperty _WFFill = null;
        MaterialProperty _WFVisibility = null;
        MaterialProperty _WFMode = null;
        MaterialProperty _EntryPos = null;
        MaterialProperty _SaturateEP = null;
        MaterialProperty _Screenspace = null;
        MaterialProperty _Position = null;
        MaterialProperty _Rotation = null;
        MaterialProperty _Range = null;
        MaterialProperty _DissolveAmount = null;
        MaterialProperty _DissolveTex = null;
        MaterialProperty _DissolveRimCol = null;
        MaterialProperty _DissolveRimWidth = null;
        MaterialProperty _DissolveScroll0 = null;
        MaterialProperty _DissolveBlendSpeed = null;
        MaterialProperty _DissolveBlending = null;
        MaterialProperty _DissolveMask = null;
        MaterialProperty _CloneToggle = null;
        MaterialProperty _ClonePattern = null;
        MaterialProperty _Clone1 = null;
        MaterialProperty _Clone2 = null;
        MaterialProperty _Clone3 = null;
        MaterialProperty _Clone4 = null;
        MaterialProperty _Clone5 = null;
        MaterialProperty _Clone6 = null;
        MaterialProperty _Clone7 = null;
        MaterialProperty _Clone8 = null;
        MaterialProperty _Visibility = null;
        MaterialProperty _ERimMask = null;
        MaterialProperty _DissolveStyle = null;
        MaterialProperty _DissolveNoiseScale = null;
        MaterialProperty _GeomDissolveAmount = null;
        MaterialProperty _GeomDissolveAxis = null;
        MaterialProperty _GeomDissolveAxisFlip = null;
        MaterialProperty _GeomDissolveWidth = null;
        MaterialProperty _GeomDissolveClip = null;
        MaterialProperty _GeomDissolveSpread = null;
        MaterialProperty _GeomDissolveWireframe = null;
        MaterialProperty _ManualSpecBright = null;
        MaterialProperty _GeomDissolveClamp = null;
        MaterialProperty _GeomDissolveFilter = null;
        MaterialProperty _ReflUseRough = null;
        MaterialProperty _ReflRough = null;
        MaterialProperty _SpecUseRough = null;
        MaterialProperty _SpecRough = null;
        MaterialProperty _MatcapUseRough = null;
        MaterialProperty _MatcapRough = null;
        MaterialProperty _UVSec = null;
        MaterialProperty _DetailRoughnessMap = null;
        MaterialProperty _DetailRoughBlending = null;
        MaterialProperty _DetailRoughStrength = null;
        MaterialProperty _HeightChannel = null;
        MaterialProperty _EnablePackedHeight = null;
        MaterialProperty _PackedMask3 = null;
        MaterialProperty _SpritesheetBrightness = null;
        MaterialProperty _SpritesheetBrightness1 = null;
        MaterialProperty _Refraction = null;
        MaterialProperty _RefractionOpac = null;
        MaterialProperty _UnlitRefraction = null;
        MaterialProperty _RefractionIOR = null;
        MaterialProperty _RefractionMask = null;
        MaterialProperty _RefractionCA = null;
        MaterialProperty _RefractionCAStr = null;
        MaterialProperty _RefractionTint = null;
        MaterialProperty _GSAA = null;
        MaterialProperty _DissolvePoint0 = null;
        MaterialProperty _DissolvePoint1 = null;
        MaterialProperty _VertexExpansion = null;
        MaterialProperty _VertexExpansionClamp = null;
        MaterialProperty _VertexRounding = null;
        MaterialProperty _VertexRoundingPrecision = null;
        MaterialProperty _VertexExpansionMask = null;
        MaterialProperty _VertexRoundingMask = null;
        MaterialProperty _UseSpritesheetAlpha = null;
        MaterialProperty _VertexManipulationToggle = null;
        MaterialProperty _RefractionMaskScroll = null;
        MaterialProperty _SpecBiasOverrideToggle = null;
        MaterialProperty _SpecBiasOverride = null;
        MaterialProperty _SpritesheetMode0 = null;
        MaterialProperty _SpritesheetMode1 = null;
        MaterialProperty _Flipbook0 = null;
        MaterialProperty _Flipbook1 = null;
        MaterialProperty _Flipbook0Scroll = null;
        MaterialProperty _Flipbook1Scroll = null;
        MaterialProperty _Flipbook0ClipEdge = null;
        MaterialProperty _Flipbook1ClipEdge = null;
        MaterialProperty _EmissIntensity = null;
        MaterialProperty _MatcapCenter = null;
        MaterialProperty _MatcapCenter1 = null;

        MaterialProperty _EnableMaskTransform = null;
        MaterialProperty _ReflectionMaskScroll = null;
        MaterialProperty _SpecularMaskScroll = null;
        MaterialProperty _ERimMaskScroll = null;
        MaterialProperty _MatcapMaskScroll = null;
        MaterialProperty _MatcapBlendMaskScroll = null;
        MaterialProperty _InterpMaskScroll = null;
        MaterialProperty _SubsurfaceMaskScroll = null;
        MaterialProperty _RimMaskScroll = null;
        MaterialProperty _DetailMaskScroll = null;
        MaterialProperty _ShadowMaskScroll = null;
        MaterialProperty _DiffuseMaskScroll = null;
        MaterialProperty _FilterMaskScroll = null;
        MaterialProperty _EmissMaskScroll = null;
        MaterialProperty _EmissPulseMaskScroll = null;
        MaterialProperty _RefractionDissolveMask = null;
        MaterialProperty _RefractionDissolveMaskScroll = null;
        MaterialProperty _RefractionDissolveMaskStr = null;
        MaterialProperty _OutlineMaskScroll = null;
        MaterialProperty _IgnoreFilterMask = null;
        MaterialProperty _DitheredShadows = null;
        MaterialProperty _DetailOcclusionMap = null;
        MaterialProperty _DetailOcclusionBlending = null;
        MaterialProperty _DetailOcclusionStrength = null;
        MaterialProperty _DetailAlbedoBlending = null;
        MaterialProperty _DetailAlbedoStrength = null;
        MaterialProperty _NearClip = null;
        MaterialProperty _NearClipMask = null;
        MaterialProperty _NearClipToggle = null;
        MaterialProperty _BCDissolveToggle = null;
        MaterialProperty _BCNoiseTex = null;
        MaterialProperty _BCDissolveStr = null;
        MaterialProperty _BCRimWidth = null;
        MaterialProperty _BCRimCol = null;
        MaterialProperty _MainTex2 = null;
        MaterialProperty _BCColor = null;
        MaterialProperty _AlphaMaskChannel = null;
        MaterialProperty _AudioLinkToggle = null;
        MaterialProperty _AudioLinkEmissionMultiplier = null;
        MaterialProperty _AudioLinkRimMultiplier = null;
        MaterialProperty _AudioLinkRimWidth = null;
        MaterialProperty _AudioLinkDissolveMultiplier = null;
        MaterialProperty _AudioLinkBCDissolveMultiplier = null;
        MaterialProperty _AudioLinkVertManipMultiplier = null;
        MaterialProperty _AudioLinkEmissionBand = null;
        MaterialProperty _AudioLinkRimBand = null;
        MaterialProperty _AudioLinkDissolveBand = null;
        MaterialProperty _AudioLinkBCDissolveBand = null;
        MaterialProperty _AudioLinkVertManipBand = null;
        MaterialProperty _AudioLinkRimPulse = null;
        MaterialProperty _AudioLinkRimPulseWidth = null;
        MaterialProperty _AudioLinkRimPulseSharp = null;
        MaterialProperty _AudioLinkTriOffsetMask = null;
        MaterialProperty _AudioLinkTriOffsetBand = null;
        MaterialProperty _AudioLinkTriOffsetStrength = null;
        MaterialProperty _AudioLinkTriOffsetMaskScroll = null;
        MaterialProperty _AudioLinkTriOffsetCoords = null;
        MaterialProperty _AudioLinkTriOffsetSize = null;
        MaterialProperty _AudioLinkWireframeMask = null;
        MaterialProperty _AudioLinkWireframeBand = null;
        MaterialProperty _AudioLinkWireframeStrength = null;
        MaterialProperty _AudioLinkWireframeMaskScroll = null;
        MaterialProperty _AudioLinkWireframeCoords = null;
        MaterialProperty _AudioLinkWireframeSize = null;
        MaterialProperty _AudioLinkWireframeColor = null;
        MaterialProperty _AudioLinkWireframeStartPos = null;
        MaterialProperty _AudioLinkWireframeEndPos = null;
        MaterialProperty _AudioLinkTriOffsetStartPos = null;
        MaterialProperty _AudioLinkTriOffsetEndPos = null;
        MaterialProperty _AudioLinkWireframeMode = null;
        MaterialProperty _AudioLinkTriOffsetMode = null;
        MaterialProperty _AudioLinkStrength = null;
        MaterialProperty _DetailShadowMap = null;
        MaterialProperty _RoughRemapMin = null;
        MaterialProperty _RoughRemapMax = null;
        MaterialProperty _SmoothRemapMin = null;
        MaterialProperty _SmoothRemapMax = null;
        MaterialProperty _AORemapMin = null;
        MaterialProperty _AORemapMax = null;
        MaterialProperty _HeightRemapMin = null;
        MaterialProperty _HeightRemapMax = null;
        MaterialProperty _Subsurface = null;
        MaterialProperty _ScatterBaseColorTint = null;
        MaterialProperty _ThicknessMap = null;
        MaterialProperty _ThicknessMapPower = null;
        MaterialProperty _ScatterTex = null;
        MaterialProperty _ScatterCol = null;
        MaterialProperty _ScatterAmbient = null;
        MaterialProperty _ScatterIntensity = null;
        MaterialProperty _ScatterPow = null;
        MaterialProperty _ScatterDist = null;
        MaterialProperty _ScatterWrap = null;
        MaterialProperty _Hide = null;
        MaterialProperty _AudioLinkRemapMin = null;
        MaterialProperty _AudioLinkRemapMax = null;
        MaterialProperty _AudioLinkRemapEmissionMin = null;
        MaterialProperty _AudioLinkRemapEmissionMax = null;
        MaterialProperty _AudioLinkRemapRimMin = null;
        MaterialProperty _AudioLinkRemapRimMax = null;
        MaterialProperty _AudioLinkRemapDissolveMin = null;
        MaterialProperty _AudioLinkRemapDissolveMax = null;
        MaterialProperty _AudioLinkRemapBCDissolveMin = null;
        MaterialProperty _AudioLinkRemapBCDissolveMax = null;
        MaterialProperty _AudioLinkRemapVertManipMin = null;
        MaterialProperty _AudioLinkRemapVertManipMax = null;
        MaterialProperty _AudioLinkRemapTriOffsetMin = null;
        MaterialProperty _AudioLinkRemapTriOffsetMax = null;
        MaterialProperty _AudioLinkRemapWireframeMin = null;
        MaterialProperty _AudioLinkRemapWireframeMax = null;
        MaterialProperty _AudioLinkUVDistortionBand = null;
        MaterialProperty _AudioLinkUVDistortionMultiplier = null;
        MaterialProperty _AudioLinkRemapUVDistortionMin = null;
        MaterialProperty _AudioLinkRemapUVDistortionMax = null;
        MaterialProperty _AudioLinkRemapRimPulseMin = null;
        MaterialProperty _AudioLinkRemapRimPulseMax = null;
        MaterialProperty _Iridescence = null;
        MaterialProperty _IridescenceStrength = null;
        MaterialProperty _IridescenceWidth = null;
        MaterialProperty _IridescenceEdge = null;
        MaterialProperty _IridescenceMask = null;
        MaterialProperty _RefractionBlur = null;
        MaterialProperty _RefractionBlurStrength = null;
        MaterialProperty _RefractionBlurRough = null;
        MaterialProperty _AddCont = null;
        MaterialProperty _AudioLinkOutlineBand = null;
        MaterialProperty _AudioLinkOutlineMultiplier = null;
        MaterialProperty _AudioLinkRemapOutlineMin = null;
        MaterialProperty _AudioLinkRemapOutlineMax = null;
        MaterialProperty _ParallaxOffset = null;
        MaterialProperty _ParallaxSteps = null;
        MaterialProperty _VertexRotation = null;
        MaterialProperty _VertexPosition = null;
        MaterialProperty _UseOutlineTexAlpha = null;
        MaterialProperty _MatcapNormal0 = null;
        MaterialProperty _MatcapNormal0Str = null;
        MaterialProperty _MatcapNormal0Scroll = null;
        MaterialProperty _MatcapNormal0Mix = null;
        MaterialProperty _MatcapNormal1 = null;
        MaterialProperty _MatcapNormal1Str = null;
        MaterialProperty _MatcapNormal1Scroll = null;
        MaterialProperty _MatcapNormal1Mix = null;
        MaterialProperty _ACES = null;
        MaterialProperty _MainTexRot = null;
        MaterialProperty _DetailRot = null;
        MaterialProperty _VertexColor = null;
        MaterialProperty _FresnelToggle = null;
        MaterialProperty _FresnelStrength = null;
        MaterialProperty _OscilloscopeStrength = null;
        MaterialProperty _OscilloscopeCol = null;
        MaterialProperty _OscilloscopeScale = null;
        MaterialProperty _OscilloscopeOffset = null;
        MaterialProperty _OscilloscopeRot = null;
        MaterialProperty _OscilloscopeMarginLR = null;
        MaterialProperty _OscilloscopeMarginTB = null;
        MaterialProperty _EmissionMap2 = null;
        MaterialProperty _EmissionColor2 = null;
        MaterialProperty _EmissScroll2 = null;
        MaterialProperty _EmissIntensity2 = null;
        MaterialProperty _AlphaStrength = null;
        MaterialProperty _WireframeTransparency = null;
        MaterialProperty _DetailMetallicBlending = null;
        MaterialProperty _DetailMetallic = null;
        MaterialProperty _DetailMetallicStrength = null;
        MaterialProperty _UseShadowsForLREmiss = null;
        MaterialProperty _HueMode = null;
        MaterialProperty _MonoTint = null;
        MaterialProperty _LitCubemap = null;
        MaterialProperty _IridescenceMode = null;
        MaterialProperty _IridescenceRamp = null;

        // Light Volumes
        MaterialProperty _LightVolumesToggle = null;
        MaterialProperty _LightVolumeStrength = null;
        MaterialProperty _LightVolumeSpecularity = null;
        MaterialProperty _LightVolumeSpecularityStrength = null;
        MaterialProperty _LightVolumeBias = null;

        MaterialProperty _VRCFallback = null;
        MaterialProperty _NaNLmao = null;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        MaterialEditor m_me;
        public override void OnGUI(MaterialEditor me, MaterialProperty[] props) {

            m_me = me;
            Material mat = (Material)me.target;

            // Find properties
            foreach (var property in GetType().GetFields(bindingFlags)){
                if (property.FieldType == typeof(MaterialProperty)){
                    property.SetValue(this, FindProperty(property.Name, props));
                }
            }

            // For shadow ramp gradient editor
            if (gradientObj == null){
                gradientObj = GradientObject.CreateInstance<GradientObject>();
                serializedGradient = new SerializedObject(gradientObj);
                colorGradient = serializedGradient.FindProperty("gradient");
                rampTex = new Texture2D(128, 16);
                GradientToTexture(ref rampTex);
            }

            // For iridescence ramp gradient editor
            if (iridescenceGradientObj == null){
                iridescenceGradientObj = GradientObject.CreateInstance<GradientObject>();
                iridescenceSerializedGradient = new SerializedObject(iridescenceGradientObj);
                iridescenceColorGradient = iridescenceSerializedGradient.FindProperty("gradient");
                iridescenceRampTex = new Texture2D(128, 16);
                IridescenceGradientToTexture(ref iridescenceRampTex);
            }

            // Using this to mercilessly murder verts when necessary (must be 0)
            _NaNLmao.floatValue = 0.0f;

            // Generated ramps must stay outside the read-only VPM package directory
            if (!AssetDatabase.IsValidFolder(rampsFolderPath))
                EnsureAssetFolder(rampsFolderPath);

            float blendMode = _BlendMode.floatValue;
            float workflow = _PBRWorkflow.floatValue;
            float renderMode = _RenderMode.floatValue;
            float cubeMode = _CubeMode.floatValue;

            // Check name of shader to determine if certain properties should be displayed
            bool isTransparent = blendMode >= 4;
            bool isCutout = blendMode > 0 && blendMode < 4;
            bool isUberX = MGUI.IsXVersion(mat);
            bool isOutline = MGUI.IsOutline(mat);

            // Add mat to foldout dictionary if it isn't in there yet
            if (!foldouts.ContainsKey(mat))
                foldouts.Add(mat, toggles);

            foreach (var obj in _BlendMode.targets)
                ApplyMaterialSettings((Material)obj);

            if (mat.GetInt("_MaterialResetCheck") == 0){
                mat.SetInt("_MaterialResetCheck", 1);
                ApplyMaterialSettings(mat);
                SetBlendMode(mat);
            }

            Texture2D standardIcon = (Texture2D)Resources.Load("StandardIcon", typeof(Texture2D));
            GUIContent standardButtonContent = new GUIContent(standardIcon, "应用在视觉上与标准着色器匹配的预设属性值。");

            string headerText = "UBER";
            if (isUberX && isOutline)
                headerText = "UBER X 描边";
            else if (isUberX && !isOutline)
                headerText = "UBER X";
            else if (!isUberX && isOutline)
                headerText = "UBER 描边";

            MGUI.DoHeader(headerText);

            // -----------------
            // Base Settings
            // -----------------
            Foldouts.DoFoldout(foldouts, mat, "基础", 2, Foldouts.Style.StandardButton);
            if (Foldouts.DoFoldoutButton(MGUI.collapseLabel, 11)) Toggles.CollapseFoldouts(mat, foldouts, 1);
            if (Foldouts.DoFoldoutButton(standardButtonContent, -13)) DoStandardLighting(mat);
            if (Foldouts.BeginFoldout(foldouts, mat, "基础")) {
                MGUI.PropertyGroupParent(()=>{
                    MGUI.PropertyGroup(()=>{
                        me.ShaderProperty(_RenderMode, Tips.renderModeLabel);
                        EditorGUI.BeginChangeCheck();
                        me.ShaderProperty(_BlendMode, Tips.blendModeLabel);
                        me.ShaderProperty(_MirrorBehavior, Tips.mirrorBehaviorLabel);
                        me.ShaderProperty(_CubeMode, Tips.cubeModeLabel);
                        if (EditorGUI.EndChangeCheck())
                            SetBlendMode(mat);
                        if (isCutout || isTransparent){
                            me.ShaderProperty(_UseAlphaMask, Tips.useAlphaMaskLabel);
                            me.ShaderProperty(_AlphaStrength, "Alpha 强度");
                        }
                        if (blendMode == 1)
                            me.ShaderProperty(_Cutoff, "镂空");
                    });
                    MGUI.PropertyGroup(()=>{
                        switch((int)cubeMode){

                            // Tex Only
                            case 0:
                                me.TexturePropertySingleLine(Tips.baseColorLabel, _MainTex, _Color, renderMode == 1 ? _ColorPreservation : null);
                                if (renderMode == 1) MGUI.TexPropLabel(Tips.colorPreservation, 123, true);
                                if (_MirrorBehavior.floatValue == 2)
                                    me.TexturePropertySingleLine(Tips.mirrorTexLabel, _MirrorTex);
                                MGUI.TextureSOScroll(me, _MainTex, _MainTexScroll);
                                me.ShaderProperty(_MainTexRot, "旋转");
                                break;

                            // Cubemap Only
                            case 1:
                                me.TexturePropertySingleLine(Tips.reflCubeLabel, _MainTexCube0, _CubeColor0, renderMode == 1 ? _ColorPreservation : null);
                                if (renderMode == 1)
                                    MGUI.TexPropLabel(Tips.colorPreservation, 123, true);
                                MGUI.Vector3Field(_CubeRotate0, "旋转", false);
                                me.ShaderProperty(_AutoRotate0, "自动旋转");
                                break;

                            // Tex and Cubemap
                            case 2:
                                me.TexturePropertySingleLine(Tips.baseColorLabel, _MainTex, _Color, renderMode == 1 ? _ColorPreservation : null);
                                if (renderMode == 1)
                                    MGUI.TexPropLabel(Tips.colorPreservation, 123, true);
                                if (_MirrorBehavior.floatValue == 2)
                                    me.TexturePropertySingleLine(Tips.mirrorTexLabel, _MirrorTex);
                                MGUI.TextureSOScroll(me, _MainTex, _MainTexScroll);
                                me.ShaderProperty(_MainTexRot, "旋转");

                                me.TexturePropertySingleLine(new GUIContent("混合"), _CubeBlendMask, _CubeBlendMask.textureValue ? null : _CubeBlend);

                                me.TexturePropertySingleLine(Tips.reflCubeLabel, _MainTexCube0, _CubeColor0, _CubeBlendMode);
                                MGUI.TexPropLabel("混合", blendingLabelPos, true);
                                MGUI.Vector3Field(_CubeRotate0, "旋转", false);
                                me.ShaderProperty(_AutoRotate0, "自动旋转");
                                break;
                            default: break;
                        }
                        if (_UseAlphaMask.floatValue == 1 && (isCutout || isTransparent)){
                            me.TexturePropertySingleLine(Tips.alphaMaskLabel, _AlphaMask, _AlphaMaskChannel);
                            MGUI.TextureSO(me, _AlphaMask);
                        }
                        me.ShaderProperty(_VertexColor, "顶点颜色");
                    });
                });
            }
            Foldouts.EndFoldout();

            // -----------------
            // Textures
            // -----------------
            if (renderMode == 1){
                if (Foldouts.DoFoldout(foldouts, mat, "贴图", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        // Primary Maps
                        MGUI.BoldLabel("主贴图");
                        me.ShaderProperty(_PBRWorkflow, "工作流");
                        MGUI.PropertyGroup(() => {
                            me.TexturePropertySingleLine(Tips.normalTexLabel, _BumpMap, _BumpMap.textureValue ? _BumpScale : null);
                            switch ((int)workflow){

                                // Metallic
                                case 0:
                                    me.TexturePropertySingleLine(Tips.metallicTexLabel, _MetallicGlossMap, _MetallicGlossMap.textureValue ? null : _Metallic);
                                    me.TexturePropertySingleLine(Tips.roughnessTexLabel, _SpecGlossMap, _SpecGlossMap.textureValue ? null : _Glossiness);
                                    MGUI.sRGBWarning(_SpecGlossMap);
                                    break;

                                // Specular (RGB)
                                case 1:
                                    me.TexturePropertySingleLine(Tips.specularTexLabel, _SpecGlossMap, _SpecCol);
                                    me.TexturePropertySingleLine(Tips.smoothTexLabel, _SmoothnessMap, _GlossMapScale);
                                    break;

                                // Specular (RGBA)
                                case 2:
                                    me.TexturePropertySingleLine(Tips.specularTexLabel, _SpecGlossMap, _SpecCol);
                                    me.ShaderProperty(_GlossMapScale, "平滑度", 2);
                                    break;

                                // Packed
                                case 3:
                                    me.TexturePropertySingleLine(Tips.packedTexLabel, _PackedMap);
                                    MGUI.sRGBWarning(_PackedMap);
                                    me.ShaderProperty(_MetallicChannel, "金属度");
                                    me.ShaderProperty(_RoughnessChannel, "粗糙度");
                                    me.ShaderProperty(_OcclusionChannel, "环境光遮蔽");
                                    me.ShaderProperty(_HeightChannel, "高度");
                                    me.ShaderProperty(_OcclusionStrength, "环境光遮蔽强度");
                                    MGUI.ToggleSlider(me, "高度强度", _EnablePackedHeight, _Parallax);
                                    if (_EnablePackedHeight.floatValue > 0){
                                        me.ShaderProperty(_ParallaxOffset, Tips.heightOffsetText);
                                        me.ShaderProperty(_ParallaxSteps, Tips.heightStepsText);
                                    }
                                    break;

                                default: break;
                            }
                            if (workflow < 3)
                                me.TexturePropertySingleLine(Tips.occlusionTexLabel, _OcclusionMap, _OcclusionMap.textureValue ? _OcclusionStrength : null);
                            if (workflow < 3){
                                me.TexturePropertySingleLine(Tips.heightTexLabel, _ParallaxMap, _ParallaxMap.textureValue ? _Parallax : null);
                                if (_ParallaxMap.textureValue){
                                    me.ShaderProperty(_ParallaxOffset, Tips.parallaxOfsText, 2);
                                    me.ShaderProperty(_ParallaxSteps, Tips.stepsText, 2);
                                }
                            }
                            if (workflow == 0){
                                bool hasAnyTexture = _OcclusionMap.textureValue != null || _SpecGlossMap.textureValue != null || _MetallicGlossMap.textureValue != null || _ParallaxMap.textureValue != null;
                                if (hasAnyTexture && MGUI.PropertyButton("打包贴图")){
                                    Undo.RecordObjects(me.targets, "打包贴图");
                                    bool allPacked = true;
                                    foreach (Material target in me.targets)
                                        allPacked &= PackPBRTextures(target);
                                    // Only switch workflows once every selected material has its packed map
                                    if (allPacked){
                                        _PBRWorkflow.floatValue = 3f;
                                        _OcclusionChannel.floatValue = 0f;
                                        _RoughnessChannel.floatValue = 1f;
                                        _MetallicChannel.floatValue = 2f;
                                        _HeightChannel.floatValue = 3f;
                                        foreach (Material target in me.targets){
                                            target.SetInt("_EnablePackedHeight", target.GetTexture("_ParallaxMap") ? 1 : 0);
                                            ApplyMaterialSettings(target);
                                        }
                                    }
                                }
                            }
                        });

                        MGUI.BoldLabel("细节贴图");
                        me.ShaderProperty(_UVSec, "UV 集");
                        MGUI.PropertyGroup(() => {
                            bool usingDetRough = _DetailRoughnessMap.textureValue;
                            bool usingDetOcc = _DetailOcclusionMap.textureValue;
                            bool usingDetAlbedo = _DetailAlbedoMap.textureValue;
                            bool usingDetNormal = _DetailNormalMap.textureValue;
                            bool usingDetMetal = _DetailMetallic.textureValue;
                            me.TexturePropertySingleLine(Tips.baseColorLabel, _DetailAlbedoMap, usingDetAlbedo ? _DetailAlbedoStrength : null, usingDetAlbedo ? _DetailAlbedoBlending : null);
                            me.TexturePropertySingleLine(Tips.normalTexLabel, _DetailNormalMap, usingDetNormal ? _DetailNormalMapScale : null);
                            if (workflow == 0 || workflow >= 3){
                                me.TexturePropertySingleLine(Tips.metallicTexLabel, _DetailMetallic, usingDetMetal ? _DetailMetallicStrength : null, usingDetMetal ? _DetailMetallicBlending : null);
                                me.TexturePropertySingleLine(Tips.roughnessTexLabel, _DetailRoughnessMap, usingDetRough ? _DetailRoughStrength : null, usingDetRough ? _DetailRoughBlending : null);
                            }
                            me.TexturePropertySingleLine(Tips.occlusionTexLabel, _DetailOcclusionMap, usingDetOcc ? _DetailOcclusionStrength : null, usingDetOcc ? _DetailOcclusionBlending : null);

                            if (usingDetAlbedo || usingDetNormal || usingDetRough || usingDetOcc || usingDetMetal) {
                                MGUI.TextureSOScroll(me, _DetailAlbedoMap, _DetailScroll);
                                me.ShaderProperty(_DetailRot, "旋转");
                            }
                        });
                    });

                    // Masking
                    if (Foldouts.DoFoldout(foldouts, mat, me, _MaskingMode, "遮罩", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_MaskingMode.floatValue == 0);
                            if (_MaskingMode.floatValue != 2){
                                me.ShaderProperty(_EnableMaskTransform, Tips.enableMaskTransformLabel);
                                MGUI.PropertyGroup(() => {
                                    me.TexturePropertySingleLine(Tips.reflLabel, _ReflectionMask);
                                    MGUI.TextureSOScroll(me, _ReflectionMask, _ReflectionMaskScroll, _ReflectionMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.specLabel, _SpecularMask);
                                    MGUI.TextureSOScroll(me, _SpecularMask, _SpecularMaskScroll, _SpecularMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.anisoBlendLabel, _InterpMask);
                                    MGUI.TextureSOScroll(me, _InterpMask, _InterpMaskScroll, _InterpMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.matcapPrimaryMask, _MatcapMask);
                                    MGUI.TextureSOScroll(me, _MatcapMask, _MatcapMaskScroll, _MatcapMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.matcapSecondaryMask, _MatcapBlendMask);
                                    MGUI.TextureSOScroll(me, _MatcapBlendMask, _MatcapBlendMaskScroll, _MatcapBlendMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.shadowLabel, _ShadowMask);
                                    MGUI.TextureSOScroll(me, _ShadowMask, _ShadowMaskScroll, _ShadowMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.basicRimLabel, _RimMask);
                                    MGUI.TextureSOScroll(me, _RimMask, _RimMaskScroll, _RimMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.eRimLabel, _ERimMask);
                                    MGUI.TextureSOScroll(me, _ERimMask, _ERimMaskScroll, _ERimMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.diffuseLabel, _DiffuseMask);
                                    MGUI.TextureSOScroll(me, _DiffuseMask, _DiffuseMaskScroll, _DiffuseMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.subsurfLabel, _SubsurfaceMask);
                                    MGUI.TextureSOScroll(me, _SubsurfaceMask, _SubsurfaceMaskScroll, _SubsurfaceMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.detailLabel, _DetailMask);
                                    MGUI.TextureSOScroll(me, _DetailMask, _DetailMaskScroll, _DetailMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.emissLabel, _EmissMask);
                                    MGUI.TextureSOScroll(me, _EmissMask, _EmissMaskScroll, _EmissMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.emissPulseLabel, _PulseMask);
                                    MGUI.TextureSOScroll(me, _PulseMask, _EmissPulseMaskScroll, _PulseMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.filterLabel, _FilterMask);
                                    MGUI.TextureSOScroll(me, _FilterMask, _FilterMaskScroll, _FilterMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    me.TexturePropertySingleLine(Tips.refractLabel, _RefractionMask);
                                    MGUI.TextureSOScroll(me, _RefractionMask, _RefractionMaskScroll, _RefractionMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    if (isOutline){
                                        me.TexturePropertySingleLine(Tips.olThickLabel, _OutlineMask);
                                        MGUI.TextureSOScroll(me, _OutlineMask, _OutlineMaskScroll, _OutlineMask.textureValue && _EnableMaskTransform.floatValue == 1);
                                    }
                                });
                            }
                            else if (_MaskingMode.floatValue == 2){
                                MGUI.PropertyGroup(() => {
                                    me.TexturePropertySingleLine(new GUIContent("遮罩 1"), _PackedMask0);
                                    GUILayout.Label("红色：\t反射\n绿色：\t高光\n蓝色：\t主 Matcap\nAlpha：\t次 Matcap");
                                    MGUI.Space8();
                                    me.TexturePropertySingleLine(new GUIContent("遮罩 2"), _PackedMask1);
                                    GUILayout.Label("红色：\t阴影\n绿色：\t漫反射着色\n蓝色：\t次表面\nAlpha：\t细节贴图");
                                    MGUI.Space8();
                                    me.TexturePropertySingleLine(new GUIContent("遮罩 3"), _PackedMask2);
                                    GUILayout.Label("红色：\t基础边缘光\n绿色：\t环境边缘光\n蓝色：\t折射\nAlpha：\t高光混合");
                                    MGUI.Space8();
                                    me.TexturePropertySingleLine(new GUIContent("遮罩 4"), _PackedMask3);
                                    GUILayout.Label("红色：\t自发光\n绿色：\t自发光脉冲\n蓝色：\t滤镜\nAlpha：\t描边厚度");
                                });
                            }
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    if (workflow == 0 || workflow >= 3){

                        // Metallic Filtering
                        if (Foldouts.DoFoldout(foldouts, mat, me, _MetallicFiltering, "金属度滤镜", Foldouts.Style.ThinToggle)) {
                            MGUI.PropertyGroupParent(() => {
                                MGUI.ToggleGroup(_MetallicFiltering.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_PreviewMetallic, "预览");
                                    MGUI.SliderMinMax01(_MetallicRemapMin, _MetallicRemapMax, "重映射", 1);
                                    me.ShaderProperty(_MetallicLightness, "亮度");
                                    me.ShaderProperty(_MetallicIntensity, "强度");
                                    me.ShaderProperty(_MetallicContrast, "对比度");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();

                        // Roughness Filtering
                        if (Foldouts.DoFoldout(foldouts, mat, me, _RoughnessFiltering, "粗糙度滤镜", Foldouts.Style.ThinToggle)) {
                            MGUI.PropertyGroupParent(() => {
                                MGUI.ToggleGroup(_RoughnessFiltering.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_PreviewRough, "预览");
                                    MGUI.SliderMinMax01(_RoughRemapMin, _RoughRemapMax, "重映射", 1);
                                    me.ShaderProperty(_RoughLightness, "亮度");
                                    me.ShaderProperty(_RoughIntensity, "强度");
                                    me.ShaderProperty(_RoughContrast, "对比度");
                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();
                    }

                    // Smoothness Filtering (for specular)
                    else {
                        if (Foldouts.DoFoldout(foldouts, mat, me, _SmoothnessFiltering, "平滑度滤镜", Foldouts.Style.ThinToggle)) {
                            MGUI.PropertyGroupParent(() => {
                                MGUI.ToggleGroup(_SmoothnessFiltering.floatValue == 0);
                                MGUI.PropertyGroup(()=>{
                                    me.ShaderProperty(_PreviewSmooth, "预览");
                                    MGUI.SliderMinMax01(_SmoothRemapMin, _SmoothRemapMax, "重映射", 1);
                                    me.ShaderProperty(_SmoothLightness, "亮度");
                                    me.ShaderProperty(_SmoothIntensity, "强度");
                                    me.ShaderProperty(_SmoothContrast, "对比度");

                                });
                                MGUI.ToggleGroupEnd();
                            });
                        }
                        Foldouts.EndFoldout();
                    }


                    if (Foldouts.DoFoldout(foldouts, mat, me, _AOFiltering, "环境光遮蔽滤镜", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.ToggleGroup(_AOFiltering.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_PreviewAO, "预览");
                                me.TexturePropertySingleLine(Tips.tintLabel, _AOTintTex, _AOTint);
                                MGUI.SliderMinMax01(_AORemapMin, _AORemapMax, "重映射", 1);
                                me.ShaderProperty(_AOLightness, "亮度");
                                me.ShaderProperty(_AOIntensity, "强度");
                                me.ShaderProperty(_AOContrast, "对比度");

                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Height Filtering
                    if (Foldouts.DoFoldout(foldouts, mat, me, _HeightFiltering, "高度滤镜", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.ToggleGroup(_HeightFiltering.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_PreviewHeight, "预览");
                                MGUI.SliderMinMax01(_HeightRemapMin, _HeightRemapMax, "重映射", 1);
                                me.ShaderProperty(_HeightLightness, "亮度");
                                me.ShaderProperty(_HeightIntensity, "强度");
                                me.ShaderProperty(_HeightContrast, "对比度");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Base Color Dissolve
                    if (Foldouts.DoFoldout(foldouts, mat, me, _BCDissolveToggle, "基础颜色溶解", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.ToggleGroup(_BCDissolveToggle.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.TexturePropertySingleLine(Tips.baseColor2Label, _MainTex2, _BCColor);
                                MGUI.TextureSO(me, _MainTex2, _MainTex2.textureValue);
                                me.TexturePropertySingleLine(Tips.dissolveTexLabel, _BCNoiseTex, _BCDissolveStr);
                                MGUI.TextureSO(me, _BCNoiseTex, _BCNoiseTex.textureValue);
                                me.ShaderProperty(_BCRimCol, "边缘光颜色");
                                me.ShaderProperty(_BCRimWidth, "边缘光宽度");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();
                    MGUI.Space6();
                }
                Foldouts.EndFoldout();

                // -----------------
                // Shading
                // -----------------

                if (Foldouts.DoFoldout(foldouts, mat, "着色", Foldouts.Style.Standard)) {
                    MGUI.Space6();

                    // Lighting
                    if (Foldouts.DoFoldout(foldouts, mat, "光照", Foldouts.Style.Thin)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.BoldLabel("常规");
                            MGUI.PropertyGroup( () => {
                                MGUI.Vector3FieldToggle(Tips.staticLightDirToggle, _StaticLightDirToggle, _StaticLightDir);
                                MGUI.ToggleGroup(!_OcclusionMap.textureValue && workflow != 3);
                                me.ShaderProperty(_DirectAO, Tips.directAO);
                                me.ShaderProperty(_IndirectAO, Tips.indirectAO);
                                MGUI.ToggleGroupEnd();
                                if (!_OcclusionMap.textureValue && workflow < 3){
                                    GUILayout.Space(-32);
                                    GUIStyle f = new GUIStyle(EditorStyles.boldLabel);
                                    f.fontSize = 10;
                                    Rect r = EditorGUILayout.GetControlRect();
                                    r.x += EditorGUIUtility.labelWidth+21f;
                                    GUI.Label(r, "需要环境光遮蔽贴图", f);
                                    MGUI.Space10();
                                }
                            });
                            MGUI.BoldLabel("漫反射着色");
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_DisneyDiffuse, Tips.disneyDiffuse);
                                me.ShaderProperty(_SHStr, Tips.shStr);
                                MGUI.ToggleGroup(_SHStr.floatValue == 0);
                                me.ShaderProperty(_NonlinearSHToggle, Tips.nonlinearSHToggle);
                                MGUI.ToggleGroupEnd();
                            });
                            MGUI.BoldLabel("实时光照");
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_RTDirectCont, Tips.rtDirectCont);
                                me.ShaderProperty(_RTIndirectCont, Tips.rtIndirectCont);
                                me.ShaderProperty(_VLightCont, Tips.vLightCont);
                                me.ShaderProperty(_AddCont, Tips.addCont);
                                me.ShaderProperty(_ClampAdditive, Tips.clampAdditive);
                            });
                            MGUI.BoldLabel("烘焙光照");
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_DirectCont, Tips.directCont);
                                me.ShaderProperty(_IndirectCont, Tips.indirectCont);
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    // Shadows
                    if (Foldouts.DoFoldout(foldouts, mat, me, _ShadowMode, "阴影", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_ShadowMode.floatValue == 0);
                            me.ShaderProperty(_ShadowConditions, Tips.shadowConditions);
                            MGUI.PropertyGroup( () => {
                                me.TexturePropertySingleLine(Tips.detailShadowMap, _DetailShadowMap);
                            });
                            if (_ShadowMode.floatValue == 1){
                                MGUI.PropertyGroup( () => {
                                    me.ShaderProperty(_ShadowTint, "色调");
                                    me.ShaderProperty(_RampWidth0, "渐变 1");
                                    me.ShaderProperty(_RampWidth1, "渐变 2");
                                    me.ShaderProperty(_RampWeight, "渐变混合");
                                    me.ShaderProperty(_RampPos, "偏移");
                                });
                            }
                            else if (_ShadowMode.floatValue == 2){
                                MGUI.PropertyGroup(() => {
                                    me.TexturePropertySingleLine(Tips.shadowRampLabel, _ShadowRamp);
                                    GUILayout.Space(-19);
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(colorGradient, new GUIContent(" "));
                                    if (EditorGUI.EndChangeCheck()){
                                        serializedGradient.ApplyModifiedProperties();
                                        GradientToTexture(ref rampTex);
                                    }
                                    if (MGUI.SimpleButton("应用", MGUI.GetPropertyWidth(), EditorGUIUtility.labelWidth)){
                                        byte[] encodedTex = rampTex.EncodeToPNG();
                                        int rampID = UnityEngine.Random.Range(0,10000000);
                                        string rampPath = rampsFolderPath+"/Ramp_"+rampID+".png";
                                        MGUI.WriteBytes(encodedTex, rampPath);
                                        AssetDatabase.ImportAsset(rampPath);

                                        TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(rampPath);
                                        importer.wrapMode = TextureWrapMode.Clamp;
                                        importer.SaveAndReimport();

                                        _ShadowRamp.textureValue = AssetDatabase.LoadAssetAtPath<Texture>(rampPath);
                                    }
                                });
                            }
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_ShadowStr, "强度");
                                me.ShaderProperty(_DitheredShadows, "抖动");
                                me.ShaderProperty(_RTSelfShadow, Tips.rtSelfShadow);
                                MGUI.ToggleGroup(_RTSelfShadow.floatValue == 0);
                                me.ShaderProperty(_AttenSmoothing, Tips.attenSmoothing);
                                MGUI.ToggleGroupEnd();
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Reflections
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Reflections, "反射", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup(()=>{
                                GUIContent reflCubeLabel = Tips.reflCubeLabel;
                                if (_Reflections.floatValue == 1)
                                    reflCubeLabel.text = "后备立方体贴图";
                                else
                                    reflCubeLabel.text = "立方体贴图";
                                me.TexturePropertySingleLine(reflCubeLabel, _ReflCube, _LitCubemap);
                                MGUI.TexPropLabel(Tips.litCubemapLabel, 100, false);
                                me.ShaderProperty(_ReflCol, "色调");
                                me.ShaderProperty(_ReflectionStr, "强度");
                                MGUI.ToggleFloat(me, "菲涅耳", _FresnelToggle, _FresnelStrength);
                                MGUI.ToggleSlider(me, "手动粗糙度", _ReflUseRough, _ReflRough);
                                MGUI.ToggleIntSlider(me, "步进", _ReflStepping, _ReflSteps);
                                me.ShaderProperty(_LightingBasedIOR, Tips.lightingBasedIOR);
                                me.ShaderProperty(_SSR, Tips.ssrText);
                            });
                        });
                        if (_SSR.floatValue == 1){
                            MGUI.PropertyGroup(() => {
                                MGUI.DisplayInfo("SSR 需要 2501 或更高的渲染队列才能正常运行。");
                                MGUI.DisplayInfo("\nVRChat 的 SSR 需要位于 VPM 包内 Unity/Prefabs 目录中的“Depth Light”预制件。\n每个场景或头像只能使用一种材质。\n\n该功能性能开销很高，请谨慎使用。\n");
                                me.ShaderProperty(_Alpha, "强度");
                                me.ShaderProperty(_MaxSteps, "最大步数");
                                me.ShaderProperty(_Step, "步长");
                                me.ShaderProperty(_LRad, "相交（L）");
                                me.ShaderProperty(_SRad, "相交（S）");
                                me.ShaderProperty(_EdgeFade, "边缘渐隐");
                            });
                        }
                    }
                    Foldouts.EndFoldout();

                    // Specular Highlights
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Specular, "高光", Foldouts.Style.ThinToggle)) {
                        if (_Specular.floatValue == 3){
                            MGUI.Space6();
                            MGUI.DisplayInfo("注意：使用遮罩选项卡中的高光混合遮罩，在 GGX 和各向异性高光之间进行插值。");
                            MGUI.Space6();
                        }
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_SpecCol, "色调");
                                if (_Specular.floatValue == 1){
                                    me.ShaderProperty(_SpecStr, "强度");
                                    MGUI.ToggleSlider(me, "手动粗糙度", _SpecUseRough, _SpecRough);
                                    MGUI.ToggleSlider(me, Tips.specBiasOverride, _SpecBiasOverrideToggle, _SpecBiasOverride);
                                    MGUI.ToggleIntSlider(me, "步进", _SharpSpecular, _SharpSpecStr);
                                    MGUI.SpaceN1();
                                    me.ShaderProperty(_RealtimeSpec, Tips.realtimeSpec);
                                }
                                else if (_Specular.floatValue == 2){
                                    me.ShaderProperty(_AnisoStr, "强度");
                                    me.ShaderProperty(_RealtimeSpec, Tips.realtimeSpec);
                                    MGUI.ToggleIntSlider(me, "步进", _SharpSpecular, _AnisoSteps);
                                    MGUI.SpaceN1();
                                }
                                else {
                                    me.ShaderProperty(_SpecStr, "GGX 强度");
                                    me.ShaderProperty(_AnisoStr, "各向异性强度");
                                    me.ShaderProperty(_RealtimeSpec, Tips.realtimeSpec);
                                    me.ShaderProperty(_ManualSpecBright, Tips.manualSpecBright);
                                    me.ShaderProperty(_SharpSpecular, "步进");
                                    if (_SharpSpecular.floatValue == 1){
                                        me.ShaderProperty(_SharpSpecStr, "GGX 步数");
                                        me.ShaderProperty(_AnisoSteps, "各向异性步数");
                                    }
                                    MGUI.ToggleGroupEnd();
                                }
                                if (_Specular.floatValue != 3){
                                    me.ShaderProperty(_ManualSpecBright, Tips.manualSpecBright);
                                }
                                if (_Specular.floatValue == 2 || _Specular.floatValue == 3){
                                    MGUI.PropertyGroup(() => {
                                        MGUI.SpaceN2();
                                        me.ShaderProperty(_AnisoAngleY, "第 1 层厚度");
                                        me.ShaderProperty(_AnisoLayerY, "第 2 层厚度");
                                        me.ShaderProperty(_AnisoLayerStr, "图层混合");
                                        me.ShaderProperty(_AnisoLerp, "线性插值混合");
                                        MGUI.SpaceN2();
                                    });
                                    MGUI.PropertyGroup(() => {
                                        MGUI.SpaceN2();
                                        me.ShaderProperty(_RippleStrength, "毛发强度");
                                        me.ShaderProperty(_RippleFrequency, "毛发密度");
                                        me.ShaderProperty(_RippleAmplitude, "毛发强度");
                                        me.ShaderProperty(_RippleContinuity, "毛发连续性");
                                        MGUI.SpaceN2();
                                    });
                                }
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    // Matcap
                    if (Foldouts.DoFoldout(foldouts, mat, me, _MatcapToggle, "Matcap", Foldouts.Style.ThinToggle)) {
                        MGUI.ToggleGroup(_MatcapToggle.floatValue == 0);
                        MGUI.Space2();
                        if (Foldouts.DoFoldout(foldouts, mat, "主 Matcap", Foldouts.Style.ThinShort)) {
                            MGUI.PropertyGroupParent(() => {
                                MGUI.PropertyGroup(() => {
                                    me.TexturePropertySingleLine(new GUIContent("Matcap"), _Matcap, _MatcapColor, _MatcapBlending);
                                    MGUI.TexPropLabel("混合", blendingLabelPos, true);
                                    if (_Matcap.textureValue){
                                        MGUI.TextureSO(me, _Matcap);
                                    };
                                    me.TexturePropertySingleLine(Tips.matcapNormal, _MatcapNormal0, _MatcapNormal0Str);
                                    if (_MatcapNormal0.textureValue){
                                        MGUI.TextureSOScroll(me, _MatcapNormal0, _MatcapNormal0Scroll);
                                    };
                                    MGUI.ToggleGroup(_MatcapNormal0.textureValue == false);
                                    me.ShaderProperty(_MatcapNormal0Mix, Tips.matcapNormalMix);
                                    MGUI.ToggleGroupEnd();
                                    MGUI.SpaceN2();
                                });
                                MGUI.PropertyGroup(() => {
                                    MGUI.SpaceN2();
                                    me.ShaderProperty(_MatcapStr, "强度");
                                    MGUI.ToggleSlider(me, "手动粗糙度", _MatcapUseRough, _MatcapRough);

                                    me.ShaderProperty(_UnlitMatcap, "无光照");
                                    me.ShaderProperty(_MatcapCenter, "VR 中无深度");
                                    MGUI.SpaceN2();
                                });
                            });
                        }
                        Foldouts.EndFoldout();

                        if (Foldouts.DoFoldout(foldouts, mat, "次级 Matcap", Foldouts.Style.ThinShort)) {
                            MGUI.PropertyGroupParent(()=>{
                                MGUI.PropertyGroup(() => {
                                    me.TexturePropertySingleLine(new GUIContent("Matcap"), _Matcap1, _MatcapColor1, _MatcapBlending1);
                                    MGUI.TexPropLabel("混合", blendingLabelPos, true);
                                    if (_Matcap1.textureValue){
                                        MGUI.TextureSO(me, _Matcap1);
                                    }
                                    me.TexturePropertySingleLine(Tips.matcapNormal, _MatcapNormal1, _MatcapNormal1Str);
                                    if (_MatcapNormal1.textureValue){
                                        MGUI.TextureSOScroll(me, _MatcapNormal1, _MatcapNormal1Scroll);
                                    };
                                    MGUI.ToggleGroup(_MatcapNormal1.textureValue == false);
                                    me.ShaderProperty(_MatcapNormal1Mix, Tips.matcapNormalMix);
                                    MGUI.ToggleGroupEnd();
                                    MGUI.SpaceN2();
                                });
                                MGUI.PropertyGroup(() => {
                                    MGUI.SpaceN2();
                                    me.ShaderProperty(_MatcapStr1, "强度");
                                    MGUI.ToggleSlider(me, "手动粗糙度", _MatcapUseRough1, _MatcapRough1);
                                    me.ShaderProperty(_UnlitMatcap1, "无光照");
                                    me.ShaderProperty(_MatcapCenter1, "VR 中无深度");
                                    MGUI.ToggleGroupEnd();
                                    MGUI.SpaceN2();
                                });
                            });
                        }
                        Foldouts.EndFoldout();
                        MGUI.ToggleGroupEnd();
                        MGUI.Space2();
                    }
                    Foldouts.EndFoldout();

                    // Subsurface Scattering
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Subsurface, "次表面散射", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.ToggleGroup(_Subsurface.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.TexturePropertySingleLine(Tips.thicknessTexLabel, _ThicknessMap, _ThicknessMapPower);
                                me.TexturePropertySingleLine(Tips.colorLabel, _ScatterTex, _ScatterCol, _ScatterBaseColorTint);
                                MGUI.TexPropLabel("基础颜色色调", 150, true);
                                me.ShaderProperty(_ScatterIntensity, "直接强度");
                                me.ShaderProperty(_ScatterAmbient, "间接强度");
                                me.ShaderProperty(_ScatterPow, "强度");
                                me.ShaderProperty(_ScatterDist, "法线强度");
                                me.ShaderProperty(_ScatterWrap, "环绕系数");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Rim
                    if (Foldouts.DoFoldout(foldouts, mat, me, _RimLighting, "基础边缘光", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.ToggleGroup(_RimLighting.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.TexturePropertySingleLine(Tips.colorLabel, _RimTex, _RimCol, _RimBlending);
                                MGUI.TexPropLabel("混合", blendingLabelPos, true);
                                MGUI.TextureSOScroll(me, _RimTex, _RimScroll, _RimTex.textureValue);
                                me.ShaderProperty(_RimStr, "强度");
                                me.ShaderProperty(_RimWidth, "宽度");
                                me.ShaderProperty(_RimEdge, "锐度");
                                me.ShaderProperty(_UnlitRim, "无光照");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Environment Rim
                    if (Foldouts.DoFoldout(foldouts, mat, me, _EnvironmentRim, "环境边缘光", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.ToggleGroup(_EnvironmentRim.floatValue == 0);
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_ERimBlending, "混合");
                                me.ShaderProperty(_ERimTint, "色调");
                                me.ShaderProperty(_ERimStr, "强度");
                                me.ShaderProperty(_ERimWidth, "宽度");
                                me.ShaderProperty(_ERimEdge, "锐度");
                                MGUI.ToggleSlider(me, "手动粗糙度", _ERimUseRough, _ERimRoughness);
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Refraction
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Refraction, "折射", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Refraction.floatValue == 0);
                            MGUI.PropertyGroup(() => {
                                MGUI.DisplayInfo("折射需要 2501 或更高的渲染队列才能正常运行。");
                                me.ShaderProperty(_RefractionTint, "色调");
                                me.ShaderProperty(_RefractionIOR, "折射率");
                                me.ShaderProperty(_RefractionOpac, "不透明度");
                                MGUI.ToggleSlider(me, "色差", _RefractionCA, _RefractionCAStr);
                                MGUI.ToggleSlider(me, "模糊", _RefractionBlur, _RefractionBlurStrength);
                                MGUI.ToggleGroup(_RefractionBlur.floatValue == 0);
                                me.ShaderProperty(_RefractionBlurRough, "使用粗糙度");
                                MGUI.ToggleGroupEnd();
                                me.ShaderProperty(_UnlitRefraction, "无光照");

                            });
                            MGUI.PropertyGroup(() => {
                                me.TexturePropertySingleLine(new GUIContent("溶解遮罩"), _RefractionDissolveMask, _RefractionDissolveMaskStr);
                                MGUI.TextureSOScroll(me, _RefractionDissolveMask, _RefractionDissolveMaskScroll);
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Iridescence
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Iridescence, "虹彩", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Iridescence.floatValue == 0);
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_IridescenceMode, "模式");
                                me.TexturePropertySingleLine(Tips.maskLabel, _IridescenceMask);
                                if (_IridescenceMode.floatValue == 1){
                                    me.TexturePropertySingleLine(Tips.shadowRampLabel, _IridescenceRamp);
                                    GUILayout.Space(-19);
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(iridescenceColorGradient, new GUIContent(" "));
                                    if (EditorGUI.EndChangeCheck()){
                                        iridescenceSerializedGradient.ApplyModifiedProperties();
                                        IridescenceGradientToTexture(ref iridescenceRampTex);
                                    }
                                    if (MGUI.SimpleButton("应用", MGUI.GetPropertyWidth(), EditorGUIUtility.labelWidth)){
                                        byte[] iridescenceEncodedTex = iridescenceRampTex.EncodeToPNG();
                                        int iridescenceRampID = UnityEngine.Random.Range(0,10000000);
                                        string iridescenceRampPath = rampsFolderPath+"/Ramp_"+iridescenceRampID+".png";
                                        MGUI.WriteBytes(iridescenceEncodedTex, iridescenceRampPath);
                                        AssetDatabase.ImportAsset(iridescenceRampPath);

                                        TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(iridescenceRampPath);
                                        importer.wrapMode = TextureWrapMode.Clamp;
                                        importer.SaveAndReimport();

                                        _IridescenceRamp.textureValue = AssetDatabase.LoadAssetAtPath<Texture>(iridescenceRampPath);
                                    }
                                }
                                me.ShaderProperty(_IridescenceStrength, "强度");
                                if (_IridescenceMode.floatValue == 0){
                                    me.ShaderProperty(_IridescenceWidth, "宽度");
                                    me.ShaderProperty(_IridescenceEdge, "锐度");
                                }
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Normals
                    if (Foldouts.DoFoldout(foldouts, mat, "法线", Foldouts.Style.Thin)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup(()=>{
                                me.ShaderProperty(_HardenNormals, "硬边缘");
                                me.ShaderProperty(_ClearCoat, Tips.clearCoat);
                                me.ShaderProperty(_GSAA, Tips.gsaa);
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    // Light volumes
                    if (Foldouts.DoFoldout(foldouts, mat, me, _LightVolumesToggle, "光照体积", Foldouts.Style.ThinToggle)) {
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
                    MGUI.Space6();
                }
                Foldouts.EndFoldout();
            }
            else {
                if (Foldouts.DoFoldout(foldouts, mat, me, _MaskingMode, "遮罩 ", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_MaskingMode.floatValue == 0);
                        if (_MaskingMode.floatValue != 2){
                            MGUI.PropertyGroup(() => {
                                me.TexturePropertySingleLine(new GUIContent("自发光"), _EmissMask);
                                me.TexturePropertySingleLine(new GUIContent("自发光脉冲"), _PulseMask);
                                me.TexturePropertySingleLine(new GUIContent("滤镜"), _FilterMask);
                                me.TexturePropertySingleLine(new GUIContent("描边厚度"), _OutlineMask);
                            });
                        }
                        else if (_MaskingMode.floatValue == 2){
                            MGUI.PropertyGroup(() => {
                                me.TexturePropertySingleLine(new GUIContent("打包遮罩"), _PackedMask3);
                                GUILayout.Label("红色：\t自发光\n绿色：\t自发光脉冲\n蓝色：\t滤镜\nAlpha：\t描边厚度");
                            });
                        }
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();
            }

            // -----------------
            // Emission
            // -----------------
            if (Foldouts.DoFoldout(foldouts, mat, me, _EmissionToggle, "自发光", Foldouts.Style.StandardToggle)) {
                MGUI.ToggleGroup(_EmissionToggle.floatValue == 0);
                MGUI.PropertyGroupParent(()=>{
                    MGUI.PropertyGroup(() => {
                        me.TexturePropertySingleLine(Tips.emissTexLabel, _EmissionMap, _EmissionColor, _EmissIntensity);
                        MGUI.TexPropLabel("强度", 111, true);
                        MGUI.TextureSOScroll(me, _EmissionMap, _EmissScroll);
                    });
                    MGUI.PropertyGroup(() => {
                        me.TexturePropertySingleLine(Tips.emissTexLabel, _EmissionMap2, _EmissionColor2, _EmissIntensity2);
                        MGUI.TexPropLabel("强度", 111, true);
                        MGUI.TextureSOScroll(me, _EmissionMap2, _EmissScroll2);
                    });
                });
                if (Foldouts.DoFoldout(foldouts, mat, me, _ReactToggle, "光照响应", Foldouts.Style.ThinToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_ReactToggle.floatValue == 0);
                        me.ShaderProperty(_UseShadowsForLREmiss, "包含阴影");
                        me.ShaderProperty(_CrossMode, Tips.crossMode);
                        MGUI.PropertyGroup(() => {
                            MGUI.ToggleGroup(_CrossMode.floatValue == 0);
                            me.ShaderProperty(_ReactThresh, Tips.reactThresh);
                            me.ShaderProperty(_Crossfade, Tips.crossFade);
                            MGUI.ToggleGroupEnd();
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                if (Foldouts.DoFoldout(foldouts, mat, me, _PulseToggle, "脉冲", Foldouts.Style.ThinToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_PulseToggle.floatValue == 0);
                        me.ShaderProperty(_PulseWaveform, "波形");
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_PulseStr, "强度");
                            me.ShaderProperty(_PulseSpeed, "速度");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();
                MGUI.ToggleGroupEnd();
                MGUI.Space6();
            }
            Foldouts.EndFoldout();

            // -----------------
            // Filters
            // -----------------
            if (Foldouts.DoFoldout(foldouts, mat, me, _Filtering, "滤镜", Foldouts.Style.StandardToggle)) {
                MGUI.PropertyGroupParent(()=>{
                    MGUI.ToggleGroup(_Filtering.floatValue == 0);
                    MGUI.PropertyGroup(() => {
                        me.ShaderProperty(_TeamFiltering, "颜色遮罩");
                        me.ShaderProperty(_PostFiltering, Tips.postFiltering);
                        me.ShaderProperty(_Invert, "反转");

                    });
                    MGUI.PropertyGroup(()=>{
                        me.ShaderProperty(_HueMode, "色调模式");
                        me.ShaderProperty(_MonoTint, Tips.monoTintText);
                        me.ShaderProperty(_AutoShift, "自动色调偏移");
                        if (_AutoShift.floatValue == 0)
                            me.ShaderProperty(_Hue, "色调");
                        else
                            me.ShaderProperty(_AutoShiftSpeed, "偏移速度");
                    });
                    MGUI.PropertyGroup(() => {

                        MGUI.Vector3FieldRGB(_RGB, "RGB 倍率");
                        me.ShaderProperty(_Saturation, "饱和度");
                        me.ShaderProperty(_Brightness, "亮度");
                        me.ShaderProperty(_Contrast, "对比度");
                        me.ShaderProperty(_HDR, "HDR");
                        me.ShaderProperty(_ACES, Tips.aces);
                    });
                    if (_TeamFiltering.floatValue == 1){
                        MGUI.PropertyGroup(() => {
                            me.TexturePropertySingleLine(new GUIContent("颜色遮罩"), _TeamColorMask);
                            me.ShaderProperty(_TeamColor0, "红色通道");
                            me.ShaderProperty(_TeamColor1, "绿色通道");
                            me.ShaderProperty(_TeamColor2, "蓝色通道");
                            me.ShaderProperty(_TeamColor3, "Alpha 通道");
                        });
                    }
                    MGUI.ToggleGroupEnd();
                });
            }
            Foldouts.EndFoldout();

            // -----------------
            // Flipbook
            // -----------------
            if (Foldouts.DoFoldout(foldouts, mat, "序列帧", Foldouts.Style.Standard)) {
                MGUI.Space6();
                if (Foldouts.DoFoldout(foldouts, mat, me, _EnableSpritesheet, "主层", Foldouts.Style.ThinToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_EnableSpritesheet.floatValue == 0);
                        me.ShaderProperty(_SpritesheetMode0, "模式");
                        MGUI.PropertyGroup(() => {
                            if (_SpritesheetMode0.floatValue == 1)
                                me.TexturePropertySingleLine(new GUIContent("精灵图集"), _Spritesheet, _SpritesheetCol, _SpritesheetBlending);
                            else
                                me.TexturePropertySingleLine(new GUIContent("序列帧资源"), _Flipbook0, _SpritesheetCol, _SpritesheetBlending);
                            MGUI.TexPropLabel("混合", blendingLabelPos, true);
                            me.ShaderProperty(_SpritesheetBrightness, "亮度");
                            me.ShaderProperty(_UnlitSpritesheet, "无光照");
                            me.ShaderProperty(_UseSpritesheetAlpha, "使用 Alpha");
                            if (_SpritesheetMode0.floatValue == 1){
                                MGUI.Vector2Field(_RowsColumns, "列 / 行");
                                MGUI.Vector2Field(_FrameClipOfs, "帧尺寸");
                            }
                            else {
                                me.ShaderProperty(_Flipbook0ClipEdge, Tips.clipEdge);
                            }
                            MGUI.Vector2Field(_SpritesheetPos, "位置");
                            MGUI.Vector2Field(_SpritesheetScale, "缩放");
                            MGUI.Vector2Field(_Flipbook0Scroll, "滚动");
                            me.ShaderProperty(_SpritesheetRot, "旋转");
                            MGUI.ToggleGroup(_ManualScrub.floatValue == 1);
                            me.ShaderProperty(_FPS, "速度");
                            MGUI.ToggleGroupEnd();
                            float frameCount0 = (_RowsColumns.vectorValue.x * _RowsColumns.vectorValue.y)-1;
                            if (_SpritesheetMode0.floatValue == 0 && _Flipbook0.textureValue){
                                Texture2DArray t2da0 = (Texture2DArray)_Flipbook0.textureValue;
                                frameCount0 = t2da0.depth;
                            }
                            MGUI.CustomToggleSlider("帧", _ManualScrub, _ScrubPos, 0f, frameCount0);
                            if (_ManualScrub.floatValue == 1 && _RowsColumns.vectorValue.x == 0 && _RowsColumns.vectorValue.y == 0 && _SpritesheetMode0.floatValue == 1)
                                MGUI.DisplayWarning("当行和列都设置为 0 时，手动帧清理将无法正确运行。");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                if (Foldouts.DoFoldout(foldouts, mat, me, _EnableSpritesheet1, "次级层", Foldouts.Style.ThinToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_EnableSpritesheet1.floatValue == 0);
                        me.ShaderProperty(_SpritesheetMode1, "模式");
                        MGUI.PropertyGroup(() => {
                            if (_SpritesheetMode1.floatValue == 1)
                                me.TexturePropertySingleLine(new GUIContent("精灵图集"), _Spritesheet1, _SpritesheetCol1, _SpritesheetBlending1);
                            else
                                me.TexturePropertySingleLine(new GUIContent("序列帧资源"), _Flipbook1, _SpritesheetCol1, _SpritesheetBlending1);
                            MGUI.TexPropLabel("混合", blendingLabelPos, true);
                            me.ShaderProperty(_SpritesheetBrightness1, "亮度");
                            me.ShaderProperty(_UnlitSpritesheet1, "无光照");
                            me.ShaderProperty(_UseSpritesheetAlpha, "使用 Alpha");
                            if (_SpritesheetMode1.floatValue == 1){
                                MGUI.Vector2Field(_RowsColumns1, "列 / 行");
                                MGUI.Vector2Field(_FrameClipOfs1, "帧尺寸");
                            }
                            else {
                                me.ShaderProperty(_Flipbook1ClipEdge, Tips.clipEdge);
                            }
                            MGUI.Vector2Field(_SpritesheetPos1, "位置");
                            MGUI.Vector2Field(_SpritesheetScale1, "缩放");
                            MGUI.Vector2Field(_Flipbook1Scroll, "滚动");
                            me.ShaderProperty(_SpritesheetRot1, "旋转");
                            MGUI.ToggleGroup(_ManualScrub1.floatValue == 1);
                            me.ShaderProperty(_FPS1, "速度");
                            MGUI.ToggleGroupEnd();
                            float frameCount1 = (_RowsColumns1.vectorValue.x * _RowsColumns1.vectorValue.y)-1;
                            if (_SpritesheetMode0.floatValue == 0 && _Flipbook1.textureValue){
                                Texture2DArray t2da1 = (Texture2DArray)_Flipbook1.textureValue;
                                frameCount1 = t2da1.depth;
                            }
                            MGUI.CustomToggleSlider("帧", _ManualScrub1, _ScrubPos1, 0f, frameCount1);
                            if (_ManualScrub1.floatValue == 1 && _RowsColumns1.vectorValue.x == 0 && _RowsColumns1.vectorValue.y == 0 && _SpritesheetMode1.floatValue == 1)
                                MGUI.DisplayWarning("当行和列都设置为 0 时，手动帧清理将无法正确运行。");


                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();
                MGUI.Space6();
            }
            Foldouts.EndFoldout();

            // -----------------
            // Outline
            // -----------------
            if (isOutline){
                if (Foldouts.DoFoldout(foldouts, mat, "描边", Foldouts.Style.Standard)) {
                    MGUI.PropertyGroupParent(()=>{
                        if (isTransparent || blendMode == 3){
                            MGUI.Space2();
                            MGUI.DisplayError("需要不透明、剪切或抖动混合模式才能运行。");
                            MGUI.Space6();
                        }
                        MGUI.ToggleGroup(isTransparent || blendMode == 3);
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_ApplyOutlineLighting, Tips.applyOutlineLighting);
                            MGUI.ToggleGroup(_EmissionToggle.floatValue == 0);
                            me.ShaderProperty(_ApplyOutlineEmiss, Tips.applyOutlineEmiss);
                            MGUI.ToggleGroupEnd();
                            me.ShaderProperty(_ApplyAlbedoTint, Tips.applyAlbedoTint);
                            EditorGUI.BeginChangeCheck();
                            me.ShaderProperty(_StencilToggle, Tips.stencilMode);
                            if (EditorGUI.EndChangeCheck()){
                                if (_StencilToggle.floatValue == 0)
                                    DoAdvancedReset();
                                else
                                    ApplyOutlineStencilConfig(mat);
                            }
                            me.ShaderProperty(_IgnoreFilterMask, Tips.ignoreFilterMask);
                            me.ShaderProperty(_UseVertexColor, Tips.useVertexColor);
                        });
                        MGUI.PropertyGroup(() => {
                            me.TexturePropertySingleLine(Tips.colorLabel, _OutlineTex, _OutlineCol, (blendMode == 1 || blendMode == 2) && _OutlineTex.textureValue ? _UseOutlineTexAlpha : null);
                            if ((blendMode == 1 || blendMode == 2) && _OutlineTex.textureValue){
                                MGUI.TexPropLabel("使用 Alpha", 115, true);
                            }
                            MGUI.TextureSOScroll(me, _OutlineTex, _OutlineScroll, _OutlineTex.textureValue);
                            me.ShaderProperty(_OutlineMult, "厚度");
                            me.ShaderProperty(_OutlineThicc, "倍率");
                            me.ShaderProperty(_OutlineRange, Tips.outlineRange);
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();
            }

            // -----------------
            // UV Distortion
            // -----------------
            if (Foldouts.DoFoldout(foldouts, mat, me, _DistortionStyle, "UV 扭曲", Foldouts.Style.StandardToggle)) {
                MGUI.PropertyGroupParent(()=>{
                    MGUI.ToggleGroup(_DistortionStyle.floatValue == 0);
                    MGUI.PropertyGroup(() => {
                        me.ShaderProperty(_DistortMainUV, "主");
                        me.ShaderProperty(_DistortDetailUV, "细节");
                        me.ShaderProperty(_DistortEmissUV, "自发光");
                        me.ShaderProperty(_DistortRimUV, "边缘光");
                        me.ShaderProperty(_DistortMatcap0, "主 Matcap");
                        me.ShaderProperty(_DistortMatcap1, "次级 Matcap");
                    });
                    if (_DistortionStyle.floatValue == 1){
                        MGUI.PropertyGroup(() => {
                            me.TexturePropertySingleLine(Tips.maskLabel, _DistortUVMask);
                            me.TexturePropertySingleLine(Tips.normalTexLabel, _DistortUVMap, _DistortUVMap.textureValue ? _DistortUVStr : null);
                            MGUI.TextureSOScroll(me, _DistortUVMap, _DistortUVScroll);
                        });
                    }
                    else if (_DistortionStyle.floatValue == 2) {
                        MGUI.PropertyGroup(() => {
                            me.TexturePropertySingleLine(Tips.maskLabel, _DistortUVMask);
                            me.ShaderProperty(_NoiseOctaves, "倍频");
                            me.ShaderProperty(_DistortUVStr, "强度");
                            me.ShaderProperty(_NoiseSpeed, "速度");
                            MGUI.Vector2Field(_NoiseScale, "缩放");
                        });
                    }
                    MGUI.ToggleGroupEnd();
                });
            }
            Foldouts.EndFoldout();

            // -----------------
            // Vertex Manip
            // -----------------
            if (Foldouts.DoFoldout(foldouts, mat, me, _VertexManipulationToggle, "顶点操作", Foldouts.Style.StandardToggle)) {
                MGUI.PropertyGroupParent(()=>{
                MGUI.ToggleGroup(_VertexManipulationToggle.floatValue == 0);
                MGUI.PropertyGroup(() => {
                    me.TexturePropertySingleLine(Tips.maskLabel, _VertexExpansionMask);
                    MGUI.Vector3Field(_VertexExpansion, "扩张", false);
                    me.ShaderProperty(_VertexExpansionClamp, "限制方向");
                });
                MGUI.PropertyGroup(() => {
                    me.TexturePropertySingleLine(Tips.maskLabel, _VertexRoundingMask);
                    me.ShaderProperty(_VertexRounding, "位置取整");
                    me.ShaderProperty(_VertexRoundingPrecision, "精度");
                });
                MGUI.PropertyGroup(() => {
                    MGUI.Vector3Field(_VertexRotation, "旋转", false);
                    MGUI.Vector3Field(_VertexPosition, "位置", false);
                });
                MGUI.ToggleGroupEnd();
                });
            }
            Foldouts.EndFoldout();

            // -----------------
            // Audio Link
            // -----------------
            if (Foldouts.DoFoldout(foldouts, mat, me, _AudioLinkToggle, "Audio Link", Foldouts.Style.StandardToggle)) {
                MGUI.PropertyGroupParent(()=>{
                    MGUI.PropertyGroup(()=>{
                        me.ShaderProperty(_AudioLinkStrength, "强度");
                        MGUI.ToggleGroup(_AudioLinkToggle.floatValue == 0);
                        MGUI.SliderMinMax(_AudioLinkRemapMin, _AudioLinkRemapMax, 0f, 2f, "重映射", 0);
                    });
                });

                if (Foldouts.DoFoldout(foldouts, mat, "示波器", Foldouts.Style.Thin)) {
                    MGUI.PropertyGroupParent(() => {
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_OscilloscopeStrength, "强度");
                            me.ShaderProperty(_OscilloscopeCol, "颜色");
                            MGUI.Vector2Field(_OscilloscopeMarginLR, "左右边距");
                            MGUI.Vector2Field(_OscilloscopeMarginTB, "上下边距");
                            MGUI.Vector2Field(_OscilloscopeScale, "缩放");
                            MGUI.Vector2Field(_OscilloscopeOffset, "偏移");
                            me.ShaderProperty(_OscilloscopeRot, "旋转");
                        });
                    });
                }
                Foldouts.EndFoldout();

                if (Foldouts.DoFoldout(foldouts, mat, "自发光 ", Foldouts.Style.Thin)) {
                    MGUI.PropertyGroupParent(() => {
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_AudioLinkEmissionBand, "色带");
                            me.ShaderProperty(_AudioLinkEmissionMultiplier, "强度");
                            MGUI.SliderMinMax(_AudioLinkRemapEmissionMin, _AudioLinkRemapEmissionMax, 0f, 2f, "重映射", 1);
                        });
                    });
                }
                Foldouts.EndFoldout();

                if (Foldouts.DoFoldout(foldouts, mat, "边缘光", Foldouts.Style.Thin)) {
                    MGUI.PropertyGroupParent(() => {
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_AudioLinkRimBand, "色带");
                            me.ShaderProperty(_AudioLinkRimMultiplier, "强度");
                            MGUI.SliderMinMax(_AudioLinkRemapRimMin, _AudioLinkRemapRimMax, 0f, 2f, "重映射", 1);
                            me.ShaderProperty(_AudioLinkRimWidth, "宽度");
                        });
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_AudioLinkRimPulse, "脉冲强度");
                            MGUI.SliderMinMax(_AudioLinkRemapRimPulseMin, _AudioLinkRemapRimPulseMax, 0f, 2f, "脉冲重映射", 1);
                            me.ShaderProperty(_AudioLinkRimPulseWidth, "脉冲宽度");
                            me.ShaderProperty(_AudioLinkRimPulseSharp, "脉冲锐度");
                        });
                    });
                }
                Foldouts.EndFoldout();

                if (Foldouts.DoFoldout(foldouts, mat, "基础颜色溶解 ", Foldouts.Style.Thin)) {
                    MGUI.PropertyGroupParent(() => {
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_AudioLinkBCDissolveBand, "色带");
                            me.ShaderProperty(_AudioLinkBCDissolveMultiplier, "强度");
                            MGUI.SliderMinMax(_AudioLinkRemapBCDissolveMin, _AudioLinkRemapBCDissolveMax, 0f, 2f, "重映射", 1);
                        });
                    });
                }
                Foldouts.EndFoldout();

                if (Foldouts.DoFoldout(foldouts, mat, "UV 扭曲 ", Foldouts.Style.Thin)) {
                    MGUI.PropertyGroupParent(() => {
                        MGUI.PropertyGroup(() => {
                            me.ShaderProperty(_AudioLinkUVDistortionBand, "色带");
                            me.ShaderProperty(_AudioLinkUVDistortionMultiplier, "强度");
                            MGUI.SliderMinMax(_AudioLinkRemapUVDistortionMin, _AudioLinkRemapUVDistortionMax, 0f, 2f, "重映射", 1);
                        });
                    });
                }
                Foldouts.EndFoldout();

                if (isOutline){
                    if (Foldouts.DoFoldout(foldouts, mat, "描边 ", Foldouts.Style.Thin)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_AudioLinkOutlineBand, "色带");
                                me.ShaderProperty(_AudioLinkOutlineMultiplier, "强度");
                                MGUI.SliderMinMax(_AudioLinkRemapOutlineMin, _AudioLinkRemapOutlineMax, 0f, 2f, "重映射", 1);
                            });
                        });
                    }
                    Foldouts.EndFoldout();
                }

                if (isUberX){
                    if (Foldouts.DoFoldout(foldouts, mat, "顶点操作 ", Foldouts.Style.Thin)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_AudioLinkVertManipBand, "色带");
                                me.ShaderProperty(_AudioLinkVertManipMultiplier, "强度");
                                MGUI.SliderMinMax(_AudioLinkRemapVertManipMin, _AudioLinkRemapVertManipMax, 0f, 2f, "重映射", 1);
                            });
                        });
                    }
                    Foldouts.EndFoldout();


                    if (Foldouts.DoFoldout(foldouts, mat, "溶解 ", Foldouts.Style.Thin)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_AudioLinkDissolveBand, "色带");
                                me.ShaderProperty(_AudioLinkDissolveMultiplier, "强度");
                                MGUI.SliderMinMax(_AudioLinkRemapDissolveMin, _AudioLinkRemapDissolveMax, 0f, 2f, "重映射", 1);
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    if (Foldouts.DoFoldout(foldouts, mat, "三角形偏移", Foldouts.Style.Thin)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_AudioLinkTriOffsetBand, "色带");
                                me.ShaderProperty(_AudioLinkTriOffsetMode, "样式");
                                me.ShaderProperty(_AudioLinkTriOffsetCoords, "轴");
                                me.TexturePropertySingleLine(Tips.maskLabel, _AudioLinkTriOffsetMask);
                                MGUI.TextureSOScroll(me, _AudioLinkTriOffsetMask, _AudioLinkTriOffsetMaskScroll, _AudioLinkTriOffsetMask.textureValue);
                            });
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_AudioLinkTriOffsetStrength, "强度");
                                MGUI.SliderMinMax(_AudioLinkRemapTriOffsetMin, _AudioLinkRemapTriOffsetMax, 0f, 2f, "重映射", 1);
                                me.ShaderProperty(_AudioLinkTriOffsetStartPos, "起始位置");
                                me.ShaderProperty(_AudioLinkTriOffsetEndPos, "结束位置");
                                me.ShaderProperty(_AudioLinkTriOffsetSize, "尺寸");
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    if (Foldouts.DoFoldout(foldouts, mat, "线框 ", Foldouts.Style.Thin)) {
                        MGUI.PropertyGroupParent(() => {
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_AudioLinkWireframeBand, "色带");
                                me.ShaderProperty(_AudioLinkWireframeMode, "样式");
                                me.ShaderProperty(_AudioLinkWireframeCoords, "轴");
                                me.TexturePropertySingleLine(Tips.maskLabel, _AudioLinkWireframeMask);
                                MGUI.TextureSOScroll(me, _AudioLinkWireframeMask, _AudioLinkWireframeMaskScroll, _AudioLinkWireframeMask.textureValue);
                            });
                            MGUI.PropertyGroup( () => {
                                me.ShaderProperty(_AudioLinkWireframeStrength, "强度");
                                MGUI.SliderMinMax(_AudioLinkRemapWireframeMin, _AudioLinkRemapWireframeMax, 0f, 2f, "重映射", 1);
                                me.ShaderProperty(_AudioLinkWireframeStartPos, "起始位置");
                                me.ShaderProperty(_AudioLinkWireframeEndPos, "结束位置");
                                me.ShaderProperty(_AudioLinkWireframeSize, "尺寸");
                                me.ShaderProperty(_AudioLinkWireframeColor, "颜色");
                            });
                        });
                    }
                    Foldouts.EndFoldout();
                    MGUI.Space6();
                }
                MGUI.ToggleGroupEnd();
            }
            Foldouts.EndFoldout();

            // -----------------
            // X Features
            // -----------------
            if (isUberX){

                if (Foldouts.DoFoldout(foldouts, mat, "特殊功能", Foldouts.Style.Standard)) {
                    MGUI.Space6();
                    if (Foldouts.DoFoldout(foldouts, mat, me, _DistanceFadeToggle, "距离渐隐", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_DistanceFadeToggle.floatValue == 0);
                            MGUI.DisplayInfo("需要非不透明混合模式才能运行。");
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_DFClones, "仅克隆");
                                if (_DistanceFadeToggle.floatValue != 2){
                                    me.ShaderProperty(_DistanceFadeMin, "范围");
                                    me.ShaderProperty(_ClipRimColor, "边缘光颜色");
                                    me.ShaderProperty(_ClipRimStr, "强度");
                                    me.ShaderProperty(_ClipRimWidth, "宽度");
                                }
                                else {
                                    me.ShaderProperty(_DistanceFadeMin, "最小范围");
                                    me.ShaderProperty(_DistanceFadeMax, "最大范围");
                                }
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    if (Foldouts.DoFoldout(foldouts, mat, me, _DissolveStyle, "溶解", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_DissolveStyle.floatValue == 0);
                            MGUI.DisplayInfo("贴图溶解和 Simplex 溶解需要非不透明混合模式才能生效。");
                            MGUI.PropertyGroup(() => {
                                if (_DissolveStyle.floatValue < 3){
                                    me.ShaderProperty(_DissolveAmount, "强度");
                                }
                                else {
                                    me.ShaderProperty(_GeomDissolveAxis, "轴");
                                    if (_GeomDissolveAxis.floatValue <= 2)
                                        me.ShaderProperty(_GeomDissolveAxisFlip, "反转轴");
                                    me.ShaderProperty(_GeomDissolveWireframe, "应用线框");
                                    me.ShaderProperty(_GeomDissolveClamp, "限制偏移方向");
                                    MGUI.ToggleGroup(_CloneToggle.floatValue == 0);
                                    me.ShaderProperty(_DissolveClones, "仅克隆");
                                    MGUI.ToggleGroupEnd();
                                    me.ShaderProperty(_GeomDissolveAmount, "裁剪位置");
                                    me.ShaderProperty(_GeomDissolveWidth, "衰减尺寸");
                                    if (_GeomDissolveAxis.floatValue > 2){
                                        MGUI.Vector3Field(_DissolvePoint0, "点 1", false);
                                        MGUI.Vector3Field(_DissolvePoint1, "点 2", false);
                                    }
                                    MGUI.Vector3Field(_GeomDissolveSpread, "偏移量", false);
                                    me.ShaderProperty(_GeomDissolveClip, "偏移裁剪");
                                    me.ShaderProperty(_GeomDissolveFilter, "偏移滤镜");
                                }

                                if (_DissolveStyle.floatValue < 3){
                                    if (_DissolveStyle.floatValue == 1)
                                        MGUI.ToggleSlider(me, "流动", _DissolveBlending, _DissolveBlendSpeed);
                                    else if (_DissolveStyle.floatValue == 2){
                                        me.ShaderProperty(_DissolveBlendSpeed, "生成速度");
                                        MGUI.Vector3Field(_DissolveNoiseScale, "噪波缩放", false);
                                    }
                                    MGUI.ToggleGroup(_CloneToggle.floatValue == 0);
                                    me.ShaderProperty(_DissolveClones, "仅克隆");
                                    MGUI.ToggleGroupEnd();
                                    me.TexturePropertySingleLine(Tips.maskLabel, _DissolveMask);
                                    if (_DissolveStyle.floatValue == 1){
                                        me.TexturePropertySingleLine(Tips.dissolveTexLabel, _DissolveTex);
                                        MGUI.TextureSOScroll(me, _DissolveTex, _DissolveScroll0, _DissolveTex.textureValue);
                                    }
                                    me.ShaderProperty(_DissolveRimCol, "边缘光颜色");
                                    me.ShaderProperty(_DissolveRimWidth, "边缘光宽度");
                                }
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Screenspace
                    if (Foldouts.DoFoldout(foldouts, mat, me, _Screenspace, "屏幕空间", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_Screenspace.floatValue == 0);
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_Range, "范围");
                                MGUI.Vector3Field(_Position, "位置", false);
                                Vector3 v = _Position.vectorValue;
                                v.z = Mathf.Clamp(v.z, 0, 10000);
                                _Position.vectorValue = v;
                                MGUI.Vector3Field(_Rotation, "旋转", false);
                                MGUI.DoResetButton(_Position, _Rotation, new Vector4(0,0,0.25f,0), new Vector4(0,0,0,0));
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Clones
                    if (Foldouts.DoFoldout(foldouts, mat, me, _CloneToggle, "克隆", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_CloneToggle.floatValue == 0);
                            MGUI.PropertyGroup(() => {
                                me.ShaderProperty(_Visibility, "启用");
                                MGUI.Vector3Field(_EntryPos, "偏移方向", false);
                                me.ShaderProperty(_SaturateEP, "限制偏移方向");
                                EditorGUI.BeginChangeCheck();
                                me.ShaderProperty(_ClonePattern, "图案预设");
                                if (EditorGUI.EndChangeCheck())
                                    ApplyClonePositions();
                                bool positionsFoldout = Foldouts.DoSmallFoldout(foldouts, mat, me, "克隆 ");
                                if (positionsFoldout){
                                    MGUI.PropertyGroup(() => {
                                        MGUI.Vector3FieldToggleW("克隆 1", (int)_Clone1.vectorValue.w, _Clone1);
                                        MGUI.Vector3FieldToggleW("克隆 2", (int)_Clone2.vectorValue.w, _Clone2);
                                        MGUI.Vector3FieldToggleW("克隆 3", (int)_Clone3.vectorValue.w, _Clone3);
                                        MGUI.Vector3FieldToggleW("克隆 4", (int)_Clone4.vectorValue.w, _Clone4);
                                        MGUI.Vector3FieldToggleW("克隆 5", (int)_Clone5.vectorValue.w, _Clone5);
                                        MGUI.Vector3FieldToggleW("克隆 6", (int)_Clone6.vectorValue.w, _Clone6);
                                        MGUI.Vector3FieldToggleW("克隆 7", (int)_Clone7.vectorValue.w, _Clone7);
                                        MGUI.Vector3FieldToggleW("克隆 8", (int)_Clone8.vectorValue.w, _Clone8);
                                        MGUI.SpaceN4();
                                    });
                                }
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Glitch
                    if (Foldouts.DoFoldout(foldouts, mat, me, _GlitchToggle, "故障效果", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.PropertyGroup(() => {
                                MGUI.ToggleGroup(_GlitchToggle.floatValue == 0);
                                MGUI.ToggleGroup(_CloneToggle.floatValue == 0);
                                me.ShaderProperty(_GlitchClones, "仅克隆");
                                MGUI.ToggleGroupEnd();
                                me.ShaderProperty(_Instability, "不稳定度");
                                me.ShaderProperty(_GlitchIntensity, "强度");
                                me.ShaderProperty(_GlitchFrequency, "频率");
                                MGUI.ToggleGroupEnd();
                            });
                        });
                    }
                    Foldouts.EndFoldout();

                    // Shatter Culling
                    if (Foldouts.DoFoldout(foldouts, mat, me, _ShatterToggle, "破碎剔除", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_ShatterToggle.floatValue == 0);
                            MGUI.PropertyGroup(() => {
                                MGUI.ToggleGroup(_CloneToggle.floatValue == 0);
                                me.ShaderProperty(_ShatterClones, "仅克隆");
                                MGUI.ToggleGroupEnd();
                                me.ShaderProperty(_ShatterSpread, "扩散");
                                me.ShaderProperty(_ShatterMin, "最小范围");
                                me.ShaderProperty(_ShatterMax, "最大范围");
                                me.ShaderProperty(_ShatterCull, "剔除范围");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();

                    // Wireframe
                    if (Foldouts.DoFoldout(foldouts, mat, me, _WireframeToggle, "线框", Foldouts.Style.ThinToggle)) {
                        MGUI.PropertyGroupParent(()=>{
                            MGUI.ToggleGroup(_WireframeToggle.floatValue == 0);
                            MGUI.PropertyGroup(() => {
                                MGUI.ToggleGroup(_CloneToggle.floatValue == 0);
                                me.ShaderProperty(_WFClones, "仅克隆");
                                MGUI.ToggleGroupEnd();
                                me.ShaderProperty(_WFMode, "图案");
                                me.ShaderProperty(_WFColor, "颜色");
                                me.ShaderProperty(_WFVisibility, "线框不透明度");
                                me.ShaderProperty(_WFFill, "填充不透明度");
                                if (isTransparent)
                                    me.ShaderProperty(_WireframeTransparency, "使用 Alpha");
                            });
                            MGUI.ToggleGroupEnd();
                        });
                    }
                    Foldouts.EndFoldout();
                    MGUI.Space6();
                }
                Foldouts.EndFoldout();
            }

            // -----------------
            // Rendering
            // -----------------
            if (Foldouts.DoFoldout(foldouts, mat, "渲染设置", Foldouts.Style.Standard)) {
                MGUI.PropertyGroupParent(()=>{
                    MGUI.BoldLabel("常规");
                    MGUI.PropertyGroup(() => {
                        me.RenderQueueField();
                        me.ShaderProperty(_CullingMode, Tips.cullingModeLabel);
                        me.ShaderProperty(_ZWrite, "Z 写入");
                        me.ShaderProperty(_ZTest, "Z 测试");
                        EditorGUI.BeginChangeCheck();
                        me.ShaderProperty(_VRCFallback, "后备着色器");
                        if (EditorGUI.EndChangeCheck())
                            SetFallback(mat);
                        me.ShaderProperty(_Hide, "不可见");
                    });
                    MGUI.BoldLabel("近裁剪");
                    MGUI.PropertyGroup(() => {
                        me.ShaderProperty(_NearClipToggle, Tips.nearClipLabel);
                        MGUI.Space4();
                        MGUI.ToggleGroup(_NearClipToggle.floatValue == 0);
                        me.TexturePropertySingleLine(Tips.maskLabel, _NearClipMask);
                        me.ShaderProperty(_NearClip, "范围");
                        MGUI.ToggleGroupEnd();
                    });
                    GUILayout.Space(23);
                    if (MGUI.MedTabButton(MGUI.resetLabel, 23f)){
                        DoStencilReset();
                    }
                    GUILayout.Space(-18);
                    MGUI.BoldLabel("模板");
                    MGUI.PropertyGroup(() => {
                        me.ShaderProperty(_StencilRef, "参考值");
                        me.ShaderProperty(_StencilPass, "Pass 操作");
                        me.ShaderProperty(_StencilFail, "失败操作");
                        me.ShaderProperty(_StencilZFail, "Z 失败操作");
                        me.ShaderProperty(_StencilCompare, "比较函数");
                        if (isOutline){
                            MGUI.ToggleGroup(_StencilToggle.floatValue == 0);
                            if (MGUI.SimpleButton("重新应用描边配置", MGUI.GetPropertyWidth(), EditorGUIUtility.labelWidth)){
                                ApplyOutlineStencilConfig(mat);
                            }
                            MGUI.ToggleGroupEnd();
                        }
                    });
                });
            }
            Foldouts.EndFoldout();

            if (isUberX){
                MGUI.Space10();
                MGUI.DisplayWarning("请注意，此着色器的 X 版本通常比非 X 版本性能开销更高。如果不使用“特殊功能”选项卡中的任何选项，强烈建议改用普通 Uber 着色器，因为两者在视觉上没有差异。");
            }
            MGUI.DoFooter(versionLabel, "Uber-Shader");
        }

        private void GradientToTexture(ref Texture2D tex){
            for (int x = 0; x < tex.width; x++){
                Color col = gradientObj.gradient.Evaluate((float)x / tex.width);
                for (int y = 0; y < tex.height; y++) tex.SetPixel(x, y, col);
            }
            tex.Apply();
        }

        private void IridescenceGradientToTexture(ref Texture2D tex){
            for (int x = 0; x < tex.width; x++){
                Color col = iridescenceGradientObj.gradient.Evaluate((float)x / tex.width);
                for (int y = 0; y < tex.height; y++) tex.SetPixel(x, y, col);
            }
            tex.Apply();
        }

        void SetKeyword(Material m, string keyword, bool state) {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }

        // The packed workflow applies occlusion and height strength itself and reads metallic and roughness
        // unscaled, so maps are packed as-is and only the constants for missing maps are baked.
        // Separate occlusion maps are sampled from green, everything else from red.
        static bool PackPBRTextures(Material mat){
            return TexturePacker.PackTextures(mat, "_PackedMap",
                new TexturePacker.Channel(mat.GetTexture("_OcclusionMap"), 1),
                new TexturePacker.Channel(mat.GetTexture("_SpecGlossMap"), 0, 1f, mat.GetFloat("_Glossiness")),
                new TexturePacker.Channel(mat.GetTexture("_MetallicGlossMap"), 0, 1f, mat.GetFloat("_Metallic")),
                new TexturePacker.Channel(mat.GetTexture("_ParallaxMap"), 0)
            ) != null;
        }

        void ApplyMaterialSettings(Material mat){
            int renderMode = mat.GetInt("_RenderMode");
            int blendMode = mat.GetInt("_BlendMode");
            int cullingMode = mat.GetInt("_CullingMode");
            int cubeMode = mat.GetInt("_CubeMode");
            int maskingMode = mat.GetInt("_MaskingMode");
            int distortMode = mat.GetInt("_DistortionStyle");
            int reflToggle = mat.GetInt("_Reflections");
            int specToggle = mat.GetInt("_Specular");
            int filterToggle = mat.GetInt("_Filtering");
            int workflow = mat.GetInt("_PBRWorkflow");
            int ssr = mat.GetInt("_SSR");
            int pulseToggle = mat.GetInt("_PulseToggle");
            int emissToggle = mat.GetInt("_EmissionToggle");
            int dissolveStyle = mat.GetInt("_DissolveStyle");
            int matcapToggle = mat.GetInt("_MatcapToggle");
            int eRimToggle = mat.GetInt("_EnvironmentRim");
            int spriteToggle0 = mat.GetInt("_EnableSpritesheet");
            int spriteToggle1 = mat.GetInt("_EnableSpritesheet1");
            int postFilterToggle = mat.GetInt("_PostFiltering");
            int stencilToggle = mat.GetInt("_StencilToggle");
            int screenspace = mat.GetInt("_Screenspace");
            int cloneToggle = mat.GetInt("_CloneToggle");
            int dissolveWireframe = mat.GetInt("_GeomDissolveWireframe");
            int refracToggle = mat.GetInt("_Refraction");
            int caToggle = mat.GetInt("_RefractionCA");
            int vManipToggle = mat.GetInt("_VertexManipulationToggle");
            int maskTransToggle = mat.GetInt("_EnableMaskTransform");
            int bcDissToggle = mat.GetInt("_BCDissolveToggle");
            int audioLinkToggle = mat.GetInt("_AudioLinkToggle");
            int rimToggle = mat.GetInt("_RimLighting");
            int subsurfToggle = mat.GetInt("_Subsurface");
            int blurToggle = mat.GetInt("_RefractionBlur");
            bool reflFallback = mat.GetTexture("_ReflCube");
            bool matcapNormal0 = mat.GetTexture("_MatcapNormal0");
            bool matcapNormal1 = mat.GetTexture("_MatcapNormal1");
            bool isUberX = MGUI.IsXVersion(mat);
            bool isOutline = MGUI.IsOutline(mat);
            bool usingNormal = mat.GetTexture("_BumpMap");
            bool usingParallax = workflow < 3 ? mat.GetTexture("_ParallaxMap") : (mat.GetTexture("_PackedMap") && mat.GetInt("_EnablePackedHeight") == 1);
            bool usingDetail = mat.GetTexture("_DetailNormalMap");
            bool usingDetailRough = mat.GetTexture("_DetailRoughnessMap");
            bool usingDetailOcc = mat.GetTexture("_DetailOcclusionMap");
            bool usingDetailAlbedo = mat.GetTexture("_DetailAlbedoMap");
            bool usingDetailMetallic = mat.GetTexture("_DetailMetallic");

            // Setting floats based on render mode/texture presence/etc
            mat.SetInt("_IsCubeBlendMask", mat.GetTexture("_CubeBlendMask") ? 1 : 0);
            mat.SetInt("_UseSmoothMap", mat.GetTexture("_SmoothnessMap") && workflow == 1 ? 1 : 0);
            mat.SetInt("_UseMatcap1", mat.GetTexture("_Matcap1") ? 1 : 0);
            mat.SetInt("_ATM", blendMode == 1 ? 1 : 0);

            // Sync the outline stencil settings with base pass stencil settings when not using stencil mode
            if (isOutline && stencilToggle == 0){
                mat.SetFloat("_OutlineStencilRef", mat.GetFloat("_StencilRef"));
                mat.SetFloat("_OutlineStencilPass", mat.GetFloat("_StencilPass"));
                mat.SetFloat("_OutlineStencilFail", mat.GetFloat("_StencilFail"));
                mat.SetFloat("_OutlineStencilZFail", mat.GetFloat("_StencilZFail"));
                mat.SetFloat("_OutlineStencilCompare", mat.GetFloat("_StencilCompare"));
            }

            // Force backface culling off if using screenspace mesh
            if (isUberX && screenspace == 1 && cullingMode != 0)
                mat.SetInt("_CullingMode", 0);

            // Outline should have culling disabled for stencils to look good at mask cutoff points
            if (stencilToggle == 1)
                mat.SetInt("_OutlineCulling", 0);
            else
                mat.SetInt("_OutlineCulling", 1);

            if (dissolveWireframe == 1 && isUberX && dissolveStyle == 3)
                mat.SetInt("_WireframeToggle", 1);

            // Use metallic or specular map based on workflow choice
            if (workflow >= 3){
                mat.SetInt("_UseMetallicMap", 1);
                mat.SetInt("_UseSpecMap", 1);
            }
            else {
                mat.SetInt("_UseMetallicMap", mat.GetTexture("_MetallicGlossMap") ? 1 : 0);
                mat.SetInt("_UseSpecMap", mat.GetTexture("_SpecGlossMap") ? 1 : 0);
            }

            // Handling some jank with PBR filter preview options
            if (workflow != 1 && workflow != 2)
                mat.SetInt("_PreviewSmooth", 0);
            else
                mat.SetInt("_PreviewRough", 0);

            if (workflow >= 3 && _RoughnessFiltering.floatValue == 1 && _PreviewRough.floatValue == 1)
                mat.SetInt("_PackedRoughPreview", 1);
            else
                mat.SetInt("_PackedRoughPreview", 0);

            if (workflow >= 3){
                if (!mat.GetTexture("_PackedMap")){
                    mat.SetInt("_AOFiltering", 0);
                }
            }
            else {
                if (!mat.GetTexture("_OcclusionMap"))
                    mat.SetInt("_AOFiltering", 0);
            }

            // Toggle detail maps so strength params can be 1 by default without messing up lerps
            if (usingDetailAlbedo)
                mat.SetInt("_UsingDetailAlbedo", 1);
            else
                mat.SetInt("_UsingDetailAlbedo", 0);

            if (usingDetailRough)
                mat.SetInt("_UsingDetailRough", 1);
            else
                mat.SetInt("_UsingDetailRough", 0);

            if (usingDetailOcc)
                mat.SetInt("_UsingDetailOcclusion", 1);
            else
                mat.SetInt("_UsingDetailOcclusion", 0);

            if (usingDetailMetallic)
                mat.SetInt("_UsingDetailMetallic", 1);
            else
                mat.SetInt("_UsingDetailMetallic", 0);

            // Matcap normal maps
            if (matcapNormal0)
                mat.SetInt("_MatcapNormal0Toggle", 1);
            else
                mat.SetInt("_MatcapNormal0Toggle", 0);

            if (matcapNormal1)
                mat.SetInt("_MatcapNormal1Toggle", 1);
            else
                mat.SetInt("_MatcapNormal1Toggle", 0);

            bool prevAO = mat.GetInt("_AOFiltering") == 1 && mat.GetInt("_PreviewAO") == 1;
            bool prevRough = mat.GetInt("_RoughnessFiltering") == 1 && mat.GetInt("_PreviewRough") == 1;
            prevRough = prevRough && (workflow == 0 || workflow >= 3);
            bool prevSmooth = mat.GetInt("_SmoothnessFiltering") == 1 && mat.GetInt("_PreviewSmooth") == 1;
            prevSmooth = prevSmooth && (workflow == 1 || workflow == 2);
            bool prevHeight = mat.GetInt("_HeightFiltering") == 1 && mat.GetInt("_PreviewHeight") == 1;
            bool prevMetal = mat.GetInt("_MetallicFiltering") == 1 && mat.GetInt("_PreviewMetallic") == 1;
            prevMetal = prevMetal && (workflow == 0 || workflow >= 3);

            // Begone grabpass
            bool ssrEnabled = ssr == 1 && reflToggle == 1 && renderMode > 0;
            bool refracEnabled = refracToggle == 1 && renderMode > 0;
            mat.SetShaderPassEnabled("Always", ssrEnabled || refracEnabled);

            SetKeyword(mat, "_PACKED_WORKFLOW_ON", workflow >= 3 && renderMode > 0);
            SetKeyword(mat, "_SPECULAR_WORKFLOW_ON", (workflow == 1 || workflow == 2) && renderMode > 0);
            SetKeyword(mat, "_REFLECTIONS_ON", reflToggle > 0 && renderMode > 0);
            SetKeyword(mat, "_SPECULAR_ON", specToggle > 0 && renderMode > 0);
            SetKeyword(mat, "_CUBEMAP_ON", cubeMode == 1);
            SetKeyword(mat, "_CUBEMAP_COMBINED_ON",  cubeMode == 2);
            SetKeyword(mat, "_PACKED_MASKING_ON", maskingMode == 2);
            SetKeyword(mat, "_SEPARATE_MASKING_ON", maskingMode == 1);
            SetKeyword(mat, "_SHADING_ON", renderMode == 1);
            SetKeyword(mat, "_SPECULAR_ANISO_ON", specToggle == 2 && renderMode > 0);
            SetKeyword(mat, "_SPECULAR_COMBINED_ON", specToggle == 3 && renderMode > 0);
            SetKeyword(mat, "_PBR_PREVIEW_ON", (prevAO || prevRough || prevSmooth || prevHeight || prevMetal) && renderMode > 0);
            SetKeyword(mat, "_UV_DISTORTION_ON", distortMode > 0);
            SetKeyword(mat, "_UV_DISTORTION_NORMALMAP_ON", distortMode == 1);
            SetKeyword(mat, "_FILTERING_ON", filterToggle == 1);
            SetKeyword(mat, "_POST_FILTERING_ON", filterToggle == 1 && postFilterToggle == 1);
            SetKeyword(mat, "_DETAIL_NORMALMAP_ON", usingDetail && renderMode > 0);
            SetKeyword(mat, "_PARALLAXMAP_ON",  usingParallax && renderMode > 0);
            SetKeyword(mat, "_NORMALMAP_ON", usingNormal && renderMode > 0);
            SetKeyword(mat, "_EMISSION_ON", emissToggle == 1);
            SetKeyword(mat, "_ALPHATEST_ON", blendMode > 0 && blendMode < 4);
            SetKeyword(mat, "_ALPHABLEND_ON", blendMode == 4);
            SetKeyword(mat, "_ALPHAPREMULTIPLY_ON", blendMode == 5);
            SetKeyword(mat, "_SCREENSPACE_REFLECTIONS_ON", ssrEnabled);
            SetKeyword(mat, "_PULSE_ON", emissToggle == 1 && pulseToggle == 1);
            SetKeyword(mat, "_CLONES_ON", isUberX && cloneToggle == 1);
            SetKeyword(mat, "_DISSOLVE_TEXTURE_ON", dissolveStyle == 1 && isUberX);
            SetKeyword(mat, "_DISSOLVE_SIMPLEX_ON", dissolveStyle == 2 && isUberX);
            SetKeyword(mat, "_DISSOLVE_GEOMETRY_ON", dissolveStyle == 3 && isUberX);
            SetKeyword(mat, "_MATCAP_ON", matcapToggle == 1 && renderMode > 0);
            SetKeyword(mat, "_ENVIRONMENT_RIM_ON", eRimToggle == 1 && renderMode > 0);
            SetKeyword(mat, "_FLIPBOOK_ON", spriteToggle0 == 1 || spriteToggle1 == 1);
            SetKeyword(mat, "_CUBEMAP_REFLECTIONS_ON", reflToggle == 2 && renderMode > 0);
            SetKeyword(mat, "_REFRACTION_ON", refracEnabled);
            SetKeyword(mat, "_CHROMATIC_ABBERATION_ON", refracToggle == 1 && caToggle == 1 && renderMode > 0);
            SetKeyword(mat, "_VERTEX_MANIP_ON", vManipToggle == 1);
            SetKeyword(mat, "_MASK_TRANSFORMS_ON", maskingMode == 1 && maskTransToggle == 1);
            SetKeyword(mat, "_REFLECTION_FALLBACK_ON", reflFallback && reflToggle > 0 && renderMode > 0);
            SetKeyword(mat, "_BASECOLOR_DISSOLVE_ON", bcDissToggle == 1 && renderMode > 0);
            SetKeyword(mat, "_AUDIOLINK_ON", audioLinkToggle == 1);
            SetKeyword(mat, "_RIM_ON", rimToggle == 1 && renderMode > 0);
            SetKeyword(mat, "_SUBSURFACE_ON", subsurfToggle == 1 && renderMode > 0);
        }

        void SetBlendMode(Material mat){
            int blendMode = mat.GetInt("_BlendMode");
            switch (blendMode){
                case 0: // Opaque
                    mat.SetOverrideTag("RenderType", "Opaque");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.renderQueue = -1;
                    break;
                case 1: // Cutout
                case 2:	// Dithered
                case 3:	// Alpha to Coverage
                    mat.SetOverrideTag("RenderType", "TransparentCutout");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case 4: // Fade
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case 5: // Transparent
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                default: break;
            }
        }

        void SetFallback(Material mat){
            int fallback = mat.GetInt("_VRCFallback");
            int blendMode = mat.GetInt("_BlendMode");
            string tag = "";
            switch (fallback){
                case 0: tag = "Unlit"; break;
                case 1: tag = "Toon"; break;
                case 2: tag = "Particle"; break;
                case 3: tag = "Matcap"; break;
                case 4: tag = "Sprite"; break;
                case 5: tag = "DoubleSided"; break;
                case 6: tag = "Hidden"; break;
                default: break;
            }
            if (fallback != 6){
                switch (blendMode){
                    case 1: // Cutout
                    case 2:	// Dithered
                    case 3:	// Alpha to Coverage
                        tag += "Cutout";
                        break;
                    case 4: tag += "Fade"; break;
                    case 5: tag += "Transparent"; break;
                    default: break;
                }
            }
            mat.SetOverrideTag("VRCFallback", tag);
        }

        void ApplyOutlineStencilConfig(Material mat){
            mat.SetFloat("_StencilPass", (float)UnityEngine.Rendering.StencilOp.Replace);
            mat.SetFloat("_StencilFail", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_StencilZFail", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_StencilCompare", (float)UnityEngine.Rendering.CompareFunction.Always);

            mat.SetFloat("_OutlineStencilPass", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_OutlineStencilFail", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_OutlineStencilZFail", (float)UnityEngine.Rendering.StencilOp.Keep);
            mat.SetFloat("_OutlineStencilCompare", (float)UnityEngine.Rendering.CompareFunction.NotEqual);
        }

        public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader) {
            if (mat.HasProperty("_Emission"))
                mat.SetColor("_EmissionColor", mat.GetColor("_Emission"));
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
            MGUI.ClearKeywords(mat);
            ApplyMaterialSettings(mat);
            SetBlendMode(mat);
        }

        void ApplyClonePositions(){

            // Diamond
            if (_ClonePattern.floatValue == 0){
                _Clone1.vectorValue = new Vector4(1,0,0,_Clone1.vectorValue.w);
                _Clone2.vectorValue = new Vector4(-1,0,0,_Clone2.vectorValue.w);
                _Clone3.vectorValue = new Vector4(0,0,1,_Clone3.vectorValue.w);
                _Clone4.vectorValue = new Vector4(0,0,-1,_Clone4.vectorValue.w);
                _Clone5.vectorValue = new Vector4(0.5f,0,0.5f,_Clone5.vectorValue.w);
                _Clone6.vectorValue = new Vector4(-0.5f,0,0.5f,_Clone6.vectorValue.w);
                _Clone7.vectorValue = new Vector4(0.5f,0,-0.5f,_Clone7.vectorValue.w);
                _Clone8.vectorValue = new Vector4(-0.5f,0,-0.5f,_Clone8.vectorValue.w);
            }

            // Pyramid
            else if (_ClonePattern.floatValue == 1){
                _Clone1.vectorValue = new Vector4(0,2.5f,0,_Clone1.vectorValue.w);
                _Clone2.vectorValue = new Vector4(0,1.25f,0,_Clone2.vectorValue.w);
                _Clone3.vectorValue = new Vector4(0,0.75f,0.25f,_Clone3.vectorValue.w);
                _Clone4.vectorValue = new Vector4(0,0.75f,-0.25f,_Clone4.vectorValue.w);
                _Clone5.vectorValue = new Vector4(0.25f,0,0.4f,_Clone5.vectorValue.w);
                _Clone6.vectorValue = new Vector4(-0.25f,0,0.4f,_Clone6.vectorValue.w);
                _Clone7.vectorValue = new Vector4(0.25f,0,-0.4f,_Clone7.vectorValue.w);
                _Clone8.vectorValue = new Vector4(-0.25f,0,-0.4f,_Clone8.vectorValue.w);
            }

            // Stack
            else if (_ClonePattern.floatValue == 2){
                _Clone1.vectorValue = new Vector4(0,1,0,_Clone1.vectorValue.w);
                _Clone2.vectorValue = new Vector4(0,2,0,_Clone2.vectorValue.w);
                _Clone3.vectorValue = new Vector4(0,3,0,_Clone3.vectorValue.w);
                _Clone4.vectorValue = new Vector4(0,4,0,_Clone4.vectorValue.w);
                _Clone5.vectorValue = new Vector4(0,5,0,_Clone5.vectorValue.w);
                _Clone6.vectorValue = new Vector4(0,6,0,_Clone6.vectorValue.w);
                _Clone7.vectorValue = new Vector4(0,7,0,_Clone7.vectorValue.w);
                _Clone8.vectorValue = new Vector4(0,8,0,_Clone8.vectorValue.w);
            }
        }

        void DoStandardLighting(Material mat){
            _DisneyDiffuse.floatValue = 0f;
            _SHStr.floatValue = 1f;
            _NonlinearSHToggle.floatValue = 0f;
            _DirectCont.floatValue = 0.6f;
            _IndirectCont.floatValue = 0.4f;
            _RTDirectCont.floatValue = 1f;
            _RTIndirectCont.floatValue = 1f;
            _ClampAdditive.floatValue = 0f;
            _Reflections.floatValue = 1f;
            _ReflectionStr.floatValue = 1f;
            _ReflCol.colorValue = Color.white;
            _Specular.floatValue = 1f;
            _SpecStr.floatValue = 1f;
            _SpecCol.colorValue = Color.white;
            _SharpSpecular.floatValue = 0f;
            _ColorPreservation.floatValue = 0f;
            _ShadowStr.floatValue = 1f;
            _ShadowMode.floatValue = 1f;
            _RampWidth0.floatValue = 1f;
            _RampWidth1.floatValue = 1f;
            _RampWeight.floatValue = 0f;
            _RTSelfShadow.floatValue = 1f;
            _AttenSmoothing.floatValue = 0f;
            _ShadowDithering.floatValue = 0f;
            _ShadowConditions.floatValue = 1f;
            _DirectAO.floatValue = 1f;
            _IndirectAO.floatValue = 0f;
            _LightingBasedIOR.floatValue = 0f;
            _RealtimeSpec.floatValue = 1;
        }

        void DoAdvancedReset(){
            DoRenderingReset();
            DoStencilReset();
        }

        void DoRenderingReset(){
            if (_BlendMode.floatValue != 5)
                _ZWrite.floatValue = 1f;
            else
                _ZWrite.floatValue = 0f;
            _ZTest.floatValue = 4f;
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
    }
}
