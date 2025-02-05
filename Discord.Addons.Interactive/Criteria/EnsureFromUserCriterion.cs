﻿using Discord.Commands;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive.Criteria
{
    public class EnsureFromUserCriterion : ICriterion<IMessage>
    {
        private readonly ulong _id;

        public EnsureFromUserCriterion(IUser user)
            => _id = user.Id;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public EnsureFromUserCriterion(ulong id)
            => _id = id;

        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, IMessage parameter)
        {
            var ok = _id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }
}