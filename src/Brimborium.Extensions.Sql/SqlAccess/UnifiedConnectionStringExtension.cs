namespace Brimborium.Extensions.SqlAccess {
    using Brimborium.Extensions.Access;
    using Brimborium.Extensions.Freezable;

    using System;

    public static class IUnifiedConnectionStringExtension {
        public static string GetAsSqlConnectionString(this IUnifiedConnectionString that) {
            if (that is null) { return null; }
#warning TODO
            throw new NotImplementedException();
        }
    }
}
