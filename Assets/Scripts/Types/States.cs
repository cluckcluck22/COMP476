
public class States {


    // FullBody states are all mutually exclusive
    public enum AnimalFullBody : int
    {
        Idle = 1 << 0,   // Default
        Dead = 1 << 1,
        Eat =  1 << 2,
        Hate = 1 << 3,
        Move = 1 << 4,
        Rest = 1 << 5,
        Sit  = 1 << 6
    }

    // Layered states are all mutually exclusive
    // Layered states can run "on top" of an existing FullBody state 
    public enum AnimalLayered : int
    {
        None =          1 << 0,   // Default
        Talk =          1 << 1,
        Attack =        1 << 2,
        HitReaction =   1 << 3
    }

    public enum MoveSpeed : int
    {
        Walk =  1 << 0,     // Default
        Run =   1 << 1
    }

    public enum FarmerFullBody : int
    {
        Idle = 1 << 0,
        Walk = 1 << 1,
        Run =  1 << 2,
        Dig =  1 << 3,
        Fire = 1 << 4,
        Interact = 1 << 5
    }

    public enum FarmerLayered : int
    {
        None = 1 << 0,
        Sign = 1 << 1
    }
}
