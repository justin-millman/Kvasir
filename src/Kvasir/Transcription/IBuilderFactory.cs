namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a factory that creates syntactically consistent declaration builders for generating
    ///   <c>CREATE TABLE</c> statements.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="IBuilderFactory"/> interface is an abstract way to represent the syntax rules of a
    ///     particular back-end RDBMS provider. Implementations of the <see cref="IBuilderFactory"/> interface and the
    ///     builders therefrom created are passed to the various components of the Schema Layer to generate
    ///     <c>CREATE TABLE</c> statements once the schema structures have been synthesized. In this way, a single
    ///     used to interface with any back-end RDBMS provider.
    ///  </para>
    ///  <para>
    ///     Examples of syntax rules that can vary from provider to provider may include, but are not limited to:
    ///     <list type="bullet">
    ///       <item>Escaping the names of Fields, Tables, and Constraints</item>
    ///       <item>Specific data types</item>
    ///       <item>Realization of enumeration-type Fields</item>
    ///       <item>Spelling of logical comparison operators</item>
    ///     </list>
    ///  </para>
    /// </remarks>
    public interface IBuilderFactory {
        /// <summary>
        ///   Creates a new instance of the <see cref="IConstraintDeclBuilder"/> interface that produces SQL consistent
        ///   with the syntax rules of this <see cref="IBuilderFactory"/>.
        /// </summary>
        /// <returns>
        ///   A new <see cref="IConstraintDeclBuilder"/>.
        /// </returns>
        IConstraintDeclBuilder NewConstraintDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the <see cref="IForeignKeyDeclBuilder"/> interface that produces SQL consistent
        ///   with the syntax rules of this <see cref="IBuilderFactory"/>.
        /// </summary>
        IForeignKeyDeclBuilder NewForeignKeyDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the <see cref="IKeyDeclBuilder"/> interface that produces SQL consistent with
        ///   the syntax rules of this <see cref="IBuilderFactory"/>.
        /// </summary>
        IKeyDeclBuilder NewKeyDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the <see cref="IFieldDeclBuilder"/> interface that produces SQL consistent with
        ///   the syntax rules of this <see cref="IBuilderFactory"/>.
        /// </summary>
        IFieldDeclBuilder NewFieldDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the <see cref="ITableDeclBuilder"/> interface that produces SQL consistent with
        ///   the syntax rules of this <see cref="IBuilderFactory"/>.
        /// </summary>
        ITableDeclBuilder NewTableDeclBuilder();
    }
}
