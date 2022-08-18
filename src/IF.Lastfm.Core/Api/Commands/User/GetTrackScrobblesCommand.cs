using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json.Linq;

namespace IF.Lastfm.Core.Api.Commands.User
{
    [ApiMethodName("user.getTrackScrobbles")]
    internal class GetTrackScrobblesCommand : GetAsyncCommandBase<PageResponse<LastScrobble>>
    {
        public string Track { get; set; }
        public string Artist { get; set; }
        public string Username { get; set; }

        public GetTrackScrobblesCommand(ILastAuth auth, string track, string artist, string username) : base(auth)
        {
            Track = track;
            Artist = artist;
            Username = username;
        }

        public override void SetParameters()
        {
            Parameters.Add("username", Username);
            Parameters.Add("track", Track);
            Parameters.Add("artist", Artist);

            AddPagingParameters();
            DisableCaching();
        }

        public override async Task<PageResponse<LastScrobble>> HandleResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            LastResponseStatus status;
            if (LastFm.IsResponseValid(json, out status) && response.IsSuccessStatusCode)
            {
                var jtoken = JToken.Parse(json);
                var itemsToken = jtoken.SelectToken("trackscrobbles").SelectToken("track");
                var pageInfoToken = jtoken.SelectToken("trackscrobbles").SelectToken("@attr");

                return PageResponse<LastScrobble>.CreateSuccessResponse(itemsToken, pageInfoToken, LastScrobble.ParseJToken, LastPageResultsType.Attr);
            }
            else
            {
                return LastResponse.CreateErrorResponse<PageResponse<LastScrobble>>(status);
            }

        }
    }
}