﻿using System;

namespace Brimborium.Extensions.Abstractions {
    public class ResourceDisposable<T> : ContainingDisposable<T>
        where T : class, IDisposable {

        public ResourceDisposable(T resource) :base(resource){
        }

        public T Resource {
            get => base.GetResource();
            set => base.SetResource(value);
        }

    }
}
