using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace startUpProject.Dialogs
{
    //  "AppId, subscription, domain"
    [LuisModel("b987b592-2376-4075-898b-3f488a7a151e", "a8c0675b551546a5a561dcad52e76843", domain: "australiaeast.api.cognitive.microsoft.com")]


    [Serializable]
    public class RecommendDialog : LuisDialog<string>
    {
        string strMessage;

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"죄송합니다. 말씀을 이해하지 못했습니다.";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        //Exhibition_Recommend
        [LuisIntent("Exhibition_Recommend")]
        public async Task Exhibition_Recommend(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            EntityRecommendation MedicalEntityRecommendation;

            String medical = "의료";

            if (result.TryFindEntity("Medical", out MedicalEntityRecommendation))
            {
                medical = MedicalEntityRecommendation.Entity.Replace(" ", " ");
            }
            else
            {

                await context.PostAsync(" 없는 카테고리를 선택하셨습니다.");
                context.Wait(this.MessageReceived);
                return;
            }
            await context.PostAsync($"{medical}를 선택하셨습니다.");
            context.Wait(this.MessageReceived);
        }
    }
}