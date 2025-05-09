namespace SocialMedia.TestUtilities
{
    public class MappingContext
    {
        private readonly Dictionary<object, MappingState> modelStates = new();

        public void SetState(object config, MappingState state)
        {
            modelStates[config] = state;
        }

        public MappingState GetState(object config)
        {
            return modelStates.TryGetValue(config, out var state) ? state : MappingState.Detached;
        }
    }
}
