﻿using Kvasir.Schema;
using Kvasir.Transcription.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.Kvasir.Schema {
    [TestClass]
    public class EnumFieldTests {
        [TestMethod, TestCategory("Default Value")]
        public void ConstructNoDefault() {
            var name = new FieldName("Color");
            var nullability = IsNullable.No;
            var defaultValue = Option.None<DBValue>();
            var allowedValues = new List<DBValue>() { DBValue.Create("Blue"), DBValue.Create("Red") };

            var field = new EnumField(name, nullability, defaultValue, allowedValues);
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(DBType.Enumeration, field.DataType);
            Assert.AreEqual(nullability, field.Nullability);
            Assert.AreEqual(defaultValue, field.DefaultValue);

            var fieldValues = new List<DBValue>(field.Enumerators);
            Assert.AreEqual(2, fieldValues.Count);
            Assert.IsTrue(allowedValues.All(v => fieldValues.Contains(v)));
        }

        [TestMethod, TestCategory("Default Value")]
        public void NullDefaultAllowed() {
            var name = new FieldName("Letter");
            var nullability = IsNullable.Yes;
            var defaultValue = Option.Some(DBValue.NULL);
            var allowedValues = new List<DBValue>() { DBValue.Create("A"), DBValue.Create("B"), DBValue.Create("X") };

            var field = new EnumField(name, nullability, defaultValue, allowedValues);
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(DBType.Enumeration, field.DataType);
            Assert.AreEqual(nullability, field.Nullability);
            Assert.AreEqual(defaultValue, field.DefaultValue);

            var fieldValues = new List<DBValue>(field.Enumerators);
            Assert.AreEqual(3, fieldValues.Count);
            Assert.IsTrue(allowedValues.All(v => fieldValues.Contains(v)));
        }

        [TestMethod, TestCategory("Default Value")]
        public void NullDefaultDisallowed() {
            var name = new FieldName("Direction");
            var nullability = IsNullable.No;
            var defaultValue = Option.Some(DBValue.NULL);
            var allowedValues = new List<DBValue>() { DBValue.Create("North"), DBValue.Create("South"),
                DBValue.Create("East"), DBValue.Create("West") };

            void action() => new EnumField(name, nullability, defaultValue, allowedValues);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("Default Value")]
        public void NonNullDefaultCorrectType() {
            var name = new FieldName("Colon");
            var nullability = IsNullable.Yes;
            var defaultValue = Option.Some(DBValue.Create(";"));
            var allowedValues = new List<DBValue>() { DBValue.Create(":"), DBValue.Create(";") };

            var field = new EnumField(name, nullability, defaultValue, allowedValues);
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(DBType.Enumeration, field.DataType);
            Assert.AreEqual(nullability, field.Nullability);
            Assert.AreEqual(defaultValue, field.DefaultValue);

            var fieldValues = new List<DBValue>(field.Enumerators);
            Assert.AreEqual(2, fieldValues.Count);
            Assert.IsTrue(allowedValues.All(v => fieldValues.Contains(v)));
        }

        [TestMethod, TestCategory("Default Value")]
        public void NonNullDefaultIncorrectType() {
            var name = new FieldName("ElementType");
            var nullability = IsNullable.No;
            var defaultValue = Option.Some(DBValue.Create(-99123L));
            var allowedValues = new List<DBValue>() { DBValue.Create("Halogen"), DBValue.Create("Noble Gas"),
                DBValue.Create("Transition Metal"), DBValue.Create("Lanthanide"), DBValue.Create("Actinide"),
                DBValue.Create("Alkali Metal"), DBValue.Create("Alkaline Earth Metal"), DBValue.Create("Metalloids") };

            void action() => new EnumField(name, nullability, defaultValue, allowedValues);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("Allowed Values")]
        public void DefaultNotInAllowedList() {
            var name = new FieldName("Position");
            var nullability = IsNullable.Yes;
            var defaultValue = Option.Some(DBValue.Create("Infielder"));
            var allowedValues = new List<DBValue>() { DBValue.Create("Pitcher"), DBValue.Create("Cather"),
                DBValue.Create("First Baseman"), DBValue.Create("Second Baseman"), DBValue.Create("Third Baseman"),
                DBValue.Create("Shortstop"), DBValue.Create("Left Fielder"), DBValue.Create("Center Fielder"),
                DBValue.Create("Right Fielder"), DBValue.Create("Designated Hitter") };

            void action() => new EnumField(name, nullability, defaultValue, allowedValues);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("Allowed Values")]
        public void NoAllowedValues() {
            var name = new FieldName("Position");
            var nullability = IsNullable.Yes;
            var defaultValue = Option.None<DBValue>();
            var allowedValues = new List<DBValue>();

            void action() => new EnumField(name, nullability, defaultValue, allowedValues);
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreNotEqual(string.Empty, exception.Message);
        }

        [TestMethod, TestCategory("Allowed Values")]
        public void DuplicateAllowedValues() {
            var name = new FieldName("Group");
            var nullability = IsNullable.Yes;
            var defaultValue = Option.None<DBValue>();
            var allowedValues = new List<DBValue>() { DBValue.Create("Offense"), DBValue.Create("Defense"),
                DBValue.Create("Offense"), DBValue.Create("Offense"), DBValue.Create("Special Teams"),
                DBValue.Create("Defense"), DBValue.Create("Special Teams") };

            var field = new EnumField(name, nullability, defaultValue, allowedValues);
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(DBType.Enumeration, field.DataType);
            Assert.AreEqual(nullability, field.Nullability);
            Assert.AreEqual(defaultValue, field.DefaultValue);

            var fieldValues = new List<DBValue>(field.Enumerators);
            Assert.AreEqual(3, fieldValues.Count);
            Assert.IsTrue(allowedValues.All(v => fieldValues.Contains(v)));
        }

        [TestMethod, TestCategory("SQL Declaration")]
        public void GenerateSQLWithDefault() {
            var name = new FieldName("Coin");
            var nullability = IsNullable.No;
            var defaultValue = DBValue.Create("Quarter");
            var allowedValues = new List<DBValue>() { DBValue.Create("Penny"), DBValue.Create("Nickel"),
                DBValue.Create("Dime"), DBValue.Create("Quarter"), DBValue.Create("Half Dollar"),
                DBValue.Create("Dollar Coin") };
            var field = new EnumField(name, nullability, Option.Some(defaultValue), allowedValues);

            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            (field as IField).GenerateDeclaration(mockBuilders.Object);
            mockBuilders.FieldBuilder().Verify(f => f.SetName(name), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.SetDataType(DBType.Enumeration), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.SetNullability(nullability), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.SetDefaultValue(defaultValue), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.SetAllowedValues(allowedValues), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.Build(), Times.Exactly(1));
            mockBuilders.FieldBuilder().VerifyNoOtherCalls();
        }

        [TestMethod, TestCategory("SQL Declaration")]
        public void GenerateSQLNoDefault() {
            var name = new FieldName("StreamingService");
            var nullability = IsNullable.Yes;
            var defaultValue = Option.None<DBValue>();
            var allowedValues = new List<DBValue>() { DBValue.Create("Netflix"), DBValue.Create("Hulu"),
                DBValue.Create("HBO Max"), DBValue.Create("Amazon Prime"), DBValue.Create("CBS All Access"),
                DBValue.Create("Peacock"), DBValue.Create("Disney+") };
            var field = new EnumField(name, nullability, defaultValue, allowedValues);

            var mockBuilders = new Mock<IBuilderCollection>().MockByDefault();
            (field as IField).GenerateDeclaration(mockBuilders.Object);
            mockBuilders.FieldBuilder().Verify(f => f.SetName(name), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.SetDataType(DBType.Enumeration), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.SetNullability(nullability), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.SetAllowedValues(allowedValues), Times.AtLeastOnce());
            mockBuilders.FieldBuilder().Verify(f => f.Build(), Times.Exactly(1));
            mockBuilders.FieldBuilder().VerifyNoOtherCalls();
        }
    }
}
