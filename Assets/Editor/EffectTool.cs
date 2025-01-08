using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using static MySampleEx.GlobalDefine;
using System.Text;

namespace MySampleEx
{
    public class EffectTool : EditorWindow
    {
        #region Variables
        //UI에 필요한 변수
        public int uiWidthLarge = 300;
        public int uiWidthMiddle = 200;
        private Vector2 sp1 = Vector2.zero;
        private Vector2 sp2 = Vector2.zero;

        private int selection = 0;              //데이터 목록 중 현재 선택된 인덱스
        private GameObject effectSource = null; //이펙트 소스

        private static EffectData effectData;   //이펙트 데이터
        #endregion

        [MenuItem("Tool/Effect Tool")]
        static void Init()
        {
            //데이터 가져오기
            //effectData = new EffectData(); //모노비헤이비어 인스턴스 생성 방법
            effectData = ScriptableObject.CreateInstance<EffectData>(); //스크립트 오브젝트 인스턴스 생성 방법
            effectData.LoadData();

            //툴 창 열기
            EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
            window.Show();
        }

        private void OnGUI()
        {
            if(effectData == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            {
                UnityObject source = effectSource;

                //상단 레이어(Add, Copy, Remove 버튼)
                EditorHelper.EditToolTopLayer(effectData, ref selection, source, uiWidthMiddle);
                effectSource = (GameObject)source;

                //이펙트 목록, 이펙트 속성창 레이어
                EditorGUILayout.BeginHorizontal();
                {
                    //이펙트 목록
                    EditorHelper.EditorToolListLayer(effectData, ref selection, ref source, uiWidthLarge, ref sp1);
                    effectSource = (GameObject)source;

                    //이펙트 속성창
                    EditorGUILayout.BeginVertical();
                    {
                       sp2 =  EditorGUILayout.BeginScrollView(sp2);
                        {
                            if(effectData.GetDataCount() > 0)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.Separator();

                                    //속성값 설정
                                    EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                    effectData.names[selection] = 
                                        EditorGUILayout.TextField("이름", effectData.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                                    effectData.effect.effectClips[selection].effectType = 
                                        (EffectType)EditorGUILayout.EnumPopup("이펙트 이름", effectData.effect.effectClips[selection].effectType, GUILayout.Width(uiWidthLarge));

                                    EditorGUILayout.Separator();
                                    if(effectSource == null && effectData.effect.effectClips[selection].effectName != string.Empty)
                                    {
                                        effectData.effect.effectClips[selection].PreLoad();
                                        effectSource = Resources.Load(effectData.effect.effectClips[selection].effectPath +
                                            effectData.effect.effectClips[selection].effectName) as GameObject;
                                    }
                                    effectSource =
                                        (GameObject)EditorGUILayout.ObjectField("이펙트", effectSource, typeof(GameObject), false, GUILayout.Width(uiWidthLarge * 1.5f));

                                    if(effectSource != null)
                                    {
                                        effectData.effect.effectClips[selection].effectPath = EditorHelper.GetPath(effectSource);
                                        effectData.effect.effectClips[selection].effectName = effectSource.name;
                                    }
                                    else
                                    {
                                        effectData.effect.effectClips[selection].effectPath = string.Empty;
                                        effectData.effect.effectClips[selection].effectName = string.Empty;
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            //하단 레이아웃: 로드, 저장
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Reload Settings"))
                {
                    effectData = ScriptableObject.CreateInstance<EffectData>();
                    effectData.LoadData();
                    selection = 0;
                    effectSource = null;
                }
                if(GUILayout.Button("Save"))
                {
                    effectData.SaveData();
                    CreateEnumStructure();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        //이름 목록 리스트 가져와서 enum 만들기
        public void CreateEnumStructure()
        {
            string enumName = "EffectList";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            for(int i = 0; i < effectData.names.Count; i++)
            {
                if (effectData.names[i] != string.Empty)
                {
                    builder.AppendLine("    " + effectData.names[i] + " = " + i + ",");
                }
            }
            EditorHelper.CreateEnumStructure(enumName, builder);
        }
    }
}