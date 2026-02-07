using System.Reflection;
using Npgsql;
using OrmLib.Metadata;

namespace OrmLib.Relationships;

/// <summary>
/// Loads related entities for navigation properties.
/// </summary>
internal static class NavigationPropertyLoader
{
    /// <summary>
    /// Loads a single related entity (one-to-one or many-to-one relationship).
    /// </summary>
    /// <param name="entity">The entity that owns the navigation property.</param>
    /// <param name="relationship">The relationship metadata.</param>
    /// <param name="referencedMetadata">The metadata for the referenced entity type.</param>
    /// <param name="connection">The database connection.</param>
    /// <returns>The related entity, or null if not found.</returns>
    public static object? LoadSingle(
        object entity,
        RelationshipMetadata relationship,
        TableMetadata referencedMetadata,
        NpgsqlConnection connection)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (relationship == null)
            throw new ArgumentNullException(nameof(relationship));
        if (referencedMetadata == null)
            throw new ArgumentNullException(nameof(referencedMetadata));
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        // Get the foreign key value from the entity
        var foreignKeyValue = relationship.ForeignKeyProperty.GetValue(entity);
        if (foreignKeyValue == null)
            return null;

        // Build SELECT query for the referenced entity
        var referencedColumnName = relationship.ReferencedColumnName ?? referencedMetadata.PrimaryKey.ColumnName;
        var sql = $"SELECT * FROM \"{referencedMetadata.TableName}\" WHERE \"{referencedColumnName}\" = @p1";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@p1", foreignKeyValue);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapToEntity(reader, referencedMetadata);
        }

        return null;
    }

    /// <summary>
    /// Loads a collection of related entities (one-to-many relationship).
    /// </summary>
    /// <param name="entity">The entity that owns the navigation property.</param>
    /// <param name="relationship">The relationship metadata.</param>
    /// <param name="referencedMetadata">The metadata for the referenced entity type (the collection element type).</param>
    /// <param name="ownerMetadata">The metadata for the entity that owns the navigation property.</param>
    /// <param name="connection">The database connection.</param>
    /// <returns>A list of related entities.</returns>
    public static List<object> LoadCollection(
        object entity,
        RelationshipMetadata relationship,
        TableMetadata referencedMetadata,
        TableMetadata ownerMetadata,
        NpgsqlConnection connection)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (relationship == null)
            throw new ArgumentNullException(nameof(relationship));
        if (referencedMetadata == null)
            throw new ArgumentNullException(nameof(referencedMetadata));
        if (ownerMetadata == null)
            throw new ArgumentNullException(nameof(ownerMetadata));
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        var results = new List<object>();

        // Get the primary key value from the owner entity
        var primaryKeyValue = ownerMetadata.PrimaryKey.GetValue(entity);
        if (primaryKeyValue == null)
            return results;

        // For a collection navigation property, the foreign key is in the referenced table
        // pointing back to the owner. The foreign key property name in the relationship
        // tells us the property name in the referenced entity that holds the foreign key.
        // We need to find the column in the referenced table that matches this.
        var foreignKeyPropertyName = relationship.ForeignKeyProperty.Name;
        var foreignKeyColumn = referencedMetadata.GetColumnByPropertyName(foreignKeyPropertyName);
        
        if (foreignKeyColumn == null)
        {
            // Try to find by column name (might be snake_case)
            var possibleColumnNames = new[]
            {
                foreignKeyPropertyName.ToLower(),
                ToSnakeCase(foreignKeyPropertyName)
            };

            foreach (var columnName in possibleColumnNames)
            {
                foreignKeyColumn = referencedMetadata.GetColumnByColumnName(columnName);
                if (foreignKeyColumn != null)
                    break;
            }
        }

        if (foreignKeyColumn == null)
        {
            throw new InvalidOperationException($"Could not find foreign key column for '{foreignKeyPropertyName}' in table '{referencedMetadata.TableName}'.");
        }

        // Build SELECT query for related entities
        var sql = $"SELECT * FROM \"{referencedMetadata.TableName}\" WHERE \"{foreignKeyColumn.ColumnName}\" = @p1";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@p1", primaryKeyValue);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var relatedEntity = MapToEntity(reader, referencedMetadata);
            if (relatedEntity != null)
            {
                results.Add(relatedEntity);
            }
        }

        return results;
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLower(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }

    private static object? MapToEntity(NpgsqlDataReader reader, TableMetadata metadata)
    {
        var entity = Activator.CreateInstance(metadata.EntityType);
        if (entity == null)
            return null;

        foreach (var column in metadata.Columns)
        {
            try
            {
                var columnIndex = reader.GetOrdinal(column.ColumnName);
                if (reader.IsDBNull(columnIndex))
                {
                    if (!column.IsRequired)
                    {
                        column.SetValue(entity, null);
                    }
                }
                else
                {
                    var value = reader.GetValue(columnIndex);
                    column.SetValue(entity, value);
                }
            }
            catch
            {
                // Column not found in result set, skip it
            }
        }

        return entity;
    }
}

