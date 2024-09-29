#nullable enable

using System;
using UnityEngine;

public readonly struct PooledOrInstantiated<T> where T : UnityEngine.Object {
    private readonly IDisposableObject<T>? pooled;
    private readonly T? instantiated;

    public static PooledOrInstantiated<T> Create(IObjectPool<T>? pool, T? prefab) {
        return pool != null ? new PooledOrInstantiated<T>(pool.Get())
            : prefab != null ? new PooledOrInstantiated<T>(UnityEngine.Object.Instantiate(prefab))
            : throw new InvalidOperationException();
    }

    public PooledOrInstantiated(IDisposableObject<T> pooled) {
        this.instantiated = null;
        this.pooled = pooled;
    }

    public PooledOrInstantiated(T instantiated) {
        this.instantiated = instantiated;
        this.pooled = null;
    }

    public T Object =>
        this.pooled != null ? this.pooled.Object
        : this.instantiated != null ? this.instantiated
        : throw new InvalidOperationException();

    public void Destroy() {
        if (this.pooled != null) {
            this.pooled.Dispose();
            return;
        }

        if (this.instantiated != null) {
            if (this.instantiated.TryGetGameObject(out GameObject? gameObj)) {
                UnityEngine.Object.Destroy(gameObj);
            }
            return;
        }

        throw new InvalidOperationException();
    }
}
