using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShipModels.ShipTypes
{
    class AirCraftCarrier : IShip
    {
        private readonly string _ShipType;
        private string _Visuals;
        private int _Speed;
        public AirCraftCarrier(string visuals, int speed) {
            _ShipType = "AirCraft Carrier";
            _Visuals = visuals;
            _Speed = speed;

        }
        public override string ShipType {
            get { return _ShipType; }
        }
        public override string Visuals {
            get { return _Visuals; }
            set { _Visuals = Visuals; }
        }
        public override int Speed
        {
            get { return _Speed; }
            set { _Speed = Speed; }
        }
    }
}
