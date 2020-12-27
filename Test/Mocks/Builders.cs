using Kvasir.Schema;
using Kvasir.Schema.Constraints;
using Kvasir.Transcription.Internal;
using System;
using System.Collections.Generic;

namespace Test.Mocks {
    internal sealed class MockBuilders : IBuilderCollection {
        public IFieldDeclBuilder FieldDeclBuilder() {
            return new MockFieldSyntaxGenerator();
        }
        public IConstraintDeclBuilder ConstraintDeclBuilder() {
            return new MockConstraintSyntaxGenerator();
        }
    }

    internal sealed class MockFieldSyntaxGenerator : IFieldDeclBuilder {
        public MockFieldSyntaxGenerator() {
            name_ = string.Empty;
            type_ = string.Empty;
            nullability_ = string.Empty;
            defaultValue_ = string.Empty;
            restriction_ = string.Empty;

            Reset();
        }
        public SqlSnippet Build() {
            return new SqlSnippet($"{name_} {type_} {nullability_} --{defaultValue_}-- := ({restriction_})");
        }
        public void Reset() {
            name_ = string.Empty;
            type_ = string.Empty;
            nullability_ = string.Empty;
            defaultValue_ = "no default";
            restriction_ = "all values";
        }
        public void SetName(FieldName name) {
            name_ = (string)name!;
        }
        public void SetDataType(DBType type) {
            type_ = type.ToString()!;
        }
        public void SetNullability(IsNullable nullability) {
            nullability_ = (nullability == IsNullable.Yes) ? "IS NULL" : "IS NOT NULL";
        }
        public void SetDefaultValue(DBValue defaultValue) {
            defaultValue_ = defaultValue.ToString();
        }
        public void SetAllowedValues(IEnumerable<DBValue> enumerators) {
            restriction_ = string.Join(", ", enumerators);
        }


        private string name_;
        private string type_;
        private string nullability_;
        private string defaultValue_;
        private string restriction_;
    }

    internal sealed class MockConstraintSyntaxGenerator : IConstraintDeclBuilder {
        public MockConstraintSyntaxGenerator() {
            tokens_ = new Stack<string>();
        }
        public SqlSnippet Build() {
            return new SqlSnippet(tokens_.Pop());
        }
        public void Reset() {
            tokens_.Clear();
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
