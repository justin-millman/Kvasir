using Cybele.Core;
using Kvasir.Core;
using Kvasir.Schema;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Translation {
    internal static class TestConstraints {
        public class CustomCheck : IConstraintGenerator {
            public static Mock<IConstraintGenerator> Generator { get; }
            public static Clause Clause { get; }
            public static object?[] LastCtorArgs { get; private set; }      

            static CustomCheck() {
                Generator = new Mock<IConstraintGenerator>();
                Clause = new Mock<Clause>().Object;
                LastCtorArgs = Array.Empty<object?>();

                Generator.Setup(g => g.MakeConstraint(
                    It.IsAny<IEnumerable<IField>>(),
                    It.IsAny<IEnumerable<DataConverter>>(),
                    It.IsAny<Settings>()
                )).Returns(Clause);
            }
            public CustomCheck(params object?[] args) {
                LastCtorArgs = args;
            }
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return Generator.Object.MakeConstraint(fields, converters, settings);
            }
        }
        public class PrivateCheck : IConstraintGenerator {
            private PrivateCheck() {}
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                throw new InvalidOperationException(CANNOT_CREATE_MSG);
            }
        }
        public class UnconstructibleCheck : IConstraintGenerator {
            public UnconstructibleCheck(params object?[] _) {
                throw new InvalidOperationException(CANNOT_CONSTRUCT_MSG);
            }
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                throw new NotImplementedException();
            }
        }
        public class UnusableCheck : IConstraintGenerator {
            public UnusableCheck(params object?[] _) {}
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                throw new InvalidOperationException(CANNOT_CREATE_MSG);
            }
        }


        public static readonly string CANNOT_CONSTRUCT_MSG = "This constraint type is not constructible";
        public static readonly string CANNOT_CREATE_MSG = "A constraint cannot be created";
    }
}
