namespace FF.Management {
    public interface ICanInitialize<T>
    {
        //
        // Summary:
        //     Signals the object that initialization is starting.
        public void DoInitialise(T args);
        bool hasBeenInitialised { get; }
    }
}