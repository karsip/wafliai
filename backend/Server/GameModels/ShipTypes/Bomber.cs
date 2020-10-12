using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShipModels.ShipTypes
{
    class Bomber:IShip
    {
        private readonly string _ShipType;
        private string _Visuals;
        private int _Speed;
        public Bomber(string visuals, int speed)
        {
            _ShipType = "Bomber";
            _Visuals = visuals;
            _Speed = speed;

        }
        public override string ShipType
        {
            get { return _ShipType; }
        }
        public override string Visuals
        {
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
