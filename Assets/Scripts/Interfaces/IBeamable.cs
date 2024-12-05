public interface IBeamable
{
    /// <summary>
    /// Performs any actions required for this object to be picked up by the gravity gun
    /// and returns false if such a state cannot be actualized.
    /// </summary>
    /// <returns>True if the object has been put into a state to be picked up, and false otherwise</returns>
    public bool PickUp();
    public void Dropped();
}