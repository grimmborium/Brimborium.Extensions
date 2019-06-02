namespace Brimborium.Extensions.Access {
    /// <summary>think of</summary>
    public interface IUnifiedConnectionString {
        /// <summary>a unique name - primary key.</summary>
        string Name { get; set; }

        /// <summary>the authentication mode e.g. Windows, Basic, ...</summary>
        string AuthenticationMode { get; set; }

        /// <summary>the url or sql connection string.</summary>
        string Url { get; set; }

        /// <summary>the suffix for a http url e.g. api.</summary>
        string Suffix { get; set; }

        /// <summary>Username.</summary>
        string User { get; set; }

        /// <summary>Password.</summary>
        string Password { get; set; }
        
        /// <summary>Reference key to the secret.</summary>
        string SecretKey { get; set; }

        /*
        string AsJson();
        IUnifiedConnectionString Copy(string name);
        IUnifiedConnectionString CreateWithSuffix(string suffix);
        string GetUrlNormalized(bool appendSlash = false);
        IUnifiedConnectionString Overwrite(IUnifiedConnectionString overwrite);
        */
    }

    /// <summary>think of - flexible or stupid??</summary>
    public interface IUnifiedConnectionStringBehaviour<TUnifiedConnectionString> 
        where TUnifiedConnectionString : IUnifiedConnectionString {
        TUnifiedConnectionString Parse(string value);
        string AsJson(TUnifiedConnectionString connectionString);
        TUnifiedConnectionString Copy(string name);
        TUnifiedConnectionString CreateWithSuffix(string suffix);
        string GetUrlNormalized(bool appendSlash = false);
        TUnifiedConnectionString Overwrite(TUnifiedConnectionString overwrite);
    }
}