// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlStringTrimTranslator : IMethodCallTranslator
    {
        // Method defined in netstandard2.0
        private static readonly MethodInfo _methodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), new Type[] { });

        // Method defined in netstandard2.0
        private static readonly MethodInfo _methodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), new[] { typeof(char[]) });

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (_methodInfoWithoutArgs.Equals(methodCallExpression.Method)
                || _methodInfoWithCharArrayArg.Equals(methodCallExpression.Method)
                // MySql LTRIM/RTRIM does not take arguments
                && ((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0)
            {
                var sqlArguments = new[] { methodCallExpression.Object };

                return new SqlFunctionExpression(
                    "LTRIM",
                    methodCallExpression.Type,
                    new[]
                    {
                        new SqlFunctionExpression(
                            "RTRIM",
                            methodCallExpression.Type,
                            sqlArguments)
                    });
            }

            return null;
        }
    }
}
