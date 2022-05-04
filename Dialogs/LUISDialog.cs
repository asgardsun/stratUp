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
    [LuisModel("d89cfb3e-4c60-48d9-a5a7-ca69091a0d32", "a8c0675b551546a5a561dcad52e76843", domain: "australiaeast.api.cognitive.microsoft.com")]


    [Serializable]
    public class LUISDialog : LuisDialog<string>
    {
        
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"죄송합니다. 말씀을 이해하지 못했습니다.";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Order")]
        public async Task Order(IDialogContext context, IAwaitable<IMessageActivity> activtity, LuisResult result)
        {
            var message = await activtity;

            EntityRecommendation menuEntityRecommendation;
            EntityRecommendation sizeEntityRecomendatrion;
            EntityRecommendation quantityEntityRecomendatrion;

            string Menu = "";
            string Size = "보통";
            string Quantity = "한그릇";

            if (result.TryFindEntity("Menu", out menuEntityRecommendation))
            {
                Menu = menuEntityRecommendation.Entity.Replace(" ", "");
            }
            else
            {
                await context.PostAsync("없는 메뉴를 선택했습니다.");
                context.Wait(this.MessageReceived);
                return;
            }


            if (result.TryFindEntity("Size", out sizeEntityRecomendatrion))
            {
                Size = sizeEntityRecomendatrion.Entity.Replace(" ", "");
            }


            if (result.TryFindEntity("Quantity", out quantityEntityRecomendatrion))
            {
                Quantity = quantityEntityRecomendatrion.Entity.Replace(" ", "");
            }


            await context.PostAsync($"{Menu}, {Size}, {Quantity}를 주문하셨습니다");

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Delivery")]
        public async Task Delivery(IDialogContext context, IAwaitable<IMessageActivity> activtity, LuisResult result)
        {
            await context.PostAsync("출발 했습니다. 잠시만 기다려 주세요.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Finish")]
        public async Task Finish(IDialogContext context, IAwaitable<IMessageActivity> activtity, LuisResult result)
        {
            await context.PostAsync("주문 완료 되었습니다. 감사합니다.");
            context.Done("주문완료");
        }
    }
}