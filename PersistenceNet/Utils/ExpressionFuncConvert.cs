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

            var conditions = new List<(string property, ExpressionType operatorx, object? value, ExpressionType expressionType)>();
            ParseExpression(sourceExpression.Body, conditions, ExpressionType.Default);

            var parameter = Expression.Parameter(typeof(TTarget), "find");
            Expression? finalExpression = null;

            foreach (var (property, operatorx, value, expressionType) in conditions)
            {
                var propertyIn = PropertyExist(parameter, property);
                if (propertyIn is null)
                {
                    var propertyRootClass = PropertyExist(parameter, findIn) ?? throw new Exception($"Property '{findIn}' not found!");
                    propertyIn = Expression.Property(propertyRootClass!, property);
                }

                var constant = GetConstantValue(propertyIn, value);
                var comparison = ParseExpression(propertyIn, constant, operatorx);

                finalExpression = finalExpression == null ? comparison : CreateExpressionType(finalExpression, comparison, expressionType);
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

        private static void ParseExpression(Expression expression, List<(string property, ExpressionType operatorx, object? value, ExpressionType expressionType)> conditions
            , ExpressionType expressionType)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                if (binaryExpression.NodeType == ExpressionType.AndAlso ||
                    binaryExpression.NodeType == ExpressionType.OrElse)
                {
                    ParseExpression(binaryExpression.Left, conditions, binaryExpression.NodeType);
                    ParseExpression(binaryExpression.Right, conditions, binaryExpression.NodeType);
                }
                else
                {
                    string? propertyName = GetPropertyName(binaryExpression.Left);
                    object? value = GetConstantValue(binaryExpression.Right);

                    if (!string.IsNullOrEmpty(propertyName))
                        conditions.Add((propertyName!, binaryExpression.NodeType, value ?? null, expressionType));
                }
            }
            else if (expression is MethodCallExpression methodCall)
            {
                string? propertyName = GetPropertyName(methodCall.Object!);
                object? value = methodCall.Arguments.Count > 0 ? GetConstantValue(methodCall.Arguments[0]) : null;

                if (!string.IsNullOrEmpty(propertyName))
                    conditions.Add((propertyName!, ExpressionType.Call, value, expressionType));
            }
        }

        private static string? GetPropertyName(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            if (expression is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression innerMember)
                return innerMember.Member.Name;

            return null;
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

        private static ConstantExpression GetConstantValue(MemberExpression propertyIn, object? value)
        {
            if (value is null)
                return Expression.Constant(value);

            var targetType = propertyIn.Type;
            object? typedValue = targetType.IsEnum ? Enum.ToObject(targetType, value!) : Convert.ChangeType(value, targetType);

            return Expression.Constant(typedValue, targetType);
        }

        private static Expression ParseExpression(MemberExpression property, ConstantExpression constant, ExpressionType operatorx)
        {
            return operatorx switch
            {
                ExpressionType.Equal => Expression.Equal(property, constant),
                ExpressionType.NotEqual => Expression.NotEqual(property, constant),
                ExpressionType.GreaterThan => Expression.GreaterThan(property, constant),
                ExpressionType.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
                ExpressionType.LessThan => Expression.LessThan(property, constant),
                ExpressionType.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
                ExpressionType.Call => Expression.Call(property, typeof(string).GetMethod("Contains", [typeof(string)])!, constant),
                _ => throw new NotSupportedException($"Operator '{operatorx}' is not supported!")
            };
        }

        private static BinaryExpression CreateExpressionType(Expression? finalExpression, Expression comparison, ExpressionType expressionType)
        {
            return expressionType.Equals(ExpressionType.AndAlso) ? Expression.AndAlso(finalExpression!, comparison) :
                   expressionType.Equals(ExpressionType.And) ? Expression.And(finalExpression!, comparison) :
                   expressionType.Equals(ExpressionType.OrElse) ? Expression.OrElse(finalExpression!, comparison) :
                    Expression.Or(finalExpression!, comparison);
        }
        #endregion
    }
}