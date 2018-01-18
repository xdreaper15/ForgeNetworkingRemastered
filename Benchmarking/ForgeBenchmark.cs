using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using System;

namespace Benchmarking
{
    public class ForgeBenchmark
    {
        private const string IP = "127.0.0.1";
        private const ushort PORT = 9500;
        private const int MY_GROUP_ID = MessageGroupIds.START_OF_GENERIC_IDS + 1; // Just a random message group id that is not being used anywhere else

        // First byte will be the length of the data
        private byte[] data = { 11, 0, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
        private NetWorker networker = null;

        public void Server(int maxClients = 32, int tickRate = 10)
        {
            UDPServer server = new UDPServer(maxClients);

            server.binaryMessageReceived += ReadBinary;
            server.Connect(IP, PORT);

            networker = server;
        }

        public void Disconnect()
        {
            networker.Disconnect(false);
        }

        public void Client(int tickRate = 10)
        {
            UDPClient client = new UDPClient();

            client.serverAccepted += TryWrite;
            client.Connect(IP, PORT);

            networker = client;
        }

        private void TryWrite(NetWorker sender)
        {
            ulong timestep = 0;
            bool isReliable = true;

            Binary bin = new Binary(timestep, false, data, Receivers.Target, MY_GROUP_ID, false);
            ((UDPClient)sender).Send(bin, isReliable);

            Console.WriteLine($"Client sent: { string.Join(", ", data)}");
        }

        private void ReadBinary(NetworkingPlayer player, Binary frame, NetWorker sender)
        {
            if (frame.GroupId != MY_GROUP_ID)
                return;

            byte length = frame.StreamData.GetBasicType<byte>();

            byte[] readData = new byte[length];
            for (int i = 0; i < length; i++)
            {
                readData[i] = frame.StreamData.GetBasicType<byte>();
            }

            Console.WriteLine($"Server read: {length}, { string.Join(", ", readData)}");
        }
    }
}