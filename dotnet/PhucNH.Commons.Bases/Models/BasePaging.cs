namespace PhucNH.Commons.Bases.Models
{
    public class BasePaging
    {
        public ulong PageIndex { get; set; }
        public ushort PageSize { get; set; }

        public ulong GetOffset()
        {
            return (PageIndex > ulong.MinValue ? ((PageIndex - 1) * PageSize) : PageIndex);
        }
    }
}