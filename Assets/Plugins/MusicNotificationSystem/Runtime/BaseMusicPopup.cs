using UnityEngine;

namespace Plugins.MusicNotificationSystem.Runtime
{
    public abstract class BaseMusicPopup : MonoBehaviour
    {
        public abstract void Show(MusicTrack track);
        public abstract void Hide();
    }
}