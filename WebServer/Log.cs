
namespace Server
{
    public static class Log
    {
        public static void Debug(string message) {
        #if MF_FRAMEWORK_VERSION_V4_1
            Microsoft.SPOT.Debug.Print(message);
        #endif
            
        }
    }
}
