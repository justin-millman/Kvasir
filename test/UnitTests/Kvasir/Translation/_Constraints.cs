using Cybele.Core;
using Kvasir.Core;
using Kvasir.Schema;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Translation {
    internal static class TestConstraints {
        public class CustomCheck : IConstraintGenerator {
            public static IConstraintGenerator Generator { get; }
            public static Clause Clause { get; }
            public static object?[] LastCtorArgs { get; private set; }      

            static CustomCheck() {
                Generator = Substitute.For<IConstraintGenerator>();
                Clause = Substitute.For<Clause>();
                LastCtorArgs = Array.Empty<object?>();

                Generator.MakeConstraint(
                    Arg.Any<IEnumerable<IField>>(),
                    Arg.Any<IEnumerable<DataConverter>>(),
                    Arg.Any<Settings>()
                ).Returns(Clause);
            }
            public CustomCheck(params object?[] args) {
                LastCtorArgs = args;
            }
            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return Generator.MakeConstraint(fields, converters, settings);
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
