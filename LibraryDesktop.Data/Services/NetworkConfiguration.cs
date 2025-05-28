using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Linq;

namespace LibraryDesktop.Data.Services
{    public static class NetworkConfiguration
    {
        // Configuration for EmbedIO server (unified server for both static files and APIs)
        private static string? _liveServerHost;        public static string LIVE_SERVER_HOST 
        { 
            get 
            {
                if (string.IsNullOrEmpty(_liveServerHost))
                {
                    _liveServerHost = DetectLocalIpAddress() ?? "localhost";
                    Console.WriteLine($"🌐 LIVE_SERVER_HOST initialized: {_liveServerHost}");
                }
                return _liveServerHost;
            } 
            set => _liveServerHost = value;
        }
        
        public const int LIVE_SERVER_PORT = 5000; // EmbedIO server port
        public const string LIVE_SERVER_PATH = "payment"; // EmbedIO payment endpoint
          // Configuration for LibraryDesktop API server (same as EmbedIO server)
        public static string API_SERVER_HOST => LIVE_SERVER_HOST; // Use the same detected IP
        public const int API_SERVER_PORT = 5000;
        
        /// <summary>
        /// Gets the complete Live Server URL for payment confirmation pages
        /// </summary>
        public static string GetLiveServerUrl()
        {
            return $"http://{LIVE_SERVER_HOST}:{LIVE_SERVER_PORT}/{LIVE_SERVER_PATH}";
        }
        
        /// <summary>
        /// Gets the API server base URL
        /// </summary>
        public static string GetApiServerUrl()
        {
            return $"http://{API_SERVER_HOST}:{API_SERVER_PORT}";
        }        /// <summary>
        /// Automatically detects the local machine's IP address on the specified network
        /// </summary>
        /// <param name="networkPrefix">Network prefix to look for (e.g., "192.168.1")</param>
        /// <returns>The detected IP address or null if not found</returns>
        public static string? DetectLocalIpAddress(string networkPrefix = "192.168.1")
        {            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up && 
                                ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                var allIpAddresses = new List<string>();

                foreach (var networkInterface in networkInterfaces)
                {
                    var ipProperties = networkInterface.GetIPProperties();
                    var ipAddresses = ipProperties.UnicastAddresses
                        .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(ip => ip.Address.ToString())
                        .Where(ip => 
                            // Common private network ranges
                            (ip.StartsWith("192.168.") || ip.StartsWith("10.0.") || ip.StartsWith("172.16.")) &&
                            !ip.EndsWith(".1") && // Exclude gateway IPs
                            !ip.EndsWith(".0")    // Exclude network IPs
                        );

                    allIpAddresses.AddRange(ipAddresses);
                }

                var selectedIp = allIpAddresses.FirstOrDefault();
                Console.WriteLine($"🌐 Network Detection:");
                Console.WriteLine($"🌐 Available IPs: {string.Join(", ", allIpAddresses)}");
                Console.WriteLine($"🌐 Selected IP: {selectedIp ?? "None"}");

                // Return the first available non-gateway IP for easy access from other devices
                return selectedIp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting IP address: {ex.Message}");
            }
            
            return null;
        }
        
        /// <summary>
        /// Validates if an IP address is reachable on the network
        /// </summary>
        /// <param name="ipAddress">IP address to test</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>True if reachable, false otherwise</returns>
        public static bool IsIpAddressReachable(string ipAddress, int timeout = 5000)
        {
            try
            {
                using var ping = new Ping();
                var reply = ping.Send(ipAddress, timeout);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets network configuration info for debugging
        /// </summary>
        public static string GetNetworkInfo()
        {
            var detectedIp = DetectLocalIpAddress();
            var isReachable = detectedIp != null && IsIpAddressReachable(detectedIp);
            
            return $@"Network Configuration:
- Live Server URL: {GetLiveServerUrl()}
- API Server URL: {GetApiServerUrl()}
- Detected Local IP: {detectedIp ?? "Not detected"}
- IP Reachable: {isReachable}
- Configured IP: {LIVE_SERVER_HOST}
- IP Match: {detectedIp == LIVE_SERVER_HOST}";
        }
    }
}
