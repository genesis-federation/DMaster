using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DMaster.Commands
{
    public class DmCommands : BaseCommandModule
    {
        public List<DiscordRole> Rr = new List<DiscordRole>();

        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("pong").ConfigureAwait(false);
        }
        [Command("getuser")]
        public async Task GetUser(CommandContext ctx,  string Role)
        {
            var ServerRoles = ctx.Guild.Roles;
            List<DiscordRole> Roles = new List<DiscordRole>();
            foreach (var Roly in ServerRoles)
            {
                Roles.Add(ServerRoles[Roly.Key]);
            }

            foreach (var discordRole in Roles)
            {
                await ctx.Channel.SendMessageAsync(discordRole.Mention);
            }

            Rr = Roles;
        }

        [Command("check")]
        public async Task Check(CommandContext ctx)
        {
            foreach (var discordRole in Rr)
            {
                await ctx.Channel.SendMessageAsync(discordRole.Mention);
            }
        }
    }
}
