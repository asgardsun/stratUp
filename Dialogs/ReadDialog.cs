
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace startUpProject.Dialogs
{
    [Serializable]
    public class ReadDialog : IDialog<string>
    {
        string strMessage;
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("전화번호를 입력해 주세요");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            strMessage = "메시지 입력";
            if (result != null)
            {
                Activity activity = await result as Activity;

                if (activity.Text.Trim() == "Exit")
                {
                    context.Done("종료한다");
                }
                else
                {
                await context.PostAsync(strMessage);
                context.Wait(MessageReceivedAsync);
                }
            }
        }
    }
}