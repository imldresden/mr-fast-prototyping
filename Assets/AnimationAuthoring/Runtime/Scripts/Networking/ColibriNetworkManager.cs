using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCIKonstanz.Colibri;
using System;
using HCIKonstanz.Colibri.Synchronization;
using Newtonsoft.Json.Linq;
using HCIKonstanz.Colibri.Networking;

namespace com.animationauthoring
{ 
public class ColibriNetworkManager : MonoBehaviour
{
    public List<Player> players;
    public Player self;
    private Player owner;
    public bool lobbyJoined = false;

    private void Start()
    {
        Debug.Log("Connecting to Web server");
        SetupListeners();
        StartCoroutine(WaitForConnectionAndJoinLobby());
    }
    public bool IsOwner => self.IsOwner;

    private void OnApplicationQuit()
        {
            if (self != null)
            {
                Sync.Send("PlayerLeft", self.Id);
            }
        }
        private IEnumerator WaitForConnectionAndJoinLobby()
    {
        while (WebServerConnection.Instance.Status != ConnectionStatus.Connected)
        {
            yield return null;
        }

        Sync.Send("QueryPlayers", "Query");
        Sync.Receive("QueryPlayers", (string name) =>
        {
            Sync.Send("ReceivePlayers", new JObject
            {
                { "Name", self.Name },
                { "Owner", self.IsOwner },
                {"Id", self.Id}
            });
        });
        //we need to wait for all players to answer
        yield return new WaitForSeconds(1);
            SortPlayers();
        var i = players.Count;
        self = new Player("Player" + i);
        self.Id = i;

        if (players.Count == 0)
        {
            self.IsOwner = true;
        }
        else
        {
            self.IsOwner = false;
        }

        AddPlayer(self);
        Sync.Send("Lobbyjoin", new JObject
        {
            { "Name", self.Name },
            { "Owner", self.IsOwner },
            {"Id", self.Id}

        });

        Debug.Log("Joined Lobby as " + self.Name);
        Debug.Log("Owner: " + owner.Name);
            lobbyJoined = true;
    }
    void SortPlayers()
    {
        //Sort players by ID, starting with the lowest ID
        players.Sort((a, b) => a.Id.CompareTo(b.Id));
    }

    void SetupListeners()
    {
 
            //Sync.Receive("Lobbyjoin", self.Name);
            Sync.Receive("PlayerLeft", (int id) =>
            {
                var player = players.Find(p => p.Id == id);
                if (player != null)
                {
                    RemovePlayer(player);
                    SortPlayers();
                    Debug.Log("Player left Lobby: " + player.Name);
                    //print owner name
                    Debug.Log("Owner: " + owner.Name);
                }
            });
            Sync.Receive("ReceivePlayers", (JToken player) =>
            {
                var name = player["Name"].ToString();
                var isOwner = player["Owner"].ToObject<bool>();
                var id = player["Id"].ToObject<int>();
                var newPlayer = new Player(name);
                newPlayer.IsOwner = isOwner;
                newPlayer.Id = id;
                if (isOwner)
                    owner = newPlayer;
                AddPlayer(newPlayer);
                Debug.Log("Player already in Lobby: " + newPlayer.Name);
            });

        Sync.Receive("Lobbyjoin", (JToken joinedPlayer) =>
        {
            var name = joinedPlayer["Name"].ToString();
            var isOwner = joinedPlayer["Owner"].ToObject<bool>();
            var id = joinedPlayer["Id"].ToObject<int>();
            var newPlayer = new Player(name);
            newPlayer.IsOwner = isOwner;
            newPlayer.Id = id;
            AddPlayer(newPlayer);
            Debug.Log("Player joined Lobby: " + newPlayer.Name);
        });

        }

    public ColibriNetworkManager()
    {
        players = new List<Player>();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        AssignOwner();
    }

    public void RemovePlayer(Player player)
    {
            players.Remove(player);
            SortPlayers();
            //check that the ids in players are filled without gaps from 0 to n, if not, reassign the next highest id to take the place of the removed id
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Id != i)
                {
                    players[i].Id = i;
                }
            }
        AssignOwner();
    }

    private void AssignOwner()
    {
        if (players.Count > 0)
        {
            owner = players[0];
            owner.IsOwner = true;
            Console.WriteLine($"Player {owner.Name} is now the owner.");
        }
        else
        {
            owner = null;
            Console.WriteLine("No players in the lobby.");
        }
    }
}

public class Player
{
    public string Name { get; set; }
    public bool IsOwner { get; set; }
    public int Id { get; set; }

    public Player(string name)
    {
        Name = name;
        IsOwner = false;
    }
}
}
