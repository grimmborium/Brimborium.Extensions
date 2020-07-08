using System;

namespace Brimborium.Extensions.Disposable {
    public class ResourceDisposable {
        public static ResourceDisposable<T> Create<T>(T resource)
            where T : class, IDisposable 
            => new ResourceDisposable<T>(resource);
    }

    public class ResourceDisposable<T> : ContainingDisposable<T>
        where T : class, IDisposable {

        public ResourceDisposable(T resource) : base(resource) {
        }

        public T Resource {
            get => base.GetResource();
            set => base.SetResource(value);
        }

        public new T ReadResourceAndForget()
            => base.ReadResourceAndForget();
    }
}
