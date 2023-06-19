namespace FF.Management {
    public interface ICanInitialize
    {
        //
        // Summary:
        //     Signals the object that initialization is starting.
        public void DoInitialise(params object[] argv);
        bool hasBeenInitialised { get; }
    }
}