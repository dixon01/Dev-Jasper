This project contains wrappers for native Windows API.

Please follow the following rules when adding types:
1.     In the namespace Gorba.Common.Utility.Win32.Api, there are only directly available APIs,
       no advanced wrappers or anything.
2.     The namespace Gorba.Common.Utility.Win32.Api.DLLs contains static classes with the name of a Win32 DLL
       that have public methods for functions provided in those DLLs.
  2.1. All those public methods have the same name as their C counterpart.
  2.2. All DLLs class file names for DLLs that are only available for one given platform (Windows or Windows CE)
       must be postfixed with .FX20 or .CF35.
  2.3. Method arguments don't contain a type prefix (as always in C#).
  2.4. Method arguments should, wherever possible, be typed (e.g. using an enum and not int, uint, ...)
  2.5. Exception to 2.4 is "HResult", it should be returned as an int and the result should be compared with
       values available in Gorba.Common.Utility.Win32.Api.HResult. This is because HResult can have different meaning
       depending on the method it is used with.
  2.6. If neccessary create multiple overloads of the same method with different parameters (e.g. IntPtr vs. "ref").
3.     The namespace Gorba.Common.Utility.Win32.Api.Enums contains enumerations
       available in header files for the given DLL, they are either C constants or C++ enums
  3.1. Enumerations are called the same way as their C++ counterpart or as the beginning of the C constant counterpart 
  3.2. Enumeration values don't contain the type prefix (not necessary in C# since enums are always accessed by type).
4.     The namespace Gorba.Common.Utility.Win32.Api.Structs contains structs (and classes in rare cases)
       available as structs in header files for the given DLL.
  4.1. Whenever possible a struct is used, not a class; in P/Invoke methods the structs are then passed as "ref"
  4.2. Struct fields are always public and never readonly.
  4.3. Struct field names don't contain a type prefix (as always in C#)
  4.4. Structs in this namespace don't contain any methods
  4.5. Every struct should contain a public static readonly field SizeOf with the right structure size.
5.     In the namespace Gorba.Common.Utility.Win32.Wrapper are classes (static or not) that provide a simpler way
       for calling the methods available in Gorba.Common.Utility.Win32.Api.DLLs.