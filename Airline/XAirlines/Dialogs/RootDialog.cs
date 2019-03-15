﻿using Airlines.XAirlines.Common;
using Airlines.XAirlines.Helpers;
using Airlines.XAirlines.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Airlines.XAirlines.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        /// <summary>
        /// Called when the dialog is started.
        /// </summary>
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        /// <summary>
        /// Called when a message is received by the dialog
        /// </summary>
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = context.MakeMessage();
            Attachment card = null;
            string message = string.Empty;
            
            if (activity.Text != null)
            {
                message = Microsoft.Bot.Connector.Teams.ActivityExtensions.GetTextWithoutMentions(activity).ToLowerInvariant();



                switch (message.Trim())
                {
                    case Constants.NextMonthRoster:
                        card = await CardHelper.GetMonthlyRosterCard();
                        break;
                    case Constants.NextWeekRoster:
                        card = await CardHelper.GetWeeklyRosterCard();
                        break;
                    case Constants.ShowDetailedRoster:
                        card = await CardHelper.GetDetailedRoster();
                        break;
                    default:
                        card = await CardHelper.GetWelcomeScreen();
                        break;
                }
                reply.Attachments.Add(card);
                await context.PostAsync(reply);
            }
            else if (activity.Value != null)
            {

                await HandleActions(context, activity);
                return;
            }
        }

        private async Task HandleActions(IDialogContext context, Activity activity)
        {
            var reply = context.MakeMessage();
            var details = JsonConvert.DeserializeObject<ActionDetails>(activity.Value.ToString());
            var type = details.ActionType;
            Attachment card = null;
            switch (type)
            {
                case Constants.ShowDetailedRoster:
                    card=await CardHelper.GetDetailedRoster();
                    break;

            }
            reply.Attachments.Add(card);
            await context.PostAsync(reply);
            return;
        }
    }
}
