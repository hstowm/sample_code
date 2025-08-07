using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;

public class SingletonScriptable<T> : SerializedScriptableObject where T : SingletonScriptable<T>
{
    private static T i;
    public static T instance
    {
        get
        {
            if (i == null)
            {
                var op = Addressables.LoadAssetAsync<T>(typeof(T).Name);

                i = op.WaitForCompletion(); //Forces synchronous load so that we can return immediately
            }
            return i;
        }
    }
}