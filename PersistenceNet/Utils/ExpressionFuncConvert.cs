using System.Linq.Expressions;

namespace PersistenceNet.Utils
{
    public static class ExpressionFuncConvert
    {
        #region Métodos publicos
        public static Expression<Func<TTarget, bool>> Builder<TSource, TTarget>(Expression<Func<TSource, bool>> sourceExpression)
        {
            if (sourceExpression.Parameters is null || sourceExpression.Parameters.Count.Equals(0))
                throw new Exception("Nenhum parâmetro encontrado. Verifique os filtros da expressão!");

            var conditions = new List<(string Property, object Value, string Operator)>();
            ParseExpression(sourceExpression.Body, conditions);

            var parameter = Expression.Parameter(typeof(TTarget), "find");
            Expression? finalExpression = null;

            foreach (var condition in conditions)
            {
                var property = Expression.Property(parameter, condition.Property);
                var constant = Expression.Constant(Convert.ChangeType(condition.Value, property.Type));
                var comparison = Expression.Equal(property, constant);

                finalExpression = finalExpression == null ? comparison : Expression.AndAlso(finalExpression, comparison);
            }

            if (finalExpression is null)
                throw new Exception("Não foi possível criar uma expressão válida! Causas: Nome da propriedade não existe entre a entidade e a modelView ou tipo da propriedade não é mesmo entre elas.");

            return Expression.Lambda<Func<TTarget, bool>>(finalExpression!, parameter);
        }

        public static Expression<Func<TTarget, object>>[] ConvertIncludesExpression<TSource, TTarget>(Expression<Func<TSource, object>>[] viewModelIncludes)
        {
            if (viewModelIncludes is null || viewModelIncludes.Length.Equals(0))
                throw new Exception("Parêmetros de entrada não localizado!");

            var includes = new List<Expression<Func<TTarget, object>>>();
            foreach (var include in viewModelIncludes)
                includes.Add(ConvertIncludeExpression<TSource, TTarget>(include));

            return [.. includes];
        }
        #endregion

        #region Métodos privados
        private static Expression<Func<TTarget, object>> ConvertIncludeExpression<TSource, TTarget>(Expression<Func<TSource, object>> viewModelInclude)
        {
            if (viewModelInclude.Body is MemberExpression memberExpr)
            {
                var parameter = Expression.Parameter(typeof(TTarget), "inc");
                var entityMember = Expression.PropertyOrField(parameter, memberExpr.Member.Name);

                return Expression.Lambda<Func<TTarget, object>>(entityMember, parameter);
            }

            throw new Exception($"Não foi possível converter a expressão '{viewModelInclude.Name}'.");
        }

        private static void ParseExpression(Expression expression, List<(string Property, object Value, string Operator)> conditions)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                if (binaryExpression.NodeType == ExpressionType.AndAlso || binaryExpression.NodeType == ExpressionType.OrElse)
                {
                    ParseExpression(binaryExpression.Left, conditions);
                    ParseExpression(binaryExpression.Right, conditions);
                }
                else
                {
                    string? propertyName = GetPropertyName(binaryExpression.Left);
                    object? value = GetConstantValue(binaryExpression.Right);
                    string? operador = GetOperator(binaryExpression.NodeType);

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
