using System;
using System.Globalization;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace olimsko
{
    public class GameState : StateMap
    {
        [JsonIgnore] public DateTime SaveDateTime { get; set; }

        [JsonIgnore] public Texture2D Thumbnail { get; set; }

        [JsonIgnore] private const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public string saveDateTime;
        public string thumbnailBase64;


        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            saveDateTime = SaveDateTime.ToString(dateTimeFormat, CultureInfo.InvariantCulture);
            thumbnailBase64 = Thumbnail ? Convert.ToBase64String(Thumbnail.EncodeToJPG()) : null;
        }


        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            SaveDateTime = string.IsNullOrEmpty(saveDateTime) ? DateTime.MinValue : DateTime.ParseExact(saveDateTime, dateTimeFormat, CultureInfo.InvariantCulture);
            Thumbnail = string.IsNullOrEmpty(thumbnailBase64) ? null : GetThumbnail();
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            saveDateTime = SaveDateTime.ToString(dateTimeFormat, CultureInfo.InvariantCulture);
            thumbnailBase64 = Thumbnail ? Convert.ToBase64String(Thumbnail.EncodeToJPG()) : null;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            SaveDateTime = string.IsNullOrEmpty(saveDateTime) ? DateTime.MinValue : DateTime.ParseExact(saveDateTime, dateTimeFormat, CultureInfo.InvariantCulture);
            Thumbnail = string.IsNullOrEmpty(thumbnailBase64) ? null : GetThumbnail();
        }

        private Texture2D GetThumbnail()
        {
            var tex = new Texture2D(2, 2);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.LoadImage(Convert.FromBase64String(thumbnailBase64));
            return tex;
        }
    }
}