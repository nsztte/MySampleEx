using UnityEngine;
using UnityObject = UnityEngine.Object; //별칭

namespace MySampleEx
{
    /// <summary>
    /// 리소스 로드 구현
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        public static UnityObject Load(string path)
        {
            return Resources.Load(path);
        }

        public static GameObject LoadAndInstantiate(string path)
        {
            UnityObject source = Load(path);
            if (source == null)
            {
                return null;
            }

            return GameObject.Instantiate(source) as GameObject;
        }
    }
}