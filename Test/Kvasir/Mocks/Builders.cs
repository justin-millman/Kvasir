using Kvasir.Transcription.Internal;
using Moq;

namespace Test.Kvasir.Schema {
    internal static partial class MoqExtensions {
        public static Mock<IBuilderCollection> MockByDefault(this Mock<IBuilderCollection> self) {
            self.DefaultValue = DefaultValue.Mock;
            return self;
        }

        public static Mock<IConstraintDeclBuilder> ConstraintBuilder(this Mock<IBuilderCollection> self) {
            return Mock.Get(self.Object.ConstraintDeclBuilder());
        }
        public static Mock<IFieldDeclBuilder> FieldBuilder(this Mock<IBuilderCollection> self) {
            return Mock.Get(self.Object.FieldDeclBuilder());
        }
        public static Mock<IKeyDeclBuilder> KeyBuilder(this Mock<IBuilderCollection> self) {
            return Mock.Get(self.Object.KeyDeclBuilder());
        }
    }
}
