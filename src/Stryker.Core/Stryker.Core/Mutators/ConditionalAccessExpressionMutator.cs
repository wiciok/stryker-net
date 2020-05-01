using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class ConditionalAccessExpressionMutator : MutatorBase<ConditionalAccessExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(ConditionalAccessExpressionSyntax node)
        {
            return Mutate(node, node);
        }

        private IEnumerable<Mutation> Mutate(ConditionalAccessExpressionSyntax node, SyntaxNode originalNode)
        {
            if (node.WhenNotNull is InvocationExpressionSyntax invocationExpression)
            {
                //var expression = node.Expression;
                //if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessExpression)
                //{
                //    if (memberAccessExpression.Expression is InvocationExpressionSyntax)
                //    {
                //        expression = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, GetName(memberAccessExpression.Expression)));
                //    }
                //}
                //var replacementNode = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, GetName(invocationExpression.Expression)));
                var removed = RemoveConditional(node);
                yield return new Mutation()
                {
                    OriginalNode = originalNode,
                    ReplacementNode = originalNode.ReplaceNode(node, removed),
                    DisplayName = "Conditional access mutation",
                    Type = Mutator.Initializer
                };
            } else if (node.WhenNotNull is MemberBindingExpressionSyntax memberBindingExpression)
            {
                yield return new Mutation()
                {
                    OriginalNode = originalNode,
                    ReplacementNode = originalNode.ReplaceNode(node, SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, node.Expression, memberBindingExpression.Name)),
                    DisplayName = "Conditional access mutation",
                    Type = Mutator.Initializer
                };
            } else if (node.WhenNotNull is ConditionalAccessExpressionSyntax conditionalAccessExpression)
            {
                var removed = RemoveConditional(node);
                yield return new Mutation()
                {
                    OriginalNode = originalNode,
                    ReplacementNode = originalNode.ReplaceNode(node, SyntaxFactory.InvocationExpression(removed)),
                    DisplayName = "Conditional access mutation",
                    Type = Mutator.Initializer
                };
                foreach (var mutation in Mutate(conditionalAccessExpression, originalNode))
                {
                    yield return mutation;
                }
            }
        }

        private ExpressionSyntax RemoveConditional(ConditionalAccessExpressionSyntax conditionalAccessExpression)
        {
            var expression = conditionalAccessExpression.Expression;
            var subExpression = expression switch
            {
                MemberAccessExpressionSyntax memberAccessExpression => memberAccessExpression.Expression,
                ConditionalAccessExpressionSyntax conditionalAccess => conditionalAccess.Expression
            };

            if (subExpression is InvocationExpressionSyntax)
            {
                expression = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, GetName(subExpression)));
                return SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, GetName(conditionalAccessExpression.Expression)));
            }
            else
            {
                expression = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, GetName(subExpression)));
                return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, GetName(conditionalAccessExpression.Expression));
            }
        }

        private SimpleNameSyntax GetName(SyntaxNode node)
        {
            if (node is MemberBindingExpressionSyntax memberBindingExpression)
            {
                return memberBindingExpression.Name;
            } else if (node is MemberAccessExpressionSyntax memberAccessExpression)
            {
                return memberAccessExpression.Name;
            } else if (node is InvocationExpressionSyntax invocationExpression)
            {
                return GetName(invocationExpression.Expression);
            }
            return null;
        }
    }
}
