using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class Station : SteamCMDAgent // SteamCMDAgent is used because Station relies on SteamCMD for installation and update process
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.Stationeers", // WindowsGSM.XXXX
            author = "Cronus",
            description = "WindowsGSM plugin for supporting Arma 3 Dedicated Server",
            version = "1.0",
            url = "", // Github repository link (Best practice)
            color = "#9eff99" // Color Hex
        };


        // - Standard Constructor and properties
        public Station(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData; // Store server start metadata, such as start ip, port, start param, etc


        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true; // Station requires to login steam account to install the server, so loginAnonymous = false
        public override string AppId => "600760"; // Game server appId, Station is 233780


        // - Game server Fixed variables
        public override string StartPath => "rocketstation_DedicatedServer.exe"; // Game server start path, for Station, it is Stationserver.exe
        public string FullName = "Stationeers Server"; // Game server FullName
        public bool AllowsEmbedConsole = true;  // Does this server support output redirect?
        public int PortIncrements = 1; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = null; // Query method should be use on current server type. Accepted value: null or new A2S() or new FIVEM() or new UT3()


        // - Game server default values
        public string Port = "27500"; // Default port
        public string QueryPort = "27515"; // Default query port
        public string Defaultmap = "Moon"; // Default map name
        public string Maxplayers = "16"; // Default maxplayers
        public string Additional = ""; // Additional server start parameter


        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG() { }


        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
		
        {
            // Prepare start parameter
			var param = new StringBuilder($"{_serverData.ServerParam} -port {_serverData.ServerPort}");
 
            // Prepare Process
            var p = new Process
			{
				StartInfo =
				{
					WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
					FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
					Arguments = param.ToString(),
					WindowStyle = ProcessWindowStyle.Minimized,
					UseShellExecute = false
				},
				EnableRaisingEvents = true
			};

            // Start Process
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                base.Error = e.Message;
                return null; // return null if fail to start
            }
        }


        // - Stop server function
        public async Task Stop(Process p) => await Task.Run(() => { p.Kill(); }); // I believe Station don't have a proper way to stop the server so just kill it
    }
}
