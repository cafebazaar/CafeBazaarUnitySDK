namespace CafeBazaar.Games.BasicApi.SavedGame
{
    public interface ISavedGameClient
    {
        void Clear();
        void DeleteKey(string key);
        bool HasKey(string key);

        float GetFloat(string key, float defaultValue);
        float GetFloat(string key);
        int GetInt(string key, int defaultValue);
        int GetInt(string key);
        string GetString(string key, string defaultValue);
        string GetString(string key);
        bool GetBool(string key, bool defaultValue);
        bool GetBool(string key);

        void SetFloat(string key, float value);
        void SetInt(string key, int value);
        void SetString(string key, string value);
        void SetBool(string key, bool value);

        bool IsSynced { get; }
    }
}