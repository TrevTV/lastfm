using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IF.Lastfm.Core.Api.Commands.Track
{
    [ApiMethodName("track.getTopTags")]
    internal class GetTopTagsCommand : GetAsyncCommandBase<LastResponse<LastTrack>>
    {
        public string TrackMbid { get; set; }

        public string TrackName { get; set; }

        public string ArtistName { get; set; }

        public bool Autocorrect { get; set; }

        public GetTopTagsCommand(ILastAuth auth) : base(auth) { }

        public override void SetParameters()
        {
            if (TrackMbid != null)
            {
                Parameters.Add("mbid", TrackMbid);
            }
            else
            {
                Parameters.Add("track", TrackName);
                Parameters.Add("artist", ArtistName);
            }

            Parameters.Add("autocorrect", Convert.ToInt32(Autocorrect).ToString());
        }

        public override async Task<LastResponse<LastTrack>> HandleResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            LastResponseStatus status;
            if (LastFm.IsResponseValid(json, out status) && response.IsSuccessStatusCode)
            {
                var token = JsonConvert.DeserializeObject<JToken>(json);
                var track = new LastTrack();
                var tagsToken = token.SelectToken("toptags");
                if (tagsToken != null)
                {
                    var tagToken = tagsToken.SelectToken("tag");
                    if (tagToken != null)
                    {
                        track.TopTags =
                            tagToken.Type == JTokenType.Array
                            ? tagToken.Children().Select(token1 => LastTag.ParseJToken(token1))
                            : new List<LastTag> { LastTag.ParseJToken(tagToken) };
                    }
                }

                return LastResponse<LastTrack>.CreateSuccessResponse(track);
            }
            else
            {
                return LastResponse.CreateErrorResponse<LastResponse<LastTrack>>(status);
            }
        }
    }
}
