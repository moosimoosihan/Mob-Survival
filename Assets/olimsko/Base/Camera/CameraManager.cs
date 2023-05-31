using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    [InitializeAtRuntime]
    public class CameraManager : IOSMEntity<CameraConfiguration>
    {
        private readonly RenderTexture thumbnailRenderTexture;

        public CameraConfiguration Configuration { get; set; }

        public bool RenderUI { get; set; }
        public Camera Camera => Camera.main;

        public CameraManager(CameraConfiguration configuration)
        {
            Configuration = configuration;
            thumbnailRenderTexture = new RenderTexture(configuration.ThumbnailResolution.x, configuration.ThumbnailResolution.y, 24);
        }

        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public void DestroyThis()
        {

        }

        public void Reset()
        {

        }

        public virtual Texture2D CaptureThumbnail()
        {
            if (Configuration.HideUIInThumbnails)
                RenderUI = false;

            // Hide the save-load menu in case it's visible.
            // var saveLoadUI = Engine.GetService<IUIManager>()?.GetUI<UI.ISaveLoadUI>();
            // var saveLoadUIWasVisible = saveLoadUI?.Visible;
            // if (saveLoadUIWasVisible.HasValue && saveLoadUIWasVisible.Value)
            //     saveLoadUI.Visible = false;

            // Confirmation UI may still be visible here (due to a fade-out time); force-hide it.
            // var confirmUI = Engine.GetService<IUIManager>()?.GetUI<UI.IConfirmationUI>();
            // var confirmUIWasVisible = confirmUI?.Visible ?? false;
            // if (confirmUI != null) confirmUI.Visible = false;

            var initialRenderTexture = Camera.targetTexture;
            Camera.targetTexture = thumbnailRenderTexture;
            // ForceTransitionalSpritesUpdate();
            Camera.Render();
            Camera.targetTexture = initialRenderTexture;

            // if (RenderUI && Configuration)
            // {
            //     initialRenderTexture = UICamera.targetTexture;
            //     UICamera.targetTexture = thumbnailRenderTexture;
            //     UICamera.Render();
            //     UICamera.targetTexture = initialRenderTexture;
            // }

            var thumbnail = thumbnailRenderTexture.ToTexture2D();

            // Restore the save-load menu and confirmation UI in case we hid them.
            // if (saveLoadUIWasVisible.HasValue && saveLoadUIWasVisible.Value)
            //     saveLoadUI.Visible = true;
            // if (confirmUIWasVisible)
            //     confirmUI.Visible = true;

            if (Configuration.HideUIInThumbnails)
                RenderUI = true;

            return thumbnail;

            // void ForceTransitionalSpritesUpdate()
            // {
            //     var updateMethod = typeof(TransitionalSpriteRenderer).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
            //     if (updateMethod is null) throw new Exception("Failed to locate `Update` method of transitional sprite renderer.");
            //     var sprites = UnityEngine.Object.FindObjectsOfType<TransitionalSpriteRenderer>();
            //     foreach (var sprite in sprites)
            //         updateMethod.Invoke(sprite, null);
            // }
        }
    }
}