namespace Kvasir.Transcription {
    /// <summary>
    ///   The interface for a factory that creates internally consistent declaration builders for generating
    ///   Table-creating declarations.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/> interface is
    ///     an abstract way to represent the rules of a particular back-end RDBMS provider. Implementations of the
    ///     <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/> interface and the
    ///     builders therefrom created are passed to the various components of the Schema Layer to generate
    ///     Table-creating declarations once the schema structures have been synthesized. In this way, a single set of
    ///     interfaces, defined by the builder factory, can be used to interface with any back-end RDBMS provider.
    ///  </para>
    ///  <para>
    ///     Examples of rules that can vary from provider to provider may include, but are not limited to:
    ///     <list type="bullet">
    ///       <item>Escaping the names of Fields, Tables, and Constraints</item>
    ///       <item>Specific data types</item>
    ///       <item>Realization of enumeration-type Fields</item>
    ///       <item>Spelling of logical comparison operators</item>
    ///     </list>
    ///  </para>
    /// </remarks>
    public interface IBuilderFactory<TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl> {
        /// <summary>
        ///   Creates a new instance of the <see cref="IConstraintDeclBuilder{TDecl}"/> interface that produces
        ///   declarations consistent with the rules of this
        ///   <see cref="IBuilderFactory{TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>.
        /// </summary>
        /// <returns>
        ///   A new <see cref="IConstraintDeclBuilder{TDecl}"/>.
        /// </returns>
        IConstraintDeclBuilder<TConstraintDecl> NewConstraintDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the <see cref="IForeignKeyDeclBuilder{TFKDecl}"/> interface that produces
        ///   declarations consistent with the rules of this
        ///   <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>.
        /// </summary>
        IForeignKeyDeclBuilder<TFKDecl> NewForeignKeyDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the <see cref="IKeyDeclBuilder{TKeyDecl}"/> interface that produces declarations
        ///   consistent with the rules of this
        ///   <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>.
        /// </summary>
        IKeyDeclBuilder<TKeyDecl> NewKeyDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the <see cref="IFieldDeclBuilder{TDecl}"/> interface that produces declarations
        ///   consistent with the rules of this
        ///   <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>.
        /// </summary>
        IFieldDeclBuilder<TFieldDecl> NewFieldDeclBuilder();

        /// <summary>
        ///   Creates a new instance of the
        ///   <see cref="ITableDeclBuilder{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/> interface that
        ///   produces declarations consistent with the rules of this
        ///   <see cref="IBuilderFactory{TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl}"/>.
        /// </summary>
        ITableDeclBuilder<TTableDecl, TFieldDecl, TKeyDecl, TConstraintDecl, TFKDecl> NewTableDeclBuilder();
    }
}
