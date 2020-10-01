using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BattleShipModels;
using System.Diagnostics;

namespace GameServer
{
    public class BattleShipHub : Hub
    {

        public async Task UpdateClients()
        {
           await Clients.All.SendAsync("MapUpdate",
                                        JsonConvert.SerializeObject(Map.MapInstance.GetMapObjects(),
                                        typeof(MapCell), new JsonSerializerSettings
                                        { TypeNameHandling = TypeNameHandling.Auto }));
        }
        public async Task HandleMapClick(string mapCoordinate)
        {
            Console.WriteLine("Key press " + mapCoordinate + " user " + Context.ConnectionId);
            await UpdateClients();
        }
        public async override Task OnConnectedAsync()
        {
            Map.MapInstance.AddPlayer(Context.ConnectionId);
            await UpdateClients();
            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            //Map.RemovePlayer(Context.ConnectionId);
            //await UpdateClients();
            //await base.OnDisconnectedAsync(exception);
        }
    }
}
