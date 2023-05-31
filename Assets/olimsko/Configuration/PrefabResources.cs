// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    [Serializable]
    public struct PrefabResources : IEquatable<PrefabResources>
    {
        public string Name => m_Name;

        public GameObject Prefab => m_Prefab;

        [SerializeField] private string m_Name;
        [SerializeField] private GameObject m_Prefab;

        public PrefabResources(string name, GameObject prefab)
        {
            this.m_Name = name;
            this.m_Prefab = prefab;
        }

        public override bool Equals(object obj)
        {
            return obj is PrefabResources variable && Equals(variable);
        }

        public bool Equals(PrefabResources other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return 363513814 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public static bool operator ==(PrefabResources left, PrefabResources right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PrefabResources left, PrefabResources right)
        {
            return !(left == right);
        }
    }
}
