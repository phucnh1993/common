using System;

namespace PhucNH.Commons.Bases.Models
{
    public class BaseItem<TId>
    {
        public TId Id { get; set; } = Activator.CreateInstance<TId>();
        public string Name { get ;set; } = string.Empty;
    }
}