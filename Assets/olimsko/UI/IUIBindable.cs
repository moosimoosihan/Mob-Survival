using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    public interface IUIBindable
    {
        string BindPath { get; }

        bool IsBind { get; }

        bool IsUseCustomBindPath { get; }
    }
}

