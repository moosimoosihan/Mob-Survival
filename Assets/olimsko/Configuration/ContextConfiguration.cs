using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace olimsko
{
    [EditInProjectSettings]
    public class ContextConfiguration : Configuration
    {
        public List<string> ListPredefinedContext = new List<string>();
    }
}