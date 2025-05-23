using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

namespace FirebaseDB
{
    public class FirebaseManager : MonoBehaviour
    {
        public static FirebaseManager Instance;
        
        [HideInInspector] public UserManager userManager;
        [HideInInspector] public MatchManager matchManager;
        
        
        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                InitializeFirebase();
                AttachManagers();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    Debug.Log("Firebase ready.");
                }
                else
                {
                    Debug.LogError("Firebase error: " + status);
                }
            });
        }

        private void AttachManagers()
        {
            userManager = gameObject.AddComponent<UserManager>();
            matchManager = gameObject.AddComponent<MatchManager>();
        }
    }
}
