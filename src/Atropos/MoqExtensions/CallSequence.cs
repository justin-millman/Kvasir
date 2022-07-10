using Ardalis.GuardClauses;
using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Atropos.Moq {
    /// <summary>
    ///   A utility for ensuring that calls made through a <see cref="Mock{T}"/> occur in a specific order.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A common need when unit testing complex components is to ensure that a series of calls to the system under
    ///     test occur in a particular order. While the <see href="https://github.com/Moq">Moq</see> library provides
    ///     many powerful facilities for mocking objects and evaluating the ensuing behavior, the library is lacking
    ///     when it comes to evaluating call sequences. <see cref="CallSequence{T}"/> fills this gap, interoperating
    ///     with the <see cref="Mock{T}"/> hierarchy seamlessly to provide a fluent way to expect a speific order of
    ///     invocations.
    ///   </para>
    ///   <para>
    ///     A <see cref="CallSequence{T}"/> begins empty, i.e. with no expectations, and can be augmented one expected
    ///     call at a time. Any expected call that can be set-up via <see cref="Mock{T}"/> can be added to a
    ///     <see cref="CallSequence{T}"/>. <see cref="CallSequence{T}"/> imposes a custom callback that verifies the
    ///     calls are made in the correct order. For this reason, however, it is not possible to add an additional
    ///     user-defined callback that is invoked after the order-check passes. This feature is under consideration as
    ///     a future extension of the library. If a call is made out-of-order, an exception is raised with robust
    ///     information regarding the expectation of the <see cref="CallSequence{T}"/> versus reality.
    ///   </para>
    ///   <para>
    ///     A <see cref="CallSequence{T}"/> necessarily imposes an immutable strict total order. This means that an
    ///     expectated call cannot be reordered or removed once added, and that every expected call must occur either
    ///     before or after each other expectation. For example, it is not possible to expect that both call <c>A</c>
    ///     and <c>B</c> occur, in any order, before call <c>C</c>. It is, however, possible to expect repeated calls,
    ///     either sequentially (e.g. <c>A</c>, <c>A</c>, <c>B</c>) or non-sequentially  (e.g. <c>A</c>, <c>B</c>,
    ///     <c>A</c>). There are limitations to repeat call suppport, as detailed below.
    ///   </para>
    ///   <para>
    ///     A <see cref="CallSequence{T}"/> is in one of three states at any given time: unstarted, incomplete, or
    ///     complete. A <see cref="CallSequence{T}"/> can be extended in any state, adding an additional expected call
    ///     to the end; specifically, adding an expected call to an incomplete sequence does not set the expectation of
    ///     the next call. A <see cref="CallSequence{T}"/> is complete when all expected calls have been made; one can
    ///     verify that a <see cref="CallSequence{T}"/> is complete using a
    ///     <see cref="VerifyCompleted">dedicated API</see>, which will throw an exception in the adverse case. An
    ///     empty <see cref="CallSequence{T}"/> (i.e. one without any expected calls) is always considered complete.
    ///   </para>
    ///   <para>
    ///     Calls can be made to the <see cref="Mock{T}"/> that backs a <see cref="CallSequence{T}"/> that are not
    ///     directly expected by the sequence. The strict total order imposed on the calls by a sequence does not
    ///     preclude other calls; to verify that no other calls are made, use a combination of the
    ///     <see cref="VerifyCompleted"/> method on the <see cref="CallSequence{T}"/> and the
    ///     <see cref="Mock{T}.VerifyNoOtherCalls"/> method on the underlying <see cref="Mock{T}"/>.
    ///   </para>
    ///   <para>
    ///     A single <see cref="Mock{T}"/> can underlie more than one <see cref="CallSequence{T}"/> as long as those
    ///     sequences contain disjoint sets of expected calls. If an expected call appears in multiple sequences that
    ///     are backed by the same <see cref="Mock{T}"/>, at least one of the sequences will fail. The other major
    ///     limitation of <see cref="CallSequence{T}"/> has to do with repeat expectations: to include a repeated
    ///     expectation in a sequence, the LINQ expression with which the expectation is established must use the same
    ///     lambda variable names. For example, <c>x => x.Foo()</c> and <c>y => y.Foo()</c>, while representing the
    ///     same invocation, will cause a <see cref="CallSequence{T}"/> containing both to fail on the first
    ///     invocation. This variable naming is case sensitive, so <c>x => x.Foo()</c> is also different than
    ///     <c>X => X.Foo()</c>.
    ///   </para>
    /// </remarks>
    /// <typeparam name="T">
    ///   The type of object being mocked by the underlying <see cref="Mock{T}"/>.
    /// </typeparam>
    public sealed class CallSequence<T> where T : class {
        /// <summary>
        ///   The name of this <see cref="CallSequence{T}"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///   Constructs a new, unnamed <see cref="CallSequence{T}"/>.
        /// </summary>
        /// <param name="mock">
        ///   The <see cref="Mock{T}"/> through which to track calls.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="mock"/> is <see langword="null"/>.
        /// </exception>
        internal CallSequence(Mock<T> mock)
            : this(mock, Guid.NewGuid().ToString()) {}

        /// <summary>
        ///   Constructs a new, named <see cref="CallSequence{T}"/>.
        /// </summary>
        /// <param name="mock">
        ///   The <see cref="Mock{T}"/> through which to track calls.
        /// </param>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="CallSequence{T}"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="mock"/> is <see langword="null"/>
        ///     --or--
        ///   if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        internal CallSequence(Mock<T> mock, string name) {
            Guard.Against.Null(mock, nameof(mock));
            Guard.Against.Null(name, nameof(name));

            Name = name;
            order_ = new List<Expression>();
            occurrenceCounts_ = new Dictionary<Expression, int>(new ExpressionEq());
            executor_ = mock;
            nextIndex_ = 0;
        }

        /// <summary>
        ///   Adds an expected call to the end of the current call sequence. This can be used to extend a sequence that
        ///   has not yet been started, a sequence that is in progrss, or a sequence that has already completed. Call
        ///   expectations cannot be reordered or removed once added.
        /// </summary>
        /// <param name="expression">
        ///   The LINQ <see cref="Expression"/> denoting the call to be added to the end of the current call sequence.
        /// </param>
        /// <returns>
        ///   The <see cref="ICallbackResult"/> produced by using <paramref name="expression"/> to
        ///   <see cref="Mock{T}.Setup(Expression{Action{T}})">set-up</see> an expectation on <see cref="Mock{T}"/>
        ///   that backs the current <see cref="CallSequence{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="expression"/> is <see langword="null"/>.
        /// </exception>
        public ICallbackResult Add(Expression<Action<T>> expression) {
            Guard.Against.Null(expression, nameof(expression));

            AddToBookkeeping(expression);
            return executor_.Setup(expression).Callback(() => VerifyInOrder(expression));
        }

        /// <summary>
        ///   Adds an expected call to the end of the current call sequence. This can be used to extend a sequence that
        ///   has not yet been started, a sequence that is in progrss, or a sequence that has already completed. Call
        ///   expectations cannot be reordered or removed once added.
        /// </summary>
        /// <param name="expression">
        ///   The LINQ <see cref="Expression"/> denoting the call to be added to the end of the current call sequence.
        /// </param>
        /// <returns>
        ///   The <see cref="ICallbackResult"/> produced by using <paramref name="expression"/> to
        ///   <see cref="Mock{T}.Setup{TResult}(Expression{Func{T, TResult}})">set-up</see> an expectation on
        ///   <see cref="Mock{T}"/> that backs the current <see cref="CallSequence{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="expression"/> is <see langword="null"/>.
        /// </exception>
        public IReturnsThrows<T, U> Add<U>(Expression<Func<T, U>> expression) {
            Guard.Against.Null(expression, nameof(expression));

            AddToBookkeeping(expression);
            return executor_.Setup(expression).Callback(() => VerifyInOrder(expression));
        }

        /// <summary>
        ///   Verifies that the current call sequence was completed through the backing <see cref="Mock{T}"/>.
        /// </summary>
        /// <exception cref="MockException">
        ///   if any call expected by this <see cref="CallSequence{T}"/> was not made.
        /// </exception>
        public void VerifyCompleted() {
            foreach ((dynamic expr, int expectedCount) in occurrenceCounts_) {
                executor_.Verify(expr, Times.AtLeast(expectedCount));
            }
        }

        /// <summary>
        ///   Adds a new LINQ <see cref="Expression"/> to the internal bookkeeping of the current
        ///   <see cref="CallSequence{T}"/>.
        /// </summary>
        /// <param name="expression">
        ///   The LINQ <see cref="Expression"/> to bookkeep.
        /// </param>
        /// <pre>
        ///   <paramref name="expression"/> is not <see langword="null"/>.
        /// </pre>
        private void AddToBookkeeping(Expression expression) {
            Debug.Assert(expression is not null);
            order_.Add(expression);

            if (!occurrenceCounts_.ContainsKey(expression)) {
                occurrenceCounts_[expression] = 1;
            }
            else {
                occurrenceCounts_[expression]++;
            }
        }

        /// <summary>
        ///   Verifies that the call modeled by a particular LINQ <see cref="Expression"/> is the next call expected by
        ///   the current <see cref="CallSequence{T}"/>.
        /// </summary>
        /// <param name="expression">
        ///   The LINQ <see cref="Expression"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="expression"/> is not <see langword="null"/>
        ///     --and--
        ///   a call modeled by <paramref name="expression"/> is part of the current <see cref="CallSequence{T}"/>,
        ///   even if it is in the wrong order.
        /// </pre>
        /// <exception cref="Exception">
        ///   if the current <see cref="CallSequence{T}"/> is completed, and therefore not expecting any additional
        ///   calls
        ///     --or--
        ///   if the current <see cref="CallSequence{T}"/> is incomplete but the next expected call does not exactly
        ///   match <paramref name="expression"/>.
        /// </exception>
        private void VerifyInOrder(Expression expression) {
            Debug.Assert(expression is not null);
            Debug.Assert(occurrenceCounts_.ContainsKey(expression));

            //
            if (nextIndex_ >= order_.Count) {
                var msg = $"Call matching setup {expression} made when call sequence {Name} already completed";
                throw new Exception(msg);
            }

            //
            if (!new ExpressionEq().Equals(expression, order_[nextIndex_])) {
                var msg = $"Expected call #{nextIndex_} in sequence {Name} to match setup {order_[nextIndex_]}, but " +
                    $"instead received call matching setup {expression}";
                throw new Exception(msg);
            }

            ++nextIndex_;
        }


        /// <summary>
        ///   An implementation of the <see cref="IEqualityComparer{T}"/> interface that compares LINQ
        ///   <see cref="Expression">expressions</see> for equality by looking at their string representations.
        /// </summary>
        private sealed class ExpressionEq : IEqualityComparer<Expression> {
            /// <summary>
            ///   Checks if two LINQ <see cref="Expression">expressions</see> are equal.
            /// </summary>
            /// <param name="lhs">
            ///   The left-hand operand.
            /// </param>
            /// <param name="rhs">
            ///   The right-hand operand.
            /// </param>
            /// <pre>
            ///   <paramref name="lhs"/> is not <see langword="null"/>
            ///     --and--
            ///   <paramref name="rhs"/> is not <see langword="null"/>.
            /// </pre>
            /// <returns>
            ///   <see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>; otherwise,
            ///   <see langword="false"/>
            /// </returns>
            public bool Equals(Expression? lhs, Expression? rhs) {
                return lhs!.ToString() == rhs!.ToString();
            }

            /// <summary>
            ///   Produces the hash code for a LINQ <see cref="Expression"/>.
            /// </summary>
            /// <param name="expr">
            ///   The LINQ <see cref="Expression"/> to hash.
            /// </param>
            /// <returns>
            ///   The hash code for <paramref name="expr"/>.
            /// </returns>
            public int GetHashCode(Expression expr) {
                return HashCode.Combine(expr.ToString());
            }
        }


        private readonly List<Expression> order_;
        private readonly Dictionary<Expression, int> occurrenceCounts_;
        private readonly Mock<T> executor_;
        private int nextIndex_;
    }
}
