using fmx_cah_host.Interfaces;
using fmx_cah_host.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace fmx_cah_host.Services
{
    public class CardDbService
    {
        private readonly object LOCK = new object();
        private List<Card> CardCollection = new List<Card>();
        private bool _loadedCards = false;

        public CardDbService()
        {
            if (!_loadedCards)
            {
                lock (LOCK)
                {
                    LoadCards();
                }
            }
        }

        /// <summary>
        /// Returns a list of cards from specified packs of specified type 
        /// </summary>
        /// <param name="type">The type of cards to retrieve</param>
        /// <param name="packs">List of packs the cards should be retrieved from</param>
        /// <returns></returns>
        public List<Card> GetCards(CardType type, List<CardPack> packs)
        {            
            var cards = new List<Card>();
            foreach(var pack in packs)
            {                
                foreach(var dbCard in CardCollection.Where(c=>c.Pack == pack && c.Type == type))
                {                    
                    cards.Add(dbCard);
                }
            }
            return cards;
        }

        // Attempts to load cards from the cards.json file
        private void LoadCards()
        {
            try
            {
                var cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText("cards.json"));
                foreach (var card in cards)
                    CardCollection.Add(card);                
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
