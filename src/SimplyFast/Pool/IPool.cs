﻿using SimplyFast.Cache;

namespace SimplyFast.Pool
{
    public interface IPool<out TGetter>: IHasCacheStat
    {
        TGetter Get { get; }
    }

    public delegate void ReturnToPool<in TGetter>(TGetter getter);
    public delegate TGetter PooledFactory<out TGetter>(ReturnToPool<TGetter> returnToPool);
}