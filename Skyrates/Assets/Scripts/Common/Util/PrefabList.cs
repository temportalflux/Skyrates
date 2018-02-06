using System;
using UnityEngine;

namespace Skyrates.Client.Util
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class PrefabList : ScriptableObject
    {

        /// <summary>
        /// All the possible category keys.
        /// </summary>
        public object[] OrderedKeys;

        /// <summary>
        /// The subclass of TValue that different categories may have. Keyed by TKey.
        /// </summary>
        public Type[] KeyValueTypes;

        [Serializable]
        public class Category
        {
            [SerializeField]
            public MonoBehaviour[] Prefabs;
            [SerializeField]
            public string[] Names;
        }

        /// <summary>
        /// The actual prefab values. First keyed by TKey, then by an integer index.
        /// </summary>
        [SerializeField]
        public Category[] Categories;

        protected void Setup(object[] orderedKeys, Type[] keyTypes)
        {
            this.OrderedKeys = orderedKeys;
            this.KeyValueTypes = keyTypes;
        }

        public abstract object GetKeyFrom(int index);
        public abstract int GetIndexFrom(object key);

        public bool TryGetValue<TKey, TValue>(TKey key, int prefabIndex, out TValue prefab) where TValue : MonoBehaviour
        {
            int keyIndex = this.GetIndexFrom(key);
            MonoBehaviour[] prefabs = this.Categories[keyIndex].Prefabs;
            if (prefabIndex >= 0 && prefabIndex < prefabs.Length)
            {
                prefab = (TValue)prefabs[prefabIndex];
                return true;
            }
            prefab = null;
            return false;
        }

        public string[] GetNames(object key)
        {
            return this.Categories[this.GetIndexFrom(key)].Names;
        }

    }

    public enum TestEnum
    {
        A, B, C
    }

    public class Test : PrefabList
    {

        public static readonly object[] TEST_VALUES = { TestEnum.A, TestEnum.B };
        public static readonly Type[] TEST_TYPES = { typeof(Rigidbody), typeof(Collider) };

        void OnEnable()
        {
            this.Setup(TEST_VALUES, TEST_TYPES);
        }

        public override object GetKeyFrom(int index)
        {
            return (TestEnum) index;
        }

        public override int GetIndexFrom(object key)
        {
            return (int)key;
        }

    }

}

