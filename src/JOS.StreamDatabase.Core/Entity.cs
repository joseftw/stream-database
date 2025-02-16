using System;
using System.Collections.Generic;

namespace JOS.StreamDatabase.Core;

public abstract class Entity<T> : IEquatable<Entity<T>>
{
    public required T Id { get; init; }

    public bool Equals(Entity<T>? other)
    {
        if(other is null)
        {
            return false;
        }

        if(!Id!.Equals(other.Id))
        {
            return false;
        }

        return this.GetType() == other.GetType();
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<T> entity && Equals(entity);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Id!);
    }
}
