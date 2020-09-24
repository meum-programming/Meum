using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public class FpsSetter : MonoBehaviour
    {
        public int targetFps = 60;

        private static FpsSetter _globalInstance;

        // Start is called before the first frame update
        void Awake()
        {
            if (_globalInstance == null)
            {
                DontDestroyOnLoad(gameObject);
                _globalInstance = this;

                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = targetFps;
            }
            else
                Destroy(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.targetFrameRate != targetFps)
                Application.targetFrameRate = targetFps;
        }
    }
}
