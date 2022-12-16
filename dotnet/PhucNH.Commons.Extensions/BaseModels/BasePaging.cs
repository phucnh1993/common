namespace PhucNH.Commons.Extensions.BaseModels
{
    public class BasePaging
    {
        public uint PageIndex { get; set; }
        public ushort PageSize { get; set; }

        public int GetOffset()
        {
            return (int)(PageIndex > ulong.MinValue ? ((PageIndex - 1) * PageSize) : PageIndex);
        }
    }
}