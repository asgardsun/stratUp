using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using startUpProject.Dialogs;
using startUpProject;
using System.Collections.Generic;

namespace startUpProject
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;

        private string _strMessage;

        private string _strWelcomeMessage = "전시장 챗봇에 오신것을 환영합니다.";


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }


        //Using Hero Card
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(_strWelcomeMessage);
            var message = context.MakeMessage();
            var actions = new List<CardAction>();



            actions.Add(new CardAction()
            {
                Title = "1.전시장 추천",
                Value = "1",
                Type = ActionTypes.ImBack
            });
            var FQA = new CardAction(title: "2.등록 확인", value: "2", type: ActionTypes.ImBack);
            actions.Add(FQA);



            message.Attachments.Add(
                new HeroCard()
                {
                    Title = "Select 1 - 2.>",
                    Buttons = actions
                }.ToAttachment()
            );
            await context.PostAsync(message);

            context.Wait(SendWelcomeMessageAsync);

        }


        private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strSelected = activity.Text.Trim();

            if (strSelected == "1")
            {
                _strMessage = "관심있는 전시장 카테고리를 말씀해주세요\n" +
                "[의료,건강,스포츠]\n" +
                "[오락,공연]\n" +
                "[전기,과학,기계]\n" +
                "[농수산,식음료,환경,관광]\n" +
                "[전자,IT,교육]\n" +
                "[인테리어,예술,건축]\n" +
                "[금융,출판,기타]";

                await context.PostAsync(_strMessage);
                context.Call(new RecommendDialog(), DialogResumAfter);

            }
            else if (strSelected == "2")
            {
                context.Call(new ReadDialog(), DialogResumAfter);
            }

            else
            {
                _strMessage = "You have made a mistake. Please select again...";
                await context.PostAsync(_strMessage);
                context.Wait(SendWelcomeMessageAsync);
            }

        }

        private async Task DialogResumAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                _strMessage = await result;

                await this.MessageReceivedAsync(context, result);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Error occurred....");
            }
        }



        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}