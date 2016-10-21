namespace Poco.Sql.NetCore.Test.Models.Mappings
{
    public class VUserMap : PocoSqlMapping<VUser>
    {
        public VUserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            this.IsVirtual();
        }
    }
}
