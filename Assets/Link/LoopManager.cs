using System.Collections.Generic;

public static class LoopManager {

    static List<Loop> loops = new List<Loop>();

    public static void Add(Loop loop) {
        loops.Add(loop);
    }

    public static bool AlreadyExists(this Loop loop) {
        foreach(Loop a in loops) {
            if (a.stars.Length == loop.stars.Length) {

                for (int i = 0; i < a.stars.Length; i++) {
                    if (a.stars[i] != loop.stars[i])
                        break;
                    else if (i == a.stars.Length - 1)
                        return true;
                }
            }
        }
        return false;
    }
}
