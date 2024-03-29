using Kvasir.Schema;
using NSubstitute;
using System;
using System.Linq;

namespace UT.Kvasir.Schema {
    internal static partial class Mocks {
        public static ConstantClause Matcher(this ConstantClause self) {
            return Arg.Is<ConstantClause>(cc =>
                ReferenceEquals(cc.LHS.Field, self.LHS.Field) &&
                cc.LHS.Function == self.LHS.Function &&
                cc.Operator == self.Operator &&
                cc.RHS == self.RHS
            );
        }
        public static CrossFieldClause Matcher(this CrossFieldClause self) {
            return Arg.Is<CrossFieldClause>(cfc =>
                ReferenceEquals(cfc.LHS.Field, self.LHS.Field) &&
                cfc.LHS.Function == self.LHS.Function &&
                cfc.Operator == self.Operator &&
                ReferenceEquals(cfc.RHS.Field, self.RHS.Field) &&
                cfc.RHS.Function == self.RHS.Function
            );
        }
        public static InclusionClause Matcher(this InclusionClause self) {
            return Arg.Is<InclusionClause>(ic =>
                ReferenceEquals(ic.LHS.Field, self.LHS.Field) &&
                ic.LHS.Field == self.LHS.Field &&
                ic.Operator == self.Operator &&
                ic.RHS.OrderBy(v => v.ToString()).SequenceEqual(self.RHS.OrderBy(v => v.ToString()))
            );
        }
        public static NullityClause Matcher(this NullityClause self) {
            return Arg.Is<NullityClause>(nc =>
                ReferenceEquals(nc.LHS.Field, self.LHS.Field) &&
                !nc.LHS.Function.HasValue &&
                nc.Operator == self.Operator
            );
        }
    }
}