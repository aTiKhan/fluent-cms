using SqlKata;
using Utils.DataDefinitionExecutor;

namespace Utils.QueryBuilder;

public class Crosstable
{
    public Entity CrossEntity { get; set; } = null!;
    public Entity TargetEntity { get; set; } = null!;
    public Attribute FromAttribute { get; set; } = null!;
    public Attribute TargetAttribute { get; set; } = null!;

    public ColumnDefinition[] GetColumnDefinitions()
    {
        CrossEntity.Attributes = [FromAttribute, TargetAttribute];
        CrossEntity.EnsureDefaultAttribute();
        return CrossEntity.ColumnDefinitions();
    }
    
    public Crosstable(Entity fromEntity, Entity targetEntity)
    {
        TargetEntity = targetEntity;
        string[] strs = [fromEntity.Name, targetEntity.Name];
        var sorted = strs.OrderBy(x => x).ToArray();
        var crossEntityName = $"{sorted[0]}_{sorted[1]}_cross";
        CrossEntity = new Entity
        {
            TableName = crossEntityName,

        };
        FromAttribute = new Attribute
        {
            Field = $"{fromEntity.Name}_id",
            Parent = CrossEntity,
        };
        TargetAttribute = new Attribute
        {
            Field = $"{targetEntity.Name}_id",
            Parent = CrossEntity
        };
    }

    public Query Delete(string fromId, Record[] targetItems)
    {
        var id = FromAttribute.CastToDatabaseType(fromId);
        var vals = targetItems.Select(x => x[TargetEntity.PrimaryKey]);
        return new Query(CrossEntity.TableName).Where(FromAttribute.Field, id)
            .WhereIn(TargetAttribute.Field, vals).AsUpdate(["deleted"],[true]);
    }
    
    public Query Insert(string fromId, Record[] targetItems)
    {
        var id = FromAttribute.CastToDatabaseType(fromId);
        string[] cols = [FromAttribute.Field, TargetAttribute.Field];
        var vals = targetItems.Select(x =>
        {
            return new object[]{id, x[TargetEntity.PrimaryKey]};
        });

        return new Query(CrossEntity.TableName).AsInsert(cols, vals);
    }
     private Query Base(Attribute[] selectAttributes)
     {
          var lstFields = selectAttributes.Select(x => x.FullName()).ToList();
          lstFields.Add(FromAttribute.FullName());
          var qry = TargetEntity.Basic().Select(lstFields);
          return qry;
     }

     public Query Many(Attribute[] selectAttributes, bool exclude, object id)
     {
         var baseQuery = Base(selectAttributes);
         var (a, b) = (TargetEntity.PrimaryKeyAttribute().FullName(), TargetAttribute.FullName());
         if (exclude)
         {
             baseQuery.LeftJoin(CrossEntity.TableName,
                 j => j.On(a, b)
                     .Where(FromAttribute.FullName(), id)
                     .Where(CrossEntity.Fullname("deleted"), false)
             ).WhereNull(FromAttribute.FullName());
         }
         else
         {
             baseQuery.Join(CrossEntity.TableName, a, b)
                 .Where(FromAttribute.FullName(), id)
                 .Where(CrossEntity.Fullname("deleted"), false);
         }

         return baseQuery;
     }

     public Query Many(Attribute[] selectAttributes, object[] ids)
     {
         var baseQuery = Base(selectAttributes);
         var (a, b) = (TargetEntity.PrimaryKeyAttribute().FullName(), TargetAttribute.FullName());
         baseQuery.Join(CrossEntity.TableName, a, b)
             .WhereIn(FromAttribute.FullName(), ids)
             .Where(CrossEntity.Fullname("deleted"), false);
         return baseQuery;
     }
}