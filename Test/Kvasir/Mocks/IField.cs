using Kvasir.Schema;
using Kvasir.Transcription.Internal;
using Moq;
using Optional;

namespace Test.Kvasir.Schema {
    internal static partial class MoqExtensions {
        public static Mock<IField> AsNullable(this Mock<IField> self) {
            self.Setup(f => f.Nullability).Returns(IsNullable.Yes);
            return self;
        }
        public static Mock<IField> WithDefault(this Mock<IField> self, object defaultValue) {
            self.Setup(f => f.DefaultValue).Returns(Option.Some(DBValue.Create(defaultValue)));
            return self;
        }
        public static Mock<IField> WithName(this Mock<IField> self, string name) {
            self.Setup(f => f.Name).Returns(new FieldName(name));
            return self;
        }
        public static Mock<IField> WithType(this Mock<IField> self, DBType dataType) {
            self.Setup(f => f.DataType).Returns(dataType);
            return self;
        }
        public static Mock<IField> WithDecl(this Mock<IField> self, string sqlDeclaration) {
            var sql = new SqlSnippet(sqlDeclaration);
            self.Setup(f => f.GenerateDeclaration(It.IsAny<IBuilderCollection>())).Returns(sql);
            return self;
        }
    }
}
