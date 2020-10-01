namespace BattleShipModels.ShipTypes
{
    abstract class IShip
    {
        public abstract string ShipType { get; }
        public abstract string Visuals { get; set; }
        public abstract int Speed { get; set; }

    }
}
