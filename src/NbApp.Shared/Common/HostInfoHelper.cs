using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class HostInfoHelper
    {
        public static HostInfoHelper Instance = new HostInfoHelper();

        #region 为保证兼容保留 

        public string GetFirstMac()
        {
            return GetHostInfo().FirstMac;
        }

        public string GetFirstIpV4()
        {
            return GetHostInfo().FirstIpV4;
        }

        #endregion

        private void LogInfo(string msg)
        {
            MyCacheLog.Instance.LogInfo(msg, "HostInfoHelper");
        }

        private HostInfo _cachedHostInfo = null;
        public HostInfo GetHostInfo(bool useCache = true)
        {
            if (_cachedHostInfo != null && useCache)
            {
                return _cachedHostInfo;
            }

            var theOne = TryLoadFromMockFile();
            if (theOne != null)
            {
                _cachedHostInfo = theOne;
                return _cachedHostInfo;
            }
           
            _cachedHostInfo = GetHostInfoFix(); ;
            return _cachedHostInfo;
        }
        
        private HostInfo GetHostInfoFix()
        {
            HostInfo theOne = null;
            //发现某些设备上，如果服务启动较早，此段代码执行过早，可能读取不到硬件信息，尝试自动延迟给与修正10秒
            LogInfo($"GetHostInfoFix => begin ");
            for (int i = 0; i < 100; i++)
            {
                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                var nicItems = NetworkInterface.GetAllNetworkInterfaces();
                theOne = CreateHostInfo(ipGlobalProperties, nicItems);
                
                var theIpV4 = theOne.GetFirstIpV4();
                LogInfo($"GetHostInfoFix => {theIpV4}");
                if (!string.IsNullOrWhiteSpace(theIpV4))
                {
                    return theOne;
                }
                Thread.Sleep(200);
            }

            return theOne;
        }

        private HostInfo CreateHostInfo(IPGlobalProperties ipGlobalProperties, IEnumerable<NetworkInterface> networkInterfaces)
        {
            var hostInfo = new HostInfo();
            hostInfo.HostName = ipGlobalProperties.HostName;
            foreach (var item in Dns.GetHostAddresses(hostInfo.HostName))
            {
                hostInfo.IpInfos.Add(item.ToString() + HostInfo.IpSplit + item.AddressFamily.ToString());
            }

            var interfaces = networkInterfaces.ToList();
            if (interfaces.Count < 1)
            {
                return hostInfo;
            }

            foreach (var networkInterface in interfaces)
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                var ipProperties = networkInterface.GetIPProperties();
                if (ipProperties.UnicastAddresses.Any(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    var address = networkInterface.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        var macItem = new NicInfo();
                        macItem.Name = networkInterface.Name;
                        macItem.OperationalStatus = networkInterface.OperationalStatus.ToString();
                        macItem.NetworkInterfaceType = networkInterface.NetworkInterfaceType.ToString();
                        macItem.Mac = address.ToString();
                        macItem.Description = networkInterface.Description;
                        macItem.Speed = networkInterface.Speed;

                        var localIpV4 = ipProperties.UnicastAddresses.Where(x
                            => x.Address.AddressFamily == AddressFamily.InterNetwork
                            && !IPAddress.IsLoopback(x.Address))
                            .Select(x => x.Address.ToString())
                            .FirstOrDefault();

                        var localIpV6 = ipProperties.UnicastAddresses.Where(x
                            => x.Address.AddressFamily == AddressFamily.InterNetworkV6
                            && !IPAddress.IsLoopback(x.Address))
                            .Select(x => x.Address.ToString())
                            .FirstOrDefault();

                        macItem.LocalIpV4 = localIpV4;
                        macItem.LocalIpV6 = localIpV6;

                        hostInfo.NicInfos.Add(macItem);
                    }
                }
            }
            return hostInfo;
        }

        private HostInfo TryLoadFromMockFile()
        {
            //load mock file if => "App_Data/fixed-host-info.json"
            var theValue = MockHostInfoData.LoadFromFile<HostInfo>(null);
            return theValue;
        }

        public JsonFileData MockHostInfoData { get; set; } = JsonFileData.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "fixed-host-info.json"));
    }

    public class HostInfo
    {
        public string HostName { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public List<string> IpInfos { get; set; } = new List<string>();
        public List<NicInfo> NicInfos { get; set; } = new List<NicInfo>();
        public List<NicInfo> GetActiveNicInfos(string filter = null)
        {
            //todo: 过滤不需要的网卡?  
            //if (mServiceName.ToLower().Contains("vmnetadapter") 
            //    ||mServiceName.ToLower().Contains("vmware") 
            //    || mServiceName.ToLower().Contains("ppoe")  
            //    || mServiceName.ToLower().Contains("bthpan")  
            //    || mServiceName.ToLower().Contains("tapvpn")  
            //    || mServiceName.ToLower().Contains("ndisip")  
            //    || mServiceName.ToLower().Contains("sinforvnic"))  
            var results = NicInfos
                .Where(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback.ToString()
                            && x.NetworkInterfaceType == NetworkInterfaceType.Ethernet.ToString()
                            && x.OperationalStatus == OperationalStatus.Up.ToString()
                            && !string.IsNullOrWhiteSpace(x.Mac))
                .OrderBy(x => (int)Enum.Parse<NetworkInterfaceType>(x.NetworkInterfaceType))
                .ToList();

            //有线网 > 无线网
            //Ethernet = 6,
            //Wireless80211 = 71,
            if (!string.IsNullOrWhiteSpace(filter))
            {
                return results.Where(x => x.Description.Contains(filter) || x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return results;
        }
        public NicInfo GetTheNicInfo(GetTheMacInfo args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            var activeMacInfos = GetActiveNicInfos(args.Filter);
            if (activeMacInfos.Count == 1)
            {
                return activeMacInfos[0];
            }

            if (!string.IsNullOrWhiteSpace(args.Mac))
            {
                var theOne = activeMacInfos.FirstOrDefault(x => x.Mac.Equals(args.Mac, StringComparison.OrdinalIgnoreCase));
                return theOne;
            }
            return activeMacInfos.FirstOrDefault();
        }

        public static string IpSplit { get; set; } = "@@";
        public List<string> FindIps(string filter)
        {
            var query = IpInfos.Select(x =>
            {
                var splits = x.Split(IpSplit, StringSplitOptions.RemoveEmptyEntries);
                return new { Ip = splits[0], Type = splits[2] };
            });

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query.Where(x => x.Type.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }
            return query.Select(x => x.Ip).ToList();
        }

        private string _firstMac = null;
        public string FirstMac
        {
            get
            {
                if (_firstMac == null)
                {
                    _firstMac = GetFirstMac();
                }
                return _firstMac;
            }
        }

        private string _firstIpV4 = null;
        public string FirstIpV4
        {
            get
            {
                if (_firstIpV4 == null)
                {
                    _firstIpV4 = GetFirstIpV4();
                }
                return _firstIpV4;
            }
        }

        public string GetFirstMac()
        {
            var hostInfo = this;
            //var theOne = hostInfo.NicInfos.OrderBy(x => x.Mac).FirstOrDefault();
            var theOne = hostInfo.GetActiveNicInfos().OrderBy(x => x.Mac).FirstOrDefault();
            return theOne?.Mac ?? "";
        }
        public string GetFirstIpV4()
        {
            var theOne = GetActiveNicInfos().OrderBy(x => x.Mac).FirstOrDefault();
            return theOne?.LocalIpV4;
        }

        //public static void ShowNetworkInterfaces()
        //{
        //    IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
        //    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        //    Console.WriteLine("Interface information for {0}.{1}     ",
        //            computerProperties.HostName, computerProperties.DomainName);
        //    if (nics == null || nics.Length < 1)
        //    {
        //        Console.WriteLine("  No network interfaces found.");
        //        return;
        //    }

        //    Console.WriteLine("  Number of interfaces .................... : {0}", nics.Length);
        //    foreach (NetworkInterface adapter in nics)
        //    {
        //        IPInterfaceProperties properties = adapter.GetIPProperties(); //  .GetIPInterfaceProperties();
        //        Console.WriteLine();
        //        Console.WriteLine(adapter.Description);
        //        Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
        //        Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
        //        Console.Write("  Physical address ........................ : ");
        //        PhysicalAddress address = adapter.GetPhysicalAddress();
        //        byte[] bytes = address.GetAddressBytes();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            // Display the physical address in hexadecimal.
        //            Console.Write("{0}", bytes[i].ToString("X2"));
        //            // Insert a hyphen after each byte, unless we are at the end of the
        //            // address.
        //            if (i != bytes.Length - 1)
        //            {
        //                Console.Write("-");
        //            }
        //        }
        //        Console.WriteLine();
        //    }
        //}
    }

    public class GetTheMacInfo
    {
        public string Mac { get; set; }
        public string Filter { get; set; }

        public static GetTheMacInfo Create(string mac, string filter)
        {
            return new GetTheMacInfo() { Mac = mac, Filter = filter };
        }
    }

    public class NicInfo
    {
        public string Name { get; set; }
        public string NetworkInterfaceType { get; set; }
        public string Mac { get; set; }
        public string Description { get; set; }
        public string OperationalStatus { get; set; }
        public long Speed { get; set; }
        public string LocalIpV4 { get; set; }
        public string LocalIpV6 { get; set; }
    }
}
