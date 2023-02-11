using Cybele.Core;
using Kvasir.Core;
using Kvasir.Schema;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Translation {
    internal static partial class TestComponents {
        public class CustomCheck : IConstraintGenerator {
            public static Mock<IConstraintGenerator> Mock { get; private set; }
            public static Clause Clause { get; } = new Mock<Clause>().Object;
            public static object[] CtorArgs { get; private set; }

            public CustomCheck() {
                Mock = new Mock<IConstraintGenerator>();
                CtorArgs = new object[] {};
                Mock.Setup(e => e.MakeConstraint(
                    It.IsAny<IEnumerable<IField>>(),
                    It.IsAny<IEnumerable<DataConverter>>(),
                    It.IsAny<Settings>())
                ).Returns(Clause);
            }
            public CustomCheck(int first, params object[] args)
                : this() {

                CtorArgs = args.Prepend(first).ToArray();
            }
            public CustomCheck(bool arg)
                : this() {

                CtorArgs = new object[] { arg };
                throw new SystemException("System Failure!");
            }

            public Clause MakeConstraint(IEnumerable<IField> fields, IEnumerable<DataConverter> converters, Settings settings) {
                return Mock.Object.MakeConstraint(fields, converters, settings);
            }
        }
    }
}
