using Discord;
using Discord.Addons.Interactive;
using Discord.Addons.Interactive.InlineReaction;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Addons.Interactive.Paginator;

namespace SampleApp.Modules
{
    public class SampleModule : InteractiveBase
    {
        // DeleteAfterAsync will send a message and asynchronously delete it after the timeout has popped
        // This method will not block.
        [Command("delete")]
        public async Task<RuntimeResult> Test_DeleteAfterAsync()
        {
            await ReplyAndDeleteAsync("this message will delete in 10 seconds", timeout: TimeSpan.FromSeconds(10));
            return Ok();
        }

        // NextMessageAsync will wait for the next message to come in over the gateway, given certain criteria
        // By default, this will be limited to messages from the source user in the source channel
        // This method will block the gateway, so it should be ran in async mode.
        [Command("next", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
        {
            await ReplyAsync("What is 2+2?");
            var response = await NextMessageAsync();
            if (response != null)
                await ReplyAsync($"You replied: {response.Content}");
            else
                await ReplyAsync("You did not reply before the timeout");
        }

        // PagedReplyAsync will send a paginated message to the channel
        // You can customize the paginator by creating a EmbedPage objects, which each represent a single page. There
        // are lots of options for you to explore.
        // You can customize the criteria for the paginator as well, which defaults to restricting to the source user.
        // This method will not block.
        [Command("paginator")]
        public async Task Test_Paginator()
        {
            // Use an object initializer ideally. This code is written the way it is for demonstration purposes.
            var pages = new List<EmbedPage>();
            
            var p1 = new EmbedPage
            {
                Title = "First Page",
                Description = "Interesting Information",
            };

            var p2 = new EmbedPage
            {
                Title = "Second Page",
                AlternateAuthorTitle = Context.User.Username,
                AlteranteAuthorIcon = Context.User.GetAvatarUrl()
            };

            var p3 = new EmbedPage
            {
                ImageUrl = "https://img2.gelbooru.com/samples/a5/9c/sample_a59c7a3eefe67b062ea37825ce6cea83.jpg",
            };
            
            pages.Add(p1);
            pages.Add(p2);
            pages.Add(p3);

            var options = new PaginatedAppearanceOptions
            {
                InformationText = "This fancy embed is called a Paginator",
                Timeout = TimeSpan.FromSeconds(30),
            };
            
            var pagedEmbed = new PaginatedMessage
            {
                Pages = pages,
                Options = options,
                FooterOverride = new EmbedFooterBuilder().WithText("Nice Footer")
            };

            await PagedReplyAsync(pagedEmbed, new ReactionList());
        }

        // InlineReactionReplyAsync will send a message and adds reactions on it.
        // Once an user adds a reaction, the callback is fired.
        // If callback was successful next callback is not handled
        // Unsuccessful callback is a reaction that did not have a callback.
        [Command("reaction")]
        public async Task Test_ReactionReply()
        {
            await InlineReactionReplyAsync(new ReactionCallbackData("text", null, false, false)
                .WithCallback(new Emoji("👍"), (c, r) => c.Channel.SendMessageAsync($"{r.User.Value.Mention} replied with 👍"))
                .WithCallback(new Emoji("👎"), (c, r) => c.Channel.SendMessageAsync($"{r.User.Value.Mention} replied with 👎"))
            );
        }
    }
}