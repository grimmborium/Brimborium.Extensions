namespace Brimborium.Extensions.Entity {
    using Brimborium.Extensions.Freezable;
    using System;
    using System.Collections.Generic;

    public sealed class FreezableDictionaryTypeIMetaEntity : FreezableDictionary<Type, IMetaEntity> {
        public FreezableDictionaryTypeIMetaEntity() : base() { }

        public FreezableDictionaryTypeIMetaEntity(IEqualityComparer<Type> comparer)
            : base(comparer) {
        }

        public FreezableDictionaryTypeIMetaEntity(IDictionary<Type, IMetaEntity> dictionary, IEqualityComparer<Type> comparer)
            : base(dictionary, comparer) {
        }
    }
}