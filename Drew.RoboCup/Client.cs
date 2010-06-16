/*
 * Created by Drew Noakes, 06/05/2010 14:07.
 */
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Drew.RoboCup.PerceptorParsing;

namespace Drew.RoboCup {
    public sealed class Client : IDisposable {
        private readonly TcpClient _client;
        private NetworkStream _stream;
        
        public Client(string hostName, int port) {
            Console.WriteLine("Connecting via TCP to {0}:{1}", hostName, port);
            try {
                _client = new TcpClient(hostName, port);
                Console.WriteLine("Connected.");
            } catch (SocketException) {
                Console.Error.WriteLine("Unable to connect to {0}:{1}.  Exiting.", hostName, port);
                throw;
            }
        }
        
        public void Run(IRobot robot) {
            using (_client) {
                _stream = _client.GetStream();
                
                Console.WriteLine("Sending initialisation messages");
                
                // Initialise with server.  We must first send the scene command, to specify which robot we'll be using.
                // NOTE We read between sends, even though no reponse will be received.  If we don't then we appear in middle, white.
                // TODO maybe just a pause is enough (rather than a read)
                // TODO make this programming model a bit nicer (don't require AppendCommand)
                var sb = new StringBuilder();
                new SceneSpecificationAction(robot.RsgPath).AppendCommand(sb);
                SendMessage(sb.ToString());
                ReadResponse();
                sb.Length = 0;
                // Specify which player on which team.
                new InitialisePlayerAction(robot.UniformNumber, robot.TeamName).AppendCommand(sb);
                SendMessage(sb.ToString());
                ReadResponse();
                
                Console.WriteLine("Press 'Q' to exit, 'P' to print next percepts, 'E' to print next effectors . . . ");
                
                bool printNextPerceptors = true;
                bool printNextEffectors = true;
                
                bool loopFinished = false;
                while (!loopFinished) {
					string data = ReadResponse();
					if (data!=null) {
                        // TODO try and reuse these parsing objects (custom s-expression parser)
                        var parser = new Parser(new Scanner(new StringBuffer(data)));
                        parser.Parse();
                                                
                        if (parser.errors.HasError)
                            Console.Error.WriteLine("PARSE ERROR: {0}\nDATA: {1}", parser.errors.ErrorMessages, data);
                        
                        if (printNextPerceptors) {
                            printNextPerceptors = false;
                            Console.WriteLine(parser.State.ToString());
                        }

                        if (!parser.State.SimulationTime.HasValue) {
                            Debug.Fail("Received message from server that did not contain simulation time: " + data);
                            continue;
                        }
                        
                        var actions = robot.Step(parser.State);
                        
                        if (actions!=null && actions.Any()) {
                            var message = new StringBuilder();
                            foreach (var action in actions)
                                action.AppendCommand(message);
                            
                            if (printNextEffectors) {
                                printNextEffectors = false;
                                Console.WriteLine(message.ToString());
                            }
                            SendMessage(message.ToString());
                        }
                    }
                    
                    if (Console.KeyAvailable) {
					    var key = char.ToUpper(Console.ReadKey(true).KeyChar);
                        if (key=='Q')
                            loopFinished = true;
                        else if (key=='P')
                            printNextPerceptors = true;
                        else if (key=='E')
                            printNextEffectors = true;
                        else if (robot is IInteractiveRobot)
                            ((IInteractiveRobot)robot).HandleUserInput(key);
                    }
                }
                
                if (robot is IDisposable)
                    ((IDisposable)robot).Dispose();
            }
        }
        
        private string ReadResponse() {
            // It seems like a good idea to pass the stream to Coco/R rather than loading the whole
            // string into memory first, however because Coco/R requires the ability to seek within
            // the stream, it would internally load the stream into a buffer anyway.  To avoid this
            // memory churn, Coco/R would need to be changed or replaced.
            int sleepCount = 0;
            while (!_stream.DataAvailable) {
                if (++sleepCount > 100) {
                    Console.WriteLine("No response received within limit.");
                    return null;
                }
                Thread.Sleep(5);
            }
            
            int length = ReadInt32();
            int totalBytesRead = 0;
            var bytes = new byte[length];
            
            do {
                totalBytesRead += _stream.Read(bytes, totalBytesRead, length - totalBytesRead);
            } while (totalBytesRead < length);
            
            return Encoding.ASCII.GetString(bytes);
        }

        private int ReadInt32() {
            var lengthBytes = new byte[4];
            _stream.Read(lengthBytes, 0, 4);
            int length = (lengthBytes[0] << 24) | (lengthBytes[1] << 16) | (lengthBytes[2] << 8) | (lengthBytes[3]);
            return length;
        }
        
        private void SendMessage(string msg) {
            /*
             * Messages exchanged between client and server use the default ASCII character set, i.e.
             * one character is encoded in a single byte. Further each individual message is prefixed with the
             * length of the payload message. The length prefix is a 32 bit unsigned integer in network order,
             * i.e. big endian notation with the most significant bits transferred first.
             */
            byte[] bytes = Encoding.ASCII.GetBytes(msg);
            uint num = (uint)bytes.Length;
            var numBytes = new byte[4];
            numBytes[3] = (byte)(num & 0xff);
            numBytes[2] = (byte)(num >> 8 & 0xff);
            numBytes[1] = (byte)(num >> 16 & 0xff);
            numBytes[0] = (byte)(num >> 24 & 0xff);
            _stream.Write(numBytes, 0, numBytes.Length);
            _stream.Write(bytes, 0, bytes.Length);
        }
        
        public void Dispose() {
            _stream.Dispose();
            _client.Close();
        }
        
    }
}