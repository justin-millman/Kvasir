// Exposes all content with internal visibility to the Test project so that the classes, structs, and methods can be
// tested. This content is still hidden from other projects.
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")]

// Exposes all content with internal visibility to the Moq project so that the interfaces can be completely mocked.
// This content is still hidden from other projects. The actual assembly granted permissions is the one used by Moq to
// dynamically generate proxies internally for mocking.
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DynamicProxyGenAssembly2")]