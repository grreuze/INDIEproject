using UnityEngine;
using System.Collections.Generic;

public static class CircuitManager {
    
    public static void CheckCircuit(Element element) {
        List<Element> path = new List<Element>();
        CheckStar(element, path);
    }
    
    static void CheckStar(Element element, List<Element> currentPath) {
        currentPath.Add(element);

        if (element.links.Count > 0) {
            foreach (Link link in element.links)
                CheckIfPathContains(currentPath, link.target);
        }
        if (element.targeted.Count > 0) {
            foreach (Link link in element.targeted)
                CheckIfPathContains(currentPath, link.parent);
        }
    }

    static void CheckIfPathContains(List<Element> path, Element element) {

        if (path.Contains(element)) {
            if (path.IndexOf(element) == path.Count - 2)
                return; //this is the star we come from, abort
            else 
                GenerateLoop(path, element);
        } else if (element.links.Count + element.targeted.Count > 1) {
            List<Element> newPath = new List<Element>(path);
            CheckStar(element, newPath);
        }
    }

    /// <summary>
    /// Generates a loop on the specified path starting and finishing from the specified loop
    /// </summary>
    /// <param name="path"> The path on which the loop is present </param>
    /// <param name="element"> The star that is used as the starting and ending point of the loop </param>
    static void GenerateLoop(List<Element> path, Element element) {
        int loopStart = path.IndexOf(element);

        if (path[0].GetType() == System.Type.GetType("Prism")) {
            Debug.Log("Prism Loop");
            return;
        }

        List<Star> loop = new List<Star>();

        for (int i = loopStart; i < path.Count; i++)
            loop.Add((Star)path[i]);

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
