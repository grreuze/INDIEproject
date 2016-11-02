using UnityEngine;
using System.Collections.Generic;

public static class CircuitManager {
    
    public static void CheckCircuit(Star star) {
        List<Star> path = new List<Star>();
        CheckStar(star, path);
    }
    
    static void CheckStar(Star star, List<Star> currentPath) {
        currentPath.Add(star);

        if (star.links.Count > 0) {
            foreach (Link link in star.links)
                CheckIfPathContains(currentPath, link.target);
        }
        if (star.targeted.Count > 0) {
            foreach (Link link in star.targeted)
                CheckIfPathContains(currentPath, link.parent);
        }
    }

    static void CheckIfPathContains(List<Star> path, Star star) {

        if (path.Contains(star)) {
            if (path.IndexOf(star) == path.Count - 2)
                return; //this is the star we come from, abort
            else 
                GenerateLoop(path, star);
        } else if (star.links.Count + star.targeted.Count > 1) {
            List<Star> newPath = new List<Star>(path);
            CheckStar(star, newPath);
        }
    }

    /// <summary>
    /// Generates a loop on the specified path starting and finishing from the specified loop
    /// </summary>
    /// <param name="path"> The path on which the loop is present </param>
    /// <param name="star"> The star that is used as the starting and ending point of the loop </param>
    static void GenerateLoop(List<Star> path, Star star) {
        int loopStart = path.IndexOf(star);

        List<Star> loop = new List<Star>();

        for (int i = loopStart; i < path.Count; i++)
            loop.Add(path[i]);

        loop.Sort(CompareStars);
        
        Loop newLoop = new Loop(loop.ToArray());

        // We should avoid generating a loop containing only stars that are already in a loop

        if (newLoop.AlreadyExists())
            return; //don't generate a loop if it already exists;
        else
            LoopManager.Add(newLoop);

        string loopString = "New Loop: ";

        for (int i = 0; i < loop.Count; i++)
            loopString += loop[i].name + ", ";

        Debug.Log(loopString);
    }

    static int CompareStars(Star a, Star b) {
        if (a == null) {
            if (b == null) return 0;
            else return -1;
        }
        else {
            if (b == null) return 1;
            else return a.id.CompareTo(b.id);
        }
    }

}
