﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.Configurations
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }
}
