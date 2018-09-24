The solution and project files ending in "_VS2013only" are meant for development only.
This is neccessary for ReSharper, StyleCop and other plugins to work without being confused by the different versions of the same DLL being referenced.
The policy should always be compiled using the solution without postfix.