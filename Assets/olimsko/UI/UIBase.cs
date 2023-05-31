using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    public class UIBase : MonoBehaviour
    {
        private Dictionary<string, IUIBindable> m_DicBind = new Dictionary<string, IUIBindable>();

        private void BindingObject()
        {
            IUIBindable[] uIDatas = this.GetComponentsInChildren<IUIBindable>(true);

            foreach (var item in uIDatas)
            {
                if (item.IsBind)
                {
                    if (!m_DicBind.ContainsKey(item.BindPath))
                        m_DicBind.Add(item.BindPath, item);
                }
            }
        }

        protected T Get<T>(string name) where T : IUIBindable
        {
            return (T)m_DicBind[name];
        }

        protected virtual void Awake()
        {
            BindingObject();
        }
    }
}