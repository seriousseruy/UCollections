namespace Extensions {
    public static class BooleanExtension {
        public static string ToJavaScript(this bool value) {
            return value.ToString()
                        .ToLower();
        }
    }
}