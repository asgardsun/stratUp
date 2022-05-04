using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using QnAMakerDialog.Models;
using QnAMakerDialog;

namespace startUpProject.Dialogs
{
    [Serializable]
    //[QnAMaker Servuce(Host, EndpointKey, Knwledgebase, MaxAnsers=0)]
    [QnAMakerService("https://greatwallqna.azurewebsites.net/qnamaker", "138fd4bd-8423-4e3d-8df1-35a2a6a140f3","cffd79d7-1623-407a-9a44-ed21cb0db514", MaxAnswers =5)]
    public class FAQDialog : QnAMakerDialog<string>
    {
        //This  method is called automatically when there are no results for the question
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync($"Sorry, I couldn't find an answer for '{originalQueryText}.'");
            context.Wait(MessageReceivedAsync);
            
        }

        public override async Task DefaultMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            if(originalQueryText == "Exit")
            {
                context.Done("");
                return;
            }
            await context.PostAsync(result.Answers.First().Answer);
            context.Wait(MessageReceivedAsync);

        }
        [QnAMakerResponseHandler(0.5)]
        public async Task LowScoreHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            var messageActivity = ProcessResultAndCreateMessageActivity(context, ref result);

            messageActivity.Text = $"I found an answer that might help..." +
                $"{result.Answers.First().Answer}.";

            await context.PostAsync(messageActivity);
            context.Wait(MessageReceived);
        }
        //using Hero Card
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("FAQ Service : ");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;

            if (activity.Text.Trim() == "Exit")
            {
                context.Done("Order Completed");
            }
            else
            {
                await context.PostAsync("FAQ Dialog");

                context.Wait(MessageReceivedAsync);
            }
        }
    }
}