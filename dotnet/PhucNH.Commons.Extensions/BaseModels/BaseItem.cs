using System;

namespace PhucNH.Commons.Extensions.BaseModels
{
    public class BaseItem<TId>
    {
        public TId Id { get; set; } = Activator.CreateInstance<TId>();
        public string Name { get ;set; } = string.Empty;
    }
}