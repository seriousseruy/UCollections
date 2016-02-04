namespace Extensions {
    public abstract class BaseObject {
        public bool IsNull(params object[] arguments) {
            return Check.IsNull(arguments);
        }

        public bool IsNotNull(params object[] arguments) {
            return Check.IsNotNull(arguments);
        }
    }

    public static class Check {
        public static bool IsNull(params object[] arguments) {
            if (arguments == null)
                return true;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in arguments) {
                if (item == null)
                    return true;
            }

            return false;
        }

        public static bool IsNotNull(params object[] arguments) {
            if (arguments == null)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in arguments) {
                if (item == null)
                    return false;
            }
            return true;
        }
    }
}