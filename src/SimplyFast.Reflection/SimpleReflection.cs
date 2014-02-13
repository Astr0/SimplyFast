using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    public static class SimpleReflection
    {
        private static bool _privateAccess;
        private static BindingFlags _bindingFlags;

        static SimpleReflection()
        {
            PrivateAccess = true;
        }

        public static bool PrivateAccess
        {
            get { return _privateAccess; }
            set
            {
                _privateAccess = value;

                _bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
                if (_privateAccess)
                    _bindingFlags |= BindingFlags.NonPublic;
            }
        }

        public static BindingFlags BindingFlags
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _bindingFlags; }
        }
    }
}