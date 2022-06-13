using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;
using startUpProject;
using System.Diagnostics;
using startUpProject.Helpers;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace startUpProject.Dialogs
{
    [Serializable]
    public class Checkinformation : IDialog<string>
    {
        private string strSQL = "SELECT * FROM Users";
        public static string _check;
        int init = 0;
        int count = 1;
        List<string> userinfo = new List<string>();
        Regex regex;


        public async Task StartAsync(IDialogContext context)
        {
            init = 0;
            context.Wait(MessageReceivedAsync);
        }

        private Task DialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string Question = "";
            String strAnswer = activity.Text.Trim();
            if (result != null)
            {
                if (activity.Text.Trim() == "종료")
                {
                    foreach (String i in userinfo)
                    {
                        Debug.WriteLine(i);
                        Debug.WriteLine(i.GetType());
                    }
                    SqlParameter[] para =
                    {
                    new SqlParameter("@UserName",SqlDbType.NVarChar),
                    new SqlParameter("@UserAge",SqlDbType.NVarChar),
                    new SqlParameter("@UserGender",SqlDbType.NVarChar),
                    new SqlParameter("@UserNumber",SqlDbType.NVarChar),
                    new SqlParameter("@UserEmail",SqlDbType.NVarChar),
                    new SqlParameter("@UserRegion",SqlDbType.NVarChar),
                    new SqlParameter("@UserExhibiton",SqlDbType.NVarChar),
                    };
                    para[0].Value = userinfo[1];
                    para[1].Value = userinfo[5];
                    para[2].Value = userinfo[3];
                    para[3].Value = userinfo[2];
                    para[4].Value = userinfo[4];
                    para[5].Value = userinfo[6];
                    para[6].Value = userinfo[0];

                    SQLHelper.ExecuteNonQuery(
                                "INSERT INTO Users(UserName, UserAge, UserGender, UserNumber,UserEmail,UserRegion,UserExhibiton) " +
                                "VALUES(@UserName, @UserAge, @UserGender, @UserNumber, @UserEmail, @UserRegion,@UserExhibiton)", para);


                    ////context.Call(new OrderDialog(), DialogResumeAfter);
                    count = 0;
                    userinfo.Clear();
                    _check = activity.Text.Trim().ToLower();
                    context.Call(new RootDialog(), DialogResumAfter);
                }
                else
                {
                    if (count == 1)
                    {
                        Question = "이름을 입력해 주세요";
                        regex = new Regex(@"^[가-힣]{3}$");
                    }
                    else if (count == 2)
                    {
                        Question = "전화번호를 입력해주세요.( - 포함 번호만 입력).";
                        regex = new Regex(@"^[가-힣]{3}$");
                    }
                    else if (count == 3)
                    {
                        Question = "성별을 입력해주세요(남성, 여성).";
                        regex = new Regex(@"^01[01678]-[0-9]{4}-[0-9]{4}$");
                    }
                    else if (count == 4)
                    {
                        Question = "이메일을 입력해주세요.";
                    }
                    else if (count == 5)
                    {
                        Question = "연령대를 입력해주세요.(20대미만,20대,30대,40대,50대,60대이상)";
                        regex = new Regex(@"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
                    }
                    else if (count == 6) Question = "지역을 입력해주세요.(서울,인천,수원,대전,광주,대구,부산,강원도,경기도,경상도,전라도,충청도)";
                    else if (count > 6) Question = "모든 입력이 끝났습니다. 종료 입력 해주세요";


                }


                String strMessage = string.Format(activity.Text);
                if (init == 0)
                {
                    userinfo.Add(strMessage);
                    await context.PostAsync(Question);
                    init = 1;
                    count++;
                    return;
                }
                else
                {
                    if ((count == 1 | count == 2 | count == 3 | count == 5) && !regex.IsMatch(strMessage))
                    {
                        await context.PostAsync("입력이 잘못되었습니다.");
                        return;
                    }
                    await context.PostAsync(Question);
                    if (count > 1)
                    {
                        userinfo.Add(strMessage);
                        count++;
                    }
                    else
                    {
                        userinfo.Add(activity.Text);
                        count++;
                    }
                    Debug.WriteLine(userinfo.ToString());
                }

            }
        }

        private Task DialogResumAfter(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}