namespace Kvasir.Core {
    /// <summary>
    ///   The settings to be used by the Kvasir framework in the course of its deductions, translations, and other
    ///   built-in behaviors.
    /// </summary>
    public sealed record Settings {
        /// <summary>
        ///   The default settings used by the Kvasir framework.
        /// </summary>
        public static Settings Default {
            get {
                return new Settings() {};
            }
        }

        /* Because Settings is record type, the following methods are synthesized automatically by the compiler:
         *   > public Settings(Settings rhs)
         *   > public bool Equals(Settings? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(Settings? lhs, Settings? rhs)
         *   > public static bool operator!=(Settings? lhs, Settings? rhs)
         */
    }
}
