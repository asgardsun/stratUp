using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace startUpProject.Helpers
{
    public class CardHelper
    {
        public static Attachment GetHeroCard(string strTitle, string strSubTitle, string strImage, string strButtonText, string strButtonValue)
        {
            List<CardImage> images = new List<CardImage>();
            images.Add(new CardImage()
            { Url = strImage });
            
            List<CardAction> buttons = new List<CardAction>();
            buttons.Add(new CardAction() { Title = strButtonText, Value = strButtonValue, Type = ActionTypes.ImBack });

            HeroCard card = new HeroCard()
            {
                Title = strTitle,
                Subtitle = strSubTitle,
                Images = images,
                Buttons = buttons
            };

            return card.ToAttachment();
         
        }


        public static Attachment GetHeroCardOpenUrl(string strTitle, string strSubTitle, string strImage, string strButtonText, string strButtonValue, string btnUrl)
        {
            List<CardImage> images = new List<CardImage>();
            images.Add(new CardImage()
            { Url = strImage });

            List<CardAction> buttons = new List<CardAction>();
            buttons.Add(new CardAction() 
            { 
                Title = strButtonText, 
                Value = strButtonValue, 
                Type = ActionTypes.ImBack }
            );
            buttons.Add(new CardAction()
            {
                Title = "자세히 보기",
                Type = "openUrl",
                Value = btnUrl,
            });

            HeroCard card = new HeroCard()
            {
                Title = strTitle,
                Subtitle = strSubTitle,
                Images = images,
                Buttons = buttons
            };

            return card.ToAttachment();

        }


        public static Attachment GetThumbnailCard(string strTitle, string strSubTitle, string strImage, string strButtonText, string strButtonValue)
        {
            List<CardImage> images = new List<CardImage>();
            images.Add(new CardImage()
            { Url = strImage });

            List<CardAction> buttons = new List<CardAction>();
            buttons.Add(new CardAction() { Title = strButtonText, Value = strButtonValue, Type = ActionTypes.ImBack });

            HeroCard card = new HeroCard()
            {
                Title = strTitle,
                Subtitle = strSubTitle,
                Images = images,
                Buttons = buttons
            };

            return card.ToAttachment();

        }

        public static Attachment GetReceiptCard(string strTitle, List<ReceiptItem> lstItems,
            string strTotal, string strTax, string strVat)
        {
            ReceiptCard card = new ReceiptCard
            {
                Title = strTitle,
                Items = lstItems,
                Total = strTotal,
                Tax = strTax,
                Vat = strVat
            };
            return card.ToAttachment();
        }

        public static Attachment GetHeroCardOpenUserInfo(string strTitle, string strSubTitle, string strImage, string strButtonText, string strButtonValue, string btnUrl)
        {
            List<CardImage> images = new List<CardImage>();
            images.Add(new CardImage()
            { Url = strImage });

            List<CardAction> buttons = new List<CardAction>();
            buttons.Add(new CardAction()
            {
                Title = strButtonText,
                Value = strButtonValue,
                Type = ActionTypes.ImBack
            }
            );
            buttons.Add(new CardAction()
            {
                Title = "자세히 보기",
                Type = "openUrl",
                Value = btnUrl,
            });

            HeroCard card = new HeroCard()
            {
                Title = strTitle,
                Subtitle = strSubTitle,
                //Text = strSubTitle,
                Images = images,
                Buttons = buttons
            };

            return card.ToAttachment();

        }



    }
}