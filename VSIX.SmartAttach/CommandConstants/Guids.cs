// Guids.cs
// MUST match guids.h
using System;

namespace Geeks.VSIX.SmartAttach.Base
{
    static class GuidList
    {
        public const string GuidGeeksProductivityToolsPkgString = "c6176957-c61c-4beb-8dd8-e7c0170b0bf5";

        const string guidGeeksProductivityToolsCmdSetString = "8d55b42e-5f7c-44dd-8b02-71c751d8c440";

        public static readonly Guid GuidGeeksProductivityToolsCmdSet = new Guid(guidGeeksProductivityToolsCmdSetString);
    };
}