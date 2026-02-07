using System.Linq.Expressions;
using System.Text;
using OrmLib.Metadata;

namespace OrmLib.Query;

/// <summary>
/// Visits expression trees and translates them to SQL WHERE and ORDER BY clauses.
/// </summary>
internal class SqlExpressionVisitor : ExpressionVisitor
{
    private readonly StringBuilder _sql = new();
    private readonly TableMetadata _metadata;
    private readonly List<object?> _parameters = new();
    private int _parameterIndex = 1;

    /// <summary>
    /// Gets the SQL clause generated from the expression.
    /// </summary>
    public string Sql => _sql.ToString();

    /// <summary>
    /// Gets the parameters for the SQL clause.
    /// </summary>
    public IReadOnlyList<object?> Parameters => _parameters.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlExpressionVisitor"/> class.
    /// </summary>
    /// <param name="metadata">The table metadata for mapping properties to columns.</param>
    public SqlExpressionVisitor(TableMetadata metadata)
    {
        _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    /// <summary>
    /// Translates an expression to SQL.
    /// </summary>
    /// <param name="expression">The expression to translate.</param>
    /// <returns>A tuple containing the SQL clause and parameters.</returns>
    public (string Sql, IReadOnlyList<object?> Parameters) Translate(Expression expression)
    {
        _sql.Clear();
        _parameters.Clear();
        _parameterIndex = 1;
        Visit(expression);
        return (_sql.ToString(), _parameters.AsReadOnly());
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _sql.Append("(");
        Visit(node.Left);

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                _sql.Append(" = ");
                break;
            case ExpressionType.NotEqual:
                _sql.Append(" <> ");
                break;
            case ExpressionType.GreaterThan:
                _sql.Append(" > ");
                break;
            case ExpressionType.GreaterThanOrEqual:
                _sql.Append(" >= ");
                break;
            case ExpressionType.LessThan:
                _sql.Append(" < ");
                break;
            case ExpressionType.LessThanOrEqual:
                _sql.Append(" <= ");
                break;
            case ExpressionType.AndAlso:
                _sql.Append(" AND ");
                break;
            case ExpressionType.OrElse:
                _sql.Append(" OR ");
                break;
            default:
                throw new NotSupportedException($"Unsupported binary expression type: {node.NodeType}");
        }

        Visit(node.Right);
        _sql.Append(")");
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        // Handle property access like x.FirstName
        if (node.Member is System.Reflection.PropertyInfo property)
        {
            var column = _metadata.GetColumnByPropertyName(property.Name);
            if (column != null)
            {
                _sql.Append($"\"{column.ColumnName}\"");
            }
            else
            {
                // If not found in metadata, use property name (fallback)
                _sql.Append($"\"{property.Name}\"");
            }
        }
        else
        {
            // Handle other member access (like DateTime.Now)
            if (node.Expression == null)
            {
                // Static member access
                throw new NotSupportedException($"Static member access not supported: {node.Member.Name}");
            }
            else if (node.Expression.NodeType == ExpressionType.Constant)
            {
                // Try to evaluate the constant value
                var constantExpression = (ConstantExpression)node.Expression;
                var value = EvaluateMemberAccess(node, constantExpression.Value);
                VisitConstant(Expression.Constant(value));
            }
            else
            {
                throw new NotSupportedException($"Complex member access not supported: {node.Member.Name}");
            }
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value == null)
        {
            _sql.Append("NULL");
        }
        else if (node.Type == typeof(string) || node.Type == typeof(DateTime) || node.Type == typeof(DateTimeOffset))
        {
            // Use parameterized queries for string and date values
            _sql.Append($"@p{_parameterIndex}");
            _parameters.Add(node.Value);
            _parameterIndex++;
        }
        else if (node.Type.IsPrimitive || node.Type == typeof(decimal))
        {
            // Use parameterized queries for numeric values
            _sql.Append($"@p{_parameterIndex}");
            _parameters.Add(node.Value);
            _parameterIndex++;
        }
        else
        {
            // Fallback: embed value directly (shouldn't happen often)
            if (node.Type == typeof(string))
            {
                _sql.Append($"'{EscapeString(node.Value.ToString() ?? "")}'");
            }
            else
            {
                _sql.Append(node.Value);
            }
        }

        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        // Handle common string methods
        if (node.Object != null && node.Object.Type == typeof(string))
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    if (node.Arguments.Count == 1)
                    {
                        Visit(node.Object);
                        _sql.Append(" LIKE ");
                        _sql.Append($"@p{_parameterIndex}");
                        var argValue = EvaluateExpression(node.Arguments[0]);
                        _parameters.Add($"%{argValue}%");
                        _parameterIndex++;
                        return node;
                    }
                    break;
                case "StartsWith":
                    if (node.Arguments.Count == 1)
                    {
                        Visit(node.Object);
                        _sql.Append(" LIKE ");
                        _sql.Append($"@p{_parameterIndex}");
                        var argValue = EvaluateExpression(node.Arguments[0]);
                        _parameters.Add($"{argValue}%");
                        _parameterIndex++;
                        return node;
                    }
                    break;
                case "EndsWith":
                    if (node.Arguments.Count == 1)
                    {
                        Visit(node.Object);
                        _sql.Append(" LIKE ");
                        _sql.Append($"@p{_parameterIndex}");
                        var argValue = EvaluateExpression(node.Arguments[0]);
                        _parameters.Add($"%{argValue}");
                        _parameterIndex++;
                        return node;
                    }
                    break;
            }
        }

        throw new NotSupportedException($"Method call not supported: {node.Method.Name}");
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        switch (node.NodeType)
        {
            case ExpressionType.Not:
                _sql.Append("NOT ");
                Visit(node.Operand);
                return node;
            case ExpressionType.Convert:
                // Ignore type conversions
                Visit(node.Operand);
                return node;
            default:
                throw new NotSupportedException($"Unsupported unary expression type: {node.NodeType}");
        }
    }

    private object? EvaluateMemberAccess(MemberExpression node, object? instance)
    {
        if (node.Member is System.Reflection.PropertyInfo property)
        {
            return property.GetValue(instance);
        }
        else if (node.Member is System.Reflection.FieldInfo field)
        {
            return field.GetValue(instance);
        }

        throw new NotSupportedException($"Cannot evaluate member access: {node.Member.Name}");
    }

    private object? EvaluateExpression(Expression expression)
    {
        if (expression is ConstantExpression constant)
        {
            return constant.Value;
        }
        else if (expression is MemberExpression member && member.Expression is ConstantExpression constExpr)
        {
            return EvaluateMemberAccess(member, constExpr.Value);
        }

        // For more complex expressions, try to compile and evaluate
        var lambda = Expression.Lambda(expression);
        var compiled = lambda.Compile();
        return compiled.DynamicInvoke();
    }

    private string EscapeString(string value)
    {
        return value.Replace("'", "''");
    }
}

