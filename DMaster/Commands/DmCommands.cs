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
        var Sendembed = new DiscordEmbedBuilder
        {
        Title = "Please enter the message",
        Description = "Enter the message that you want to send to a group of people with selected role"
        };
        var q1 = new DiscordEmbedBuilder
        {
            Title = "Do you want to send message anonymously/under other name?",
            Description = "If you want to send this message as anonymous or using other name, please send `yes`. Else, send `no`",
        };
        var q11 = new DiscordEmbedBuilder
        {
            Title = "Do you want to replace your name?",
            Description = "If you want to send this message using other name, please send `yes`. Else, send `no` to send this message anonymously",
        };
        var q111 = new DiscordEmbedBuilder
        {
            Title = "Please enter the name/pseudonym",
        };
        var q2= new DiscordEmbedBuilder
        {
            Title = "Do you want to mention server?",
            Description = "If you want to mentioned current server in your message, send `yes`. Else, send `no`",
        };
        var sucess = new DiscordEmbedBuilder
        {
            Title = "Sending Messages...",
            Color = DiscordColor.DarkGreen
        };
            var cancel = new DiscordEmbedBuilder
        {
            Title = "Message setup successfully canceled",
            Color = DiscordColor.DarkRed
        };
        var roleError = new DiscordEmbedBuilder
        {
            Title = "Role " + RoleName + " doesn't exists",
            Color = DiscordColor.DarkRed
        };
        var yessnoError = new DiscordEmbedBuilder
        {
            Title = "Please, enter `yes` or `no`",
            Color = DiscordColor.DarkRed
        };


            StringBuilder SB = new StringBuilder();

            var interactivity = ctx.Client.GetInteractivity();

            GetDiscordRoles(ctx);

            GetServerMembers(ctx);

            bool RoleExists = false;
            bool Canceled = false;
            foreach (var role in Rr)
            {
                if (RoleName.ToLower() == role.Name.ToLower() || RoleName == role.Mention || RoleName == "everyone")
                {
                    RoleExists = true;
                    break;
                }
            }
            if(RoleExists)
            {
                string NameContent = "";
                string ServerContent = "";
                await ctx.Channel.SendMessageAsync(embed: Sendembed);
                var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(embed:q1);
                point1:
                var reply1 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
                if (reply1.Result.Content.ToLower() == "yes")
                {
                    await ctx.Channel.SendMessageAsync(embed:q11);
                    point2:
                    var reply2 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel)
                        .ConfigureAwait(false);
                    if (reply2.Result.Content.ToLower() == "yes" && !Canceled)
                    {
                        await ctx.Channel.SendMessageAsync(embed:q111);
                        var reply3 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel)
                            .ConfigureAwait(false);
                        NameContent = reply3.Result.Content;
                    }
                    else if(reply2.Result.Content.ToLower() != "cancel")
                    {
                        if (reply2.Result.Content.ToLower() == "no")
                            NameContent = "anonymous";
                        else
                        {
                            await ctx.Channel.SendMessageAsync(embed: yessnoError);
                            goto point2;
                        }
                    }
                    else
                    {
                        Canceled = true;
                    }
                }
                else if(reply1.Result.Content.ToLower() != "cancel")
                {
                    if (reply1.Result.Content.ToLower() == "no")
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
                    else
                    {
                        await ctx.Channel.SendMessageAsync(embed: yessnoError);
                        goto point1;
                    }
                }
                else
                {
                    Canceled = true;
                }
                if(!Canceled)
                {
                    await ctx.Channel.SendMessageAsync(embed: q2);
                    point3:
                    var server = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel)
                        .ConfigureAwait(false);
                    if (server.Result.Content.ToLower() == "yes" && !Canceled)
                    {
                        ServerContent = " from " + ctx.Guild.Name + " server";
                    }
                    else if (server.Result.Content.ToLower() != "cancel")
                    {
                        if (server.Result.Content.ToLower() == "no")
                            ServerContent = "";
                        else
                        {
                            await ctx.Channel.SendMessageAsync(embed: yessnoError);
                            goto point3;
                        }
                    }
                    else
                    {
                        Canceled = true;
                    }
                    if (!Canceled)
                    {
                        await ctx.Channel.SendMessageAsync(embed: sucess);
                        foreach (var serverMember in Uu)
                        {
                            var MemeberRoles = serverMember.Roles;
                            foreach (var role in MemeberRoles)
                            {
                                if (role.Name == RoleName || role.Mention == RoleName || RoleName == "everyone")
                                {
                                    SB.AppendLine("You got a message from " + NameContent + ServerContent + ":");
                                    SB.AppendLine();
                                    SB.AppendLine(message.Result.Content);
                                    serverMember.SendMessageAsync(SB.ToString());
                                    SB.Clear();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync(embed: cancel);
                    }
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(embed: cancel);
                }
            }
            else
            {
                await ctx.Channel.SendMessageAsync(embed:roleError);
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
