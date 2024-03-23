using Kvasir.Annotations;
using System.Collections.Generic;

namespace Kvasir.Translation2 {
    ///
    internal abstract class FieldGroup {
        ///
        public abstract void ApplyConstraint(Context context, Nested<CheckAttribute> annotation);

        ///
        public abstract void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation);

        ///
        public abstract void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation);

        ///
        public abstract void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation);

        ///
        public abstract void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation);

        ///
        public abstract FieldGroup Clone();

        ///
        public abstract void SetColumn(Context context, int index);

        ///
        public abstract void SetDefault(Context context, Nested<DefaultAttribute> annotaton);

        ///
        public abstract void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotaton);

        ///
        public abstract void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotaton);

        ///
        public abstract void SetName(Context context, Nested<DefaultAttribute> annotation);

        ///
        public abstract void SetNamePrefix(Context context, IReadOnlyList<string> prefix);

        ///
        public abstract void SetNullability(Context context, bool nullable);
    }
}
