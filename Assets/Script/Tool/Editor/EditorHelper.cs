using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using System.Text;
using System.IO;

namespace MySampleEx
{
    /// <summary>
    /// 툴과 관련된 공통 기능 구현
    /// 경로 얻어오기, 이름 목록 리스트를 enum으로 만들기
    /// </summary>
    public class EditorHelper
    {
        //매개변수로 받은 UnityObject의 위치 경로 얻어오기
        public static string GetPath(UnityObject p_clip)
        {
            string retString = string.Empty;

            //p_clip 클립의 전체 경로      : Asset/ResourcesData/Resources/EffectData...
            retString = AssetDatabase.GetAssetPath(p_clip);     //리소스 경로 가져오기
            string[] path_node = retString.Split('/');
            bool findResources = false;
            for(int i = 0; i < path_node.Length - 1; i++)
            {
                if(findResources == false)
                {
                    if(path_node[i] == "Resources")     //Resources는 고정폴더
                    {
                        findResources = true;
                        retString = string.Empty;
                    }
                }
                else
                {
                    retString += path_node[i] + "/";
                }
            }

            return retString;
        }

        //이름 목록 리스트를 enum으로 만들기
        public static void CreateEnumStructure(string enumName, StringBuilder data)
        {
            string templateFilePath = "Assets/Editor/EnumTemplate.txt";
            string entittyTemplate = File.ReadAllText(templateFilePath);

            entittyTemplate = entittyTemplate.Replace("$ENUM$", enumName);
            entittyTemplate = entittyTemplate.Replace("$DATA$", data.ToString());

            string folderPath = "Assets/Script/GameData/";
            if(Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = folderPath + enumName + ".cs";

            //파일이 존재하면 파일을 삭제한다
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.WriteAllText(filePath, entittyTemplate);
        }

        //데이터 툴 상단 레이어, Add, Copy, Remove 버튼 그리기
        public static void EditToolTopLayer(BaseData data, ref int selection, UnityObject source, int uiWidth)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add", GUILayout.Width(uiWidth)))
                {
                    //추가
                    data.AddData("NewData");
                    selection = data.GetDataCount() - 1;
                    source = null;
                }
                if (GUILayout.Button("Copy", GUILayout.Width(uiWidth)))
                {
                    //복사
                    data.Copy(selection);
                    selection = data.GetDataCount() - 1;
                    source = null;
                }
                if (data.GetDataCount() > 1)
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(uiWidth)))
                    {
                        source = null;
                        data.RemoveData(selection);
                    }
                }
                //현재 셀렉션 값 체크(인덱스는 길이 -1)
                if(selection > data.GetDataCount() - 1)
                {
                    selection = data.GetDataCount() - 1;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        //데이터 목록 리스트 그리기
        public static void EditorToolListLayer(BaseData data, ref int selection, ref UnityObject source, int uiWidth, ref Vector2 scrollPosition)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(uiWidth));
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical("box");   //배경이 되는 박스
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);   //스크롤 포지션 리턴
                    {
                        int lastSelect = selection;
                        selection = GUILayout.SelectionGrid(selection, data.GetNameList(true), 1);  //선택한 번호 리턴
                        //다른 데이터를 선택하면 소스 비우기
                        if(lastSelect != selection)
                        {
                            source = null;
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

    }
}
