﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gallery.Builder
{
    public class WallInfoAdder : MonoBehaviour
    {
        [SerializeField] private float placeHeight = 0.5f;
        [SerializeField] private float placeDistance = 0.3f;
        [SerializeField] private bool runOnStart = false;

        private void Start()
        {
            if (runOnStart)
                Add();
        }

        public void Add()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                var wallInfo = transform.GetChild(i).gameObject.AddComponent<WallInfo>();
                wallInfo.placeHeight = placeHeight;
                wallInfo.placeDistance = placeDistance;
            }
        }
    }
}
