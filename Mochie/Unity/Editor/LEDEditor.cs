using System.Reflection;
using UnityEditor;
using UnityEngine;
using Mochie;

namespace Mochie {

    public class LEDEditor : ShaderGUI {

        public static GUIContent mainTex = new GUIContent("主贴图", "主要贴图，用于驱动自发光。");
        public static GUIContent RGBMatrixTex = new GUIContent("RGB 矩阵贴图", "RGB 像素布局图案。");
        public static GUIContent smoothTex = new GUIContent("粗糙度");
        public static GUIContent flipbookTex = new GUIContent("序列帧");
        string versionLabel = "v1.4.2 | LGC 中文版 1.78.1";
        bool isTransparent = false;
        MaterialProperty _MainTex = null;
        MaterialProperty _RGBSubPixelTex = null;
        MaterialProperty _EmissionIntensity = null;
        MaterialProperty _Glossiness = null;
        MaterialProperty _LightmapEmissionScale = null;
        MaterialProperty _Backlight = null;
        MaterialProperty _SpecGlossMap = null;
        MaterialProperty _UVScroll = null;
        MaterialProperty _BoostAmount = null;
        MaterialProperty _BoostThreshold = null;
        MaterialProperty _FlipbookMode = null;
        MaterialProperty _Flipbook = null;
        MaterialProperty _FPS = null;
        MaterialProperty _Color = null;
        MaterialProperty _ZWrite = null;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public override void OnGUI(MaterialEditor me, MaterialProperty[] props){
            Material material = (Material)me.target;
            isTransparent = material.shader.name.Contains("(Transparent)");
            foreach (var property in GetType().GetFields(bindingFlags)){
                if (property.FieldType == typeof(MaterialProperty)){
                    property.SetValue(this, FindProperty(property.Name, props));
                }
            }

            EditorGUI.BeginChangeCheck(); {
                MGUI.BoldLabel("图像");
                MGUI.Space2();
                if (_FlipbookMode.floatValue == 0){
                    me.TexturePropertySingleLine(mainTex, _MainTex, _FlipbookMode);
                    MGUI.TexPropLabel("序列帧", 105, false);
                    me.ShaderProperty(_Color, "颜色");
                    if (_MainTex.textureValue){
                        MGUI.TextureSOScroll(me, _MainTex, _UVScroll);
                        MGUI.Space6();
                    }
                }
                else {
                    me.TexturePropertySingleLine(flipbookTex, _Flipbook, _FlipbookMode);
                    MGUI.TexPropLabel("序列帧", 105, false);
                    if (_Flipbook.textureValue){
                        MGUI.TextureSO(me, _MainTex);
                        MGUI.SpaceN2();
                        me.ShaderProperty(_FPS, "FPS");
                        MGUI.Space6();
                    }
                }
                MGUI.SetKeyword(material, "_FLIPBOOK_MODE", material.GetInt("_FlipbookMode") == 1);
                me.TexturePropertySingleLine(smoothTex, _SpecGlossMap, _Glossiness);
                MGUI.TextureSO(me, _SpecGlossMap, _SpecGlossMap.textureValue);
                MGUI.SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
                MGUI.Space4();

                MGUI.SetKeyword(material, "_EMISSION", true);
                me.ShaderProperty(_EmissionIntensity, "自发光强度");
                me.ShaderProperty(_LightmapEmissionScale, "光照贴图自发光强度");
                me.ShaderProperty(_BoostAmount, "增强倍率");
                me.ShaderProperty(_BoostThreshold, "增强阈值");
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                MGUI.Space4();

                MGUI.BoldLabel("面板");
                MGUI.Space2();
                me.TexturePropertySingleLine(RGBMatrixTex, _RGBSubPixelTex);
                MGUI.TextureSO(me, _RGBSubPixelTex, _RGBSubPixelTex.textureValue);
                me.ShaderProperty(_Backlight, "背光");
                MGUI.Space4();
                MGUI.BoldLabel("渲染设置");
                if (isTransparent){
                    me.ShaderProperty(_ZWrite, "Z 写入");
                }
                me.RenderQueueField();
                MGUI.Space8();
            }

            MGUI.DoFooter(versionLabel, "LED-Screen-Shader");
        }

        public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader) {
            if (mat.HasProperty("_Glossiness"))
                mat.SetFloat("_Glossiness", 0.035f);
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
        }
    }
}