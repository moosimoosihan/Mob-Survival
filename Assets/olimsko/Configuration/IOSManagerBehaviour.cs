// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;
using System.Collections;
using UnityEngine;

namespace olimsko
{
    public interface IOSManagerBehaviour
    {
        event Action OnBehaviourUpdate;

        event Action OnBehaviourLateUpdate;

        event Action OnBehaviourDestroy;

        GameObject GetRootObject();

        void AddChildObject(GameObject obj);

        void Destroy();

        Coroutine StartCoroutine(IEnumerator routine);

        void StopCoroutine(Coroutine routine);
    }
}
