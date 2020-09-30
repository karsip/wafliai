using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BattleShipModels;

namespace GameServer
{
    public class BattleShipHub : Hub
    {

        public async Task UpdateClients()
        {
           await Clients.All.SendAsync("StateUpdate",
                                        JsonConvert.SerializeObject(Map.MapInstance.GetMapObjects(),
                                        typeof(MapCell), new JsonSerializerSettings
                                        { TypeNameHandling = TypeNameHandling.Auto }));
        }
        public async Task HandleMapClick(string mapCoordinate)
        {
            Console.WriteLine("Key press " + mapCoordinate + " user " + Context.ConnectionId);
            // Map.Instance.MovePlayer(Context.ConnectionId, KeyPress);
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
