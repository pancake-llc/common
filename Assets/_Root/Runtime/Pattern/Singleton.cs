using System;

namespace Pancake.Common
{
    public class Singleton<T> where T : class
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null) instance = Activator.CreateInstance<T>();
                return instance;
            }
        }
    }
}