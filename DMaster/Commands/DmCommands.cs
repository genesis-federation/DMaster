using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Exceptions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;

namespace DMaster.Commands
{
    public class DmCommands : BaseCommandModule
    {
        public static List<DiscordRole> Rr = new List<DiscordRole>();
        public static List<DiscordMember> Uu = new List<DiscordMember>();

        
        [Command("send")]
        [Description("Sends a message to a group of people with a specific role")]
        public async Task Send(CommandContext ctx, [Description("Target role")]string RoleName)
        {
            var interactivity = ctx.Client.GetInteractivity();

            GetDiscordRoles(ctx);

            GetServerMembers(ctx);

            bool RoleExists = false;
            foreach (var role in Rr)
            {
                if (RoleName == role.Name || RoleName == role.Mention || RoleName == "everyone")
                {
                    RoleExists = true;
                    break;
                }
            }
            if(RoleExists)
            {
                string NameContent = "";
                string ServerContent = "";
                await ctx.Channel.SendMessageAsync("Please enter the message, that you want to send");
                var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync("Do you want to send message anonymously/under other name?");
                var reply1 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
                if (reply1.Result.Content.ToLower() == "yes")
                {
                    await ctx.Channel.SendMessageAsync("Do you want to replace your name?");
                    var reply2 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel)
                        .ConfigureAwait(false);
                    if (reply2.Result.Content.ToLower() == "yes")
                    {
                        await ctx.Channel.SendMessageAsync("Please enter the name/pseudonym");
                        var reply3 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel)
                            .ConfigureAwait(false);
                        NameContent = reply3.Result.Content;
                    }
                    else
                    {
                        NameContent = "anonymous";
                    }
                }
                else
                {
                    if (ctx.Member.Nickname != null)
                    {
                        NameContent = ctx.Member.Nickname;
                    }
                    else
                    {
                        NameContent = ctx.Member.Username;
                    }
                }

                await ctx.Channel.SendMessageAsync("Do you want to mention server?");
                var server = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
                if (server.Result.Content.ToLower() == "yes")
                {
                    ServerContent = " from " + ctx.Guild.Name + " server";
                }

                await ctx.Channel.SendMessageAsync("Message successfully sent!");
                foreach (var serverMember in Uu)
                {
                    var MemeberRoles = serverMember.Roles;
                    foreach (var role in MemeberRoles)
                    {
                        if (role.Name == RoleName || role.Mention == RoleName || RoleName == "everyone")
                        {
                            serverMember.SendMessageAsync("You got a message from " + NameContent + ServerContent + ":");
                            serverMember.SendMessageAsync(message.Result.Content);
                            break;
                        }
                    }
                }
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Role " + RoleName + " doesn't exists");
            }
        }

        public static void GetServerMembers(CommandContext ctx)
        {
            Uu.Clear();
            var guild = ctx.Guild;
            var users = guild.GetAllMembersAsync().Result;
            foreach (var user in users)
            {
                Uu.Add(user);
            }
        }

        public static void GetDiscordRoles(CommandContext ctx)
        {
            Rr.Clear();
            var ServerRoles = ctx.Guild.Roles;
            List<DiscordRole> Roles = new List<DiscordRole>();
            foreach (var Roly in ServerRoles)
            {
                Roles.Add(ServerRoles[Roly.Key]);
            }

            Rr = Roles;
        }
    }
}
