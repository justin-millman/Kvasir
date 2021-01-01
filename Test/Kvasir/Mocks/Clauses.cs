using Kvasir.Schema.Constraints;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Test.Kvasir.Schema {
    internal static partial class MockExtensions {
        public static Expression<Func<ConstantValueClause, bool>> Matcher(this ConstantValueClause self) {
            return cvc => ReferenceEquals(cvc.LHS.Field, self.LHS.Field) &&
                          cvc.LHS.Function == self.LHS.Function &&
                          cvc.Operator == self.Operator &&
                          cvc.RHS == self.RHS;
        }
        public static Expression<Func<CrossFieldValueConstraint, bool>> Matcher(this CrossFieldValueConstraint self) {
            return cfvc => ReferenceEquals(cfvc.LHS.Field, self.LHS.Field) &&
                           cfvc.LHS.Function == self.LHS.Function &&
                           cfvc.Operator == self.Operator &&
                           ReferenceEquals(cfvc.RHS.Field, self.RHS.Field) &&
                           cfvc.RHS.Function == self.RHS.Function;
        }
        public static Expression<Func<InclusionClause, bool>> Matcher(this InclusionClause self) {
            return ic => ReferenceEquals(ic.LHS.Field, self.LHS.Field) &&
                         ic.LHS.Function == self.LHS.Function &&
                         ic.Operator == self.Operator &&
                         ic.RHS.OrderBy(v => v.ToString()).SequenceEqual(self.RHS.OrderBy(v => v.ToString()));
        }
        public static Expression<Func<NullityClause, bool>> Matcher(this NullityClause self) {
            return nc => ReferenceEquals(nc.LHS.Field, self.LHS.Field) &&
                         !nc.LHS.Function.HasValue &&
                         nc.Operator == self.Operator;
        }
    }
}
