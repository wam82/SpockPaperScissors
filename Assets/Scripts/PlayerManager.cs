using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerManager : MonoBehaviour
{
   public static PlayerManager Instance { get; private set; }

   [SerializeField] private List<GameObject> Rocks = new List<GameObject>();
   [SerializeField] private List<GameObject> Papers = new List<GameObject>();
   [SerializeField] private List<GameObject> Scissors = new List<GameObject>();
   [SerializeField] private List<GameObject> Lizards = new List<GameObject>();
   [SerializeField] private List<GameObject> Spocks = new List<GameObject>();
   private List<List<GameObject>> Factions = new List<List<GameObject>>();
   
   public List<GameObject> factionMembers = new List<GameObject>();
   private int factionIndex;

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }

   public void UpdateList(GameObject obj)
   {
      foreach (List<GameObject> faction in Factions)
      {
         if (faction.Contains(obj))
         {
            faction.Remove(obj);
            break;
         }
      }

      if (factionMembers.Contains(obj))
      {
         factionMembers.Remove(obj);
      }
   }

   public void SetFaction(int faction)
   {
      factionIndex = faction;
      factionMembers = Factions[factionIndex];
   }

   private void Start()
   {
      Factions.Add(Rocks);
      Factions.Add(Papers);
      Factions.Add(Scissors);
      Factions.Add(Lizards);
      Factions.Add(Spocks);
      
      SetFaction(0);
   }
   
   
}
