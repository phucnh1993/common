namespace PhucNH.Commons.Extensions.BaseModels
{
    public class BaseOrder : BasePaging
    {
        public string ColumnOrder { get; set; } = string.Empty;
        public bool IsDesc { get; set; }
    }
}