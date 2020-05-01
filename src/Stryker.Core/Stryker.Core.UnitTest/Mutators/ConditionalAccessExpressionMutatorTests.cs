using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ConditionalAccessExpressionMutatorTests
    {
        [Fact]
        public void ShouldMutateConditionalInvocations()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("myArray?.ToList()") as ConditionalAccessExpressionSyntax;

            var target = new ConditionalAccessExpressionMutator();

            var result = target.ApplyMutations(expressionSyntax);

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ToString().ShouldBe("myArray.ToList()");
        }

        [Fact]
        public void ShouldMutateConditionalMemberAccess()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("myArray?.Count") as ConditionalAccessExpressionSyntax;

            var target = new ConditionalAccessExpressionMutator();

            var result = target.ApplyMutations(expressionSyntax);

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ToString().ShouldBe("myArray.Count");
        }

        [Fact]
        public void ShouldMutateRecursive()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("myArray?.ToList()?.Count()") as ConditionalAccessExpressionSyntax;

            var target = new ConditionalAccessExpressionMutator();

            var result = target.ApplyMutations(expressionSyntax);

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void ShouldMutateRecursiveMax()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("myArray?.ToList()?.MyList?.ToList()?.Count") as ConditionalAccessExpressionSyntax;

            var target = new ConditionalAccessExpressionMutator();

            var result = target.ApplyMutations(expressionSyntax);

            result.Count().ShouldBe(4);
        }

        [Theory]
        [InlineData("myArray.ToList()?.Count()")]
        [InlineData("myArray?.ToList().Count()")]
        public void ShouldMutateInAnyAccessExpression(string expression)
        {
            var expressionSyntax = SyntaxFactory.ParseExpression(expression) as ConditionalAccessExpressionSyntax;

            var target = new ConditionalAccessExpressionMutator();

            var result = target.ApplyMutations(expressionSyntax);

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ToString().ShouldBe("myArray.ToList().Count()");
        }
    }
}
