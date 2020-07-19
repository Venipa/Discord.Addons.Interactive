﻿using Discord.Addons.Interactive.Criteria;
using Discord.Addons.Interactive.InlineReaction;
using Discord.Addons.Interactive.Paginator;
using Discord.Addons.Interactive.Results;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public abstract class InteractiveBase : InteractiveBase<SocketCommandContext>
    {
    }

    public abstract class InteractiveBase<T> : ModuleBase<T> where T : SocketCommandContext
    {
        public InteractiveService Interactive { get; set; }

        public Task<SocketMessage> NextMessageAsync(ICriterion<SocketMessage> criterion, TimeSpan? timeout = null, CancellationToken token = default)
            => Interactive.NextMessageAsync(Context, criterion, timeout, token);

        public Task<SocketMessage> NextMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null, CancellationToken token = default)
            => Interactive.NextMessageAsync(Context, fromSourceUser, inSourceChannel, timeout, token);

        public Task<IUserMessage> ReplyAndDeleteAsync(string content, bool isTTS = false, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
            => Interactive.ReplyAndDeleteAsync(Context, content, isTTS, embed, timeout, options);

        public Task<IUserMessage> InlineReactionReplyAsync(ReactionCallbackData data, bool fromSourceUser = true)
            => Interactive.SendMessageWithReactionCallbacksAsync(Context, data, fromSourceUser);
        
        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ReactionList reactions, bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            return PagedReplyAsync(pager, reactions, criterion);
        }

        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ReactionList reactions, ICriterion<SocketReaction> criterion)
            => Interactive.SendPaginatedMessageAsync(Context, pager, reactions, criterion);

        public RuntimeResult Ok(string reason = null) => new OkResult(reason);
    }
}