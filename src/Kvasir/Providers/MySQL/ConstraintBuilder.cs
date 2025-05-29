using Cybele.Extensions;
using Kvasir.Schema;
using Kvasir.Transcription;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   A type tag to be used as the return value for a MySQL <see cref="IConstraintDeclBuilder{TDecl}"/>
    /// </summary>
    /// <remarks>
    ///   Dealing with constraints in MySQL is a bit tough because the type of MySQL textual Fields, in part, depends on
    ///   what (if any) length constraints are imposed. Specifically, a textual Field with no length constraint is
    ///   realized as a <c>BLOB</c>, while one with a maximum length constraint is realized as a <c>VARCHAR</c>. This
    ///   creates a somewhat intricate interplay, and I haven't come up with a great design to handle it. So the pattern
    ///   is this: the Declaration Builder returns one of two declaration types unified by the
    ///   <see cref="IConstraintDecl"/> base interface, and then does type-based discrimination between the two. Not a
    ///   great solution, but one I can at least live with.
    /// </remarks>
    internal interface IConstraintDecl {}

    /// <summary>
    ///   A MySQL declaration for a constraint that does not impose a maximum length constraint on a Field, or does so
    ///   as part of a complex clause.
    /// </summary>
    /// <param name="DDL">
    ///   The SQL declaration of the constraint.
    /// </param>
    internal readonly record struct BasicConstraintDecl(SqlSnippet DDL) : IConstraintDecl {}

    /// <summary>
    ///   A MySQL declaration for a constraint that imposes a maximum length constraint on a Field.
    /// </summary>
    /// <param name="Field">
    ///   The name of the Field being constrained.
    /// </param>
    /// <param name="MaxLength">
    ///   The constraint value.
    /// </param>
    internal readonly record struct MaxLengthConstraintDecl(FieldName Field, ulong MaxLength) : IConstraintDecl {}


    /// <summary>
    ///   An implementation of the <see cref="IConstraintDeclBuilder{TDecl}"/> interface for a MySQL provider.
    /// </summary>
    internal sealed class ConstraintBuilder : IConstraintDeclBuilder<IConstraintDecl> {
        /// <summary>
        ///   Constructs a new <see cref="ConstraintBuilder"/>.
        /// </summary>
        public ConstraintBuilder() {
            clauses_ = new Stack<string>();
            declaration_ = CONSTRAINT_TEMPLATE;
            maxLengthDecl_ = Option.None<MaxLengthConstraintDecl>();
        }

        /// <inheritdoc/>
        public void SetName(CheckName name) {
            Debug.Assert(name is not null);
            Debug.Assert(declaration_.Contains(NAME_PLACEHOLDER));

            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, $"CONSTRAINT {name.Render()} ");
        }

        /// <inheritdoc/>
        public void StartClause() {
            clauses_.Push(CLAUSE_TEMPLATE);
        }

        /// <inheritdoc/>
        public void EndClause() {
            Debug.Assert(clauses_.Count >= 4);
            
            var rhs = clauses_.Pop();
            var op = clauses_.Pop();
            var lhs = clauses_.Pop();

            var clause = clauses_.Pop();
            clause = clause.Replace(LHS_PLACEHOLDER, lhs);
            clause = clause.Replace(OPERATOR_PLACEHOLDER, op);
            clause = clause.Replace(RHS_PLACEHOLDER, rhs);

            if (clauses_.IsEmpty()) {
                clauses_.Push(clause);
            }
            else {
                clauses_.Push($"({clause})");
            }
        }

        /// <inheritdoc/>
        public void And() {
            Debug.Assert(!clauses_.IsEmpty());

            clauses_.Push("&&");
            maxLengthDecl_.Filter(false);       // empties the optional
        }

        /// <inheritdoc/>
        public void Or() {
            Debug.Assert(!clauses_.IsEmpty());

            clauses_.Push("||");
            maxLengthDecl_.Filter(false);       // empties the optional
        }

        /// <inheritdoc/>
        public void AddClause(ConstantClause clause) {
            Debug.Assert(clause is not null);

            if (clauses_.IsEmpty() && clause.LHS.Function.Exists(fn => fn == FieldFunction.LengthOf)) {
                if (clause.Operator == ComparisonOperator.LT || clause.Operator == ComparisonOperator.LTE) {
                    ulong max = (ulong)Convert.ChangeType(clause.RHS.Datum, typeof(ulong));
                    if (clause.Operator == ComparisonOperator.LT) {
                        max -= 1;
                    }

                    maxLengthDecl_ = Option.Some(new MaxLengthConstraintDecl(clause.LHS.Field.Name, max));
                }
            }
            
            // Even if we just processed a maximum length constraint, we still want to fill in the clause; otherwise, if
            // the maximum length constraint was the first in a compound constraint, we would lose its information.
            bool forBoolean = clause.RHS.Datum.GetType() == typeof(bool);
            var newClause = CLAUSE_TEMPLATE;
            newClause = newClause.Replace(LHS_PLACEHOLDER, clause.LHS.Render());
            newClause = newClause.Replace(OPERATOR_PLACEHOLDER, clause.Operator.Render(forBoolean));
            newClause = newClause.Replace(RHS_PLACEHOLDER, clause.RHS.Render());
            clauses_.Push(newClause);
        }

        /// <inheritdoc/>
        public void AddClause(CrossFieldClause clause) {
            Debug.Assert(clause is not null);

            var newClause = CLAUSE_TEMPLATE;
            newClause = newClause.Replace(LHS_PLACEHOLDER, clause.LHS.Render());
            newClause = newClause.Replace(OPERATOR_PLACEHOLDER, clause.Operator.Render(false));
            newClause = newClause.Replace(RHS_PLACEHOLDER, clause.RHS.Render());
            clauses_.Push(newClause);
        }

        /// <inheritdoc/>
        public void AddClause(InclusionClause clause) {
            Debug.Assert(clause is not null);

            var newClause = CLAUSE_TEMPLATE;
            newClause = newClause.Replace(LHS_PLACEHOLDER, clause.LHS.Render());
            newClause = newClause.Replace(OPERATOR_PLACEHOLDER, clause.Operator.Render());
            newClause = newClause.Replace(RHS_PLACEHOLDER, $"({string.Join(", ", clause.RHS.Select(v => v.Render()))})");
            clauses_.Push(newClause);
        }

        /// <inheritdoc/>
        public void AddClause(NullityClause clause) {
            Debug.Assert(clause is not null);

            var newClause = CLAUSE_TEMPLATE;
            newClause = newClause.Replace(LHS_PLACEHOLDER, clause.LHS.Render());
            newClause = newClause.Replace(OPERATOR_PLACEHOLDER, clause.Operator.Render());
            newClause = newClause.Replace(RHS_PLACEHOLDER, Rendering.NULL);
            clauses_.Push(newClause);
        }

        /// <inheritdoc/>
        public IConstraintDecl Build() {
            Debug.Assert(clauses_.Count == 1);

            var clause = clauses_.Pop();
            declaration_ = declaration_.Replace(NAME_PLACEHOLDER, "");          // by default, a constraint has no name
            declaration_ = declaration_.Replace(PREDICATE_PLACEHOLDER, clause); // fill in the conditional clause

            return maxLengthDecl_.Match<IConstraintDecl>(
                some: decl => decl,
                none: () => new BasicConstraintDecl(new SqlSnippet(declaration_))
            );
        }


        private readonly static string LHS_PLACEHOLDER = "{:0:}";
        private readonly static string OPERATOR_PLACEHOLDER = "{:1:}";
        private readonly static string RHS_PLACEHOLDER = "{:2:}";
        private readonly static string CLAUSE_TEMPLATE = $"{LHS_PLACEHOLDER} {OPERATOR_PLACEHOLDER} {RHS_PLACEHOLDER}";

        private readonly static string NAME_PLACEHOLDER = "{:N:}";
        private readonly static string PREDICATE_PLACEHOLDER = "{:P:}";
        private readonly static string CONSTRAINT_TEMPLATE = $"{NAME_PLACEHOLDER}CHECK ({PREDICATE_PLACEHOLDER})";

        private Stack<string> clauses_;
        private string declaration_;
        private Option<MaxLengthConstraintDecl> maxLengthDecl_;
    }
}
