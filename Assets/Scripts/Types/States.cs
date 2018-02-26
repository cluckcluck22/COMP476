
public class States {


    // FullBody states are all mutually exclusive
    public enum FullBody : int
    {
        Idle = 1 << 0,   // Default
        Dead = 1 << 1,
        Eat =  1 << 2,
        Hate = 1 << 3,
        Move = 1 << 4,
        Rest = 1 << 5
    }

    // Layered states are all mutually exclusive
    // Layered states can run "on top" of an existing FullBody state 
    public enum Layered : int
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
}
