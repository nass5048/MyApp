using Blazored.LocalStorage;
using Microsoft.JSInterop;
using WordleGuesser.Classes;

namespace test1.Wordle.Classes
{
    public class Stats
    {
        private const string StorageKey = "Stats";

        private readonly ILocalStorageService _localStorage;

        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int RobotWins { get; set; }
        public int RobotLosses { get; set; }
        public int CurrentStreak { get; set; }
        public DateTime? LastPlayed { get; set; }
        public int MaxStreak { get; set; }
        public List<WordGuessResponse?> Guesses { get; set; }

        public int robotCount { get; set; }

        public Stats()
        {
        }
        public Stats(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public async Task FinishGame(bool win, List<WordGuessResponse?> guesses)
        {
            //prevents multiple games in one day from counting
            if (!LastPlayed.HasValue || !(LastPlayed.Value.Date == DateTime.Today))
            {
                GamesPlayed++;
                if (LastPlayed.HasValue && LastPlayed.Value.Date.AddDays(1) == DateTime.Today)
                {
                    CurrentStreak++;
                    if (CurrentStreak > MaxStreak)
                    {
                        MaxStreak = CurrentStreak;
                    }
                }
                else
                {
                    CurrentStreak = 1;
                }
                LastPlayed = DateTime.Today;
                if (win)
                {
                    GamesWon++;
                }
                Guesses = guesses;
                if (robotCount <= Guesses.Count(p => p != null))
                {
                    RobotWins++;
                }
                else
                {
                    RobotLosses++;
                }
            }
            await SaveAsync();
        }
        public async Task LoadAsync()
        {
            var saved = await _localStorage.GetItemAsync<Stats>(StorageKey);
            if (saved != null)
            {
                GamesPlayed = saved.GamesPlayed;
                GamesWon = saved.GamesWon;
                RobotWins = saved.RobotWins;
                CurrentStreak = saved.CurrentStreak;
                LastPlayed = saved.LastPlayed;
                MaxStreak = saved.MaxStreak;
                Guesses = saved.Guesses;
                RobotLosses = saved.RobotLosses;
            }
        }

        public async Task SaveAsync() { 
            await _localStorage.SetItemAsync(StorageKey, this); 
        }
    }
}
