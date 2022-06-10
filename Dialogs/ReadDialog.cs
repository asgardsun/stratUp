
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using startUpProject.Helpers;

namespace startUpProject.Dialogs
{
    [Serializable]
    public class ReadDialog : IDialog<string>
    {
        string strMessage;
        string url = "http://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port.ToString() + "/Images/";
        string phoneNumber;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("전화번호를 입력해 주세요");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            var message = context.MakeMessage();
            phoneNumber = activity.Text.Trim();
            string strSQL = $"select * FROM dbo.memberinfo where UserNumber = N'{phoneNumber}'";
            DataSet DB_DS = SQLHelper.RunSQL(strSQL);
            if(DB_DS != null)
            {
                message = context.MakeMessage();
                foreach (DataRow row in DB_DS.Tables[0].Rows)
                {
                    //Hero Card-01~04 attachment 
                    String name = row["Username"].ToString();
                    String UserNumber = row["UserNumber"].ToString();
                    String startDate = row["Startdate"].ToString();
                    String endDate = row["Enddate"].ToString();
                   
                string subtitle = $"시작일 : {startDate}, 종료일 :{endDate} ,\n" +
                    $"이름:{name} \n";
                    subtitle += $"번호 : {UserNumber}";
                        System.Diagnostics.Debug.WriteLine(subtitle);




                 //   string subtitle = $"이름 : {name}\n" + $"번호 : {UserNumber}\n" +
                 //       $"시작날짜 : {startDate}\n" + $"종료날짜 : {endDate}\n";
                    message.Attachments.Add(CardHelper.GetHeroCardOpenUrl(row["title"].ToString(),
                                            subtitle,
                                            this.url + row["image"].ToString(),
                                            row["Title"].ToString(), row["title"].ToString(), row["homepage"].ToString()));
                }

                message.Attachments.Add(CardHelper.GetHeroCard("Exit food order...", "Exit",
                                        null, "Exit Order", "Exit"));
                message.AttachmentLayout = "carousel";
                await context.PostAsync(message);
                

            }
            else
            {

            }

            if (result != null)
            {

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