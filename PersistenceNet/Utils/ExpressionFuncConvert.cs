using System.Linq.Expressions;

namespace PersistenceNet.Utils
{
    public static class ExpressionFuncConvert
    {
        #region Métodos publicos
        public static Expression<Func<TTarget, bool>> Builder<TSource, TTarget>(Expression<Func<TSource, bool>> sourceExpression
            , string findIn = "")
        {
            if (sourceExpression.Parameters is null || sourceExpression.Parameters.Count.Equals(0))
                throw new Exception("No parameters found. Check the Expression Filters!");

            var conditions = new List<(string property, object value, string operatorx)>();
            ParseExpression(sourceExpression.Body, conditions);

            var parameter = Expression.Parameter(typeof(TTarget), "find");
            Expression? finalExpression = null;

            foreach (var (property, value, operatorx) in conditions)
            {
                var propertyIn = PropertyExist(parameter, property);

                if (propertyIn is null)
                {
                    var propertyRootClass = PropertyExist(parameter, findIn) ?? throw new Exception($"Property '{findIn}' not found!");
                    propertyIn = Expression.Property(propertyRootClass!, property);
                }
                var constant = Expression.Constant(Convert.ChangeType(value, propertyIn.Type));
                var comparison = ParseExpression(propertyIn, constant, operatorx);

                finalExpression = finalExpression == null ? comparison : Expression.AndAlso(finalExpression, comparison);
            }

            if (finalExpression is null)
                throw new Exception("Couldn't create a valid expression! " +
                    "Causes: Property name does not exist between the entity and the modelView or " +
                    "Property type is not even between them.");

            return Expression.Lambda<Func<TTarget, bool>>(finalExpression!, parameter);
        }

        public static Expression<Func<TTarget, object>>[] ConvertIncludesExpression<TSource, TTarget>(Expression<Func<TSource, object>>[] viewModelIncludes
            , string findIn = "")
        {
            if (viewModelIncludes is null || viewModelIncludes.Length.Equals(0))
                throw new Exception("Input parameters not found!");

            var includes = new List<Expression<Func<TTarget, object>>>();
            foreach (var include in viewModelIncludes)
                includes.Add(ConvertIncludeExpression<TSource, TTarget>(include, findIn));

            return [.. includes];
        }
        #endregion

        #region Métodos privados
        private static Expression<Func<TTarget, object>> ConvertIncludeExpression<TSource, TTarget>(Expression<Func<TSource, object>> viewModelInclude
            , string findIn = "")
        {
            if (viewModelInclude.Body is MemberExpression memberExpr)
            {
                var parameter = Expression.Parameter(typeof(TTarget), "inc");

                var entityMember = PropertyOrFieldExist(parameter, memberExpr.Member.Name);
                if (entityMember is null)
                {
                    entityMember = PropertyOrFieldExist(parameter, findIn);
                    if (entityMember is null)
                        throw new Exception($"Property '{findIn}' not found!");

                    entityMember = PropertyOrFieldExist(entityMember, memberExpr.Member.Name);
                }

                return Expression.Lambda<Func<TTarget, object>>(entityMember!, parameter);
            }

            throw new Exception($"Could not convert expression '{viewModelInclude.Name}'.");
        }

        private static MemberExpression? PropertyExist(Expression parameter, string propertyPath)
        {
            try { return Expression.Property(parameter, propertyPath); }
            catch { }
            return null;
        }

        private static MemberExpression? PropertyOrFieldExist(Expression parameter, string propertyPath)
        {
            try { return Expression.PropertyOrField(parameter, propertyPath); }
            catch { }
            return null;
        }

        private static void ParseExpression(Expression expression, List<(string property, object value, string operatorx)> conditions)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                if (binaryExpression.NodeType == ExpressionType.AndAlso || 
                    binaryExpression.NodeType == ExpressionType.OrElse)
                {
                    ParseExpression(binaryExpression.Left, conditions);
                    ParseExpression(binaryExpression.Right, conditions);
                }
                else
                {
                    string? propertyName = GetPropertyName(binaryExpression.Left);
                    string? operador = GetOperator(binaryExpression.NodeType);
                    object? value = GetConstantValue(binaryExpression.Right);
                   
                    if (!string.IsNullOrEmpty(propertyName) && value is not null)
                        conditions.Add((propertyName!, value!, operador));
                }
            }
        }

        private static string? GetPropertyName(Expression expression)
        {
            return expression is MemberExpression memberExpression ? memberExpression.Member.Name : null;
        }

        private static object? GetConstantValue(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
                return constantExpression.Value;

            if (expression is MemberExpression memberExpression)
            {
                var objectMember = Expression.Convert(memberExpression, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);

                return getterLambda.Compile()();
            }

            return null;
        }

        private static BinaryExpression ParseExpression(MemberExpression property, ConstantExpression constant, string operatorx)
        {
            return operatorx switch
            {
                "==" => Expression.Equal(property, constant),
                "!=" => Expression.NotEqual(property, constant),
                ">" => Expression.GreaterThan(property, constant),
                ">=" => Expression.GreaterThanOrEqual(property, constant),
                "<" => Expression.LessThan(property, constant),
                "<=" => Expression.LessThanOrEqual(property, constant),
                _ => throw new NotSupportedException($"Operator '{operatorx}' is not supported!")
            };
        }

        private static string GetOperator(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.Equal => "==",
                ExpressionType.NotEqual => "!=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                _ => nodeType.ToString()
            };
        }
        #endregion
    }
}