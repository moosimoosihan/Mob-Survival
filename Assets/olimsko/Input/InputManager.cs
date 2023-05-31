using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace olimsko
{
    [InitializeAtRuntime]
    public class InputManager : IOSMEntity<InputConfiguration>
    {
        public InputConfiguration Configuration { get; }

        public InputManager(InputConfiguration configuration)
        {
            Configuration = configuration;
        }

        public UniTask InitializeAsync()
        {
            foreach (var action in Configuration.InputActionAsset)
            {
                action.Enable();
            }
            return UniTask.CompletedTask;
        }

        public InputAction GetAction(string actionName)
        {
            return Configuration.InputActionAsset.FindAction(actionName, true);
        }

        public void Reset()
        {

        }

        public void DestroyThis()
        {
            foreach (var action in Configuration.InputActionAsset)
            {
                action.Disable();
            }
        }
    }
}