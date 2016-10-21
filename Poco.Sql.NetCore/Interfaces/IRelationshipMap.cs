using System;

namespace Poco.Sql.NetCore.Interfaces
{
    public interface IRelationshipMap
    {
        string GetForeignKey();
        Type GetRelatedObjectType();
    }
}
