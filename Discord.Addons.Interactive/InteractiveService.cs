﻿using Discord.Addons.Interactive.Callbacks;
using Discord.Addons.Interactive.Criteria;
using Discord.Addons.Interactive.InlineReaction;
using Discord.Addons.Interactive.Paginator;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class InteractiveService : IDisposable
    {
        public BaseSocketClient Discord { get; }

        private readonly Dictionary<ulong, IReactionCallback> _callbacks;
        private readonly TimeSpan _defaultTimeout;

        // Helpers to allow DI containers to resolve without a custom factory
        public InteractiveService(DiscordSocketClient discord, InteractiveServiceConfig config = null)
            : this((BaseSocketClient)discord, config) { }

        public InteractiveService(DiscordShardedClient discord, InteractiveServiceConfig config = null)
            : this((BaseSocketClient)discord, config) { }

        public InteractiveService(BaseSocketClient discord, InteractiveServiceConfig config = null)
        {
            Discord = discord;
            Discord.ReactionAdded += HandleReactionAsync;

            config = config ?? new InteractiveServiceConfig();
            _defaultTimeout = config.DefaultTimeout;

            _callbacks = new Dictionary<ulong, IReactionCallback>();
        }

        public Task<SocketMessage> NextMessageAsync(SocketCommandContext context,
            bool fromSourceUser = true,
            bool inSourceChannel = true,
            TimeSpan? timeout = null,
            CancellationToken token = default)
        {
            var criterion = new Criteria<SocketMessage>();
            
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureSourceUserCriterion());
            
            if (inSourceChannel)
                criterion.AddCriterion(new EnsureSourceChannelCriterion());
            
            return NextMessageAsync(context, criterion, timeout, token);
        }

        public async Task<SocketMessage> NextMessageAsync(SocketCommandContext context,
            ICriterion<SocketMessage> criterion,
            TimeSpan? timeout = null,
            CancellationToken token = default)
        {
            timeout = timeout ?? _defaultTimeout;

            var eventTrigger = new TaskCompletionSource<SocketMessage>();
            var cancelTrigger = new TaskCompletionSource<bool>();

            token.Register(() => cancelTrigger.SetResult(true));

            async Task Handler(SocketMessage message)
            {
                var result = await criterion.JudgeAsync(context, message).ConfigureAwait(false);
                if (result)
                    eventTrigger.SetResult(message);
            }

            context.Client.MessageReceived += Handler;

            var trigger = eventTrigger.Task;
            var cancel = cancelTrigger.Task;
            var delay = Task.Delay(timeout.Value, token);
            var task = await Task.WhenAny(trigger, delay, cancel).ConfigureAwait(false);

            context.Client.MessageReceived -= Handler;

            if (task == trigger)
                return await trigger.ConfigureAwait(false);
            return null;
        }

        public async Task<IUserMessage> ReplyAndDeleteAsync(SocketCommandContext context,
            string content, bool isTTS = false,
            Embed embed = null,
            TimeSpan? timeout = null,
            RequestOptions options = null)
        {
            timeout = timeout ?? _defaultTimeout;
            var message = await context.Channel.SendMessageAsync(content, isTTS, embed, options).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value)
                .ContinueWith(_ => message.DeleteAsync().ConfigureAwait(false))
                .ConfigureAwait(false);
            return message;
        }
        
        public async Task<IUserMessage> SendMessageWithReactionCallbacksAsync(SocketCommandContext context, 
            ReactionCallbackData reactionCallbackData, 
            bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if (fromSourceUser)
            {
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            }

            var callback = new InlineReactionCallback(this, context, reactionCallbackData, criterion);
            await callback.DisplayAsync().ConfigureAwait(false);
            return callback.Message;
        }

        public async Task<IUserMessage> SendPaginatedMessageAsync(SocketCommandContext context,
            PaginatedMessage pager,
            ReactionList reactions,
            ICriterion<SocketReaction> criterion = null)
        {
            var callback = new PaginatedMessageCallback(this, context, pager, criterion);
            await callback.DisplayAsync(reactions).ConfigureAwait(false);
            return callback.Message;
        }

        public void AddReactionCallback(IMessage message, IReactionCallback callback)
            => _callbacks[message.Id] = callback;

        public void RemoveReactionCallback(IMessage message)
            => RemoveReactionCallback(message.Id);

        public void RemoveReactionCallback(ulong id)
            => _callbacks.Remove(id);

        public void ClearReactionCallbacks()
            => _callbacks.Clear();

        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message,
            Cacheable<IMessageChannel, ulong> channel,
            SocketReaction reaction)
        {
            if (reaction.UserId == Discord.CurrentUser.Id)
                return;

            if (!_callbacks.TryGetValue(message.Id, out var callback))
                return;

            if (!await callback.Criterion.JudgeAsync(callback.Context, reaction).ConfigureAwait(false))
                return;

            switch (callback.RunMode)
            {
                case RunMode.Async:
                    _ = Task.Run(async () =>
                    {
                        if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                            RemoveReactionCallback(message.Id);
                    });
                    break;

                default:
                    if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                        RemoveReactionCallback(message.Id);
                    break;
            }
        }

        public void Dispose()
        {
            Discord.ReactionAdded -= HandleReactionAsync;
        }
    }
}