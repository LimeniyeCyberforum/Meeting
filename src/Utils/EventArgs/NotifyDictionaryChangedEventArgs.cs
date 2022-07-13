using System.Collections.Generic;

namespace Utils.EventArgs
{
    public enum NotifyDictionaryChangedAction
    {
        Added,
        Removed,
        Changed,
        Cleared,
        Initialized
    }

    public class NotifyDictionaryChangedEventArgs<TKey, TValue> : NotifyActionDictionaryChangedEventArgs
    {
        public TKey Key { get; }

        public TValue OldValue { get; }

        public TValue NewValue { get; }

        public int Number { get; }

        public long SenderTime { get; }

        public IDictionary<TKey, TValue> NewDictionary { get; }

        public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action, TKey key, TValue oldValue, TValue newValue, int number, long senderTime)
            : base(action)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;

            Number = number;
            SenderTime = senderTime;
        }

        public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action, IDictionary<TKey, TValue> newDictionary, int number, long senderTime)
            : base(action)
        {
            NewDictionary = newDictionary;

            Number = number;
            SenderTime = senderTime;
        }
    }

    public class NotifyActionDictionaryChangedEventArgs : System.EventArgs
    {
        public NotifyDictionaryChangedAction Action { get; }

        public NotifyActionDictionaryChangedEventArgs(NotifyDictionaryChangedAction action)
        {
            Action = action;
        }

        public static NotifyDictionaryChangedEventArgs<TKey, TValue> AddKeyValuePair<TKey, TValue>(TKey key, TValue value, int number, long senderTime)
          => new NotifyDictionaryChangedEventArgs<TKey, TValue>(NotifyDictionaryChangedAction.Added, key, default, value, number, senderTime);

        public static NotifyDictionaryChangedEventArgs<TKey, TValue> RemoveKeyValuePair<TKey, TValue>(TKey key, int number, long senderTime)
            => new NotifyDictionaryChangedEventArgs<TKey, TValue>(NotifyDictionaryChangedAction.Removed, key, default, default, number, senderTime);

        public static NotifyDictionaryChangedEventArgs<TKey, TValue> ChangeKeyValuePair<TKey, TValue>(TKey key, TValue oldValue, TValue newValue, int number, long senderTime)
            => new NotifyDictionaryChangedEventArgs<TKey, TValue>(NotifyDictionaryChangedAction.Changed, key, oldValue, newValue, number, senderTime);

        public static NotifyDictionaryChangedEventArgs<TKey, TValue> InitializeKeyValuePairs<TKey, TValue>(IDictionary<TKey, TValue> values, int number, long senderTime)
            => new NotifyDictionaryChangedEventArgs<TKey, TValue>(NotifyDictionaryChangedAction.Initialized, values, number, senderTime);

        public static NotifyDictionaryChangedEventArgs<TKey, TValue> ClearKeyValuePairs<TKey, TValue>(int number, long senderTime)
            => new NotifyDictionaryChangedEventArgs<TKey, TValue>(NotifyDictionaryChangedAction.Cleared, default, default, default, number, senderTime);
    }
}
