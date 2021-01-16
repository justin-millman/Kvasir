using Kvasir.Transcription;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the types in the Schema Layer of Kvasir.
    /// </summary>
    internal static class SchemaExtensions {
        /// <summary>
        ///   Produces a SQL expression that, when used in the body of a <c>CREATE TABLE</c> statement, declares a
        ///   <see cref="BasicField"/> as a member of the subject Table.
        /// </summary>
        /// <remarks>
        ///   To keep the <see cref="IField"/> interface closed to assemblies outside of Kvasir, the
        ///   <see cref="IField.GenerateDeclaration(IFieldDeclBuilder)"/> method is marked as <c>internal</c>; this
        ///   requires that <see cref="BasicField"/> class implement the method explicitly. This extension method
        ///   allows for natural invocation of the interface method, encapsulating the cast-to-interface required to
        ///   invoke an explicit interface method.
        /// </remarks>
        /// <param name="self">
        ///   The <see cref="BasicField"/> on which the extension method is invoked.
        /// </param>
        /// <param name="builder">
        ///   The <see cref="IFieldDeclBuilder"/> to use to create the declaratory SQL expression.
        /// </param>
        /// <pre>
        ///   <paramref name="self"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="builder"/> is not <see langword="null"/>
        /// </pre>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> whose body declares <paramref name="self"/>.
        /// </returns>
        public static SqlSnippet GenerateSqlDeclaration(this BasicField self, IFieldDeclBuilder builder) {
            Debug.Assert(self is not null);
            Debug.Assert(builder is not null);

            return (self as IField).GenerateDeclaration(builder);
        }

        /// <summary>
        ///   Produces a SQL expression that, when used in the body of a <c>CREATE TABLE</c> statement, declares a
        ///   <see cref="EnumField"/> as a member of the subject Table.
        /// </summary>
        /// <remarks>
        ///   To keep the <see cref="IField"/> interface closed to assemblies outside of Kvasir, the
        ///   <see cref="IField.GenerateDeclaration(IFieldDeclBuilder)"/> method is marked as <c>internal</c>; this
        ///   requires that <see cref="EnumField"/> class implement the method explicitly. This extension method allows
        ///   for natural invocation of the interface method, encapsulating the cast-to-interface required to invoke an
        ///   explicit interface method.
        /// </remarks>
        /// <param name="self">
        ///   The <see cref="BasicField"/> on which the extension method is invoked.
        /// </param>
        /// <param name="builder">
        ///   The <see cref="IFieldDeclBuilder"/> to use to create the declaratory SQL expression.
        /// </param>
        /// <pre>
        ///   <paramref name="self"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="builder"/> is not <see langword="null"/>
        /// </pre>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> whose body declares <paramref name="self"/>.
        /// </returns>
        public static SqlSnippet GenerateSqlDeclaration(this EnumField self, IFieldDeclBuilder builder) {
            Debug.Assert(self is not null);
            Debug.Assert(builder is not null);

            return (self as IField).GenerateDeclaration(builder);
        }

        /// <summary>
        ///   Produces an SQL expression that, when used in the body of a <c>CREATE TABLE</c> statement, declares a
        ///   <see cref="PrimaryKey"/> as applying to the subject Table.
        /// </summary>
        /// <remarks>
        ///   To keep the <see cref="IKey"/> interface closed to assemblies outside of Kvasir, the
        ///   <see cref="IKey.GenerateDeclaration(IKeyDeclBuilder)"/> method is marked as <c>internal</c>; this
        ///   requires that <see cref="PrimaryKey"/> class implement the method explicitly. This extension method
        ///   allows for natural invocation of the interface method, encapsulating the cast-to-interface required to
        ///   invoke an explicit interface method.
        /// </remarks>
        /// <param name="self">
        ///   The <see cref="PrimaryKey"/> on which the extension method is invoked.
        /// </param>
        /// <param name="builder">
        ///   The <see cref="IKeyDeclBuilder"/> to use to create the declaratory SQL expression.
        /// </param>
        /// <pre>
        ///   <paramref name="builder"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> whose body declares this Key.
        /// </returns>
        public static SqlSnippet GenerateSqlDeclaration(this PrimaryKey self, IKeyDeclBuilder builder) {
            Debug.Assert(self is not null);
            Debug.Assert(builder is not null);

            return (self as IKey).GenerateDeclaration(builder);
        }

        /// <summary>
        ///   Produces an SQL expression that, when used in the body of a <c>CREATE TABLE</c> statement, declares a
        ///   <see cref="CandidateKey"/> as applying to the subject Table.
        /// </summary>
        /// <remarks>
        ///   To keep the <see cref="IKey"/> interface closed to assemblies outside of Kvasir, the
        ///   <see cref="IKey.GenerateDeclaration(IKeyDeclBuilder)"/> method is marked as <c>internal</c>; this
        ///   requires that <see cref="CandidateKey"/> class implement the method explicitly. This extension method
        ///   allows for natural invocation of the interface method, encapsulating the cast-to-interface required to
        ///   invoke an explicit interface method.
        /// </remarks>
        /// <param name="self">
        ///   The <see cref="CandidateKey"/> on which the extension method is invoked.
        /// </param>
        /// <param name="builder">
        ///   The <see cref="IKeyDeclBuilder"/> to use to create the declaratory SQL expression.
        /// </param>
        /// <pre>
        ///   <paramref name="builder"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> whose body declares this Key.
        /// </returns>
        public static SqlSnippet GenerateSqlDeclaration(this CandidateKey self, IKeyDeclBuilder builder) {
            Debug.Assert(self is not null);
            Debug.Assert(builder is not null);

            return (self as IKey).GenerateDeclaration(builder);
        }

        /// <summary>
        ///   Produces an SQL <c>CREATE TABLE</c> statement that declares a <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        ///   To keep the <see cref="ITable"/> interface closed to assemblies outside of Kvasir, the
        ///   <see cref="ITable.GenerateDeclaration(IBuilderFactory)"/> method is marked as <c>internal</c>; this
        ///   requires that <see cref="Table"/> class implement the method explicitly. This extension method allows for
        ///   natural invocation of the interface method, encapsulating the cast-to-interface required to invoke an
        ///   explicit interface method.
        /// </remarks>
        /// <param name="self">
        ///   The <see cref="Table"/> on which the extension method is invoked.
        /// </param>
        /// <param name="builderFactory">
        ///   The <see cref="IBuilderFactory"/> with which to create the various Builders needed to compose the
        ///   <c>CREATE TABLE</c> statement for this Table.
        /// </param>
        /// <pre>
        ///   <paramref name="self"/> is not <see langword="null"/>
        ///     --and--
        ///   <paramref name="builderFactory"/> is not <see langword="null"/>
        /// </pre>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> whose body is a <c>CREATE TABLE</c> statement that declares
        ///   <paramref name="self"/>.
        /// </returns>
        public static SqlSnippet GenerateSqlDeclaration(this Table self, IBuilderFactory builderFactory) {
            Debug.Assert(self is not null);
            Debug.Assert(builderFactory is not null);

            return (self as ITable).GenerateDeclaration(builderFactory);
        }
    }
}
