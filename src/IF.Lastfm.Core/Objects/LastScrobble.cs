using System;
using Newtonsoft.Json.Linq;

namespace IF.Lastfm.Core.Objects
{
    public class LastScrobble : ILastfmObject
    {
        public LastTrack Track { get; set; }
        public DateTime ScrobbleDate { get; set; }

        internal static LastScrobble ParseJToken(JToken token)
        {
            LastScrobble s = new LastScrobble();

            LastTrack track = LastTrack.ParseJToken(token);
            s.Track = track;
            s.ScrobbleDate = DateTime.Parse(token.SelectToken("date").Value<string>("#text"));
            return s;
        }
    }
}