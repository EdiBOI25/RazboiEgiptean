using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

namespace FirebaseDB
{
    public class UserManager : MonoBehaviour
    {
        private FirebaseFirestore _db;

        private void Awake()
        {
            _db = FirebaseFirestore.DefaultInstance;
        }
        
        public void GetUserTotalMatches(string steamId, Action<int> onSuccess, Action<Exception> onError = null)
        {
            EnsureUserExists(steamId, () =>
            {
                _db.Collection("users").Document(steamId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully && task.Result.Exists)
                    {
                        int totalMatches = task.Result.GetValue<int>("totalMatches");
                        onSuccess?.Invoke(totalMatches);
                    }
                    else
                    {
                        onError?.Invoke(task.Exception ?? new Exception("User not found"));
                    }
                });
            });
        }
        
        public void IncrementUserTotalMatches(string steamId, Action onSuccess = null, Action<Exception> onError = null)
        {
            EnsureUserExists(steamId, () =>
            {
                DocumentReference docRef = _db.Collection("users").Document(steamId);
                Dictionary<string, object> incrementData = new Dictionary<string, object>
                {
                    { "totalMatches", FieldValue.Increment(1) }
                };

                docRef.UpdateAsync(incrementData).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                        onSuccess?.Invoke();
                    else
                        onError?.Invoke(task.Exception);
                });
            });
            
        }
        
        private void EnsureUserExists(string steamId, Action onComplete = null, Action<Exception> onError = null)
        {
            DocumentReference docRef = _db.Collection("users").Document(steamId);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompletedSuccessfully)
                {
                    onError?.Invoke(task.Exception);
                    return;
                }

                if (task.Result.Exists)
                {
                    onComplete?.Invoke();
                }
                else
                {
                    Dictionary<string, object> userData = new Dictionary<string, object>
                    {
                        { "steamId", steamId },
                        { "totalMatches", 0 }
                    };

                    docRef.SetAsync(userData).ContinueWithOnMainThread(createTask =>
                    {
                        if (createTask.IsCompletedSuccessfully)
                            onComplete?.Invoke();
                        else
                            onError?.Invoke(createTask.Exception);
                    });
                }
            });
        }

        private void AddUser(string steamId, int totalMatches = 0, Action onSuccess = null, Action<Exception> onError = null)
        {
            DocumentReference docRef = _db.Collection("users").Document(steamId);
            Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "steamId", steamId },
                { "totalMatches", totalMatches }
            };

            docRef.SetAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    onSuccess?.Invoke();
                else
                    onError?.Invoke(task.Exception);
            });
        }

        private void UpdateUser(string steamId, int? totalMatches = null, Action onSuccess = null, Action<Exception> onError = null)
        {
            DocumentReference docRef = _db.Collection("users").Document(steamId);
            Dictionary<string, object> updates = new Dictionary<string, object>();

            if (totalMatches.HasValue)
                updates["totalMatches"] = totalMatches.Value;

            docRef.UpdateAsync(updates).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    onSuccess?.Invoke();
                else
                    onError?.Invoke(task.Exception);
            });
        }

        private void GetUser(string steamId, Action<DocumentSnapshot> onSuccess, Action<Exception> onError = null)
        {
            _db.Collection("users").Document(steamId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    onSuccess?.Invoke(task.Result);
                else
                    onError?.Invoke(task.Exception);
            });
        }
    }
}