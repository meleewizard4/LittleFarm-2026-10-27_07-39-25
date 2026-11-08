using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

public class LoadDataManager : MonoBehaviour
{
    public static FirebaseUser firebaseUser;
    public static User UserData;

    private DatabaseReference reference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;

        if (firebaseUser != null)
        {
            GetUserData();
        }
        else
        {
            Debug.LogError("Không có Firebase user đang đăng nhập.");
        }
    }

    public void GetUserData()
    {
        if (firebaseUser == null)
        {
            Debug.LogError("Cannot load user data because FirebaseUser is null.");
            return;
        }

        reference.Child("Users").Child(firebaseUser.UserId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Đọc dữ liệu thất bại: " + task.Exception);
                    return;
                }

                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists && snapshot.Value != null)
                {
                    UserData = JsonConvert.DeserializeObject<User>(
                        snapshot.Value.ToString());

                    Debug.Log(
                        "Dữ liệu đã đọc: " + UserData.ToString());
                }
                else
                {
                    UserData = new User("", 100, 50, new Map());

                    reference
                        .Child("Users")
                        .Child(firebaseUser.UserId)
                        .SetRawJsonValueAsync(UserData.ToString())
                        .ContinueWithOnMainThread(writeTask =>
                        {
                            if (writeTask.IsFaulted || writeTask.IsCanceled)
                            {
                                Debug.LogError(
                                    "Tạo dữ liệu user thất bại: " + writeTask.Exception);
                                return;
                            }

                            Debug.Log(
                                "Đã tạo dữ liệu mặc định: " + UserData.ToString());
                        });
                }
            });
    }
}
