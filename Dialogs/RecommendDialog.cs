using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using startUpProject.Helpers;

namespace startUpProject.Dialogs
{
    //  "AppId, subscription, domain"
    [LuisModel("b987b592-2376-4075-898b-3f488a7a151e", "a8c0675b551546a5a561dcad52e76843", domain: "australiaeast.api.cognitive.microsoft.com")]


    [Serializable]
    public class RecommendDialog : LuisDialog<string>
    {
        string strMessage;
        string recommend;
        string strServerURL = "http://exhibition-bot.azurewebsites.net/Images/";

        //string strClientURL = "http://localhost:3978/Images/";

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

            String Exhibition = "";

            if (result.TryFindEntity("Exhibition", out MedicalEntityRecommendation))
            {
                Exhibition = MedicalEntityRecommendation.Entity.Replace(" ", "");
                recommend = Exhibition;
            }
            else
            {
                await context.PostAsync("없는 카테고리를 선택하셨습니다.");
                context.Wait(this.MessageReceived);
                return;
            }
            await context.PostAsync($"{Exhibition}를 선택하셨습니다.");

            #region
            strMessage = $"{recommend}에 대한 전시장 입니다. ";
            await context.PostAsync(strMessage);

            //연결
            string strSQL = $"select * FROM [dbo].[exhibition] where class = N'{recommend}' and startdate > getdate()";
            DataSet DB_DS = SQLHelper.RunSQL(strSQL);
            //DB 연결

            //Menu
            message = context.MakeMessage();
            foreach (DataRow row in DB_DS.Tables[0].Rows)
            {
                //Hero Card-01~04 attachment 
                message.Attachments.Add(CardHelper.GetHeroCardOpenUrl(row["title"].ToString(),
                                        row["title"].ToString(),
                                        this.strServerURL + row["image"].ToString(),
                                        row["Title"].ToString(),Int16.Parse(row["Number"].ToString()), row["homepage"].ToString()));
            }

            message.Attachments.Add(CardHelper.GetHeroCard("Exit food order...", "Exit",
                                    null, "Exit Order", "Exit"));


            message.AttachmentLayout = "carousel";

            await context.PostAsync(message);

            #endregion


            context.Wait(this.MessageReceived);

            
        }
    }
}