using UnityEngine;

namespace Plugins.MusicNotificationSystem.Runtime
{
    [CreateAssetMenu(fileName = "NewTrack", menuName = "MusicSystem/Track")]
    public class MusicTrack : ScriptableObject
    {
        public string trackName;
        public string artistName;
        public AudioClip clip;
    }
}