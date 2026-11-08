using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
    private DatabaseReference reference;

    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void UpdateUsername(string userId, string username)
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("Không thể cập nhật username: userId rỗng.");
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Không thể cập nhật username: username rỗng.");
            return;
        }

        reference
            .Child("Users")
            .Child(userId)
            .Child("Name")
            .SetValueAsync(username)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Cập nhật username bị hủy.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Cập nhật username thất bại: " + task.Exception);
                    return;
                }

                Debug.Log("Cập nhật username thành công.");
            });
    }

    public void WriteDatabase(string id, string json)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Không thể ghi Firebase: id rỗng.");
            return;
        }

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Không thể ghi Firebase: dữ liệu rỗng.");
            return;
        }

        reference
            .Child("Users")
            .Child(id)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Ghi dữ liệu bị hủy.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Ghi dữ liệu thất bại: " + task.Exception);
                    return;
                }

                Debug.Log("Ghi dữ liệu lên Firebase thành công.");
            });
    }
}
