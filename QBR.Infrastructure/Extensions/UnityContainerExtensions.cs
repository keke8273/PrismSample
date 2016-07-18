using System;
using Microsoft.Practices.Unity;

namespace QBR.Infrastructure.Extensions
{
    public static class UnityContainerExtensions
    {
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container)
        {
            container.RegisterType(typeof(object), typeof(T), typeof(T).FullName);
        }
    }
}
