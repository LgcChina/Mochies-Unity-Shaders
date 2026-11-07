using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Mochie;

namespace Mochie {

    public class UnderwaterEditor : ShaderGUI {

        GUIContent causticsTexLabel = new GUIContent("焦散贴图");
        GUIContent causticsFlipbookLabel = new GUIContent("焦散序列帧");
        GUIContent distortionTexLabel = new GUIContent("扭曲贴图");

        public static Dictionary<Material, Toggles> foldouts = new Dictionary<Material, Toggles>();
        Toggles toggles = new Toggles(new string[] {
            "基础",
            "模糊",
            "焦散",
            "雾"
        }, 0);

        string versionLabel = "v1.3 | LGC 中文版 1.78.3";

        // Base
        MaterialProperty _StencilRef = null;

        // Depth of Field
        MaterialProperty _DoFToggle = null;
        MaterialProperty _DepthToggle = null;
        MaterialProperty _HQBlur = null;
        MaterialProperty _BlurStr = null;
        MaterialProperty _Radius = null;
        MaterialProperty _Fade = null;
        MaterialProperty _Color = null;
        MaterialProperty _AutoShift = null;
        MaterialProperty _AutoShiftSpeed = null;
        MaterialProperty _Hue = null;
        MaterialProperty _HueMode = null;
        MaterialProperty _MonoTint = null;

        // Caustics
        MaterialProperty _CausticsTex = null;
        MaterialProperty _CausticsToggle = null;
        MaterialProperty _CausticsMode = null;
        MaterialProperty _CausticsOpacity = null;
        MaterialProperty _CausticsScale = null;
        MaterialProperty _CausticsSpeed = null;
        MaterialProperty _CausticsDisp = null;
        MaterialProperty _CausticsDistortion = null;
        MaterialProperty _CausticsDistortionTex = null;
        MaterialProperty _CausticsDistortionScale = null;
        MaterialProperty _CausticsDistortionSpeed = null;
        MaterialProperty _CausticsRotateWithLight = null;
        MaterialProperty _CausticsRotation = null;
        MaterialProperty _CausticsColor = null;
        MaterialProperty _CausticsPower = null;
        MaterialProperty _CausticsTexArray = null;
        MaterialProperty _CausticsFlipbookSpeed = null;
        MaterialProperty _CausticsFlipbookDisp = null;
        MaterialProperty _CausticsRange = null;
        MaterialProperty _CausticsFade = null;

        // Fog
        MaterialProperty _FogToggle = null;
        MaterialProperty _FogTint = null;
        MaterialProperty _FogOpacity = null;
        MaterialProperty _FogRadius = null;
        MaterialProperty _FogFade = null;

        // Falloff
        MaterialProperty _Falloff = null;
        MaterialProperty _BoxSize = null;
        MaterialProperty _BoxOffset = null;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public override void OnGUI(MaterialEditor me, MaterialProperty[] props) {
            if (!me.isVisible)
                return;

            foreach (var property in GetType().GetFields(bindingFlags)){
                if (property.FieldType == typeof(MaterialProperty))
                    property.SetValue(this, FindProperty(property.Name, props));
            }
            Material mat = (Material)me.target;

            if (!foldouts.ContainsKey(mat))
                foldouts.Add(mat, toggles);

            if (mat.GetInt("_MaterialResetCheck") == 0){
                mat.SetInt("_MaterialResetCheck", 1);
                ApplyMaterialSettings(mat);
            }

            MGUI.DoHeader("水下");

            EditorGUI.BeginChangeCheck(); {

                // Base
                Foldouts.DoFoldout(foldouts, mat, "基础", 1, Foldouts.Style.StandardButton);
                if (Foldouts.DoFoldoutButton(MGUI.collapseLabel, 11)) Toggles.CollapseFoldouts(mat, foldouts, 1);
                if (Foldouts.BeginFoldout(foldouts, mat, "基础")) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.PropertyGroup(()=>{
                            me.RenderQueueField();
                            me.ShaderProperty(_StencilRef, "模板参考值");
                            me.ShaderProperty(_Falloff, "衰减");
                            if (_Falloff.floatValue == 1f){
                                MGUI.Vector3Field(_BoxSize, "尺寸", false);
                                MGUI.Vector3Field(_BoxOffset, "偏移", false);
                            }
                        });
                        MGUI.DisplayInfo("   该着色器需要场景中存在“Depth Light”预制件。\n （发现于：VPM 包内的 Unity/Prefabs 目录）");
                    });
                }
                Foldouts.EndFoldout();

                // Blur
                if (Foldouts.DoFoldout(foldouts, mat, me, _DoFToggle, "模糊", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_DoFToggle.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_BlurStr, "强度");
                            me.ShaderProperty(_HQBlur, "高质量");
                            me.ShaderProperty(_DepthToggle, "景深");
                            if (_DepthToggle.floatValue == 1){
                                me.ShaderProperty(_Radius, "半径");
                                me.ShaderProperty(_Fade, "渐隐");
                            }
                        });
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_Color, "屏幕色调");
                            if (_AutoShift.floatValue == 0)
                                me.ShaderProperty(_Hue, "色调");
                            else
                                me.ShaderProperty(_AutoShiftSpeed, "偏移速度");
                            me.ShaderProperty(_HueMode, Tips.hueModeText);
                            me.ShaderProperty(_MonoTint, Tips.monoTintText);
                            me.ShaderProperty(_AutoShift, "自动色调偏移");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Caustics
                if (Foldouts.DoFoldout(foldouts, mat, me, _CausticsToggle, "焦散", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_CausticsToggle.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_CausticsMode, "样式");
                            if (_CausticsMode.floatValue == 1){
                                me.TexturePropertySingleLine(causticsTexLabel, _CausticsTex);
                            }
                            else if (_CausticsMode.floatValue == 2){
                                me.TexturePropertySingleLine(causticsFlipbookLabel, _CausticsTexArray);
                            }
                            me.ShaderProperty(_CausticsColor, "颜色");
                            me.ShaderProperty(_CausticsOpacity, "强度");
                            me.ShaderProperty(_CausticsRange, "范围");
                            me.ShaderProperty(_CausticsFade, "渐隐");

                            if (_CausticsMode.floatValue == 0){
                                me.ShaderProperty(_CausticsPower, "强度");
                                me.ShaderProperty(_CausticsDisp, "色散");
                            }
                            if (_CausticsMode.floatValue != 2){
                                me.ShaderProperty(_CausticsSpeed, "速度");
                                me.ShaderProperty(_CausticsScale, "缩放");
                            }
                            else {
                                me.ShaderProperty(_CausticsFlipbookSpeed, "速度");
                                me.ShaderProperty(_CausticsScale, "缩放");
                                me.ShaderProperty(_CausticsFlipbookDisp, "色散");
                            }

                            MGUI.ToggleGroup(_CausticsRotateWithLight.floatValue == 1);
                            MGUI.Vector3Field(_CausticsRotation, Tips.causticsRotation, false);
                            MGUI.ToggleGroupEnd();
                            me.ShaderProperty(_CausticsRotateWithLight, Tips.causticsRotateWithLight);
                        });
                        if (_CausticsMode.floatValue != 2){
                            MGUI.PropertyGroup(()=>{
                                me.TexturePropertySingleLine(distortionTexLabel, _CausticsDistortionTex);
                                me.ShaderProperty(_CausticsDistortion, "扭曲强度");
                                me.ShaderProperty(_CausticsDistortionScale, "扭曲缩放");
                                MGUI.Vector2Field(_CausticsDistortionSpeed, "扭曲速度");
                            });
                        }
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

                // Fog
                if (Foldouts.DoFoldout(foldouts, mat, me, _FogToggle, "雾", Foldouts.Style.StandardToggle)) {
                    MGUI.PropertyGroupParent(()=>{
                        MGUI.ToggleGroup(_FogToggle.floatValue == 0);
                        MGUI.PropertyGroup(()=>{
                            me.ShaderProperty(_FogTint, "颜色");
                            me.ShaderProperty(_FogOpacity, "不透明度");
                            me.ShaderProperty(_FogRadius, "半径");
                            me.ShaderProperty(_FogFade, "渐隐");
                        });
                        MGUI.ToggleGroupEnd();
                    });
                }
                Foldouts.EndFoldout();

            }
            ApplyMaterialSettings(mat);
            MGUI.DoFooter(versionLabel, "Underwater-Visuals");
        }

        public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader) {
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);
            MGUI.ClearKeywords(mat);
            ApplyMaterialSettings(mat);
        }

        void ApplyMaterialSettings(Material mat){
            mat.SetShaderPassEnabled("Always", mat.GetInt("_DoFToggle") == 1);
            int causticsMode = mat.GetInt("_CausticsMode");
            MGUI.SetKeyword(mat, "_CAUSTICS_VORONOI_ON", causticsMode == 0);
            MGUI.SetKeyword(mat, "_CAUSTICS_TEXTURE_ON", causticsMode == 1);
            MGUI.SetKeyword(mat, "_CAUSTICS_FLIPBOOK_ON", causticsMode == 2);
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmo(MeshRenderer meshRenderer, GizmoType gizmoType){
            if (meshRenderer.sharedMaterial != null){
                Material material = meshRenderer.sharedMaterial;
                if (!material.shader.name.Contains("Mochie/Underwater Visuals")) return;
                if (material.GetInt("_Falloff") == 1){
                    Vector3 position = meshRenderer.transform.position;
                    Matrix4x4 oldMatrix = Gizmos.matrix;
                    Transform t = meshRenderer.transform;
                    Vector3 boxOffset = material.GetVector("_BoxOffset");
                    Vector3 boxSize = material.GetVector("_BoxSize");
                    Vector3 center = position + t.rotation * boxOffset;
                    Gizmos.matrix = Matrix4x4.TRS(center, t.rotation, Vector3.one);

                    Gizmos.color = new Color(0.2f, 0.6f, 1f, 1f);
                    Gizmos.DrawWireCube(Vector3.zero, boxSize);

                    Gizmos.matrix = oldMatrix;
                }
            }
        }
    }
}
