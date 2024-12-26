using HotChocolate.Language.Visitors;
using HotChocolate.Language;
using HotChocolate.Types.Filters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System;

namespace FilterIssueTest.GraphQl
{
    public class QueryableCustomFilterVisitor : QueryableFilterVisitor
    {
        protected override ISyntaxVisitorAction Leave(ObjectValueNode node, QueryableFilterVisitorContext context)
        {
            var operations = context.PopLevel();

            if (TryCombineOperations(operations, (a, b) => Expression.AndAlso(a, b), out var combined))
            {
                context.GetLevel().Enqueue(combined);
            }

            return Continue;
        }

        protected override ISyntaxVisitorAction Leave(ListValueNode node, QueryableFilterVisitorContext context)
        {
            var combine = context.Operations.Peek() is OrField
                    ? new Func<Expression, Expression, Expression>((a, b) => Expression.OrElse(a, b))
                    : new Func<Expression, Expression, Expression>((a, b) => Expression.AndAlso(a, b));

            var operations = context.PopLevel();

            if (TryCombineOperations(operations, combine, out var combined))
            {
                context.GetLevel().Enqueue(combined);
            }

            return Continue;
        }

        private static bool TryCombineOperations(
            Queue<Expression> operations,
            Func<Expression, Expression, Expression> combine,
            [NotNullWhen(true)] out Expression combined)
        {
            if (operations.Count != 0)
            {
                combined = operations.Dequeue();

                while (operations.Count != 0)
                {
                    // Only this line was changed
                    // from (combined, operations.Dequeue())
                    // to (operations.Dequeue(), combined)
                    combined = combine(operations.Dequeue(), combined);
                }

                return true;
            }

            combined = null;

            return false;
        }
    }
}
