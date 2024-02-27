using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Translation2 {
    ///
    internal readonly struct Context : IDisposable {
        ///
        public int Depth => trace_.Count;

        ///
        public Context() {
            trace_ = new Stack<Type>();
        }

        ///
        public void Extend(Type next) {
            trace_.Push(next);
        }

        ///
        public readonly void Dispose() {
            trace_.Pop();
        }

        ///
        public override string ToString() {
            return string.Join(" → ", trace_.Select(t => t.Name));
        }


        private readonly Stack<Type> trace_;
    }
}
