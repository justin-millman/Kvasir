using Moq;

namespace Atropos.Moq {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see href="https://github.com/Moq">Moq</see> library.
    /// </summary>
    public static class MoqExtensions {
        /// <summary>
        ///   Creates a new, unnamed call sequence to be tracked by a <see cref="Mock{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of object on which the sequence-tracked calls are to be made.
        /// </typeparam>
        /// <param name="mock">
        ///   The <see cref="Mock{T}"/> with which to track the call sequence.
        /// </param>
        /// <returns>
        ///   A new <see cref="CallSequence{T}"/> that can be used to track a sequence of calls made through
        ///   <paramref name="mock"/>.
        /// </returns>
        public static CallSequence<T> MakeSequence<T>(this Mock<T> mock) where T : class {
            return new CallSequence<T>(mock);
        }

        /// <summary>
        ///   Creates a new, named call sequence to be tracked by a <see cref="Mock{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        ///   [deduced] The type of object on which the sequence-tracked calls are to be made.
        /// </typeparam>
        /// <param name="mock">
        ///   The <see cref="Mock{T}"/> with which to track the call sequence.
        /// </param>
        /// <param name="name">
        ///   The <see cref="CallSequence{T}.Name">name</see> of the new call sequence.
        /// </param>
        /// <returns>
        ///   A new <see cref="CallSequence{T}"/> that can be used to track a sequence of calls made through
        ///   <paramref name="mock"/>.
        /// </returns>
        public static CallSequence<T> MakeSequence<T>(this Mock<T> mock, string name) where T : class {
            return new CallSequence<T>(mock, name);
        }
    }
}
