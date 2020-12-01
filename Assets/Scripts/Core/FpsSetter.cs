using UnityEngine;

namespace Core
{
    /*
     * @brief fps를 고정해주기 위해 Application.targetFrameRate를 업데이트해주는 컴포넌트, Singleton임
     */
    public class FpsSetter : Singleton<FpsSetter>
    {
        [SerializeField] private int targetFps = 60;

        private static FpsSetter _globalInstance;
        
        private void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Application.targetFrameRate != targetFps)
                Application.targetFrameRate = targetFps;
        }
    }
}
