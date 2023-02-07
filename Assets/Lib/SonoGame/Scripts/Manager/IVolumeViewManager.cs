
namespace SonoGame
{
    /// <summary>
    /// Interface for managers like <see cref="LiveViewManager"/>
    /// </summary>
    public interface IVolumeViewManager
    {   
        void OnVolumeReady(int layer);

        void RenderVolumeView(int layer=0);

        void SetVisibleVolume(int layer);

        void HideVolumes();
    }
}
