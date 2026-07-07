using System;
using System.IO;

namespace Nursery.Core.Infrastructure;

public static class DataPaths
{
    public static readonly string DataDirectory =
        Path.Combine(AppContext.BaseDirectory, "Data");

    public static readonly string NurseryDbFile =
        Path.Combine(DataDirectory, "nursery.db");
}