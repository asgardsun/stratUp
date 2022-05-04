using startUpProject.Helpers;
using startUpProject.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;



namespace startUpProject
{
    [Serializable]
    public class OrderDialog : IDialog<string>
    {
        string strMessage = null;
        string _strOrder;
        string query;

        string strServerURL = "https://greatwallqna-bot-a8f3.azurewebsites.net/Images/";
        List<OrderItem> MenuItems = new List<OrderItem>();

        string strSQL = "select * FROM menus";

        public async Task StartAsync(IDialogContext context)
        {
            strMessage = null;
            _strOrder = "[Order Menu List]\n";

            await this.MessageReceivedAsync(context, null);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            #region using Hero Card

            //using Hero Card            
            //if(result != null){
            //    Activity activity = await result as Activity;
            //    if (activity.Text.Trim() == "Exit")
            //    {
            //        await context.PostAsync(strOrder);
            //        strOrder = null;
            //        context.Done("Order Completed");
            //    }
            //    else
            //    {
            //        strMessage = string.Format("You ordered {0}.", activity.Text);
            //        strOrder += activity.Text + "\n";
            //        await context.PostAsync(strMessage);

            //        context.Wait(MessageReceivedAsync);
            //    }
            //}
            //else
            //{
            //    strMessage = "[Food order Menu] Select the menu you want to order > ";
            //    await context.PostAsync(strMessage);



            //    //Menu_01 자장면
            //    List<CardImage> menu01_images = new List<CardImage>();
            //    menu01_images.Add(new CardImage()
            //    {
            //        Url = this.strServerURL + "menu_01.jpg"
            //    });

            //    List<CardAction> menu01_Button = new List<CardAction>();
            //    menu01_Button.Add(new CardAction()
            //    {
            //        Title = "자장면",
            //        Value = "자장면",
            //        Type = ActionTypes.ImBack
            //    });
            //    HeroCard menu01_Card = new HeroCard()
            //    {
            //        Title = "자장면",
            //        Subtitle = "옛날 자장면",
            //        Images = menu01_images,
            //        Buttons = menu01_Button
            //    };

            //    //메뉴 짬뽕
            //    List<CardImage> menu02_images = new List<CardImage>();
            //    menu02_images.Add(new CardImage()
            //    {
            //        Url = this.strServerURL + "menu_02.jpg"
            //    });
            //    //Create Button-02
            //    List<CardAction> menu02_Button = new List<CardAction>();
            //    menu02_Button.Add(new CardAction()
            //    {
            //        Title = "짬뽕",
            //        Value = "짬뽕",
            //        Type = ActionTypes.ImBack
            //    });


            //    //create Hero Card - 02
            //    HeroCard menu02_Card = new HeroCard()
            //    {
            //        Title = "짬봉",
            //        Subtitle = "굴 짬봉",
            //        Images = menu02_images,
            //        Buttons = menu02_Button
            //    };



            //    //탕수육 
            //    List<CardImage> menu03_images = new List<CardImage>();
            //    menu03_images.Add(new CardImage()
            //    {
            //        Url = this.strServerURL + "menu_03.jpg"
            //    });
            //    //Create Button-03
            //    List<CardAction> menu03_Button = new List<CardAction>();
            //    menu03_Button.Add(new CardAction()
            //    {
            //        Title = "탕수육",
            //        Value = "탕수육",
            //        Type = ActionTypes.ImBack
            //    });


            //    //create Hero Card - 03
            //    HeroCard menu03_Card = new HeroCard()
            //    {
            //        Title = "탕수육",
            //        Subtitle = "찹살 탕수육",
            //        Images = menu03_images,
            //        Buttons = menu03_Button
            //    };

            //    //exit
            //    //Create exit
            //    List<CardAction> menu04_Button = new List<CardAction>();
            //    menu04_Button.Add(new CardAction()
            //    {
            //        Title = "Exit oreder",
            //        Value = "Exit",
            //        Type = ActionTypes.ImBack
            //    });


            //    //create Hero Card - 02
            //    HeroCard menu04_Card = new HeroCard()
            //    {
            //        Title = "Exit food order....",
            //        Subtitle = null,
            //        Buttons = menu04_Button
            //    };

            //    var message = context.MakeMessage();
            //    message.Attachments.Add(menu01_Card.ToAttachment());
            //    message.Attachments.Add(menu02_Card.ToAttachment());
            //    message.Attachments.Add(menu03_Card.ToAttachment());
            //    message.Attachments.Add(menu04_Card.ToAttachment());

            //    await context.PostAsync(message);

            //    context.Wait(this.MessageReceivedAsync);
            #endregion



            //using Hero Card            
            if (result != null)
            {
                Activity activity = await result as Activity;

                if(activity.Text.Trim() == "Finish")
                {
                    context.Done("order Completed");
                }
                else if (activity.Text.Trim() == "Exit")
                {
                    List<ReceiptItem> receiptItems = new List<ReceiptItem>();
                    Decimal totalPrice = 0;

                    foreach (var item in MenuItems)
                    {
                        receiptItems.Add(new ReceiptItem()
                        {
                            Title = item.Title,
                            Price = item.Price.ToString("##########"),
                            Quantity = item.Quantity.ToString(),
                        });
                        totalPrice += item.Price;
                    }

                    SqlParameter[] para =
                    {
                        new SqlParameter("@TotalPrice", SqlDbType.SmallMoney),
                        new SqlParameter("@UserID", SqlDbType.NVarChar, 50)
                    };

                    para[0].Value = totalPrice;
                    para[1].Value = activity.Id;

                    string query = $"insert into orders(totalprice, userid, orderDate) values (@totalPrice, @UserID, GETDATE())";
                    SQLHelper.ExecuteNonQuery(query, para);

                    query = $"select max(orderid) FROM orders where userid = '{activity.Id}'";
                    DataSet orderNumber = SQLHelper.RunSQL(query);

                    DataRow row = orderNumber.Tables[0].Rows[0];

                    int orderID = (int)row[0];

                    foreach(var orderItem in MenuItems)
                    {
                        SqlParameter[] par2 =
                        {
                            new SqlParameter("@OrderID", SqlDbType.Int),
                            new SqlParameter("@ItemName", SqlDbType.NVarChar),
                            new SqlParameter("@ItemPrice", SqlDbType.SmallMoney),
                            new SqlParameter("@Quantity", SqlDbType.Int)

                        };

                        par2[0].Value = orderID;
                        par2[1].Value = orderItem.Title;
                        par2[2].Value = orderItem.Price;
                        par2[3].Value = orderItem.Quantity;

                        query = "insert into Items(orderid, itemname, ItemPrice, Quantity) values(@orderId, @ItemName, @ItemPrice, @Quantity)";
                        SQLHelper.ExecuteNonQuery(query, par2);
                    }

                    var cardMessage = context.MakeMessage();
                    cardMessage.Attachments.Add(
                        CardHelper.GetReceiptCard("[order Menu List] \n", receiptItems, totalPrice.ToString(), "2%", "10%"));


                    cardMessage.Attachments.Add(CardHelper.GetHeroCard("Exit food order...", "Finish",
                        null, "Finish Order", "Finish"));
                    MenuItems.Clear();

                    await context.PostAsync(cardMessage);

                    context.Wait(MessageReceivedAsync);

                }
                else
                {
                    query = $"select * FROM menus WHERE MenuId = {activity.Text}";
                    //strMessage = string.Format("You ordered {0}.", activity.Text);
                    //strOrder += activity.Text + "\n";

                    //Db 연결
                    DataSet DB_DS = SQLHelper.RunSQL(query);
                    DataRow row = DB_DS.Tables[0].Rows[0];
                    
                    MenuItems.Add(new OrderItem
                    {
                        ItemID = (int)row["MenuID"],
                        Title = row["Title"].ToString(),
                        Price = (Decimal)row["Price"],
                        Quantity = 1
                    });

                    string strOrderMenu = "you order...\n";
                    foreach(var item in MenuItems)
                    {
                        strOrderMenu += item.Title + " : " + item.Price.ToString("########") + "\n\n";
                    }
                    await context.PostAsync(strOrderMenu);

                    context.Wait(MessageReceivedAsync);
                }
            }
            else
            {
                strMessage = "[Food order Menu] Select the menu you want to order > ";
                await context.PostAsync(strMessage);

                //연결
                DataSet DB_DS = SQLHelper.RunSQL(strSQL);
                //DB 연결

                //Menu
                var message = context.MakeMessage();
                foreach (DataRow row in DB_DS.Tables[0].Rows)
                {
                    //Hero Card-01~04 attachment 
                    message.Attachments.Add(CardHelper.GetHeroCard(row["Title"].ToString(),
                                            row["Price"].ToString(),
                                            this.strServerURL + row["Images"].ToString(),
                                            row["Title"].ToString(), row["MenuID"].ToString()));
                }

                message.Attachments.Add(CardHelper.GetHeroCard("Exit food order...", "Exit",
                                        null, "Exit Order", "Exit"));


                message.AttachmentLayout = "carousel";

                await context.PostAsync(message);

                context.Wait(this.MessageReceivedAsync);

            }

        }
    }
}