﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircuitManager : MonoBehaviour {

    public static CircuitManager instance;
    bool prismLoop, signal;
    List<Element> uniquePath;

    public void CheckCircuit(Element element) {
        signal = prismLoop = false;
        List<Element> path = new List<Element>();
        CheckElement(element, path);
    }

    public void SendSignal(Element element) {
        prismLoop = false;
        signal = true;
        uniquePath = new List<Element>();
        CheckElement(element, uniquePath);
        StartCoroutine(_EmptySignal(element));
    }

    void Awake() {
        instance = this;
    }

    #region Recursive Methods
    
    IEnumerator _SendSignalToElement(Element element) {

        yield return new WaitForSeconds(0.2f);
        uniquePath.Add(element);
        
        if (element.links.Count > 0) {
            foreach (Link link in element.links) {
                CheckIfPathContains(uniquePath, link.target);
                if (prismLoop) yield break;
                yield return new WaitForEndOfFrame();
            }
        }
        if (element.targeted.Count > 0) {
            foreach (Link link in element.targeted) {
                CheckIfPathContains(uniquePath, link.origin);
                if (prismLoop) yield break;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator _CheckElement(Element element, List<Element> currentPath) {

        currentPath.Add(element);

        if (element.links.Count > 0) {
            foreach (Link link in element.links) {
                CheckIfPathContains(currentPath, link.target);
                if (prismLoop) yield break;
                yield return new WaitForEndOfFrame();
            }
        }
        if (element.targeted.Count > 0) {
            foreach (Link link in element.targeted) {
                CheckIfPathContains(currentPath, link.origin);
                if (prismLoop) yield break;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator _EmptySignal(Element element) {
        yield return new WaitForSeconds(0.1999f);
        if (uniquePath.Contains(element)) yield break;
        element.VertexPing();
    }

    void CheckElement(Element element, List<Element> currentPath) {
        if (signal)
            StartCoroutine(_SendSignalToElement(element));
        else
            StartCoroutine(_CheckElement(element, currentPath));
    }

    void CheckIfPathContains(List<Element> path, Element element) {
        
        if (path.Contains(element)) {
            if (signal) return;
            if (path.IndexOf(element) == path.Count - 2)
                return; //this is the star we come from, abort
            else 
                GenerateLoop(path, element);
        } else if (element.links.Count + element.targeted.Count > 1) {
            List<Element> newPath = new List<Element>(path);
            CheckElement(element, newPath);
        }
        if (signal) StartCoroutine(_EmptySignal(element));
    }

    #endregion

    /// <summary>
    /// Generates a loop on the specified path starting and finishing from the specified loop
    /// </summary>
    /// <param name="path"> The path on which the loop is present </param>
    /// <param name="element"> The star that is used as the starting and ending point of the loop </param>
    void GenerateLoop(List<Element> path, Element element) {
        int loopStart = path.IndexOf(element);

        if (path[0].GetComponent<Prism>()) { // Prism Loop

            prismLoop = true;

            Chroma newChroma = Chroma.zero;

            foreach (Element prism in path)
                newChroma += prism.chroma;

            newChroma.ReBalance();
            Vector3 positionNewStar = AveragePoint(path);

            foreach (Element prism in path) {
                prism.GetComponent<Prism_Movement>().StartCoroutine("bringPrismTowardsCenterOfPath", positionNewStar);
            }

            Instantiate(PrefabManager.starCreationParticles, positionNewStar, Quaternion.identity); //spawn the creation particles

            while (path.Count > 0) path.Remove(path[0]);

            WorldWrapper.singleton.currentInstance.CreateStar(positionNewStar, newChroma); //create the new star

            SoundManager.singleton.Play(SoundManager.singleton.starCreation, 1f, WorldWrapper.singleton.currentInstance.stars[WorldWrapper.singleton.currentInstance.stars.Length - 1].GetComponent<AudioSource>());

        } else if (path[0].GetComponent<Star>()) { // Star Loop
            List<Star> loop = new List<Star>();
            for (int i = loopStart; i < path.Count; i++)
                loop.Add((Star)path[i]);

            loop.Sort(CompareStars);
            // We should avoid generating a loop containing only stars that are already in a loop?
            Loop newLoop = new Loop(loop.ToArray());
            if (newLoop.AlreadyExists()) return;
            else LoopManager.Add(newLoop);
        }
    }

    /// <summary>
    /// Returns the point in the middle of all defined points using their average coordinates.
    /// </summary>
    /// <param name="points"> All the points to average. </param>
    /// <returns></returns>
    Vector3 AveragePoint(List<Element> points) {
        Vector3 sum = points[0].transform.position;

        for (int i = 1; i < points.Count; i++)
            sum += points[i].transform.position;

        return sum / points.Count;
    }

    int CompareStars(Star a, Star b) {
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
