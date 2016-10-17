using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
//追加
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace WeatherJpBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new WeatherDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [LuisModel("9cb0b86f-2990-45ba-86ef-2b900cf6c0f0", "fef35cf63a1e4bc89244a93744971eb5")]
        [Serializable]
        public class WeatherDialog : LuisDialog<object>
        {
            [LuisIntent("")]
            public async Task None(IDialogContext context, Microsoft.Bot.Builder.Luis.Models.LuisResult result)
            {
                string message = $"ゴメンナサイ．．．日本語分かりませんでした";
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }

            [LuisIntent("Greeting")]
            public async Task Greeting(IDialogContext context, Microsoft.Bot.Builder.Luis.Models.LuisResult result)
            {
                string message = $"こんにちは！日本の各地の天気をお答えするお天気BOTです。";
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }

            [LuisIntent("GetWeather")]
            public async Task GetWeather(IDialogContext context, Microsoft.Bot.Builder.Luis.Models.LuisResult result)
            {
                string city = "東京";
                string datetime = "今日";

                EntityRecommendation eRecommend;
                if (result.TryFindEntity("City", out eRecommend))
                {
                    city = eRecommend.Entity;
                }
                if (result.TryFindEntity("DateTime::Day", out eRecommend))
                {
                    datetime = eRecommend.Entity;
                }

                string message = "[都市名]" + city + " [日付]" + datetime + " の天気を調べますね";
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }

            [LuisIntent("ConfirmWeather")]
            public async Task ConfirmWeather(IDialogContext context, Microsoft.Bot.Builder.Luis.Models.LuisResult result)
            {
                string city = "東京";
                string datetime = "今日";

                EntityRecommendation eRecommend;
                if (result.TryFindEntity("City", out eRecommend))
                {
                    city = eRecommend.Entity;
                }
                if (result.TryFindEntity("DateTime::Day", out eRecommend))
                {
                    datetime = eRecommend.Entity;
                }

                string message = "[都市名]" + city + " [日付]" + datetime + " の天気を確認しますね";
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}