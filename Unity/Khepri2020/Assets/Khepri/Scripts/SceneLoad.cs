using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using KhepriUnity;
using UnityEngine;

using UnityProcessor = KhepriUnity.Processor<KhepriUnity.Channel, KhepriUnity.Primitives>;

public enum State { StartingServer, WaitingConnections, WaitingClient, Visualizing }

public class SceneLoad {

    TcpListener server;
    UnityProcessor processor;
    public State currentState = State.StartingServer;
    private GameObject mainObject;
    public Primitives primitives;
    public static bool visualizing = false;

    public SceneLoad() {
        mainObject = GameObject.Find("MainObject");
        if (mainObject == null) {
            mainObject = new GameObject("MainObject");
            mainObject.isStatic = true;
        }

        primitives = new Primitives(mainObject);
    }

    public bool StartServer() {
        try {
            if (server == null) {
                Int32 port = 11002;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
            } else {
                server.Stop();
            }
            server.Start();
            currentState = State.WaitingConnections;
            return true;

        } catch (Exception e) {
            WriteMessage(e.ToString() + "\n");
            WriteMessage("Couldn't start server\n");
            return false;
        }
    }

    public void StopServer() {
        if (server != null) {
            server.Stop();
            WriteMessage("Disconnecting from client\n");
            server = null;
        }
    }

    private Channel channel;

    public void WaitForConnections() {
        try {
            WriteMessage("Waiting for connections\n");
            channel = new Channel(server.AcceptTcpClient().GetStream());
            primitives.SetChannel(channel);
            processor =
                new UnityProcessor(
                    channel,
                    primitives);
            primitives.SetProcessor(processor);
            WriteMessage("Connection established\n");
            currentState = State.WaitingClient;
        } catch (IOException) {
            currentState = State.WaitingConnections;
            processor = null;
            WriteMessage("Disconnecting from client\n");
        } catch (Exception e) {
            currentState = State.WaitingConnections;
            processor = null;
            WriteMessage(e.ToString() + "\n");
            WriteMessage("Terminating client\n");
        }
    }

    public void WriteMessage(String msg) {
        Debug.Log(msg);
    }

    public bool Serve() {
        switch (currentState) {
            case State.StartingServer:
                //We don't do nothing. Supposedly, the server was started before
                return true;
            case State.WaitingConnections:
                WaitForConnections();
                return true;
            case State.WaitingClient:
                int op = processor.TryReadOperation();
                switch (op) {
                    case -2: //Timeout
                        return true;
                    case -1: //EOF
                        currentState = State.WaitingConnections;
                        processor = null;
                        WriteMessage("Disconnecting from client\n");
                        return false;
                    default:
                        processor.ExecuteReadAndRepeat(op);
                        return true;
                }
            default:
                return true;
        }
    }

    public void Update() {
        if (visualizing) {
            return;
        } else {
            Serve();
        }
    }

    void Awake() {
        //DontDestroyOnLoad(this.gameObject);
        //GameObject.Find("MainObject").transform.Rotate(-90, 0, 0);
        StartServer();
        visualizing = ! Application.isEditor;
    }

    public void OnDestroy() {
        StopServer();
    }
    
    /*    [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad() {
            new Thread(() => {
                new SceneLoadScript().WaitForConnections();
            }).Start();
        }
    */
    /*	private static void tick() {
            //Primitives.MakeCube(new Vector3(0, 0, 0), 1);
            Primitives.MakeCylinder(new Vector3(0, 0, 0), 1, new Vector3(2, 2, 2));
            Primitives.MakeCube(new Vector3(2, 2, 2), 1);
        }
        */
}
