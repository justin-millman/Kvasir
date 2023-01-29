using Cybele.Core;
using FluentAssertions;
using Kvasir.Transcription;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UT.Kvasir.Transcription {
    [TestClass, TestCategory("SqlSnippet")]
    public class SqlSnippetTests {
        [TestMethod] public void UsesConceptString() {
            // Arrange
            var type = typeof(SqlSnippet);
            var conceptType = typeof(ConceptString<>);

            // Act
            var baseType = type.BaseType;
            while (baseType is not null) {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == conceptType) {
                    break;
                }
                baseType = baseType.BaseType;
            }

            // Assert
            baseType.Should().NotBeNull();
        }

        [TestMethod] public void WhitespaceIsStripped() {
            // Arrange
            var rawSql = "   Name\t\n\r\n";

            // Act
            var sql = new SqlSnippet(rawSql);
            var trimmed = rawSql.Trim();
            var asString = sql.ToString();

            // Assert
            asString.Should().Be(trimmed);
        }

        [TestMethod] public void EmptyIsIllegal() {
            // Arrange
            var rawSql = string.Empty;

            // Act
            Func<SqlSnippet> action = () => new SqlSnippet(rawSql);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }

        [TestMethod] public void WhitespaceOnlyIsIllegal() {
            // Arrange
            var rawSql = "   \t    \r\n   ";

            // Act
            Func<SqlSnippet> action = () => new SqlSnippet(rawSql);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithAnyMessage()
                .And
                .ParamName.Should().NotBeNullOrEmpty();
        }
    }
}
