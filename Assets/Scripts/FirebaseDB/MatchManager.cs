using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

namespace FirebaseDB
{
    public class MatchManager : MonoBehaviour
    {
        private FirebaseFirestore _db;

        private void Awake()
        {
            _db = FirebaseFirestore.DefaultInstance;
        }

        public void AddMatch(string user1Id, string user2Id, int winner = 0, Action<string> onSuccess = null, Action<Exception> onError = null)
        {
            if (winner < 0 || winner > 2)
            {
                onError?.Invoke(new ArgumentException("Winner must be 0 (not finished), 1 (user1), or 2 (user2)"));
                return;
            }
            
            DocumentReference docRef = _db.Collection("matches").Document();
            Dictionary<string, object> matchData = new Dictionary<string, object>
            {
                { "user1Id", user1Id },
                { "user2Id", user2Id },
                { "winner", winner },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            docRef.SetAsync(matchData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    onSuccess?.Invoke(docRef.Id);
                else
                    onError?.Invoke(task.Exception);
            });
        }

        public void UpdateMatchWinner(string matchId, int winner, Action onSuccess = null, Action<Exception> onError = null)
        {
            if (winner < 0 || winner > 2)
            {
                onError?.Invoke(new ArgumentException("Winner must be 0 (not finished), 1 (user1), or 2 (user2)"));
                return;
            }
            
            DocumentReference docRef = _db.Collection("matches").Document(matchId);
            Dictionary<string, object> updateData = new Dictionary<string, object>
            {
                { "winner", winner }
            };

            docRef.UpdateAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    onSuccess?.Invoke();
                else
                    onError?.Invoke(task.Exception);
            });
        }

        public void GetScoreBetweenPlayers(string user1Id, string user2Id, Action<int, int> onSuccess, Action<Exception> onError = null)
        {
            _db.Collection("matches")
                .WhereIn("user1Id", new List<object> { user1Id, user2Id })
                .WhereIn("user2Id", new List<object> { user1Id, user2Id })
                .GetSnapshotAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.Exception != null)
                    {
                        onError?.Invoke(task.Exception);
                        return;
                    }

                    int user1Wins = 0;
                    int user2Wins = 0;

                    foreach (var doc in task.Result.Documents)
                    {
                        int winner = doc.GetValue<int>("winner");
                        string u1 = doc.GetValue<string>("user1Id");
                        string u2 = doc.GetValue<string>("user2Id");

                        if (winner == 1 && u1 == user1Id) user1Wins++;
                        else if (winner == 1 && u1 == user2Id) user2Wins++;
                        else if (winner == 2 && u2 == user1Id) user1Wins++;
                        else if (winner == 2 && u2 == user2Id) user2Wins++;
                    }

                    onSuccess?.Invoke(user1Wins, user2Wins);
                });
        }
    }
}
