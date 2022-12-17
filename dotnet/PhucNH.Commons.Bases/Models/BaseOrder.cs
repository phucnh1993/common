namespace PhucNH.Commons.Bases.Models
{
    public class BaseOrder : BasePaging
    {
        public string ColumnOrder { get; set; } = string.Empty;
        public bool IsDesc { get; set; }
    }
}