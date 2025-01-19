using System;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public abstract class BaseProjectile
{
    [SerializeField] float speed;
    [Description("Viên đạn bắn ra có vận tốc giảm dần theo thời gian để thể hiện môi trường nước")]
    [SerializeField] float deceleration;

    protected BaseProjectile()
    {
    }
}
