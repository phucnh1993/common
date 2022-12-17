using System;
namespace PhucNH.Commons.Bases.Models
{
    public abstract class BaseMessage<TCode>
    {
        public TCode Code { get; set; } = Activator.CreateInstance<TCode>();
        public string ItemName { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}