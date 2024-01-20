namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample.Messages
    {
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        public class DispenseChangeCoinDispenserDeviceRequest:Request
        {
            public DispenseChangeCoinDispenserDeviceRequest(int amount)
            {
                this.Amount = amount;
            }

            public int Amount { get; private set; }

        }

    }
}
