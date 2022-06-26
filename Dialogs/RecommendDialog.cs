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
    [LuisModel("d3633b0a-0286-4d34-b1da-1a906f3a6b48", "a8c0675b551546a5a561dcad52e76843", domain: "australiaeast.api.cognitive.microsoft.com")]


    [Serializable]
    public class RecommendDialog : LuisDialog<string>
    {
        string strMessage;
        string recommend;
        string url = "http://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port.ToString() + "/Images/";


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"죄송합니다. 말씀을 이해하지 못했습니다.";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        //Exhibition_Recommend
        [LuisIntent("Exhibition_Re")]
        public async Task Exhibition_Recommend(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            EntityRecommendation MedicalEntityRecommendation;

            String Exhibition = "";

            if (result.TryFindEntity("Topic", out MedicalEntityRecommendation))
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


            string strSQL = $"select * FROM [dbo].[exhibition] where class = N'{recommend}' and startdate > getdate()";
            DataSet DB_DS = SQLHelper.RunSQL(strSQL);
            //DB 연결
            if (DB_DS.Tables[0].Rows.Count == 0)
            {
                strMessage = $"{recommend} 전시장이 없습니다. 초기 화면으로 다시 돌아갑니다.";
                await context.PostAsync(strMessage);
                context.Done(strMessage);
                return;
            }

            #region
            strMessage = $"{recommend}에 대한 전시장 입니다. ";
            await context.PostAsync(strMessage);

            //연결


            //Menu
            message = context.MakeMessage();
            foreach (DataRow row in DB_DS.Tables[0].Rows)
            {
                String start = row["startdate"].ToString();
                String end = row["enddate"].ToString();
                String expense = row["expense"].ToString();

                //row["expense"].ToString()
                string subtitle = $"시작일 : {start}, 종료일 :{end} ,\n" +
                    $"가격:{expense} \n";
                //Hero Card-01~04 attachment 
                message.Attachments.Add(CardHelper.GetHeroCardOpenUrl(row["title"].ToString(),
                                        subtitle,
                                        this.url + row["image"].ToString(),
                                        row["Title"].ToString(), row["Number"].ToString(), row["homepage"].ToString()));
            }



            message.AttachmentLayout = "carousel";

            await context.PostAsync(message);

            #endregion


            context.Call(new Checkinformation(), DialogResumeAfter);

        }


        private async Task DialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                strMessage = await result;

                //await context.PostAsync(strWelcomeMessage);


            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Error occured....");
            }
        }
    }
}