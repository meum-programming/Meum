using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    /*
     * @brief Singleton 베이스 클래스
     */
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        protected void Awake()
        {
            Assert.IsNull(_instance);
            _instance = GetComponent<T>();
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static T Get()
        {
            return _instance;
        }
        
        public static bool InstanceExist() {
            return !ReferenceEquals(_instance, null);
        }
    }
}