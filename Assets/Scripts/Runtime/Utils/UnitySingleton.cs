using System;
using UnityEngine;

namespace HappyCard.Utils
{
    public abstract class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
    {
        private static T _instanced;

        public static T Instance 
        {
            get
            {
                //简单的写法....
                if(_instanced == null)
                {
                    _instanced = GameObject.FindObjectOfType<T>();

                    if(_instanced == null)
                    {
                        Type type = typeof(T);
                        GameObject gameObject = new GameObject(type.Name);
                        _instanced = gameObject.AddComponent<T>();
                        DontDestroyOnLoad(gameObject);
                    }
                }
                return _instanced;
            }
        }
        
        /// <summary>
        /// 销毁单例对象
        /// </summary>
        public void Dispose()
        {
            Destroy(_instanced);
            _instanced = null;
        }
    }
}
