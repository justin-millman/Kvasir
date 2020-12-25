using Kvasir.Schema;
using Kvasir.Transcription.Internal;
using Optional;
using System.Collections.Generic;

namespace Test.Mocks {
    internal sealed class MockFactories : IGeneratorCollection {
        public IFieldDeclarationGenerator FieldDeclarationGenerator { get; } = new MockFieldSyntaxGenerator();
    }

    internal sealed class MockFieldSyntaxGenerator : IFieldDeclarationGenerator {
        public SqlSnippet GenerateSql(FieldName name, DBType dataType, IsNullable nullability,
            Option<DBValue> defaultValue, IEnumerable<DBValue> allowedValues) {

            var isNull = nullability == IsNullable.Yes ? "MAYBE NULL" : "NOT NULL";
            var defaultVal = defaultValue.Match(none: () => "--no default--", some: v => $"--{v}--");
            var values = $"( {string.Join(", ", allowedValues)} )";

            return new SqlSnippet($"[{name}] of type [{dataType}] <{isNull}> {defaultVal} := {values}");
        }
    }
}
