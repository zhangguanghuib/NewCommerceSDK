namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample
    {
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;

        public interface ICoinDispenser : IPeripheral
        {
            void DispenseChange(int amout);
        }
    }
}
