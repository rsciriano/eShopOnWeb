﻿using System;
using System.Reflection;

namespace Microsoft.eShopWeb.ApplicationCore.Entities;

// This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
// Using non-generic integer types for simplicity and to ease caching logic
public abstract class BaseEntity: BaseEntity<int>
{
}

public abstract class BaseEntity<TIndex> 
{
    public virtual TIndex Id { get; protected set; }
}
