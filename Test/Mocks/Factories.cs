using Kvasir.Schema;
using Kvasir.Schema.Constraints;
using Kvasir.Transcription.Internal;
using Optional;
using System;
using System.Collections.Generic;

namespace Test.Mocks {
    internal sealed class MockFactories : IGeneratorCollection {
        public IFieldDeclGenerator FieldDeclGenerator { get; } = new MockFieldSyntaxGenerator();
        public IConstraintDeclGenerator ConstraintDeclGenerator { get; } = new MockConstraintSyntaxGenerator();
    }

    internal sealed class MockFieldSyntaxGenerator : IFieldDeclGenerator {
        public SqlSnippet GenerateSql(FieldName name, DBType dataType, IsNullable nullability,
            Option<DBValue> defaultValue, IEnumerable<DBValue> allowedValues) {

            var isNull = nullability == IsNullable.Yes ? "MAYBE NULL" : "NOT NULL";
            var defaultVal = defaultValue.Match(none: () => "--no default--", some: v => $"--{v}--");
            var values = $"( {string.Join(", ", allowedValues)} )";

            return new SqlSnippet($"[{name}] of type [{dataType}] <{isNull}> {defaultVal} := {values}");
        }
    }

    internal sealed class MockConstraintSyntaxGenerator : IConstraintDeclGenerator {
        public MockConstraintSyntaxGenerator() {
            tokens_ = new Stack<string>();
        }
        public SqlSnippet MakeSnippet() {
            return new SqlSnippet(tokens_.Pop());
        }
        public IDisposable NewAndClause() {
            tokens_.Push(AND);
            return new CompoundClauseHandle();
        }
        public IDisposable NewOrClause() {
            tokens_.Push(OR);
            return new CompoundClauseHandle();
        }
        public void AddCheck(NullityClause clause) {
            var op = clause.Operator == NullityOperator.IsNull ? "IS" : "IS NOT";
            var sql = $"{Stringify(clause.LHS)} {op} NULL";
            CondenseStack(sql);
        }
        public void AddCheck(InclusionClause clause) {
            var op = clause.Operator == InclusionOperator.In ? "IN" : "NOT IN";
            var sql = $"{Stringify(clause.LHS)} {op} ({string.Join(", ", clause.RHS)})";
            CondenseStack(sql);
        }
        public void AddCheck(ConstantValueClause clause) {
            var sql = $"{Stringify(clause.LHS)} {Stringify(clause.Operator)} {clause.RHS}";
            CondenseStack(sql);
        }
        public void AddCheck(CrossFieldValueConstraint clause) {
            var sql = $"{Stringify(clause.LHS)} {Stringify(clause.Operator)} {Stringify(clause.RHS)}";
            CondenseStack(sql);
        }
        private void CondenseStack(string newToken) {
            if (!tokens_.TryPeek(out string? topToken)) {
                tokens_.Push(newToken);
            }
            else if (topToken == AND || topToken == OR) {
                tokens_.Push(newToken);
            }
            else {
                tokens_.Pop();
                var op = tokens_.Pop();
                CondenseStack($"({topToken} {op} {newToken})");
            }
        }
        private static string Stringify(FieldExpression expr) {
            return expr.Function.Match(
                none: () => (string)expr.Field.Name!,
                some: fn => $"{fn}({expr.Field.Name})"
            );
        }
        private static string Stringify(ComparisonOperator op) {
            return op switch {
                ComparisonOperator.Equal => "==",
                ComparisonOperator.NotEqual => "!=",
                ComparisonOperator.LessThan => "<",
                ComparisonOperator.LessThanOrEqual => "<=",
                ComparisonOperator.GreaterThan => ">",
                ComparisonOperator.GreaterThanOrEqual => ">=",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private sealed class CompoundClauseHandle : IDisposable {
            public void Dispose() {}
        }


        private readonly Stack<string> tokens_;
        private static readonly string AND = "<AND>";
        private static readonly string OR = "<OR>";
    }
}
