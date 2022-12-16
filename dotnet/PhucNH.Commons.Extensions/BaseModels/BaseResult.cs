using System;
using System.Collections.Generic;

namespace PhucNH.Commons.Extensions.BaseModels
{
    public abstract class BaseResult<TData, TMessage>
    {
        public TData Data { get; set; } = Activator.CreateInstance<TData>();
        public ulong Total { get; set; } = ulong.MinValue;
        public List<TMessage> Messages { get; set; } = new List<TMessage>();
    }
}