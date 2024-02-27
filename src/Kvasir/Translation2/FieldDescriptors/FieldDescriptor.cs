using Cybele.Core;
using Kvasir.Annotations;
using Kvasir.Core;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal abstract class FieldDescriptor {
        ///
        public Type FieldType {
            get {
                var type = converter_.Match(some: c => c.ResultType, none: () => source_.PropertyType);
                return Nullable.GetUnderlyingType(type) ?? type;
            }
        }

        ///
        public FieldDescriptor WithColumn(int column) {
            Debug.Assert(column >= 0);

            var clone = Clone();
            clone.column_ = column;
            return clone;
        }

        ///
        protected FieldDescriptor(FieldDescriptor source) {
            Debug.Assert(source is not null);

            source_ = source.source_;
            name_ = source.name_;
            nullable_ = source.nullable_;
            converter_ = source.converter_;
            inPrimaryKey_ = source.inPrimaryKey_;
            keyMemberships_ = new HashSet<string>(source.keyMemberships_);
            allowedValues_ = new HashSet<object>(source.allowedValues_);
            disallowedValues_ = new HashSet<object>(source.disallowedValues_);
            checks_ = new List<IConstraintGenerator>(source.checks_);
        }

        ///
        protected FieldDescriptor(PropertyInfo source) {
            Debug.Assert(source is not null);

            source_ = source;
            name_ = source.Name;
            nullable_ = new NullabilityInfoContext().Create(source).ReadState == NullabilityState.Nullable;
            column_ = 0;
            converter_ = Option.None<DataConverter>();
            inPrimaryKey_ = false;
            keyMemberships_ = new HashSet<string>();
            allowedValues_ = new HashSet<object>();
            disallowedValues_ = new HashSet<object>();
            checks_ = new List<IConstraintGenerator>();
        }

        ///
        protected abstract FieldDescriptor Clone();


        private PropertyInfo source_;
        private string name_;
        private bool nullable_;
        private int column_;
        private Option<DataConverter> converter_;
        private bool inPrimaryKey_;
        private IReadOnlySet<string> keyMemberships_;
        private IReadOnlySet<object> allowedValues_;
        private IReadOnlySet<object> disallowedValues_;
        private IReadOnlyList<IConstraintGenerator> checks_;
    }
}
