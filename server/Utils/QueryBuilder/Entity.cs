using Utils.DataDefinitionExecutor;
using SqlKata;

namespace Utils.QueryBuilder;
public enum InListOrDetail
{
    InList,
    InDetail,
}
public sealed class Entity
{
    private string _name;
    private string _tableName;
    private string _primaryKey;
    private string _titleAttribute;
    public string Name {
        get{return _name;}
        set{_name = NameFormatter.LowerNoSpace(value);}
    }
    public string TableName
    {
        get =>  _tableName;
        set => _tableName = NameFormatter.LowerNoSpace(value);
    }
    public string Title { get; set; } = "";

    public string PrimaryKey
    {
        get => _primaryKey;
        set =>_primaryKey =NameFormatter.LowerNoSpace(value); 
    }

    public string TitleAttribute
    {
        get => _titleAttribute;
        set => _titleAttribute = NameFormatter.LowerNoSpace(value);
    }
    
    public int DefaultPageSize { get; set; } = 20;
    //must be public to expose to json parser
    public Attribute[] Attributes { get; set; } = [];

    public void Init()
    {
        foreach (var attribute in Attributes)
        {
            attribute.Parent = this;
        }
    }
    public void LoadAttributesByColDefines(ColumnDefinition[] cols )
    {
        Attributes = cols.Select(x => new Attribute(x)).ToArray();
    }
    
    public ColumnDefinition[] AddedColumnDefinitions(ColumnDefinition[] columnDefinitions)
    {
        var set = columnDefinitions.Select(x => x.ColumnName.ToLower()).ToHashSet();
        var attr = LocalAttributes();
        var items = attr.Where(x=>!set.Contains(x.Field.ToLower().Trim()));
        return items.Select(x => new ColumnDefinition { ColumnName = x.Field, DataType = x.DataType}).ToArray();
    }

    public ColumnDefinition[] ColumnDefinitions()
    {
        return LocalAttributes().Select(x => new ColumnDefinition { ColumnName = x.Field, DataType = x.DataType })
            .ToArray();
    }
    public void EnsureDefaultAttribute()
    {
        var list = new List<Attribute>();
        if (FindOneAttribute("id") == null)
        {
            list.Add(new Attribute
            {
                Field = "id", Header = "id",InList = true, InDetail = true, IsDefault = true, DataType = DataType.Int,
                Type = DisplayType.text
            });
        }
        
        list.AddRange(Attributes);

        if (FindOneAttribute("created_at") == null)
        {
            list.Add(new Attribute
            {
                Field = "created_at", Header = "Created At",InList = true, InDetail = true, IsDefault = true, DataType = DataType.Datetime
            });
        }

        if (FindOneAttribute("updated_at") == null)
        {
            list.Add(new Attribute
            {
                Field = "updated_at", Header = "Updated At", InList = true, InDetail = true, IsDefault = true, DataType = DataType.Datetime
            });
        }

        if (FindOneAttribute("deleted") == null)
        {
            list.Add(new Attribute
            {
                Field = "deleted", InList = true, InDetail = true, IsDefault = true, DataType = DataType.Int
            });
        }
        Attributes = list.ToArray();
    }

    public void RemoveDeleted()
    {
        Attributes = Attributes.Where(x => x.Field != "deleted").ToArray();
    }

    public Attribute[] ReferencedAttributes()
    {
        return Attributes.Where(x => x.Field == PrimaryKey || x.Field == TitleAttribute).ToArray();
    }

    public string Fullname(string fieldName)
    {
        return TableName + "." + fieldName;
    }

    public Attribute[] LocalAttributes()
    {
        return Attributes.Where(x=>x.Type != DisplayType.crosstable).ToArray();
    }
    
    public Attribute[] LocalAttributes(InListOrDetail listOrDetail)
    {
        return Attributes.Where(x =>
                x.Type != DisplayType.crosstable && 
                (listOrDetail == InListOrDetail.InList ? x.InList : x.InDetail))
            .ToArray();
    }

    public Attribute[] LocalAttributes(string[] attributes)
    {
        return Attributes
            .Where(x => x.Type != DisplayType.crosstable && attributes.Contains(x.Field)).ToArray();
    }

    public Attribute[] GetAttributesByType(DisplayType type)
    {
        return Attributes.Where(x => x.Type == type).ToArray();
    }

    public Attribute[] GetAttributesByType(DisplayType type, string[] attributes)
    {
        return Attributes.Where(x => x.Type == type && attributes.Contains(x.Field)).ToArray();
    }
     public Attribute[] GetAttributesByType(DisplayType type, InListOrDetail listOrDetail)
     {
         return Attributes.Where(x => x.Type == type && (listOrDetail == InListOrDetail.InList ? x.InList : x.InDetail))
             .ToArray();
     }
   

    public Attribute PrimaryKeyAttribute()
    {
        return Attributes.First(x => x.Field == PrimaryKey);
    }
    
    public Attribute? FindOneAttribute(string name)
    {
        return Attributes.FirstOrDefault(x => x.Field == name);
    }
    public Query One(Filters? filters, Attribute[]attributes)
    {
        var query = Basic().Select(attributes.Select(x=>x.FullName()));
        filters?.Apply(this, query);
        return query;
    }
    
    public Query? ById(string key, Attribute[]attributes)
    {
        var id = PrimaryKeyAttribute().CastToDatabaseType(key);
        return Basic().Where(PrimaryKey, id)
            .Select(attributes.Select(x=>x.FullName()));
    }
    
    public Query? List(Filters? filters,Sorts? sorts, Pagination? pagination,Cursor? cursor,  Attribute[]? attributes)
    {
        if (attributes is null)
        {
            return null;
        }
        var query = Basic().Select(attributes.Select(x=>x.FullName()));
        pagination?.Apply (query);
        cursor?.Apply (this,query,sorts);
        sorts?.Apply(this, query);
        filters?.Apply(this, query);
        return query;
    }

    public Query Count(Filters? filters)
    {
        var query = Basic();
        filters?.Apply(this, query);
        return query;
    }


    public Query Many(object[]ids, Attribute[] attributes)
    {
        if (ids.Length == 0)
        {
            throw new Exception("ids is empty");
        }
        var lstFields = attributes.Select(x => x.Field);
        return Basic().Select(lstFields.ToArray()).WhereIn(PrimaryKey,ids);
    }
   
    public Query Insert(Record item)
    {
        return new Query(TableName).AsInsert(item, true);
    }

    public Query? Update(Record item)
    {
        return item.TryGetValue(PrimaryKey, out var val)
            ? new Query(TableName).Where(PrimaryKey, val).AsUpdate(item.Keys, item.Values)
            : null;
    }

    public Query? Delete(Record item)
    {
        return item.TryGetValue(PrimaryKey, out var key)
            ? new Query(TableName).Where(PrimaryKey, key).AsUpdate(["deleted"], [true])
            : null;
    }
    
    public Query Basic()
    {
        return new Query(TableName).Where(TableName + ".deleted", false);
    }
}